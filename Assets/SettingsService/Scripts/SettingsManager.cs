using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CoreTeamGamesSDK.SettingsService
{
    [AddComponentMenu("CoreTeam Games SDK/Settings/Settings Manager", 0)]
    [DefaultExecutionOrder(-100)]
    public class SettingsManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SettingsValues _defaultValues;
        [SerializeField] private string _fileName = "Settings.json";
        [SerializeField] private bool _loadSettingsOnLevelLoad = true;
        private static SettingsValue[] _settingsValues = new SettingsValue[0];
        #endregion

        #region Properties
        public static SettingsManager Manager { get; private set; }
        public static SettingsValue[] SettingsValues { get => _settingsValues; }
        #endregion

        #region Events
        public delegate void OnSettingsSaved(SettingsValue[] values);
        public static OnSettingsSaved OnSettingsSavedEvent;
        public delegate void OnSettingsUpdates(SettingsValue[] values);
        public static OnSettingsUpdates OnSettingsUpdatesEvent;
        public delegate void OnSettingsLoaded(SettingsValue[] values, bool isDefaultValues);
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
            SceneManager.activeSceneChanged += (s1, s2) => { UpdateSettings(); };
        }

        private void Start()
        {
            LoadSettings();

        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= (s1, s2) => { UpdateSettings(); };
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
                    Dictionary<string, object> values = new Dictionary<string, object>();

                    foreach (var value in _settingsValues)
                    {
                        if (!value.ExcludedRuntimePlatformsForValue.Contains(Application.platform))
                            values.Add(value.Name, value.Value);
                    }

                    string file = JsonConvert.SerializeObject(values, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                    });

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

            _settingsValues = new SettingsValue[Manager._defaultValues.Values.Length];
            Array.Copy(Manager._defaultValues.Values, _settingsValues, Manager._defaultValues.Values.Length);

            bool loadDefault = true;

            if (File.Exists(Path.Combine(Application.persistentDataPath, Manager._fileName)))
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();

                using (FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, Manager._fileName), FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string file = reader.ReadToEnd();

                        temp = JsonConvert.DeserializeObject<Dictionary<string, object>>(file);
                    }
                }

                if (temp.Count != 0)
                {
                    loadDefault = false;

                    foreach (var value in _settingsValues)
                    {
                        if (temp.ContainsKey(value.Name))
                            value.SetValue(temp.Single(keyValuePair => keyValuePair.Key == value.Name).Value);
                    }
                }
            }

            OnSettingsLoadedEvent?.Invoke(_settingsValues, loadDefault);
        }

        public static void UpdateSettings()
        {
            if (_settingsValues.Length > 0)
                OnSettingsUpdatesEvent?.Invoke(_settingsValues);
        }
    }
    #endregion
}