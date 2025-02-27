using CoreTeamGamesSDK.SettingsService;
using UnityEngine;

namespace Game.Settings.Handlers
{
    public class ResolutionUIHandler : SettingsValueUIHandler
    {
 	public override object ValueFromUIElement {get;}

        // SetUIElementValueinvokes when settings value changes
        public override void SetUIElementValue(object value)
        {

        }

        // SetUIElementValueWithoutNotifyinvokes when settings value changes but you need another behaviour
        public override void SetUIElementValueWithoutNotify(object value)
        {

        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {

        }

    }
}