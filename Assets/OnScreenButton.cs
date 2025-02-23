using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

[RequireComponent(typeof(Graphic))]
public class OnScreenButton : OnScreenControl
{
    [SerializeField] protected float normalAlpha = 1f;
    [SerializeField] protected float pressedAlpha = 0.9f;
    [SerializeField] protected float fadeDuration = 0.1f;
    [SerializeField] protected Graphic targetGraphic;

    private bool _isInit = false;

    private void Awake() => Init();

    public void Init()
    {
        if (_isInit)
            return;

        if (targetGraphic == null)
        {
            Debug.LogError("Target Graphic was not assigned in OnScreenButton");
            return;
        }

        _isInit = true;
    }

    private void Start()
    {
        if (!_isInit)
            return;

        targetGraphic.CrossFadeAlpha(normalAlpha, 0, true);
    }

    protected override void OnPress(PointerEventData eventData)
    {
        Trigger();
        Debug.Log("f");
        targetGraphic.CrossFadeAlpha(pressedAlpha, fadeDuration, true);
    }

    protected override void OnRelease(PointerEventData eventData)
    {
        targetGraphic.CrossFadeAlpha(normalAlpha, fadeDuration, true);
    }

    protected override void OnSwipe(PointerEventData eventData)
    {
    }
}