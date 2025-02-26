﻿using CoreTeamGamesSDK.MovementController.ThreeD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    MovementController _movementController;
    Rigidbody _headRigidbody;
    SphereCollider _headCollider;
    CameraController _cameraController;
    CharacterController _controller;
    [SerializeField] float _onDeathVelocityMultipiler = 1.5f;
    private bool _isDead = false;
    public delegate void OnDeath(); 
    public OnDeath OnDeathEvent; 

    private void Awake()
    {
        _movementController = GetComponentInChildren<MovementController>();
        _cameraController = GetComponentInChildren<CameraController>();
        _headCollider = GetComponentInChildren<SphereCollider>();
        _headRigidbody = _headCollider.GetComponent<Rigidbody>();
        _controller = GetComponent<CharacterController>();
    }

    public void Death()
    {
        if (_isDead)
            return;

        FindObjectOfType<StopwatchUI>().StopStopwatch();

        PauseMenu.canPause = false;
        _isDead = true;
        Vector3 velocity = _controller.velocity;

        Destroy(_movementController);
        Destroy(_cameraController);

        _controller.enabled = false;

        _headCollider.enabled = true;
        _headRigidbody.isKinematic = false;
        _headRigidbody.velocity = velocity * _onDeathVelocityMultipiler;

        ChaseManager.EndChase();
        ChaseManager.canChase = false;

        OnDeathEvent?.Invoke();
    }
}
