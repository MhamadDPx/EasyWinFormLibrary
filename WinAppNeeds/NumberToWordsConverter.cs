using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Provides comprehensive number-to-words conversion with multi-language support.
    /// Supports English, Arabic, Kurdish, and other languages with proper formatting and validation.
    /// Optimized for .NET Framework 4.8 with thread-safe operations and extensible architecture.
    /// </summary>
    public static class NumberToWordsConverter
    {
        #region Language Definitions

        /// <summary>
        /// Supported languages for number-to-words conversion
        /// </summary>
        public enum SupportedLanguage
        {
            /// <summary>English language</summary>
            English = 0,
            /// <summary>Arabic language</summary>
            Arabic = 1,
            /// <summary>Kurdish language</summary>
            Kurdish = 2,
            /// <summary>Persian/Farsi language</summary>
            Persian = 3
        }

        /// <summary>
        /// Gender for languages that require it (Arabic, etc.)
        /// </summary>
        public enum NumberGender
        {
            /// <summary>Masculine gender</summary>
            Masculine,
            /// <summary>Feminine gender</summary>
            Feminine
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Thread-safe dictionary to store language-specific number words
        /// </summary>
        private static readonly Dictionary<SupportedLanguage, LanguageData> _languageData =
            new Dictionary<SupportedLanguage, LanguageData>();

        /// <summary>
        /// Lock object for thread-safe initialization
        /// </summary>
        private static readonly object _initLock = new object();

        /// <summary>
        /// Flag to track if language data has been initialized
        /// </summary>
        private static bool _isInitialized = false;

        #endregion

        #region Language Data Class

        /// <summary>
        /// Contains language-specific data for number-to-words conversion
        /// </summary>
        private class LanguageData
        {
            public string[] Units { get; set; }
            public string[] Tens { get; set; }
            public string[] ScaleWords { get; set; }
            public string Connector { get; set; }
            public string OnlyPrefix { get; set; }
            public string HundredWord { get; set; }
            public CultureInfo Culture { get; set; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor to initialize language data
        /// </summary>
        static NumberToWordsConverter()
        {
            InitializeLanguageData();
        }

        /// <summary>
        /// Initializes all language data with thread safety
        /// </summary>
        private static void InitializeLanguageData()
        {
            if (_isInitialized) return;

            lock (_initLock)
            {
                if (_isInitialized) return;

                InitializeEnglish();
                InitializeArabic();
                InitializeKurdish();
                InitializePersian();

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Initializes English language data
        /// </summary>
        private static void InitializeEnglish()
        {
            _languageData[SupportedLanguage.English] = new LanguageData
            {
                Units = new string[]
                {
                    "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
                    "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
                    "seventeen", "eighteen", "nineteen"
                },
                Tens = new string[]
                {
                    "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
                },
                ScaleWords = new string[]
                {
                    "", "thousand", "million", "billion", "trillion", "quadrillion"
                },
                Connector = " and ",
                OnlyPrefix = "Only ",
                HundredWord = "hundred",
                Culture = CultureInfo.GetCultureInfo("en-US")
            };
        }

        /// <summary>
        /// Initializes Arabic language data
        /// </summary>
        private static void InitializeArabic()
        {
            _languageData[SupportedLanguage.Arabic] = new LanguageData
            {
                Units = new string[]
                {
                    "صفر", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
                    "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر", "ستة عشر",
                    "سبعة عشر", "ثمانية عشر", "تسعة عشر"
                },
                Tens = new string[]
                {
                    "", "", "عشرون", "ثلاثون", "أربعون", "خمسون", "ستون", "سبعون", "ثمانون", "تسعون"
                },
                ScaleWords = new string[]
                {
                    "", "الف", "مليون", "ملیار", "تريليون", "كوادريليون"
                },
                Connector = " و ",
                OnlyPrefix = "فقط ",
                HundredWord = "مئة",
                Culture = CultureInfo.GetCultureInfo("ar-SA")
            };
        }

        /// <summary>
        /// Initializes Kurdish language data
        /// </summary>
        private static void InitializeKurdish()
        {
            _languageData[SupportedLanguage.Kurdish] = new LanguageData
            {
                Units = new string[]
                {
                    "سفر", "یەک", "دوو", "سێ", "چوار", "پێنج", "شەش", "حەوت", "هەشت", "نۆ",
                    "دە", "یانزە", "دوانزە", "سیانزە", "چواردە", "پانزە", "شانزە",
                    "حەڤدە", "هەژدە", "نۆزدە"
                },
                Tens = new string[]
                {
                    "", "", "بیست", "سی", "چل", "پەنجا", "شەست", "حەفتا", "هەشتا", "نەوەت"
                },
                ScaleWords = new string[]
                {
                    "", "هەزار", "ملیۆن", "ملیار", "تریلیۆن", "کوادریلیۆن"
                },
                Connector = " و ",
                OnlyPrefix = "تەنها ",
                HundredWord = "سەد",
                Culture = CultureInfo.GetCultureInfo("ku-Arab-IQ")
            };
        }

        /// <summary>
        /// Initializes Persian language data
        /// </summary>
        private static void InitializePersian()
        {
            _languageData[SupportedLanguage.Persian] = new LanguageData
            {
                Units = new string[]
                {
                    "صفر", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه",
                    "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده",
                    "هفده", "هجده", "نوزده"
                },
                Tens = new string[]
                {
                    "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود"
                },
                ScaleWords = new string[]
                {
                    "", "هزار", "میلیون", "میلیارد", "تریلیون", "کوادریلیون"
                },
                Connector = " و ",
                OnlyPrefix = "فقط ",
                HundredWord = "صد",
                Culture = CultureInfo.GetCultureInfo("fa-IR")
            };
        }

        #endregion

        #region Enhanced Public Methods

        /// <summary>
        /// Converts an amount to words with currency labels in the specified language.
        /// Enhanced version of the original method with better error handling and validation.
        /// </summary>
        /// <param name="amount">The amount to convert</param>
        /// <param name="mainCurrencyText">Text for the main currency unit (e.g., "Dollar", "Dinar")</param>
        /// <param name="subCurrencyText">Text for the sub currency unit (e.g., "Cent", "Fils")</param>
        /// <param name="language">Target language for conversion</param>
        /// <param name="includeOnlyPrefix">Whether to include "Only" prefix</param>
        /// <param name="decimalPlaces">Number of decimal places to consider (default: 2)</param>
        /// <returns>Amount converted to words with currency labels</returns>
        /// <exception cref="ArgumentException">Thrown when amount is negative or currency texts are invalid</exception>
        public static string ConvertAmountToWords(double amount, string mainCurrencyText,
            string subCurrencyText, SupportedLanguage language = SupportedLanguage.English,
            bool includeOnlyPrefix = true, int? decimalPlaces = null)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (string.IsNullOrWhiteSpace(mainCurrencyText))
                throw new ArgumentException("Main currency text cannot be null or empty", nameof(mainCurrencyText));
            if (string.IsNullOrWhiteSpace(subCurrencyText))
                throw new ArgumentException("Sub currency text cannot be null or empty", nameof(subCurrencyText));

            try
            {
                InitializeLanguageData();
                var langData = _languageData[language];

                long amountWhole = (long)amount;
                long amountDecimal = (long)Math.Round((amount - amountWhole) * Math.Pow(10, decimalPlaces ?? LibrarySetting.NumberDefaultRound));

                var result = new StringBuilder();

                if (includeOnlyPrefix)
                {
                    result.Append(langData.OnlyPrefix);
                }

                result.Append(ConvertNumberToWords(amountWhole, language));
                result.Append(" ");
                result.Append(mainCurrencyText);

                if (amountDecimal > 0)
                {
                    result.Append(langData.Connector);
                    result.Append(ConvertNumberToWords(amountDecimal, language));
                    result.Append(" ");
                    result.Append(subCurrencyText);
                }

                return result.ToString().Trim();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert amount to words: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts a number to words in the specified language.
        /// Enhanced version with better structure and error handling.
        /// </summary>
        /// <param name="number">The number to convert (0 to long.MaxValue)</param>
        /// <param name="language">Target language for conversion</param>
        /// <param name="gender">Gender for languages that require it</param>
        /// <returns>Number converted to words</returns>
        /// <exception cref="ArgumentException">Thrown when number is negative</exception>
        public static string ConvertNumberToWords(long number, SupportedLanguage language = SupportedLanguage.English,
            NumberGender gender = NumberGender.Masculine)
        {
            if (number < 0)
                throw new ArgumentException("Number cannot be negative", nameof(number));

            InitializeLanguageData();

            if (number == 0)
                return _languageData[language].Units[0];

            return ConvertNumberRecursive(number, language, gender);
        }

        /// <summary>
        /// Converts a decimal number to words in the specified language.
        /// </summary>
        /// <param name="number">The decimal number to convert</param>
        /// <param name="language">Target language for conversion</param>
        /// <param name="decimalPlaces">Number of decimal places to include in words</param>
        /// <param name="includeDecimalLabel">Whether to include "point" or equivalent word</param>
        /// <returns>Decimal number converted to words</returns>
        public static string ConvertDecimalToWords(decimal number, SupportedLanguage language = SupportedLanguage.English,
            int decimalPlaces = 2, bool includeDecimalLabel = true)
        {
            if (number < 0)
                throw new ArgumentException("Number cannot be negative", nameof(number));

            InitializeLanguageData();
            var langData = _languageData[language];

            long wholePart = (long)Math.Truncate(number);
            long decimalPart = (long)Math.Round((number - wholePart) * (decimal)Math.Pow(10, decimalPlaces));

            var result = new StringBuilder();
            result.Append(ConvertNumberToWords(wholePart, language));

            if (decimalPart > 0)
            {
                if (includeDecimalLabel)
                {
                    string decimalWord = GetDecimalPointWord(language);
                    result.Append(" ");
                    result.Append(decimalWord);
                    result.Append(" ");
                }
                else
                {
                    result.Append(langData.Connector);
                }
                result.Append(ConvertNumberToWords(decimalPart, language));
            }

            return result.ToString().Trim();
        }

        #endregion

        #region Private Conversion Methods

        /// <summary>
        /// Recursively converts a number to words using the specified language
        /// </summary>
        private static string ConvertNumberRecursive(long number, SupportedLanguage language, NumberGender gender)
        {
            var langData = _languageData[language];

            if (number < 20)
            {
                return langData.Units[number];
            }

            if (number < 100)
            {
                long tensPlace = number / 10;
                long remainder = number % 10;

                string result = langData.Tens[tensPlace];
                if (remainder > 0)
                {
                    result += GetConnectorForLanguage(language, false) + ConvertNumberRecursive(remainder, language, gender);
                }
                return result;
            }

            if (number < 1000)
            {
                return ConvertHundreds(number, language, gender);
            }

            // Handle larger numbers with scale words
            return ConvertLargeNumbers(number, language, gender);
        }

        /// <summary>
        /// Converts numbers in the hundreds range
        /// </summary>
        private static string ConvertHundreds(long number, SupportedLanguage language, NumberGender gender)
        {
            var langData = _languageData[language];
            long hundreds = number / 100;
            long remainder = number % 100;

            string result;
            if (hundreds == 1)
            {
                result = langData.HundredWord;
            }
            else
            {
                result = langData.Units[hundreds] + " " + langData.HundredWord;
            }

            if (remainder > 0)
            {
                result += GetConnectorForLanguage(language, true) + ConvertNumberRecursive(remainder, language, gender);
            }

            return result;
        }

        /// <summary>
        /// Converts large numbers using scale words (thousand, million, etc.)
        /// </summary>
        private static string ConvertLargeNumbers(long number, SupportedLanguage language, NumberGender gender)
        {
            var langData = _languageData[language];
            var scaleWords = langData.ScaleWords;

            for (int scale = scaleWords.Length - 1; scale >= 1; scale--)
            {
                long scaleValue = (long)Math.Pow(1000, scale);

                if (number >= scaleValue)
                {
                    long quotient = number / scaleValue;
                    long remainder = number % scaleValue;

                    string result = ConvertNumberRecursive(quotient, language, gender) + " " + scaleWords[scale];

                    if (remainder > 0)
                    {
                        result += GetConnectorForLanguage(language, true) + ConvertNumberRecursive(remainder, language, gender);
                    }

                    return result;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the appropriate connector for the language and context
        /// </summary>
        private static string GetConnectorForLanguage(SupportedLanguage language, bool isLargeNumber)
        {
            var langData = _languageData[language];

            // Some languages might use different connectors for different contexts
            switch (language)
            {
                case SupportedLanguage.English:
                    return isLargeNumber ? " " : langData.Connector;
                case SupportedLanguage.Kurdish:
                case SupportedLanguage.Arabic:
                case SupportedLanguage.Persian:
                    // Kurdish, Arabic, and Persian always use "و" connector for all contexts
                    return langData.Connector;
                default:
                    return langData.Connector;
            }
        }

        /// <summary>
        /// Gets the word for decimal point in the specified language
        /// </summary>
        private static string GetDecimalPointWord(SupportedLanguage language)
        {
            switch (language)
            {
                case SupportedLanguage.English:
                    return "point";
                case SupportedLanguage.Arabic:
                    return "فاصلة";
                case SupportedLanguage.Kurdish:
                    return "خاڵ";
                case SupportedLanguage.Persian:
                    return "ممیز";
                default:
                    return "point";
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Validates if a language is supported
        /// </summary>
        /// <param name="language">Language to validate</param>
        /// <returns>True if language is supported</returns>
        public static bool IsLanguageSupported(SupportedLanguage language)
        {
            InitializeLanguageData();
            return _languageData.ContainsKey(language);
        }

        /// <summary>
        /// Gets all supported languages
        /// </summary>
        /// <returns>Array of supported languages</returns>
        public static SupportedLanguage[] GetSupportedLanguages()
        {
            InitializeLanguageData();
            var languages = new SupportedLanguage[_languageData.Keys.Count];
            _languageData.Keys.CopyTo(languages, 0);
            return languages;
        }

        /// <summary>
        /// Gets the culture info for a specific language
        /// </summary>
        /// <param name="language">Language to get culture for</param>
        /// <returns>CultureInfo for the language</returns>
        public static CultureInfo GetCultureInfo(SupportedLanguage language)
        {
            InitializeLanguageData();
            return _languageData.TryGetValue(language, out LanguageData data) ? data.Culture : CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Converts currency amount with automatic language detection based on culture
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="mainCurrency">Main currency text</param>
        /// <param name="subCurrency">Sub currency text</param>
        /// <param name="culture">Culture to determine language</param>
        /// <returns>Amount in words</returns>
        public static string ConvertAmountWithCulture(double amount, string mainCurrency, string subCurrency, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            SupportedLanguage language = DetectLanguageFromCulture(culture);

            return ConvertAmountToWords(amount, mainCurrency, subCurrency, language);
        }

        /// <summary>
        /// Detects supported language from culture info
        /// </summary>
        private static SupportedLanguage DetectLanguageFromCulture(CultureInfo culture)
        {
            string languageCode = culture.TwoLetterISOLanguageName.ToLower();

            switch (languageCode)
            {
                case "ar":
                    return SupportedLanguage.Arabic;
                case "ku":
                    return SupportedLanguage.Kurdish;
                case "fa":
                    return SupportedLanguage.Persian;
                default:
                    return SupportedLanguage.English;
            }
        }

        /// <summary>
        /// Formats number with ordinal suffix (1st, 2nd, 3rd, etc.) for supported languages
        /// </summary>
        /// <param name="number">Number to format</param>
        /// <param name="language">Target language</param>
        /// <returns>Number with ordinal suffix in words</returns>
        public static string ConvertToOrdinal(long number, SupportedLanguage language = SupportedLanguage.English)
        {
            if (number <= 0)
                throw new ArgumentException("Number must be positive for ordinal conversion", nameof(number));

            string baseNumber = ConvertNumberToWords(number, language);
            string ordinalSuffix = GetOrdinalSuffix(number, language);

            return baseNumber + ordinalSuffix;
        }

        /// <summary>
        /// Gets ordinal suffix for a number in the specified language
        /// </summary>
        private static string GetOrdinalSuffix(long number, SupportedLanguage language)
        {
            switch (language)
            {
                case SupportedLanguage.English:
                    if (number % 100 >= 11 && number % 100 <= 13)
                        return "th";
                    switch (number % 10)
                    {
                        case 1: return "st";
                        case 2: return "nd";
                        case 3: return "rd";
                        default: return "th";
                    }

                default:
                    return "";
            }
        }

        /// <summary>
        /// Refreshes language data (for backward compatibility)
        /// </summary>
        [Obsolete("This method is kept for backward compatibility. Language data is automatically initialized.")]
        public static void RefreshLanguage()
        {
            // This method is kept for backward compatibility with the original code
            InitializeLanguageData();
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// Extension method to convert double to words
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="mainCurrency">Main currency text</param>
        /// <param name="subCurrency">Sub currency text</param>
        /// <param name="language">Target language</param>
        /// <returns>Amount converted to words</returns>
        public static string ToWords(this double amount, string mainCurrency, string subCurrency,
            SupportedLanguage language = SupportedLanguage.English)
        {
            return ConvertAmountToWords(amount, mainCurrency, subCurrency, language);
        }

        /// <summary>
        /// Extension method to convert long to words
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="language">Target language</param>
        /// <returns>Number converted to words</returns>
        public static string ToWords(this long number, SupportedLanguage language = SupportedLanguage.English)
        {
            return ConvertNumberToWords(number, language);
        }

        /// <summary>
        /// Extension method to convert int to words
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="language">Target language</param>
        /// <returns>Number converted to words</returns>
        public static string ToWords(this int number, SupportedLanguage language = SupportedLanguage.English)
        {
            return ConvertNumberToWords(number, language);
        }

        /// <summary>
        /// Extension method to convert decimal to words
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <param name="language">Target language</param>
        /// <param name="decimalPlaces">Number of decimal places</param>
        /// <returns>Number converted to words</returns>
        public static string ToWords(this decimal number, SupportedLanguage language = SupportedLanguage.English,
            int decimalPlaces = 2)
        {
            return ConvertDecimalToWords(number, language, decimalPlaces);
        }

        #endregion
    }
}