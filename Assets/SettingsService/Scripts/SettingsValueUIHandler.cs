using System.Linq;
using UnityEngine;

namespace CoreTeamGamesSDK.SettingsService
{
    public abstract class SettingsValueUIHandler : MonoBehaviour
    {
        #region Variables
        [SerializeField] protected SettingsValue value;
        #endregion

        #region Properties
        public SettingsValue Value => value;
        public abstract object ValueFromUIElement { get; }
        #endregion

        #region Code
        public abstract void SetUIElementValue(object value);
        public abstract void SetUIElementValueWithoutNotify(object value);
        public abstract void OnReset();
        #endregion
    }
}