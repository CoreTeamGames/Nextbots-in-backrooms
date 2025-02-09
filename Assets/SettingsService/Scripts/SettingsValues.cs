using UnityEngine;

namespace CoreTeamGamesSDK.SettingsService
{
    /// <summary>
    /// The asset with values for settings
    /// </summary>
    [CreateAssetMenu(menuName = "CoreTeamSDK/Settings/Values")]
    public class SettingsValues : ScriptableObject
    {
        #region Variables
        [SerializeField] private SettingsValueWithName[] _values;
        #endregion

        #region Properties
        public SettingsValueWithName[] Values => _values;
        #endregion
    }
}