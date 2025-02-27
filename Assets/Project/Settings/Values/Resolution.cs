using CoreTeamGamesSDK.SettingsService;
using System.Linq;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Resolution")]
    public class Resolution : SettingsValue
    {
        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            ChangeResolution((int)value);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            int defaultIndex = System.Array.IndexOf(Screen.resolutions, Screen.resolutions.Single(res => res.width == Display.main.renderingWidth && res.height == Display.main.renderingHeight));

            ChangeResolution(defaultIndex);
        }

        private void ChangeResolution(int index)
        {
            if (index < 0 || index > Screen.resolutions.Length)
                return;

            Screen.SetResolution(Screen.resolutions[index].width, Screen.resolutions[index].height,Screen.fullScreen);
        }
    }
}