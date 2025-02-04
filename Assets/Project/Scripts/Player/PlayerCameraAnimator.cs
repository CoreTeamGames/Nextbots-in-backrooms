using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraAnimator : MonoBehaviour
{
    [SerializeField] private Transform _head;
    [SerializeField] private float _stepDelay = .8f;
    [SerializeField] private float _runStepDelay = .4f;
    [SerializeField] private float _walkBobbingAmount = 0.05f;
    [SerializeField] private float _walkBobbingSpeed = 10f;
    [SerializeField] private float _runBobbingAmount = 0.1f;
    [SerializeField] private float _runBobbingSpeed = 15f;

    private bool _isInitialized = false;
    private Vector3 initialCameraPosition;
    private MovementController _characterController;
    private float _bobbingTimer;

    private void Awake() => Initialize();

    public void Initialize()
    {
        _characterController = gameObject.transform.parent.gameObject.GetComponentInChildren<MovementController>();

        if (_characterController == null)
        {
            Debug.LogError("Can not find the CharacterController in player!");
            return;
        }

        initialCameraPosition = _head.localPosition;

        _characterController.OnMoveEvent += HandleCameraBobbing;

        _isInitialized = true;
    }

    private void HandleCameraBobbing(Vector2 moveVector, float velocity, bool isRun, bool isDuck, bool isProne)
    {
        if (_characterController == null)
            return;


        if (_characterController.IsMove && _characterController.IsGrounded)
        {
            // Определяем амплитуду и частоту покачивания в зависимости от скорости игрока
            float bobbingAmount = _characterController.IsRun ? _runBobbingAmount : _walkBobbingAmount;
            float bobbingSpeed = _characterController.IsRun ? _runBobbingSpeed : _walkBobbingSpeed;

            // Обновляем таймер покачивания
            _bobbingTimer += Time.deltaTime * bobbingSpeed;

            // Вычисляем новую позицию камеры с эффектом покачивания
            float newY = initialCameraPosition.y + Mathf.Sin(_bobbingTimer) * bobbingAmount;
            _head.localPosition = new Vector3(initialCameraPosition.x, newY, initialCameraPosition.z);
        }
        else
        {
            // Плавно возвращаем камеру в исходное положение
            _head.localPosition = Vector3.Lerp(_head.localPosition, initialCameraPosition, Time.deltaTime * _walkBobbingSpeed);
            _bobbingTimer = 0f;
        }
    }
}
