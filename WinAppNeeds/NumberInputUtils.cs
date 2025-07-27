using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Provides comprehensive utilities for number input handling, conversion between different numeral systems,
    /// and input validation for WinForms applications. Supports Arabic, Persian, Hindi, and English numerals.
    /// Optimized for .NET Framework 4.8.
    /// </summary>
    public static class NumberInputUtils
    {
        #region Character Mappings

        /// <summary>
        /// Arabic-Indic numerals (٠-٩) commonly used in Arabic text
        /// </summary>
        private static readonly char[] ArabicNumerals = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };

        /// <summary>
        /// Persian/Farsi numerals (۰-۹) used in Persian and Urdu
        /// </summary>
        private static readonly char[] PersianNumerals = { '۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹' };

        /// <summary>
        /// Hindi/Devanagari numerals (०-९) used in Hindi and other Indian languages
        /// </summary>
        private static readonly char[] HindiNumerals = { '०', '१', '२', '३', '४', '५', '६', '७', '८', '९' };

        /// <summary>
        /// English/Latin numerals (0-9) - standard ASCII digits
        /// </summary>
        private static readonly char[] EnglishNumerals = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// Common decimal separators used across different cultures
        /// </summary>
        private static readonly char[] DecimalSeparators = { '.', ',', '٫', '⸲' };

        /// <summary>
        /// Comprehensive mapping of all supported numeral systems to English numerals
        /// </summary>
        private static readonly Dictionary<char, char> NumeralMappings = new Dictionary<char, char>();

        #endregion

        #region Static Constructor

        /// <summary>
        /// Static constructor to initialize numeral mappings
        /// </summary>
        static NumberInputUtils()
        {
            InitializeNumeralMappings();
        }

        /// <summary>
        /// Initializes the comprehensive numeral mapping dictionary
        /// </summary>
        private static void InitializeNumeralMappings()
        {
            // Map Arabic numerals to English
            for (int i = 0; i < ArabicNumerals.Length; i++)
            {
                NumeralMappings[ArabicNumerals[i]] = EnglishNumerals[i];
            }

            // Map Persian numerals to English
            for (int i = 0; i < PersianNumerals.Length; i++)
            {
                NumeralMappings[PersianNumerals[i]] = EnglishNumerals[i];
            }

            // Map Hindi numerals to English
            for (int i = 0; i < HindiNumerals.Length; i++)
            {
                NumeralMappings[HindiNumerals[i]] = EnglishNumerals[i];
            }

            // Map decimal separators to standard period
            foreach (var separator in DecimalSeparators.Skip(1)) // Skip the first one (period)
            {
                NumeralMappings[separator] = '.';
            }
        }

        #endregion

        #region KeyPress Event Handlers

        /// <summary>
        /// Converts Arabic numerals to English numerals in KeyPress events.
        /// Enhanced version of the original method with null checking and broader support.
        /// </summary>
        /// <param name="e">KeyPressEventArgs from the KeyPress event</param>
        /// <exception cref="ArgumentNullException">Thrown when e is null</exception>
        public static void ToEnglishNumber(KeyPressEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "KeyPressEventArgs cannot be null");

            // Convert Arabic numerals and decimal point to English equivalents
            char[] arabicNum = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩', '.' };
            char[] englishNum = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

            for (int i = 0; i < arabicNum.Length; i++)
            {
                if (e.KeyChar == arabicNum[i])
                {
                    e.KeyChar = englishNum[i];
                    break;
                }
            }
        }

        /// <summary>
        /// Converts any supported numeral system (Arabic, Persian, Hindi) to English numerals in KeyPress events.
        /// </summary>
        /// <param name="e">KeyPressEventArgs from the KeyPress event</param>
        /// <exception cref="ArgumentNullException">Thrown when e is null</exception>
        public static void ConvertToEnglishNumerals(KeyPressEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "KeyPressEventArgs cannot be null");

            if (NumeralMappings.TryGetValue(e.KeyChar, out char englishChar))
            {
                e.KeyChar = englishChar;
            }
        }

        /// <summary>
        /// Restricts input to numeric characters only, converting non-English numerals to English.
        /// </summary>
        /// <param name="e">KeyPressEventArgs from the KeyPress event</param>
        /// <param name="allowDecimal">Whether to allow decimal point input (default: true)</param>
        /// <param name="allowNegative">Whether to allow negative sign input (default: false)</param>
        public static void RestrictToNumericInput(KeyPressEventArgs e, bool allowDecimal = true, bool allowNegative = false)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), "KeyPressEventArgs cannot be null");

            // Allow control characters (backspace, delete, etc.)
            if (char.IsControl(e.KeyChar))
                return;

            // Convert non-English numerals to English
            ConvertToEnglishNumerals(e);

            // Check if the character is a valid numeric input
            bool isValidInput = char.IsDigit(e.KeyChar) ||
                               (allowDecimal && e.KeyChar == '.') ||
                               (allowNegative && e.KeyChar == '-');

            if (!isValidInput)
            {
                e.Handled = true; // Block the input
            }
        }

        /// <summary>
        /// Advanced numeric input handler with culture-specific validation and formatting.
        /// </summary>
        /// <param name="e">KeyPressEventArgs from the KeyPress event</param>
        /// <param name="textBox">The TextBox control to validate against</param>
        /// <param name="allowDecimal">Whether to allow decimal input</param>
        /// <param name="allowNegative">Whether to allow negative input</param>
        /// <param name="maxDecimalPlaces">Maximum number of decimal places allowed (default: 2)</param>
        /// <param name="culture">Culture info for number formatting (default: current culture)</param>
        public static void HandleAdvancedNumericInput(KeyPressEventArgs e, TextBox textBox,
            bool allowDecimal = true, bool allowNegative = false, int maxDecimalPlaces = 2, CultureInfo culture = null)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            culture = culture ?? CultureInfo.CurrentCulture;

            // Allow control characters
            if (char.IsControl(e.KeyChar))
                return;

            // Convert to English numerals
            ConvertToEnglishNumerals(e);

            var currentText = textBox.Text;
            var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
            var negativeSeparator = culture.NumberFormat.NegativeSign;

            // Handle negative sign
            if (allowNegative && (e.KeyChar == '-' || e.KeyChar.ToString() == negativeSeparator))
            {
                // Allow negative sign only at the beginning and if not already present
                if (currentText.Contains(negativeSeparator) || currentText.Contains("-") ||
                    (textBox.SelectionStart != 0 && textBox.SelectionLength == 0))
                {
                    e.Handled = true;
                    return;
                }

                // Convert to culture-specific negative sign
                e.KeyChar = negativeSeparator[0];
                return;
            }

            // Handle decimal point
            if (allowDecimal && (e.KeyChar == '.' || e.KeyChar.ToString() == decimalSeparator))
            {
                // Allow only one decimal point
                if (currentText.Contains(decimalSeparator))
                {
                    e.Handled = true;
                    return;
                }

                // Convert to culture-specific decimal separator
                e.KeyChar = decimalSeparator[0];
                return;
            }

            // Handle digits
            if (char.IsDigit(e.KeyChar))
            {
                // Check decimal places limit
                if (allowDecimal && currentText.Contains(decimalSeparator))
                {
                    var decimalIndex = currentText.IndexOf(decimalSeparator);
                    var decimalPlaces = currentText.Length - decimalIndex - 1;

                    if (decimalPlaces >= maxDecimalPlaces && textBox.SelectionStart > decimalIndex)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                return;
            }

            // Block all other characters
            e.Handled = true;
        }

        #endregion

        #region String Conversion Methods

        /// <summary>
        /// Converts a string containing mixed numeral systems to English numerals.
        /// </summary>
        /// <param name="input">Input string with mixed numerals</param>
        /// <returns>String with all numerals converted to English</returns>
        public static string ConvertToEnglishNumerals(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var result = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                result.Append(NumeralMappings.TryGetValue(c, out char englishChar) ? englishChar : c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts English numerals in a string to the specified numeral system.
        /// </summary>
        /// <param name="input">Input string with English numerals</param>
        /// <param name="targetSystem">Target numeral system</param>
        /// <returns>String with numerals converted to target system</returns>
        public static string ConvertFromEnglishNumerals(string input, NumeralSystem targetSystem)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] targetNumerals = GetNumeralArray(targetSystem);
            var result = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    int digit = c - '0';
                    result.Append(targetNumerals[digit]);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Normalizes a numeric string by converting all numerals to English and cleaning formatting.
        /// </summary>
        /// <param name="input">Input string to normalize</param>
        /// <param name="preserveDecimal">Whether to preserve decimal points</param>
        /// <returns>Normalized numeric string</returns>
        public static string NormalizeNumericString(string input, bool preserveDecimal = true)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Convert to English numerals
            string normalized = ConvertToEnglishNumerals(input);

            // Remove all non-numeric characters except decimal point and minus sign
            var pattern = preserveDecimal ? @"[^\d\.-]" : @"[^\d-]";
            normalized = Regex.Replace(normalized, pattern, "");

            // Ensure only one decimal point
            if (preserveDecimal)
            {
                var parts = normalized.Split('.');
                if (parts.Length > 2)
                {
                    normalized = parts[0] + "." + string.Join("", parts.Skip(1));
                }
            }

            return normalized;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates if a string contains only valid numeric characters from any supported numeral system.
        /// </summary>
        /// <param name="input">String to validate</param>
        /// <param name="allowDecimal">Whether decimal points are allowed</param>
        /// <param name="allowNegative">Whether negative signs are allowed</param>
        /// <returns>True if string contains only valid numeric characters</returns>
        public static bool IsValidNumericString(string input, bool allowDecimal = true, bool allowNegative = false)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            bool hasDecimal = false;
            bool hasNegative = false;

            foreach (char c in input)
            {
                // Check if it's a supported numeral
                if (IsNumericCharacter(c))
                    continue;

                // Check decimal point
                if (allowDecimal && DecimalSeparators.Contains(c))
                {
                    if (hasDecimal) return false; // Multiple decimal points
                    hasDecimal = true;
                    continue;
                }

                // Check negative sign
                if (allowNegative && c == '-')
                {
                    if (hasNegative || input.IndexOf(c) != 0) return false; // Multiple negatives or not at start
                    hasNegative = true;
                    continue;
                }

                return false; // Invalid character
            }

            return true;
        }

        /// <summary>
        /// Checks if a character is a numeral in any supported numeral system.
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if character is a numeral in any supported system</returns>
        public static bool IsNumericCharacter(char c)
        {
            return char.IsDigit(c) || // English digits
                   ArabicNumerals.Contains(c) ||
                   PersianNumerals.Contains(c) ||
                   HindiNumerals.Contains(c);
        }

        /// <summary>
        /// Detects the primary numeral system used in a string.
        /// </summary>
        /// <param name="input">Input string to analyze</param>
        /// <returns>Detected numeral system</returns>
        public static NumeralSystem DetectNumeralSystem(string input)
        {
            if (string.IsNullOrEmpty(input))
                return NumeralSystem.English;

            int arabicCount = input.Count(c => ArabicNumerals.Contains(c));
            int persianCount = input.Count(c => PersianNumerals.Contains(c));
            int hindiCount = input.Count(c => HindiNumerals.Contains(c));
            int englishCount = input.Count(char.IsDigit);

            var counts = new Dictionary<NumeralSystem, int>
            {
                { NumeralSystem.Arabic, arabicCount },
                { NumeralSystem.Persian, persianCount },
                { NumeralSystem.Hindi, hindiCount },
                { NumeralSystem.English, englishCount }
            };

            return counts.OrderByDescending(x => x.Value).First().Key;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the numeral array for the specified numeral system.
        /// </summary>
        /// <param name="system">Numeral system</param>
        /// <returns>Array of numerals for the system</returns>
        private static char[] GetNumeralArray(NumeralSystem system)
        {
            switch (system)
            {
                case NumeralSystem.Arabic: return ArabicNumerals;
                case NumeralSystem.Persian: return PersianNumerals;
                case NumeralSystem.Hindi: return HindiNumerals;
                case NumeralSystem.English: return EnglishNumerals;
                default: throw new ArgumentException($"Unsupported numeral system: {system}");
            }
        }

        /// <summary>
        /// Configures a TextBox for optimal numeric input handling.
        /// </summary>
        /// <param name="textBox">TextBox to configure</param>
        /// <param name="allowDecimal">Whether to allow decimal input</param>
        /// <param name="allowNegative">Whether to allow negative input</param>
        /// <param name="maxDecimalPlaces">Maximum decimal places</param>
        public static void ConfigureNumericTextBox(TextBox textBox, bool allowDecimal = true,
            bool allowNegative = false, int maxDecimalPlaces = 2)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.KeyPress += (sender, e) =>
                HandleAdvancedNumericInput(e, textBox, allowDecimal, allowNegative, maxDecimalPlaces);
        }

        /// <summary>
        /// Formats a numeric string according to the specified culture and format.
        /// </summary>
        /// <param name="numericString">Numeric string to format</param>
        /// <param name="format">Number format string (e.g., "N2", "C", "P")</param>
        /// <param name="culture">Culture for formatting</param>
        /// <returns>Formatted string</returns>
        public static string FormatNumericString(string numericString, string format = "N2", CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(numericString))
                return string.Empty;

            culture = culture ?? CultureInfo.CurrentCulture;
            string normalized = NormalizeNumericString(numericString);

            if (double.TryParse(normalized, out double value))
            {
                return value.ToString(format, culture);
            }

            return numericString; // Return original if parsing fails
        }

        #endregion
    }

    /// <summary>
    /// Enumeration of supported numeral systems
    /// </summary>
    public enum NumeralSystem
    {
        /// <summary>English/Latin numerals (0-9)</summary>
        English,
        /// <summary>Arabic-Indic numerals (٠-٩)</summary>
        Arabic,
        /// <summary>Persian/Farsi numerals (۰-۹)</summary>
        Persian,
        /// <summary>Hindi/Devanagari numerals (०-९)</summary>
        Hindi
    }
}