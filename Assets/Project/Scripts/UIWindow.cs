using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour
{
    #region Variables
    private CanvasGroup _group;
    [SerializeField] private float _fadeDuration = 0.4f;
    [SerializeField] private Color _notShownCategoryButtonColor;
    [SerializeField] private CanvasGroupWithButton[] _categories;
    #endregion

    #region Custom Classes
    [Serializable]
    public class CanvasGroupWithButton
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _button;

        public CanvasGroup CanvasGroup => _canvasGroup;
        public Button Button => _button;
    }
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
        _group.DOFade(1, _fadeDuration).SetUpdate(true);
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    public void HideWindow()
    {
        _group.DOFade(0, _fadeDuration).SetUpdate(true);
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }

    public void ShowCategory(int categoryIndex)
    {
        if (_categories.Length == 0)
        {
            Debug.LogError("The categories array does not cotain any item!");
            return;
        }
        if (_categories.Length < categoryIndex)
        {
            Debug.LogWarning($"The category index ({categoryIndex}) is bigger than categories array length ({_categories.Length})");
            return;
        }

        for (int i = 0; i < _categories.Length; i++)
        {
            _categories[i].CanvasGroup.DOFade(i == categoryIndex ? 1 : 0, _fadeDuration).SetUpdate(true);
            _categories[i].CanvasGroup.interactable = i == categoryIndex ? true : false;
            _categories[i].CanvasGroup.blocksRaycasts = i == categoryIndex ? true : false;
            _categories[i].Button.GetComponentInChildren<TMPro.TMP_Text>().DOColor(i == categoryIndex ? Color.white : _notShownCategoryButtonColor, _fadeDuration).SetUpdate(true);
        }
    }
    #endregion
}