using CoreTeamGamesSDK.SettingsService;
using UnityEngine.Audio;
using UnityEngine;
using System;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Volume")]
    public class Volume : SettingsValue
    {
        [SerializeField] private AudioMixer _targetMixer;

        public override void OnApply()
        {
            _targetMixer.SetFloat(Name, Convert.ToSingle(value == null ? 0 : value));
        }

        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        public override void OnReset() => _targetMixer.SetFloat(Name, 0);
    }
}