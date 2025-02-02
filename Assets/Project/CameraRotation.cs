using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private Vector2 maxRotationAngle; // Максимальный угол отклонения

    private Vector3 _initialRotation;
    private Vector2 _mousePos;
    private Vector2 _screenPosition;
    private Vector3 _cameraRotation;

    private void Start()
    {
        // Сохраняем начальный поворот камеры
        _initialRotation = transform.eulerAngles;
    }

    private void Update()
    {
        _mousePos = Mouse.current.position.ReadValue();

        _screenPosition = new Vector2(
            (_mousePos.x / Screen.width) * 2 - 1,
            (_mousePos.y / Screen.height) * 2 - 1);

        _cameraRotation.x = _initialRotation.x + -_screenPosition.y * maxRotationAngle.y; // X rotation (pitch)
        _cameraRotation.y = _initialRotation.y + _screenPosition.x * maxRotationAngle.x; // Y rotation (yaw)
        _cameraRotation.z = _initialRotation.z; // Сохраняем исходный Z rotation

        transform.eulerAngles = _cameraRotation;
    }
}
