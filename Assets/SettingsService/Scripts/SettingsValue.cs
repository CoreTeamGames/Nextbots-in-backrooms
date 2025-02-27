using System.Collections.Generic;
using UnityEngine;
using System;

namespace CoreTeamGamesSDK.SettingsService
{
    public abstract class SettingsValue : ScriptableObject
    {
        #region Variables
        [SerializeField] private string _name;
        [SerializeField] private Type _valueType = typeof(int);
        [SerializeField] protected object value;
        [SerializeField] protected object defaultValue;
        [SerializeField] protected bool canChangeValue = true;
        [SerializeField] private List<RuntimePlatform> _excludedRuntimePlatformsForValue;
        #endregion

        #region Properties
        /// <summary>
        /// The name of Settings Value
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// The object value
        /// </summary>
        public object Value => value;
        /// <summary>
        /// if this flag equals false, you can not change the settings value usings SetValue and SetValueWithoutNotify
        /// </summary>
        public bool CanChangeValue => canChangeValue;
        /// <summary>
        /// The runtime platforms for value. Sample: resolution can be only set on Desktop platforms
        /// </summary>
        public List<RuntimePlatform> ExcludedRuntimePlatformsForValue => _excludedRuntimePlatformsForValue;
        #endregion

        #region Events
        public delegate void OnValueSets(string name, object value);
        public OnValueSets OnValueSetsEvent;

        public delegate void OnValueResets(string name);
        public OnValueResets OnValueResetsEvent;
        #endregion

        #region Code
        public void SetValue(object value)
        {
            if (!canChangeValue)
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'}!");
                return;
            }

            if (_excludedRuntimePlatformsForValue.Contains(Application.platform))
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'} because current runtime platform does not support this value!");
                return;
            }

            if (value.GetType() != _valueType)
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'} because type of saved value does not equals to in-game value type!");
                return;
            }

            this.value = value;
            OnValueSetsEvent?.Invoke(name, value);
            OnApply();
        }

        public void SetValueWithoutNotify(object value)
        {
            if (!canChangeValue)
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'}!");
                return;
            }

            if (_excludedRuntimePlatformsForValue.Contains(Application.platform))
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'} because current runtime platform does not support this value!");
                return;
            }

            if (value.GetType() != _valueType)
            {
                Debug.LogWarning($"Can not change value of SettingsValue with name: {'"'}{_name}{'"'} because type of saved value does not equals to in-game value type!");
                return;
            }

            this.value = value;
            OnApplyWithoutNotify();
        }

        public void ResetValue()
        {
            OnValueResetsEvent?.Invoke(Name);
            OnReset();
        }

        public abstract void OnApply();
        public abstract void OnApplyWithoutNotify();

        public abstract void OnReset();
        #endregion
    }
}