using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;

namespace CoreTeamGamesSDK.SettingsService
{
    [AddComponentMenu("CoreTeam Games SDK/Settings/Settings Manager",0)]
    [DefaultExecutionOrder(-100)]
    public class SettingsManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SettingsValues _defaultValues;
        [SerializeField] private string _fileName = "Settings.json";
        [SerializeField] private bool _loadSettingsOnLevelLoad = true;
        private static SettingsValueWithName[] _settingsValues = new SettingsValueWithName[0];
        #endregion

        #region Properties
        public static SettingsManager Manager { get; private set; }
        public static SettingsValueWithName[] SettingsValues { get => _settingsValues;}
        #endregion

        #region Events
        public delegate void OnSettingsSaved(SettingsValueWithName[] values);
        public static OnSettingsSaved OnSettingsSavedEvent;
        public delegate void OnSettingsUpdates(SettingsValueWithName[] values);
        public static OnSettingsUpdates OnSettingsUpdatesEvent;
        public delegate void OnSettingsLoaded(SettingsValueWithName[] values, bool isDefaultValues);
        public static OnSettingsLoaded OnSettingsLoadedEvent;
        #endregion

        #region Code
        private void Awake()
        {
            if (Manager != null)
            {
                Destroy(gameObject);
                return;
            }

            Manager = this;
            DontDestroyOnLoad(Manager);
            LoadSettings();
        }

        public static void SaveSettings()
        {
            if (Manager == null)
            {
                Debug.LogError("SettingsManager is null!");
                return;
            }
            if (_settingsValues.Length == 0)
                return;

            using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, Manager._fileName), FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    string file = JsonConvert.SerializeObject(_settingsValues);

                    writer.WriteLine(file);
                }
            }

            OnSettingsSavedEvent?.Invoke(_settingsValues);
        }

        public static void LoadSettings()
        {
            if (Manager == null)
            {
                Debug.LogError("SettingsManager is null!");
                return;
            }
            if (Manager._defaultValues == null)
            {
                Debug.LogError("The DefaultValues in SettingsManager is null!");
                return;
            }
            if (Manager._defaultValues.Values.Length == 0)
                return;

            _settingsValues = new SettingsValueWithName[Manager._defaultValues.Values.Length];
            Array.Copy(Manager._defaultValues.Values, _settingsValues, Manager._defaultValues.Values.Length);

            bool loadDefault = true;

            if (File.Exists(Path.Combine(Application.persistentDataPath, Manager._fileName)))
            {

                SettingsValueWithName[] temp = new SettingsValueWithName[0];

                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, Manager._fileName), FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string file = reader.ReadToEnd();
                        temp = JsonConvert.DeserializeObject<SettingsValueWithName[]>(file);
                    }
                }

                if (temp.Length != 0)
                {
                    loadDefault = false;

                    foreach (var item in temp)
                    {
                        foreach (var value in _settingsValues)
                        {
                            if (value.Name == item.Name && value.Value.GetType().Name == item.Value.GetType().Name)
                                value.Value = item.Value;
                        }
                    }
                }
            }
            OnSettingsLoadedEvent?.Invoke(_settingsValues, loadDefault);
        }
    
        public static void UpdateSettings()
        {
            OnSettingsUpdatesEvent?.Invoke(_settingsValues);
        }
    }
    #endregion
}