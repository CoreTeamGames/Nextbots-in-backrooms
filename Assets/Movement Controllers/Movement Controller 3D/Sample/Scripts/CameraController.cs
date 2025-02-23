using CoreTeamGamesSDK.MovementController.ThreeD;
using UnityEngine;

public class CameraController : CameraControllerBase
{
    #region Variables
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool _blockAndHideCursor;
    [SerializeField] private bool _blockInput = false;

    private float xRotation = 0f;
    #endregion

    #region Code
    public void BlockInput(bool isBlocked)
    {
        _blockInput = isBlocked;
    }

    public override void Initialize()
    {
        if (_blockAndHideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void Look(Vector2 lookVector)
    {
        if (cameraTransform == null || playerBody == null || Time.timeScale == 0 || _blockInput)
            return;
        
        float mouseX = lookVector.x * Sensivity * (InvertX ? -1 : 1);
        float mouseY = lookVector.y * Sensivity * (InvertY ? -1 : 1);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, MinXAngle, MaxXAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetSensivity(float sensivity)
    {
        if (sensivity > 0)
            base.sensivity = sensivity;
    }
    #endregion
}