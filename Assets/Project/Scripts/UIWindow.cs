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
        _group.DOFade(1, _fadeDuration);
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    public void HideWindow()
    {
        _group.DOFade(0, _fadeDuration);
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
            if (i == categoryIndex)
            {
                _categories[i].CanvasGroup.DOFade(1, _fadeDuration);
                _categories[i].CanvasGroup.interactable = true;
                _categories[i].CanvasGroup.blocksRaycasts = true;
                _categories[i].Button.GetComponentInChildren<TMPro.TMP_Text>().DOColor(Color.white, _fadeDuration);

            }
            else
            {
                _categories[i].CanvasGroup.DOFade(0, _fadeDuration);
                _categories[i].CanvasGroup.interactable = false;
                _categories[i].CanvasGroup.blocksRaycasts = false;
                _categories[i].Button.GetComponentInChildren<TMPro.TMP_Text>().DOColor(_notShownCategoryButtonColor, _fadeDuration);
            }
        }
    }
    #endregion
}