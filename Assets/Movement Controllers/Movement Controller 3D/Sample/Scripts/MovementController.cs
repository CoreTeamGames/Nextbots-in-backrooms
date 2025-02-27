using CoreTeamGamesSDK.MovementController.ThreeD;
using System.Collections;
using UnityEngine;

public class MovementController : MovementControllerBase
{
    #region Variables
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _movementSmoothTime = 0.1f;
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _minimalStaminaToEndRest = 25f;
    [SerializeField] private float _staminaDrainRatePerSecond = 10f; // Трата стамины в секунду при беге
    [SerializeField] private float _staminaCostPerJump = 20f; // Трата стамины за прыжок
    [SerializeField] private float _staminaRegenRatePerSecond = 10f; // Восстановление стамины в секунду
    [SerializeField] private float _staminaRegenRatePerSecondNoMove = 15f; // Восстановление стамины в секунду если не двигаться

    [SerializeField] private bool _blockInput = false;

    public float Stamina { get; private set; }
    public float MaxStamina => _maxStamina;
    private bool _isResting = false;
    private float _verticalVelocity;
    private Vector3 _currentVelocity;
    private Vector3 _smoothVelocity;
    private Vector2 _moveVector;
    private bool _isJumping;
    private bool _isProne;
    private bool _isRun;
    private bool _isDuck;
    private float _defaultHeight;
    private Vector3 _defaultCenter;
    [SerializeField] private Transform _cameraRoot;
    private float _defaultCameraHeight;
    private float _defaultRadius;
    private float deltaSpeedMultiplier = 1f;
    [SerializeField] private float currentSpeedMultiplier = 1f;
    [SerializeField] private float maxCurrentSpeedMultiplier = 4f;

    // Добавляем переменную для банихопа
    [SerializeField] private bool _isBunnyHopping = false;
    [SerializeField] private float _bunnyHopSpeedMultiplier = 1.2f; // Множитель скорости при банихопе
    [SerializeField] private float _bunnyHopresetTimer = 1;
    [SerializeField] private float _bunnyHopresetTimerStep = 1;
    private float _bunnyHopresetTimerTemp = 1;
    #endregion

    #region Constants
    private const float DUCK_RADIUS_MULTIPLIER = 1f;
    private const float PRONE_RADIUS_MULTIPLIER = 0.3f;
    private const float DUCK_HEIGHT_MULTIPLIER = 0.5f;
    private const float PRONE_HEIGHT_MULTIPLIER = 0.02f;
    #endregion

    #region Properties
    public override bool IsMove => _controller != null && _controller.velocity.magnitude != 0;
    public override bool IsGrounded => _controller == null || _controller.isGrounded;
    public override float Velocity => _controller != null ? _controller.velocity.magnitude : 0;
    public override bool IsProne => _isProne;
    public override bool IsRun => _isRun;
    public override bool IsDuck => _isDuck;
    #endregion

    #region Code
    public override void Initialize()
    {
        if (_controller != null)
        {
            _defaultHeight = _controller.height;
            _defaultCenter = _controller.center;
            _defaultRadius = _controller.radius;
        }

        if (_cameraRoot != null)
        {
            _defaultCameraHeight = _cameraRoot.localPosition.y;
        }

        Stamina = _maxStamina;
    }

    public override void Jump()
    {
        if (!EnableJump || Parameters == null || !_controller.isGrounded || _blockInput || _isJumping || Stamina - _staminaCostPerJump <= 0 || _isResting)
            return;

        if (_isProne || _isDuck)
        {
            StandUp();
            return;
        }

        Stamina -= _staminaCostPerJump;
        _verticalVelocity = Mathf.Sqrt(Parameters.JumpForce * -2f * Physics.gravity.y);
        _isJumping = true;

        // Активируем банихоп, если игрок стоит на месте
        if (IsMove)
        {
            _isBunnyHopping = true;
            _bunnyHopresetTimerTemp = _bunnyHopresetTimer;
        }

        // Увеличиваем скорость при банихопе
        if (_isBunnyHopping)
        {
            deltaSpeedMultiplier *= _bunnyHopSpeedMultiplier;
            deltaSpeedMultiplier = Mathf.Clamp(deltaSpeedMultiplier * _bunnyHopSpeedMultiplier, currentSpeedMultiplier, maxCurrentSpeedMultiplier);
        }
    }

    public override void Duck()
    {
        if (!EnableDuck || _blockInput)
            return;

        if (_isDuck)
        {
            if (!CanStandUp())
                return;

            _isDuck = false;
            _isProne = false;
            AdjustCharacterHeight(1f);
        }
        else
        {
            if (_isProne)
            {
                _isDuck = true;
                _isProne = false;
                AdjustCharacterHeight(DUCK_HEIGHT_MULTIPLIER);
            }
            else
            {
                _isDuck = true;
                _isProne = false;
                AdjustCharacterHeight(DUCK_HEIGHT_MULTIPLIER);
            }
        }
    }

    public override void Prone()
    {
        if (!EnableProne || _blockInput)
            return;

        if (_isProne)
        {
            if (!CanStandUp())
                return;

            _isDuck = false;
            _isProne = false;
            AdjustCharacterHeight(1f);
        }
        else
        {
            if (_isDuck)
            {
                _isDuck = false;
                _isProne = true;
                AdjustCharacterHeight(PRONE_HEIGHT_MULTIPLIER);
            }
            else
            {
                _isDuck = false;
                _isProne = true;
                AdjustCharacterHeight(PRONE_HEIGHT_MULTIPLIER);
            }
        }
    }

    public override void StandUp()
    {
        if (!_isDuck && !_isProne || !CanStandUp() || _blockInput)
            return;

        _isDuck = false;
        _isProne = false;
        AdjustCharacterHeight(1f);
    }

    private void AdjustCharacterHeight(float heightMultiplier)
    {
        if (_controller == null) return;

        Vector3 position = transform.position;
        float currentHeight = _controller.height;
        float targetHeight = _defaultHeight * heightMultiplier;
        float heightDifference = currentHeight - targetHeight;

        float radiusMultiplier = 1f;
        if (heightMultiplier == DUCK_HEIGHT_MULTIPLIER)
            radiusMultiplier = DUCK_RADIUS_MULTIPLIER;
        else if (heightMultiplier == PRONE_HEIGHT_MULTIPLIER)
            radiusMultiplier = PRONE_RADIUS_MULTIPLIER;

        float newRadius = _defaultRadius * radiusMultiplier;
        bool canAdjustRadius = CheckRadiusAdjustment(newRadius);

        if (canAdjustRadius)
        {
            _controller.radius = newRadius;
        }

        bool hasGroundObstacle = Physics.SphereCast(
            position + Vector3.up * (targetHeight + newRadius),
            newRadius,
            Vector3.down,
            out RaycastHit hitInfo,
            targetHeight + 0.1f,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        _controller.height = targetHeight;
        _controller.center = new Vector3(
            _defaultCenter.x,
            _defaultCenter.y * heightMultiplier,
            _defaultCenter.z
        );

        if (hasGroundObstacle)
        {
            float adjustedY = hitInfo.point.y + targetHeight * 0.5f;
            position.y = adjustedY;
        }
        else if (heightDifference > 0)
        {
            position.y -= heightDifference * 0.5f;
        }

        transform.position = position;

        if (_cameraRoot != null)
        {
            Vector3 newCameraPosition = _cameraRoot.localPosition;
            newCameraPosition.y = _defaultCameraHeight * heightMultiplier;
            StartCoroutine(SmoothCameraAdjustment(newCameraPosition));
        }
    }

    private bool CheckRadiusAdjustment(float newRadius)
    {
        Vector3 center = transform.position + Vector3.up * (_controller.height * 0.5f);
        return !Physics.CheckSphere(
            center,
            newRadius,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );
    }

    private IEnumerator SmoothCameraAdjustment(Vector3 targetPosition)
    {
        float elapsedTime = 0;
        float transitionDuration = 0.2f; // Время перехода
        Vector3 startPosition = _cameraRoot.localPosition;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            t = t * t * (3f - 2f * t);

            _cameraRoot.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        _cameraRoot.localPosition = targetPosition;
    }

    public override void Move(Vector2 moveVector)
    {
        if (!EnableMove || Parameters == null || _blockInput)
            return;

        _moveVector = moveVector;
    }

    public void FixedUpdate()
    {
        if (_isResting)
            _isRun = false;

        if (_controller.isGrounded)
        {
            _isJumping = false;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_bunnyHopresetTimerTemp != 0)
                _bunnyHopresetTimerTemp -= _bunnyHopresetTimerStep * Time.fixedDeltaTime;

            _bunnyHopresetTimerTemp = Mathf.Clamp(_bunnyHopresetTimerTemp, 0, _bunnyHopresetTimer);

            if (_bunnyHopresetTimerTemp == 0 && _isBunnyHopping)
            {
                _isBunnyHopping = false;
                deltaSpeedMultiplier = currentSpeedMultiplier;
            }
        }
        else
        {
            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        Vector3 targetDirection = new Vector3(
            _moveVector.x * Parameters.SideSpeedMultipiler,
            0,
            _moveVector.y * (_moveVector.y < 0 ? Parameters.BackSpeedMultipiler : 1f)
        );

        targetDirection = _controller.transform.TransformDirection(targetDirection);

        if (!_isDuck && !_isProne && _isRun) deltaSpeedMultiplier = Parameters.RunSpeedMultipiler;
        else if (_isDuck) deltaSpeedMultiplier = Parameters.DuckSpeedMultipiler;
        else if (_isProne) deltaSpeedMultiplier = Parameters.ProneSpeedMultipiler;

        

        Vector3 targetVelocity = targetDirection * Parameters.Speed * deltaSpeedMultiplier;

        _currentVelocity = Vector3.SmoothDamp(
            _currentVelocity,
            targetVelocity,
            ref _smoothVelocity,
            _movementSmoothTime
        );

        _currentVelocity.y = _verticalVelocity;

        if (_currentVelocity.x != 0 || _currentVelocity.z != 0 && !_blockInput)
            OnMoveEvent?.Invoke(_moveVector, Velocity, IsRun, IsDuck, IsProne);

        // Обновление стамины
        UpdateStamina();

        _controller.Move(_currentVelocity * Time.deltaTime);
    }

    public void BlockInput(bool isBlocked)
    {
        _blockInput = isBlocked;
    }

    private void UpdateStamina()
    {
        if (IsMove && _isRun && !_isResting)
        {
            // Тратим стамину при беге
            Stamina -= _staminaDrainRatePerSecond * Time.deltaTime;
        }
        else if ((!_isRun || !IsMove) && IsGrounded)
        {
            // Восстанавливаем стамину, если не бежим или не двигаемся
            Stamina += (IsMove ? _staminaRegenRatePerSecond : _staminaRegenRatePerSecondNoMove) * Time.deltaTime;
        }

        // Ограничиваем стамину в пределах [0, MaxStamina]
        Stamina = Mathf.Clamp(Stamina, 0, _maxStamina);

        // Если стамина опустилась до 0, начинаем отдых
        if (Stamina <= 0)
        {
            _isResting = true;
        }

        // Если стамина восстановилась до минимального значения, заканчиваем отдых
        if (_isResting && Stamina >= _minimalStaminaToEndRest)
        {
            _isResting = false;
        }
    }

    private bool CanStandUp()
    {
        if (_controller == null) return false;

        float heightDifference = _defaultHeight - _controller.height;

        Vector3 checkPosition = transform.position + Vector3.up * _controller.height;

        float radius = _controller.radius;

        bool hasObstacle = Physics.SphereCast(
            checkPosition,
            radius,
            Vector3.up,
            out RaycastHit hit,
            heightDifference,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        return !hasObstacle;
    }

    public override void Run(bool isRun)
    {
        if (_isResting || !EnableRun || _blockInput)
            return;

        _isRun = isRun;
    }
    #endregion
}