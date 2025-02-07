using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private CanvasGroup _hud;
    [SerializeField] private CanvasGroup _staminaBarGroup;
    [SerializeField] private Slider _staminaBar;
    [SerializeField] private float _staminaBarFadeDuration;
    [SerializeField] private float _staminaBarChange = 0.5f;
    [SerializeField] private UIWindow _deathHUD;
    private Player _player;
    private MovementController _movementController;
    private bool _isRun;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _movementController = _player.GetComponentInChildren<MovementController>();

        _movementController.OnMoveEvent += OnMove;
        _player.OnDeathEvent += OnDeath;

        _staminaBar.maxValue = _movementController.MaxStamina;
        _staminaBarGroup.alpha = 0f;
    }

    private void OnMove(Vector2 moveVector, float velocity, bool isRun, bool isDuck, bool isProne)
    {
        if (!_isRun && _movementController.Stamina != _movementController.MaxStamina)
        {
            _isRun = true;
            _staminaBarGroup.DOFade(1, _staminaBarFadeDuration);
        }
        else if (!isRun &&_movementController.Stamina == _movementController.MaxStamina)
        {
            _isRun = false;
            _staminaBarGroup.DOFade(0, _staminaBarFadeDuration);
        }
    }

    private void Update()
    {
        if (_staminaBar == null || _movementController == null)
            return;

        _staminaBar.value = Mathf.Lerp(_staminaBar.value, _movementController.Stamina, _staminaBarChange);
    }

    private void OnDestroy()
    {
        _movementController.OnMoveEvent -= OnMove;
        _player.OnDeathEvent -= OnDeath;
    }

    public void OnDeath()
    {
        _hud.alpha = 0;
        _deathHUD.ShowWindow();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
}
