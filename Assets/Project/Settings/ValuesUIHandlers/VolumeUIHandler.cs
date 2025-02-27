using CoreTeamGamesSDK.SettingsService;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Settings.Handlers
{
    public class VolumeUIHandler : SettingsValueUIHandler
    {
        #region Variables
        [SerializeField] private Slider _slider;

        private bool _isFirstLoad = true;
        #endregion

        #region Properties
        public override object ValueFromUIElement => _slider.value;
        #endregion

        #region Code
        private void Awake()
        {
            _slider.onValueChanged.AddListener(OnValueChanges);
        }

        private void OnValueChanges(float volume)
        {
            Value.SetValueWithoutNotify(volume);
        }

        public override void SetUIElementValue(object value)
        {
            _slider.value = Convert.ToSingle(value);
        }

        public override void SetUIElementValueWithoutNotify(object value)
        {
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                Value.SetValueWithoutNotify(value);
            }

            _slider.SetValueWithoutNotify(Convert.ToSingle(value));
        }

        public override void OnReset()
        {
            _slider.SetValueWithoutNotify(0);
        }
        #endregion
    }
}