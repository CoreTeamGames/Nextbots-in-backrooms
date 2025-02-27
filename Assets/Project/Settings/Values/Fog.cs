using CoreTeamGamesSDK.SettingsService;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Fog")]
    public class Fog : SettingsValue
    {
        public override void OnApply()
        {
            RenderSettings.fog = (bool)value;
        }

        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        public override void OnReset()
        {
            RenderSettings.fog = true;
        }
    }
}