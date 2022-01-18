using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectNameApi.Extensions
{
    public static class StringExt
    {
        public static string SanitizeSql(this string value, bool withLike = false)
        {
            if (withLike)
                value = $"%{value}%";

            if (value.Contains("$pgval"))
                return $"$pgtag${value}$pgtag$";

            return $"$pgval${value}$pgval$";
        }

        /// <summary>
        ///     Gets substring from the last occurrence of "text" to the end of string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchedText">String to get the last occurrence of</param>
        /// <param name="includeText">Whether to include searchedText in the output string</param>
        /// <returns></returns>
        public static string SubstringAfterLast(this string value, string searchedText, bool includeText = false)
        {
            var startIndex = value.LastIndexOf(searchedText, StringComparison.Ordinal);

            if (!includeText)
                startIndex += searchedText.Length;

            return value.Substring(startIndex, value.Length - startIndex);
        }

        /// <summary>
        ///     Gets substring from the first occurrence of "text" to the end of string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="searchedText">String to get the last occurrence of</param>
        /// <param name="includeText">Whether to include searchedText in the output string</param>
        /// <returns></returns>
        public static string SubstringAfter(this string value, string searchedText, bool includeText = false)
        {
            var startIndex = value.IndexOf(searchedText, StringComparison.Ordinal);

            if (!includeText)
                startIndex += searchedText.Length;

            return value.Substring(startIndex, value.Length - startIndex);
        }

        /// <summary>
        ///     Converts double tabs, newlines and whitespaces to a single space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CleanWhitespace(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string ToTitleCase(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string ToSnakeCase(this string value)
        {
            var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            return string.Join("_", pattern.Matches(value)).ToLower();
        }

        public static string ToBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string FromBase64(this string value)
        {
            var valueBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
        
        public static Guid ToGuid(this string input)
        {
            Guid.TryParse(input, out var value);

            return value;
        }

        /// <summary>
        /// Gets a substring of length equal to string length or maxChars (whichever is larger) 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxChars"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string MaxSubstring(this string input, int maxChars, int startIndex = 0)
        {
            return input.Substring(startIndex, maxChars > input.Length ? input.Length : maxChars);
        }
        
        public static string RemoveDiacritics(this string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            
            foreach (var c in normalizedString.EnumerateRunes())
            {
                var unicodeCategory = Rune.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            result = result.Replace("œ", "oe");

            return result;
        }


        /// <summary>
        /// Splits string into a list of maxCount elements, discarding all elements after that
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="maxCount">Maximum number of list elements returned</param>
        /// <param name="delimiters">Delimiters characters to split by</param>
        /// <param name="maxChars">Maximum number of characters per list element</param>
        /// <returns></returns>
        public static List<string> SplitIntoList(this string value, int maxCount, string delimiters, int maxChars)
        {
            // If the string is less than maxChars it doesn't need to be split
            if (value.Length < maxChars)
                return new List<string> { value };

            var splitString = value.SplitIntoBlocks(delimiters.ToCharArray(), maxChars);

            if (splitString.Count > maxCount)
                return splitString.GetRange(0, maxCount);

            return splitString;
        }
        
        /// <summary>
        /// Splits a string into a list of substrings of maxBlockLength, favoring splitting over delimiters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="delimiters">List of delimiters to split over</param>
        /// <param name="maxBlockLength">Maximum string length</param>
        /// <returns></returns>
        public static List<string> SplitIntoBlocks(this string input, char[] delimiters, int maxBlockLength)
        {
            if (input.Length <= maxBlockLength)
                return new List<string> { input };

            var splitIndex = input.Substring(0, maxBlockLength).LastIndexOfAny(delimiters);
            if (splitIndex < 1)
                splitIndex = maxBlockLength;

            var result = new List<string>
            {
                input.Substring(0, splitIndex).Trim()
            };
        
            result.AddRange(SplitIntoBlocks(input.Substring(splitIndex, input.Length - splitIndex).Trim(), delimiters, maxBlockLength));

            return result;
        }
        
        /// <summary>
        /// Removes every character that is not in the validCharacters string
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="validCharacters">Valid characters (eg. "123+.,abc") </param>
        /// <returns></returns>
        public static string ToLimitedString(this string text,
            string validCharacters)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var result = new string(text
                .Where(validCharacters.Contains)
                .ToArray());

            return result;
        }
    }
}