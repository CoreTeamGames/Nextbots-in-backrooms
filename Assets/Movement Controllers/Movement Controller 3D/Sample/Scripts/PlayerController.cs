using CoreTeamGamesSDK.MovementController.ThreeD;
using CoreTeamGamesSDK.Input.Management;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputManager _manager;
    [SerializeField] private MovementControllerBase _movementController;
    [SerializeField] private CameraControllerBase _cameraController;
    #endregion

    #region Code
    private void Start()
    {
        if (_movementController != null)
        {
            _movementController.Initialize();
        }

        if (_cameraController != null)
        {
            _cameraController.Initialize();
        }
    }

    private void OnEnable()
    {

        _manager.SubscribeToInput("Move", OnMove);
        _manager.SubscribeToInput("Jump", OnJump);
        _manager.SubscribeToInput("Duck", OnDuck);
        _manager.SubscribeToInput("Prone", OnProne);
        _manager.SubscribeToInput("Run", OnRun);
        _manager.SubscribeToInput("Look", OnLook);
    }

    private void OnDisable()
    {
        _manager.UnsubscribeFromInput("Move", OnMove);
        _manager.UnsubscribeFromInput("Jump", OnJump);
        _manager.UnsubscribeFromInput("Duck", OnDuck);
        _manager.UnsubscribeFromInput("Prone", OnProne);
        _manager.UnsubscribeFromInput("Run", OnRun);
        _manager.UnsubscribeFromInput("Look", OnLook);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (_movementController != null)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            _movementController.Move(moveInput);
        }
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        if (_cameraController != null)
        {
            Vector2 lookVector = context.ReadValue<Vector2>();
            _cameraController.Look(lookVector);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (_movementController != null && context.performed)
        {
            _movementController.Jump();
        }
    }
    private void OnDuck(InputAction.CallbackContext context)
    {
        if (_movementController != null)
        {
            _movementController.Duck();
        }
    }
    private void OnProne(InputAction.CallbackContext context)
    {
        if (_movementController != null)
        {
            _movementController.Prone();
        }
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        if (_movementController != null)
        {
            _movementController.Run(context.ReadValueAsButton());
        }
    }
    #endregion
}