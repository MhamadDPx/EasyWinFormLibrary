using EasyWinFormLibrary.WinAppNeeds;
using System.Drawing;

namespace EasyWinFormLibrary
{
    /// <summary>
    /// 
    /// This class contains settings for the EasyWinFormLibrary.
    /// </summary>
    public class LibrarySettings
    {
        public static Color ProgramPrimaryColor = Color.FromArgb(9, 26, 52);
        public static Color ProgramPrimaryDimColor = Color.FromArgb(91, 102, 119);
        public static Color ProgramSecondaryColor = Color.FromArgb(158, 28, 32);
        public static Color ProgramSecondaryDimColor = Color.FromArgb(190, 103, 106);

        /// <summary>
        /// Gets or sets the default number of rounds for the application.
        /// </summary>
        public static int NumberDefaultRound { get; set; } = 0;

        public static string PriceNumberTextKu;
        public static string PriceNumberTextAr;
        public static string PriceNumberTextEn;
        public static string PriceNumberText(ReportLanguage reportLanguage)
        {
            switch (reportLanguage)
            {
                case ReportLanguage.Ku:
                    return PriceNumberTextKu;
                case ReportLanguage.Ar:
                    return PriceNumberTextAr;
                case ReportLanguage.En:
                    return PriceNumberTextEn;
                default:
                    return PriceNumberTextKu;
            }
        }
        public static string PricePointTextKu;
        public static string PricePointTextAr;
        public static string PricePointTextEn;
        public static string PricePointText(ReportLanguage reportLanguage)
        {
            switch (reportLanguage)
            {
                case ReportLanguage.Ku:
                    return PricePointTextKu;
                case ReportLanguage.Ar:
                    return PricePointTextAr;
                case ReportLanguage.En:
                    return PricePointTextEn;
                default:
                    return PricePointTextKu;
            }
        }
        public static string CurrencySymbol { get; set; }
    }
}
