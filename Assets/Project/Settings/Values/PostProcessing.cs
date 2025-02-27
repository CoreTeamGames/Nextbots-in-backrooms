using CoreTeamGamesSDK.SettingsService;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/PostProcessing")]
    public class PostProcessing : SettingsValue
    {
        [SerializeField] private string _ignoreTagInName = "ICPP";
        [SerializeField] private bool _defaultPostProcessEnabled = true;


        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            EnablePostProcess((bool)value);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            EnablePostProcess(_defaultPostProcessEnabled);
        }

        private void EnablePostProcess(bool enabled)
        {
            PostProcessLayer[] postProcessLayers = FindObjectsOfType<PostProcessLayer>().Where(camera => !camera.name.Contains(_ignoreTagInName)).ToArray();

            if (postProcessLayers.Length == 0)
                return;

            foreach (var postProcessLayer in postProcessLayers)
            {
                postProcessLayer.enabled = enabled;
            }
        }
    }
}