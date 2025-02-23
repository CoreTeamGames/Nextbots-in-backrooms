using CoreTeamGamesSDK.Input.Management;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;

public abstract class OnScreenControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string _actionName;

    public string ActionName => _actionName;
    public static MobileControlsUI MobileControls { get; set; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (MobileControls == null)
        {
            Debug.LogError($"Can not find the MobileControlsUI");
            return;
        }
    }

    protected void Trigger(InputAction.CallbackContext context = new InputAction.CallbackContext())
    {
        Debug.Log("d");

        if (!InputManager.InputEvents.ContainsKey(_actionName))
            return;

        Debug.Log("r");

        InputManager.InputEvents[_actionName]?.Invoke(context);
    }

    /// <summary>
    /// This method calls when pointer press the OnScreenControl
    /// </summary>
    protected abstract void OnPress(PointerEventData eventData);
    /// <summary>
    /// This method calls when pointer releases the OnScreenControl
    /// </summary>
    protected abstract void OnRelease(PointerEventData eventData);
    /// <summary>
    /// This method calls when pointer drag the OnScreenControl (swipe)
    /// </summary>
    protected abstract void OnSwipe(PointerEventData eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (MobileControls == null || !MobileControls.Interactable)
            return;

        MobileControls.OnDrag();

        OnSwipe(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileControls == null || !MobileControls.Interactable)
            return;

        MobileControls.OnPress();

        OnPress(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MobileControls == null || !MobileControls.Interactable)
            return;

        MobileControls.OnRelease();

        OnRelease(eventData);
    }
}