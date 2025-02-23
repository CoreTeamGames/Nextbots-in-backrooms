using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class OnScreenStick : OnScreenControl
{
    [Header("Settings")]
    public float maxRadius = 100f;
    public bool smoothReturn = true;
    public float returnSpeed = 5f;

    [Header("References")]
    public RectTransform background;
    public RectTransform handle;

    private Vector2 inputVector = Vector2.zero;
    private Vector2 _delta = Vector2.zero;

    // Возвращает нормализованный вектор ввода (значения от -1 до 1)
    public Vector2 Direction => inputVector;

    private void Start()
    {
        if (background == null || handle == null)
        {
            Debug.LogError("Background and Handle must be assigned!");
            enabled = false;
        }
    }

    // Обработка перемещения стика
    protected override void OnSwipe(PointerEventData eventData)
    {
        if (!GetComponentInParent<CanvasGroup>().interactable)
            return;

        Vector2 touchPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out touchPos))
        {
            // Ограничиваем позицию ручки в пределах радиуса
            touchPos = Vector2.ClampMagnitude(touchPos, maxRadius);
            handle.anchoredPosition = touchPos;

            // Вычисляем вектор ввода
            inputVector = touchPos / maxRadius;
        }
    }

    // Обработка нажатия на стик
    protected override void OnPress(PointerEventData eventData)
    {
        if (!GetComponentInParent<CanvasGroup>().interactable)
            return;

        OnDrag(eventData); // Начинаем перемещение стика
    }

    // Обработка отпускания стика
    protected override void OnRelease(PointerEventData eventData)
    {
        if (!GetComponentInParent<CanvasGroup>().interactable)
            return;

        // Возвращаем стик в центр
        if (smoothReturn)
        {
            StartCoroutine(SmoothReturn());
        }
        else
        {
            handle.anchoredPosition = Vector2.zero;
            inputVector = Vector2.zero;
        }
    }

    // Плавное возвращение стика в центр
    private IEnumerator SmoothReturn()
    {
        while (handle.anchoredPosition != Vector2.zero)
        {
            handle.anchoredPosition = Vector2.Lerp(handle.anchoredPosition, Vector2.zero, returnSpeed * Time.unscaledDeltaTime);
            inputVector = handle.anchoredPosition / maxRadius;
            yield return null;
        }
    }

    private void Update()
    {
        if (_delta == Direction)
            return;

        _delta = Direction;
        OnDirectionChange();
    }

    public abstract void OnDirectionChange();
}