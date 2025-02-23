using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class OnScreenLook : OnScreenControl
{
    private CameraController _controller;

    private void Awake()
    {
        _controller = FindObjectOfType<CameraController>();
    }

    protected override void OnPress(PointerEventData eventData)
    {
    }

    protected override void OnRelease(PointerEventData eventData)
    {
    }

    protected override void OnSwipe(PointerEventData eventData)
    {
        if ( _controller == null)
            return;

        _controller.Look(eventData.delta * Time.deltaTime);
    }
}