using CoreTeamGamesSDK.SettingsService;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Fullscreen")]
    public class Fullscreen : SettingsValue
    {
        [SerializeField] private bool _defaultFullscreen = true;

        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            Screen.fullScreen = (bool)value;
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            Screen.fullScreen = _defaultFullscreen;
        }

    }
}