using CoreTeamGamesSDK.SettingsService;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Sensivity")]
    public class Sensivity : SettingsValue
    {
        [SerializeField] private float _defaultsensivity = 2f;

        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            SetSensivity((float)value);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            SetSensivity(_defaultsensivity);
        }

        private void SetSensivity(float sensivity)
        {
            if (sensivity < 0)
                return;

            CameraController controller = FindObjectOfType<CameraController>();

            if (controller == null)
                return;

            controller.SetSensivity(sensivity);
        }
    }
}