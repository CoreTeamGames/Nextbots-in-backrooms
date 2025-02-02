using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour
{
    #region Variables
    private CanvasGroup _group;
    #endregion

    #region Code
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _group = GetComponent<CanvasGroup>();
    }

    public void ShowWindow()
    {
        _group.alpha = 1;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    public void HideWindow()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }
    #endregion
}