using UnityEngine;
using System;

namespace CoreTeamGamesSDK.SettingsService
{
    /// <summary>
    /// The settings value
    /// </summary>
    [Serializable]
    public class SettingsValueWithName
    {
        #region Variables
        [SerializeField] private string _name = "Name";
        [SerializeField] [SerializeReference] private object _value = new object();
        #endregion

        #region Properties
        /// <summary>
        /// The name of SettingsValue
        /// </summary>
        public string Name => _name;
        /// <summary>
        /// The value of SettingsValue
        /// </summary>
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion

        #region Constructor
        public SettingsValueWithName(string name, object value)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _value = value;
        }
        #endregion
    }
}