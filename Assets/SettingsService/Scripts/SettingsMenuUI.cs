using UnityEngine;
using System.Linq;

namespace CoreTeamGamesSDK.SettingsService
{
    [AddComponentMenu("CoreTeam Games SDK/Settings/Settings Menu UI", 1)]
    [RequireComponent(typeof(SettingsMenu))]
    public class SettingsMenuUI : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SettingsValueUIHandler[] _settingsValueUIHandlers;

        private SettingsMenu _targetMenu;
        private bool _isInitialized;
        #endregion

        #region Code
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_settingsValueUIHandlers.Length == 0)
            {
                Debug.LogError("The _settingsValueUIParsers does not contain any item");
                return;
            }

            _targetMenu = GetComponent<SettingsMenu>();

            SettingsManager.OnSettingsLoadedEvent += (v, d) => { UpdateUIValues(v); };
            SettingsManager.OnSettingsUpdatesEvent += UpdateUIValues;

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if (!_isInitialized)
                return;

            SettingsManager.OnSettingsLoadedEvent -= (v,d) => { UpdateUIValues(v); };
            SettingsManager.OnSettingsUpdatesEvent -= UpdateUIValues;
        }

        public void UpdateUIValues()
        {
            UpdateUIValues(SettingsManager.SettingsValues);
        }

        public void UpdateUIValues(SettingsValue[] values)
        {
            if (!_isInitialized)
                return;

            foreach (var handler in _settingsValueUIHandlers)
            {
                if (SettingsManager.SettingsValues.Where(value => value.Name == handler.Value.Name).Count() > 0)
                    handler.SetUIElementValueWithoutNotify(SettingsManager.SettingsValues.Single(value => value.Name == handler.Value.Name).Value);
            }
        }

        public void SaveSettings()
        {
            if (!_isInitialized)
                return;

            foreach (var handler in _settingsValueUIHandlers)
            {
                SettingsManager.SettingsValues.Single(settingsValue => settingsValue.Name == handler.Value.Name).SetValue(handler.ValueFromUIElement);
                _targetMenu.SaveSettings();
            }
        }
        #endregion
    }
}