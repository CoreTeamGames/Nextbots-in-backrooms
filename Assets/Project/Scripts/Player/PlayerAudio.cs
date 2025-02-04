using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private Transform _foots;
    [SerializeField] private AudioClip[] _steps;
    [SerializeField] private float _stepDelay = .8f;
    [SerializeField] private float _runStepDelay = .4f;

    private bool _isInitialized = false;
    private MovementController _controller;
    private float _time;

    private void Awake() => Initialize();

    public void Initialize()
    {
        _controller = gameObject.transform.parent.gameObject.GetComponentInChildren<MovementController>();

        if (_controller == null)
        {
            Debug.LogError("Can not find the MovementController in player!");
            return;
        }

        _controller.OnMoveEvent += OnMove;

        _isInitialized = true;
    }

    private void OnDestroy()
    {
        if (!_isInitialized)
            return;

        _controller.OnMoveEvent -= OnMove;
    }

    public void OnMove(Vector2 moveVector, float velocity, bool isRun, bool isDuck, bool isProne)
    {
        if (Time.time < _time || _steps.Length == 0 || !_controller.IsMove || !_controller.IsGrounded)
            return;

        AudioSource.PlayClipAtPoint(_steps[Random.Range(0, _steps.Length)], _foots.position);

        _time = Time.time + (isRun ? _runStepDelay : _stepDelay);
    }
}
