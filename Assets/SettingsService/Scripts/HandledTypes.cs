using UnityEngine;
using System;

namespace CoreTeamGamesSDK.SettingsService
{
    /// <summary>
    /// The Array of HandledTypes in SettingsService
    /// </summary>
    [Serializable]
    public static class HandledTypes
    {
        #region Variables
        private static Type[] _handledTypes =
        {
            typeof(string),
            typeof(bool),
            typeof(int),
            typeof(float),
            //typeof(Vector2),
            //typeof(Vector2Int),
            //typeof(Vector3),
            //typeof(Vector3Int),
            typeof(Resolution),
        };
        #endregion

        #region Properties
        public static Type[] HandledTypesArray => _handledTypes;
        public static string[] HandledTypesNamesArray => GetNames();

        private static string[] GetNames()
        {
            if (HandledTypesArray.Length == 0)
                return null;

            string[] names = new string[HandledTypesArray.Length];

            for (int i = 0; i < _handledTypes.Length; i++)
            {
                names[i] = _handledTypes[i].Name;
            }

            return names;
        }
        #endregion
    }
}