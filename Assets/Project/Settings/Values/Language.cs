using CoreTeamGamesSDK.Localization;
using CoreTeamGamesSDK.SettingsService;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Game.Settings.Values
{
    [CreateAssetMenu(menuName = "Game/SettingsValues/Language")]
    public class Language : SettingsValue
    {
        // OnApply invokes when settings value changes
        public override void OnApply()
        {
            SetLanguage((string)value);
        }

        // OnApplyWithoutNotify invokes when settings value changes but you need another behaviour
        public override void OnApplyWithoutNotify()
        {
            OnApply();
        }

        // OnReset invokes when settings value sets to default
        public override void OnReset()
        {
            LocalizationService service = FindObjectOfType<LocalizationService>();

            if (service == null || service.Languages.Count != 0)
                return;

            string defaultLocale = service.Languages.Single(lang => lang.LangLocale.ToLower() == CultureInfo.CurrentCulture.Name.ToLower()).LangLocale;

            defaultLocale = defaultLocale != "" ? defaultLocale : service.Languages.Single(lang => lang.LangLocale.ToLower() == "en-us").LangLocale;

            SetLanguage(defaultLocale);
        }

        private void SetLanguage(string locale)
        {
            LocalizationService service = FindObjectOfType<LocalizationService>();

            if (service == null || service.Languages.Count != 0)
                return;

            locale = locale.ToLower();

            if (service.Languages.Where(lang => lang.LangLocale.ToLower() == locale).Count() == 0)
                return;

            if (service.CurrentLanguage.LangLocale == service.Languages.Single(lang => lang.LangLocale.ToLower() == locale).LangLocale)
                return;

            service.ChangeLanguage(service.Languages.Single(lang => lang.LangLocale.ToLower() == locale));
        }
    }
}