using CoreTeamGamesSDK.SettingsService;
using System.Linq;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/DrawDistance")]
    public class DrawDistance : SettingsValue
    {
        [SerializeField] private string _ignoreTagInName = "ICDD";

        public readonly int minDrawDistance = 30;
        public readonly int maxDrawDistance = 100;
        public int CurrentDrawDistance { get; private set; }

        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            value = (int)value >= minDrawDistance ? ((int)value <= maxDrawDistance ? (int)value : maxDrawDistance) : minDrawDistance;
            CurrentDrawDistance = (int)value;

            ChangeDrawDistance(CurrentDrawDistance);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            CurrentDrawDistance = maxDrawDistance;
            ChangeDrawDistance(CurrentDrawDistance);
        }

        private void ChangeDrawDistance(int distance)
        {
            Camera[] cameras = FindObjectsOfType<Camera>().Where(camera => !camera.name.Contains(_ignoreTagInName)).ToArray();

            if (cameras.Length == 0)
                return;

            foreach (var camera in cameras)
            {
                camera.farClipPlane = distance;
            }
        }
    }
}