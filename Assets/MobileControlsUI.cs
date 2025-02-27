using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[DefaultExecutionOrder(-50)]
public class MobileControlsUI : MonoBehaviour
{
    private CanvasGroup _group;
    public int Touches { get; private set; }
    public bool Interactable => _group.interactable;
    public CanvasGroup TargetCanvasGroup => _group;

    private void Awake()
    {
        OnScreenControl.MobileControls = this;
        _group = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        _group.alpha = Application.isMobilePlatform ? 1 : 0;
        _group.interactable = Application.isMobilePlatform ? true : false;
        _group.blocksRaycasts = Application.isMobilePlatform ? true : false;
        PauseMenu.OnGamePauseEvent += OnPause;
    }

    private void OnDestroy()
    {
        PauseMenu.OnGamePauseEvent -= OnPause;
    }

    private void OnPause(bool isPaused)
    {
        if (!Application.isMobilePlatform)
            return;

        _group.alpha = isPaused ? 0 : 1;
        _group.interactable = isPaused ? false : true;
        _group.blocksRaycasts = isPaused ? false : true;
    }

    public void OnPress()
    {
        Touches++;
    }

    public void OnDrag()
    {

    }

    public void OnRelease()
    {
        Touches = Touches - 1 >= 0 ? Touches - 1 : 0;
    }
}