using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStick : OnScreenStick
{
    [SerializeField] private MovementController _controller;
    [SerializeField] private float _directionToEnableRun = 0.8f;

    public override void OnDirectionChange()
    {
        _controller.Move(Direction.normalized);
        _controller.Run(Direction.magnitude >= _directionToEnableRun ? true : false);
    }
}
