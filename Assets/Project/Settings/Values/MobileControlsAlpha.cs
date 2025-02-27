using CoreTeamGamesSDK.SettingsService;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/MobileControlsAlpha")]
    public class MobileControlsAlpha : SettingsValue
    {
        [SerializeField] private float _defaultAlpha = 1f;

        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            SetUIAlpha((float)value);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            SetUIAlpha(_defaultAlpha);
        }

        private void SetUIAlpha(float alpha)
        {
            if (alpha > 1 || alpha < 0)
                return;

            MobileControlsUI targetUI = FindObjectOfType<MobileControlsUI>();

            if (targetUI == null)
                return;

            targetUI.TargetCanvasGroup.alpha = alpha;
        }
    }
}