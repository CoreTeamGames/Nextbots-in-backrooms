﻿using System.Linq;
using UnityEngine;

namespace CoreTeamGamesSDK.SettingsService
{
    [AddComponentMenu("CoreTeam Games SDK/Settings/Settings Menu", 1)]
    public class SettingsMenu : MonoBehaviour
    {
        #region Code

        public void SaveSettings()
        {
            SettingsManager.SaveSettings();
        }

        public void LoadSettings()
        {
            SettingsManager.LoadSettings();
        }
        #endregion
    }
}