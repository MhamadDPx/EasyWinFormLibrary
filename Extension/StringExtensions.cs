using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EasyWinFormLibrary.Extension
{
    /// <summary>
    /// Provides extension methods for string manipulation, conversion, and validation.
    /// Includes methods for type conversion, formatting, hashing, and text analysis.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to a double value with a fallback default.
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="defaultValue">The default value to return if conversion fails (default: 0)</param>
        /// <returns>The converted double value or the default value if conversion fails</returns>
        public static double TextToDouble(this string value, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            double.TryParse(value, out double result);
            return result;
        }

        /// <summary>
        /// Converts a string to an integer value with a fallback default.
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="defaultValue">The default value to return if conversion fails (default: 0)</param>
        /// <returns>The converted integer value or the default value if conversion fails</returns>
        public static int TextToInt(this string value, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            int.TryParse(value, out int result);
            return result;
        }

        /// <summary>
        /// Converts a string to a formatted double string with specified decimal places.
        /// </summary>
        /// <param name="value">The string to convert and format</param>
        /// <param name="roundNumber">Number of decimal places (uses LibrarySetting.NumberDefaultRound if null)</param>
        /// <returns>A formatted number string with thousand separators, or "0" if conversion fails</returns>
        public static string TextToDoubleFormat(this string value, int? roundNumber = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";
            if (!double.TryParse(value, out double result))
                return "0";
            int actualRoundNumber = roundNumber ?? LibrarySetting.NumberDefaultRound;
            return result.ToString($"n{actualRoundNumber}");
        }

        /// <summary>
        /// Converts a string to a formatted integer string with thousand separators.
        /// </summary>
        /// <param name="value">The string to convert and format</param>
        /// <returns>A formatted integer string with thousand separators, or "0" if conversion fails</returns>
        public static string TextToIntFormat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "0";
            if (!double.TryParse(value, out double result))
                return "0";
            return result.ToString($"n0");
        }

        /// <summary>
        /// Converts a string to a standardized date format (yyyy-MM-dd) using multiple culture parsing attempts.
        /// Tries invariant culture, British, German, and current system culture for maximum compatibility.
        /// </summary>
        /// <param name="value">The string to convert to date</param>
        /// <param name="defaultValue">The default value to return if conversion fails (default: empty string)</param>
        /// <returns>A date string in yyyy-MM-dd format, or the default value if conversion fails</returns>
        public static string TextToDate(this string value, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            // Try multiple cultures for better parsing
            var cultures = new[]
            {
                CultureInfo.InvariantCulture,    // MM-dd-yyyy
                CultureInfo.GetCultureInfo("en-GB"), // dd-MM-yyyy  
                CultureInfo.GetCultureInfo("de-DE"),  // dd.MM.yyyy
                CultureInfo.CurrentCulture        // User's system culture
            };
            foreach (var culture in cultures)
            {
                if (DateTime.TryParse(value, culture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    if (parsedDate.Year > 1753 && parsedDate.Year <= 9999)
                    {
                        return parsedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Generates a SHA256 hash of the input string.
        /// </summary>
        /// <param name="value">The string to hash</param>
        /// <returns>A hexadecimal SHA256 hash string in uppercase, or empty string if input is null/empty</returns>
        public static string TextToHash(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Converts a string to a boolean value with a fallback default.
        /// </summary>
        /// <param name="value">The string to convert</param>
        /// <param name="defaultValue">The default value to return if conversion fails (default: false)</param>
        /// <returns>The converted boolean value or the default value if conversion fails</returns>
        public static bool TextToBool(this string value, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Checks if a string is null, empty, or contains only whitespace characters.
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <returns>True if the string is null, empty, or whitespace; otherwise false</returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Determines if a string contains only English (ASCII) characters.
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <returns>True if all characters are within ASCII range (0-127); otherwise false</returns>
        public static bool IsEnglish(this string text)
        {
            return text.All(c => c <= '\u007F'); // ASCII range for English
        }
    }
}