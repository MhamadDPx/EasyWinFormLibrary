using System.Globalization;
using System.Threading;

namespace EasyWinFormLibrary.WinAppNeeds
{
    public enum FormLanguage
    {
        English,
        Kurdish,
        Arabic
    }
    public static class LanguageManager
    {
        #region Private Fields
        private static FormLanguage _selectedLanguage = FormLanguage.Kurdish;
        #endregion

        public static FormLanguage SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                ChangeLanguage(value);
            }
        }
        public static string LanguageCultureName = "en-US";
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
