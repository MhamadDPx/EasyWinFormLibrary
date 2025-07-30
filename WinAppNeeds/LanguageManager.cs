using System.Globalization;
using System.Threading;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Represents the supported languages in the application's user interface.
    /// </summary>
    public enum FormLanguage
    {
        English,
        Kurdish,
        Arabic
    }

    /// <summary>
    /// Manages language and culture settings for the application.
    /// Provides functionality to change the application's language and date format settings.
    /// </summary>
    public static class LanguageManager
    {
        #region Private Fields
        private static FormLanguage _selectedLanguage = FormLanguage.Kurdish;
        #endregion

        /// <summary>
        /// Gets or sets the currently selected language.
        /// Setting a new language automatically applies the corresponding culture settings.
        /// </summary>
        public static FormLanguage SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                ChangeLanguage(value);
            }
        }

        /// <summary>
        /// Gets the current language culture name. Default is "en-US".
        /// </summary>
        public static string LanguageCultureName = "en-US";

        /// <summary>
        /// Changes the application's language and culture settings based on the selected language.
        /// Updates both the current thread's culture and UI culture, and sets date format patterns.
        /// </summary>
        /// <param name="selectedLanguage">The language to switch to</param>
        public static void ChangeLanguage(FormLanguage selectedLanguage)
        {
            CultureInfo ci;
            switch (selectedLanguage)
            {
                case FormLanguage.English:
                    ci = new CultureInfo("en-001");
                    break;
                case FormLanguage.Kurdish:
                    ci = new CultureInfo("en-US");
                    break;
                case FormLanguage.Arabic:
                    ci = new CultureInfo("en-GB");
                    break;
                default:
                    ci = new CultureInfo("en-US");
                    break;
            }

            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            ci.DateTimeFormat.LongDatePattern = "yyyy-MM-dd HH:mm tt";
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            _selectedLanguage = selectedLanguage;
        }
    }
}
