using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Provides comprehensive barcode and unique identifier generation utilities with support for
    /// various barcode standards, validation, check digits, and cryptographically secure random generation.
    /// Optimized for .NET Framework 4.8.
    /// </summary>
    public static class BarcodeGenerator
    {
        #region Private Fields

        /// <summary>
        /// Thread-safe random number generator instance
        /// </summary>
        private static readonly ThreadLocal<Random> _threadLocalRandom =
            new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        /// <summary>
        /// Cryptographically secure random number generator
        /// </summary>
        private static readonly RNGCryptoServiceProvider _cryptoRandom = new RNGCryptoServiceProvider();

        /// <summary>
        /// Cache for generated barcodes to ensure uniqueness
        /// </summary>
        private static readonly HashSet<string> _generatedBarcodes = new HashSet<string>();

        /// <summary>
        /// Lock object for thread-safe operations on the cache
        /// </summary>
        private static readonly object _cacheLock = new object();

        #endregion

        #region Character Sets

        /// <summary>
        /// Alphanumeric characters (uppercase letters and digits)
        /// </summary>
        public static readonly string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Numeric characters only
        /// </summary>
        public static readonly string NumericChars = "0123456789";

        /// <summary>
        /// Alphabetic characters only (uppercase)
        /// </summary>
        public static readonly string AlphabeticChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Mixed case alphanumeric characters
        /// </summary>
        public static readonly string MixedAlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Hexadecimal characters
        /// </summary>
        public static readonly string HexadecimalChars = "0123456789ABCDEF";

        /// <summary>
        /// Base32 characters (Crockford's Base32)
        /// </summary>
        public static readonly string Base32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        #endregion

        #region Enhanced Original Method

        /// <summary>
        /// Generates a random barcode with the specified length using alphanumeric characters.
        /// Enhanced version of the original method with better thread safety and validation.
        /// </summary>
        /// <param name="length">Length of the barcode to generate</param>
        /// <returns>Random alphanumeric barcode string</returns>
        /// <exception cref="ArgumentException">Thrown when length is less than 1</exception>
        public static string GenerateRandomBarcode(int length)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));

            return GenerateRandomCode(length, AlphanumericChars, false);
        }

        #endregion

        #region Core Generation Methods

        /// <summary>
        /// Generates a random code with specified parameters and character set.
        /// </summary>
        /// <param name="length">Length of the code to generate</param>
        /// <param name="characterSet">Set of characters to use for generation</param>
        /// <param name="ensureUnique">Whether to ensure the generated code is unique</param>
        /// <param name="useCryptoRandom">Whether to use cryptographically secure random generation</param>
        /// <returns>Generated random code</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
        public static string GenerateRandomCode(int length, string characterSet, bool ensureUnique = false, bool useCryptoRandom = false)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));
            if (string.IsNullOrEmpty(characterSet))
                throw new ArgumentException("Character set cannot be null or empty", nameof(characterSet));

            const int maxAttempts = 1000;
            var attempts = 0;

            while (attempts < maxAttempts)
            {
                var code = useCryptoRandom
                    ? GenerateCodeCryptographically(length, characterSet)
                    : GenerateCodeStandard(length, characterSet);

                if (!ensureUnique || IsUnique(code))
                {
                    if (ensureUnique)
                    {
                        lock (_cacheLock)
                        {
                            _generatedBarcodes.Add(code);
                        }
                    }
                    return code;
                }

                attempts++;
            }

            throw new InvalidOperationException($"Could not generate unique code after {maxAttempts} attempts");
        }

        /// <summary>
        /// Generates a code using standard random number generation.
        /// </summary>
        private static string GenerateCodeStandard(int length, string characterSet)
        {
            var random = _threadLocalRandom.Value;
            var code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = characterSet[random.Next(characterSet.Length)];
            }

            return new string(code);
        }

        /// <summary>
        /// Generates a code using cryptographically secure random number generation.
        /// </summary>
        private static string GenerateCodeCryptographically(int length, string characterSet)
        {
            var code = new char[length];
            var bytes = new byte[4];

            for (int i = 0; i < length; i++)
            {
                _cryptoRandom.GetBytes(bytes);
                var randomValue = Math.Abs(BitConverter.ToInt32(bytes, 0));
                code[i] = characterSet[randomValue % characterSet.Length];
            }

            return new string(code);
        }

        #endregion

        #region Specialized Barcode Types

        /// <summary>
        /// Generates a numeric-only barcode (suitable for EAN, UPC, etc.).
        /// </summary>
        /// <param name="length">Length of the numeric barcode</param>
        /// <param name="includeCheckDigit">Whether to include a check digit</param>
        /// <returns>Numeric barcode string</returns>
        public static string GenerateNumericBarcode(int length, bool includeCheckDigit = false)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));

            var actualLength = includeCheckDigit ? length - 1 : length;
            var code = GenerateRandomCode(actualLength, NumericChars);

            return includeCheckDigit ? code + CalculateCheckDigit(code) : code;
        }

        /// <summary>
        /// Generates an EAN-13 barcode with proper check digit.
        /// </summary>
        /// <param name="countryCode">2-3 digit country code (optional)</param>
        /// <returns>Valid EAN-13 barcode</returns>
        public static string GenerateEAN13(string countryCode = null)
        {
            var code = new StringBuilder();

            // Add country code or generate random prefix
            if (!string.IsNullOrEmpty(countryCode) && countryCode.Length <= 3)
            {
                code.Append(countryCode.PadLeft(3, '0'));
            }
            else
            {
                code.Append(GenerateRandomCode(3, NumericChars));
            }

            // Add manufacturer and product codes
            code.Append(GenerateRandomCode(9, NumericChars));

            // Calculate and add check digit
            var checkDigit = CalculateEAN13CheckDigit(code.ToString());
            code.Append(checkDigit);

            return code.ToString();
        }

        /// <summary>
        /// Generates a UPC-A barcode with proper check digit.
        /// </summary>
        /// <param name="manufacturerCode">5-digit manufacturer code (optional)</param>
        /// <returns>Valid UPC-A barcode</returns>
        public static string GenerateUPCA(string manufacturerCode = null)
        {
            var code = new StringBuilder();

            // Add manufacturer code or generate random
            if (!string.IsNullOrEmpty(manufacturerCode) && manufacturerCode.Length == 5 && IsNumeric(manufacturerCode))
            {
                code.Append(manufacturerCode);
            }
            else
            {
                code.Append(GenerateRandomCode(5, NumericChars));
            }

            // Add product code
            code.Append(GenerateRandomCode(6, NumericChars));

            // Calculate and add check digit
            var checkDigit = CalculateUPCACheckDigit(code.ToString());
            code.Append(checkDigit);

            return code.ToString();
        }

        /// <summary>
        /// Generates a Code 128 compatible barcode.
        /// </summary>
        /// <param name="length">Length of the barcode</param>
        /// <param name="subset">Code 128 subset (A, B, or C)</param>
        /// <returns>Code 128 compatible barcode</returns>
        public static string GenerateCode128(int length, Code128Subset subset = Code128Subset.B)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));

            string characterSet;
            switch (subset)
            {
                case Code128Subset.A:
                    characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 !\"#$%&'()*+,-./:;<=>?@[\\]^_";
                    break;
                case Code128Subset.B:
                    characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                    break;
                case Code128Subset.C:
                    characterSet = NumericChars;
                    break;
                default:
                    characterSet = AlphanumericChars;
                    break;
            }

            return GenerateRandomCode(length, characterSet);
        }

        /// <summary>
        /// Generates a QR Code compatible data string.
        /// </summary>
        /// <param name="length">Length of the data string</param>
        /// <param name="dataType">Type of data to generate</param>
        /// <returns>QR Code compatible data string</returns>
        public static string GenerateQRCodeData(int length, QRCodeDataType dataType = QRCodeDataType.Alphanumeric)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));

            string characterSet;
            switch (dataType)
            {
                case QRCodeDataType.Numeric:
                    characterSet = NumericChars;
                    break;
                case QRCodeDataType.Alphanumeric:
                    characterSet = AlphanumericChars;
                    break;
                case QRCodeDataType.Binary:
                    characterSet = MixedAlphanumericChars + " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
                    break;
                default:
                    characterSet = AlphanumericChars;
                    break;
            }

            return GenerateRandomCode(length, characterSet);
        }

        #endregion

        #region Formatted Identifiers

        /// <summary>
        /// Generates a formatted product code with separators.
        /// </summary>
        /// <param name="pattern">Pattern for the code (e.g., "XXX-NNNN-XXX" where X=letter, N=number)</param>
        /// <param name="separator">Separator character (default: '-')</param>
        /// <returns>Formatted product code</returns>
        public static string GenerateFormattedCode(string pattern, char separator = '-')
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException("Pattern cannot be null or empty", nameof(pattern));

            var result = new StringBuilder();
            var random = _threadLocalRandom.Value;

            foreach (var character in pattern)
            {
                switch (character)
                {
                    case 'X': // Uppercase letter
                        result.Append(AlphabeticChars[random.Next(AlphabeticChars.Length)]);
                        break;
                    case 'x': // Lowercase letter
                        result.Append(AlphabeticChars[random.Next(AlphabeticChars.Length)].ToString().ToLower());
                        break;
                    case 'N': // Number
                        result.Append(NumericChars[random.Next(NumericChars.Length)]);
                        break;
                    case 'A': // Alphanumeric
                        result.Append(AlphanumericChars[random.Next(AlphanumericChars.Length)]);
                        break;
                    case 'H': // Hexadecimal
                        result.Append(HexadecimalChars[random.Next(HexadecimalChars.Length)]);
                        break;
                    default:
                        result.Append(character); // Keep separator or other characters as-is
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a GUID-based barcode with customizable format.
        /// </summary>
        /// <param name="format">GUID format (N, D, B, P, or X)</param>
        /// <param name="removeHyphens">Whether to remove hyphens from the result</param>
        /// <returns>GUID-based barcode</returns>
        public static string GenerateGuidBarcode(string format = "N", bool removeHyphens = true)
        {
            var guid = Guid.NewGuid().ToString(format).ToUpper();
            return removeHyphens ? guid.Replace("-", "").Replace("{", "").Replace("}", "") : guid;
        }

        /// <summary>
        /// Generates a timestamped barcode with random suffix.
        /// </summary>
        /// <param name="suffixLength">Length of the random suffix</param>
        /// <param name="includeMilliseconds">Whether to include milliseconds in timestamp</param>
        /// <param name="separator">Separator between timestamp and suffix</param>
        /// <returns>Timestamped barcode</returns>
        public static string GenerateTimestampedBarcode(int suffixLength = 4, bool includeMilliseconds = false, string separator = "")
        {
            if (suffixLength < 0)
                throw new ArgumentException("Suffix length cannot be negative", nameof(suffixLength));

            var timestamp = includeMilliseconds
                ? DateTime.Now.ToString("yyyyMMddHHmmssfff")
                : DateTime.Now.ToString("yyyyMMddHHmmss");

            var suffix = suffixLength > 0 ? GenerateRandomCode(suffixLength, AlphanumericChars) : "";

            return timestamp + separator + suffix;
        }

        #endregion

        #region Validation and Check Digits

        /// <summary>
        /// Calculates a simple check digit using modulo 10 algorithm.
        /// </summary>
        /// <param name="code">Numeric code to calculate check digit for</param>
        /// <returns>Check digit (0-9)</returns>
        public static int CalculateCheckDigit(string code)
        {
            if (string.IsNullOrEmpty(code) || !IsNumeric(code))
                throw new ArgumentException("Code must be numeric", nameof(code));

            var sum = 0;
            var alternate = false;

            // Process from right to left
            for (int i = code.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(code[i].ToString());

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit = (digit % 10) + 1;
                }

                sum += digit;
                alternate = !alternate;
            }

            return (10 - (sum % 10)) % 10;
        }

        /// <summary>
        /// Calculates EAN-13 check digit.
        /// </summary>
        /// <param name="code">12-digit EAN code</param>
        /// <returns>Check digit (0-9)</returns>
        public static int CalculateEAN13CheckDigit(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 12 || !IsNumeric(code))
                throw new ArgumentException("Code must be exactly 12 numeric digits", nameof(code));

            var sum = 0;
            for (int i = 0; i < 12; i++)
            {
                var digit = int.Parse(code[i].ToString());
                sum += digit * (i % 2 == 0 ? 1 : 3);
            }

            return (10 - (sum % 10)) % 10;
        }

        /// <summary>
        /// Calculates UPC-A check digit.
        /// </summary>
        /// <param name="code">11-digit UPC code</param>
        /// <returns>Check digit (0-9)</returns>
        public static int CalculateUPCACheckDigit(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 11 || !IsNumeric(code))
                throw new ArgumentException("Code must be exactly 11 numeric digits", nameof(code));

            var sum = 0;
            for (int i = 0; i < 11; i++)
            {
                var digit = int.Parse(code[i].ToString());
                sum += digit * (i % 2 == 0 ? 3 : 1);
            }

            return (10 - (sum % 10)) % 10;
        }

        /// <summary>
        /// Validates if a barcode format is correct for its type.
        /// </summary>
        /// <param name="barcode">Barcode to validate</param>
        /// <param name="barcodeType">Type of barcode to validate against</param>
        /// <returns>True if barcode format is valid</returns>
        public static bool ValidateBarcode(string barcode, BarcodeType barcodeType)
        {
            if (string.IsNullOrEmpty(barcode))
                return false;

            switch (barcodeType)
            {
                case BarcodeType.EAN13:
                    return ValidateEAN13(barcode);
                case BarcodeType.UPCA:
                    return ValidateUPCA(barcode);
                case BarcodeType.Numeric:
                    return IsNumeric(barcode);
                case BarcodeType.Alphanumeric:
                    return IsAlphanumeric(barcode);
                case BarcodeType.Code128:
                    return ValidateCode128(barcode);
                default:
                    return true; // Generic validation
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks if a string contains only numeric characters.
        /// </summary>
        private static bool IsNumeric(string value)
        {
            return !string.IsNullOrEmpty(value) && value.All(char.IsDigit);
        }

        /// <summary>
        /// Checks if a string contains only alphanumeric characters.
        /// </summary>
        private static bool IsAlphanumeric(string value)
        {
            return !string.IsNullOrEmpty(value) && value.All(c => char.IsLetterOrDigit(c));
        }

        /// <summary>
        /// Validates EAN-13 barcode format and check digit.
        /// </summary>
        private static bool ValidateEAN13(string barcode)
        {
            if (string.IsNullOrEmpty(barcode) || barcode.Length != 13 || !IsNumeric(barcode))
                return false;

            var code = barcode.Substring(0, 12);
            var checkDigit = int.Parse(barcode[12].ToString());

            return CalculateEAN13CheckDigit(code) == checkDigit;
        }

        /// <summary>
        /// Validates UPC-A barcode format and check digit.
        /// </summary>
        private static bool ValidateUPCA(string barcode)
        {
            if (string.IsNullOrEmpty(barcode) || barcode.Length != 12 || !IsNumeric(barcode))
                return false;

            var code = barcode.Substring(0, 11);
            var checkDigit = int.Parse(barcode[11].ToString());

            return CalculateUPCACheckDigit(code) == checkDigit;
        }

        /// <summary>
        /// Validates Code 128 barcode format.
        /// </summary>
        private static bool ValidateCode128(string barcode)
        {
            // Basic validation - Code 128 can contain most ASCII characters
            return !string.IsNullOrEmpty(barcode) && barcode.All(c => c >= 32 && c <= 126);
        }

        /// <summary>
        /// Checks if a code is unique in the generated cache.
        /// </summary>
        private static bool IsUnique(string code)
        {
            lock (_cacheLock)
            {
                return !_generatedBarcodes.Contains(code);
            }
        }

        /// <summary>
        /// Clears the uniqueness cache.
        /// </summary>
        public static void ClearUniquenessCache()
        {
            lock (_cacheLock)
            {
                _generatedBarcodes.Clear();
            }
        }

        /// <summary>
        /// Gets the count of generated unique barcodes in cache.
        /// </summary>
        /// <returns>Number of unique barcodes generated</returns>
        public static int GetGeneratedBarcodeCount()
        {
            lock (_cacheLock)
            {
                return _generatedBarcodes.Count;
            }
        }

        /// <summary>
        /// Removes a specific barcode from the uniqueness cache.
        /// </summary>
        /// <param name="barcode">Barcode to remove from cache</param>
        /// <returns>True if barcode was removed, false if not found</returns>
        public static bool RemoveFromCache(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return false;

            lock (_cacheLock)
            {
                return _generatedBarcodes.Remove(barcode);
            }
        }

        /// <summary>
        /// Disposes resources used by the barcode generator.
        /// </summary>
        public static void Dispose()
        {
            _cryptoRandom?.Dispose();
            _threadLocalRandom?.Dispose();
            ClearUniquenessCache();
        }

        #endregion

        #region Batch Generation

        /// <summary>
        /// Generates multiple unique barcodes in a single operation.
        /// </summary>
        /// <param name="count">Number of barcodes to generate</param>
        /// <param name="length">Length of each barcode</param>
        /// <param name="characterSet">Character set to use</param>
        /// <param name="useCryptoRandom">Whether to use cryptographically secure generation</param>
        /// <returns>List of unique barcodes</returns>
        public static List<string> GenerateBatch(int count, int length, string characterSet = null, bool useCryptoRandom = false)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0", nameof(count));
            if (length < 1)
                throw new ArgumentException("Length must be at least 1", nameof(length));

            characterSet = characterSet ?? AlphanumericChars;
            var barcodes = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                var barcode = GenerateRandomCode(length, characterSet, true, useCryptoRandom);
                barcodes.Add(barcode);
            }

            return barcodes;
        }

        #endregion
    }

    #region Enumerations

    /// <summary>
    /// Supported barcode types for validation
    /// </summary>
    public enum BarcodeType
    {
        /// <summary>Generic barcode</summary>
        Generic,
        /// <summary>EAN-13 barcode</summary>
        EAN13,
        /// <summary>UPC-A barcode</summary>
        UPCA,
        /// <summary>Numeric only barcode</summary>
        Numeric,
        /// <summary>Alphanumeric barcode</summary>
        Alphanumeric,
        /// <summary>Code 128 barcode</summary>
        Code128
    }

    /// <summary>
    /// Code 128 subsets
    /// </summary>
    public enum Code128Subset
    {
        /// <summary>Subset A - uppercase letters, numbers, and control characters</summary>
        A,
        /// <summary>Subset B - uppercase and lowercase letters, numbers, and printable characters</summary>
        B,
        /// <summary>Subset C - numeric pairs only</summary>
        C
    }

    /// <summary>
    /// QR Code data types
    /// </summary>
    public enum QRCodeDataType
    {
        /// <summary>Numeric data only</summary>
        Numeric,
        /// <summary>Alphanumeric data</summary>
        Alphanumeric,
        /// <summary>Binary data</summary>
        Binary
    }

    #endregion
}