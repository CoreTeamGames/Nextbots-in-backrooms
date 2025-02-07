using DG.Tweening;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private Transform _foots;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _chaseSource;
    [SerializeField] private AudioClip[] _steps;
    [SerializeField] private float _stepDelay = .8f;
    [SerializeField] private float _runStepDelay = .4f;
    [SerializeField] private float _chaseMusicFade = 2f;

    private bool _isInitialized = false;
    private MovementController _controller;
    private Player _player;
    private float _time;

    private void Awake() => Initialize();

    public void Initialize()
    {
        _controller = gameObject.transform.parent.gameObject.GetComponentInChildren<MovementController>();
        _player = gameObject.transform.parent.gameObject.GetComponent<Player>();

        if (_controller == null)
        {
            Debug.LogError("Can not find the MovementController in player!");
            return;
        }

        if (_player == null)
        {
            Debug.LogError("Can not find the Player in player!");
            return;
        }

        _chaseSource.volume = 0f;

        _controller.OnMoveEvent += OnMove;
        _player.OnDeathEvent += OnDeath;
        ChaseManager.OnChaseStartEvent += OnChaseStart;
        ChaseManager.OnChaseEndEvent += OnChaseEnd;

        _isInitialized = true;
    }

    private void OnDeath()
    {
        _sfxSource.Play();
    }

    private void OnDestroy()
    {
        if (!_isInitialized)
            return;

        _controller.OnMoveEvent -= OnMove;
        _player.OnDeathEvent -= OnDeath;
        ChaseManager.OnChaseStartEvent -= OnChaseStart;
        ChaseManager.OnChaseEndEvent -= OnChaseEnd;
    }

    public void OnMove(Vector2 moveVector, float velocity, bool isRun, bool isDuck, bool isProne)
    {
        if (Time.time < _time || _steps.Length == 0 || !_controller.IsMove || !_controller.IsGrounded)
            return;

        _sfxSource.PlayOneShot(_steps[Random.Range(0, _steps.Length)]);

        _time = Time.time + (isRun ? _runStepDelay : _stepDelay);
    }

    private void OnChaseStart()
    {
        _chaseSource.Play();
        _chaseSource.DOFade(1, _chaseMusicFade);
    }

    private void OnChaseEnd()
    {
        _chaseSource.DOFade(0, _chaseMusicFade).OnComplete(() =>
        {
            if (!ChaseManager.IsChasing)
            _chaseSource.Stop();
        });
    }
}