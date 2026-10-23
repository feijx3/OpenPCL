using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MeloongCore.Extensions
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithF([Nullable(2)] this string value, string prefix, bool ignoreCase = false)
        {
            return value != null && value.StartsWith(prefix, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [NullableContext(2)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StartsWithF(this string value, char prefix, bool ignoreCase = false)
        {
            return value != null && value.StartsWith(prefix.ToString(), ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithF([Nullable(2)] this string value, string suffix, bool ignoreCase = false)
        {
            return value != null && value.EndsWith(suffix, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [NullableContext(2)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EndsWithF(this string value, char suffix, bool ignoreCase = false)
        {
            return value != null && value.EndsWith(suffix.ToString(), ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsIgnoreCase(this string value, string subString)
        {
            return value.IndexOf(subString, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfF(this string value, string subString, bool ignoreCase = false)
        {
            return value.IndexOf(subString, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfF(this string value, string subString, int startIndex, bool ignoreCase = false)
        {
            return value.IndexOf(subString, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOfF(this string value, string subString, bool ignoreCase = false)
        {
            return value.LastIndexOf(subString, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndexOfF(this string value, string subString, int startIndex, bool ignoreCase = false)
        {
            return value.LastIndexOf(subString, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Lower(this string str)
        {
            return str.ToLowerInvariant();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Upper(this string str)
        {
            return str.ToUpperInvariant();
        }

        public static string[] Split(this string fullStr, string splitStr, bool removeEmptyEntries = false)
        {
            if (splitStr.IsSingle<char>())
            {
                return fullStr.Split(splitStr[0], removeEmptyEntries);
            }
            return fullStr.Split(new string[] { splitStr }, removeEmptyEntries);
        }

        public static string[] Split(this string fullStr, char splitChar, bool removeEmptyEntries = false)
        {
            return fullStr.Split(new char[] { splitChar }, removeEmptyEntries);
        }

        public static string[] Split(this string fullStr, string[] splitStrings, bool removeEmptyEntries = false)
        {
            return fullStr.Split(splitStrings, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static string[] Split(this string fullStr, char[] splitChars, bool removeEmptyEntries = false)
        {
            return fullStr.Split(splitChars, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static string[] SplitLines(this string input, bool skipEmptyLines = false)
        {
            return input.ReplaceLineEndings("\n", false).Split('\n', skipEmptyLines);
        }

        public static string BeforeFirst(this string str, string text, bool ignoreCase = false)
        {
            int num = (string.IsNullOrEmpty(text) ? (-1) : str.IndexOfF(text, ignoreCase));
            if (num < 0)
            {
                return str;
            }
            return str.Substring(0, num);
        }

        public static string BeforeFirstOfAny(this string str, [Nullable(new byte[] { 2, 1 })] IEnumerable<string> texts, bool ignoreCase = false)
        {
            if (texts == null)
            {
                return str;
            }
            int num = -1;
            foreach (string text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    int num2 = str.IndexOfF(text, ignoreCase);
                    if (num2 >= 0 && (num < 0 || num > num2))
                    {
                        num = num2;
                    }
                }
            }
            if (num < 0)
            {
                return str;
            }
            return str.Substring(0, num);
        }

        public static string BeforeLast(this string str, string text, bool ignoreCase = false)
        {
            int num = (string.IsNullOrEmpty(text) ? (-1) : str.LastIndexOfF(text, ignoreCase));
            if (num < 0)
            {
                return str;
            }
            return str.Substring(0, num);
        }

        public static string BeforeLastOfAny(this string str, [Nullable(new byte[] { 2, 1 })] IEnumerable<string> texts, bool ignoreCase = false)
        {
            if (texts == null)
            {
                return str;
            }
            int num = -1;
            foreach (string text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    int num2 = str.LastIndexOfF(text, ignoreCase);
                    if (num2 >= 0 && (num < 0 || num < num2))
                    {
                        num = num2;
                    }
                }
            }
            if (num < 0)
            {
                return str;
            }
            return str.Substring(0, num);
        }

        public static string AfterFirst(this string str, [Nullable(2)] string text, bool ignoreCase = false)
        {
            int num = (string.IsNullOrEmpty(text) ? (-1) : str.IndexOfF(text, ignoreCase));
            if (num < 0)
            {
                return str;
            }
            return str.Substring(num + text.Length);
        }

        public static string AfterFirstOfAny(this string str, [Nullable(new byte[] { 2, 1 })] IEnumerable<string> texts, bool ignoreCase = false)
        {
            if (texts == null)
            {
                return str;
            }
            int num = -1;
            int num2 = 0;
            foreach (string text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    int num3 = str.IndexOfF(text, ignoreCase);
                    if (num3 >= 0 && (num < 0 || num > num3))
                    {
                        num = num3;
                        num2 = text.Length;
                    }
                }
            }
            if (num < 0)
            {
                return str;
            }
            return str.Substring(num + num2);
        }

        public static string AfterLast(this string str, [Nullable(2)] string text, bool ignoreCase = false)
        {
            int num = (string.IsNullOrEmpty(text) ? (-1) : str.LastIndexOfF(text, ignoreCase));
            if (num < 0)
            {
                return str;
            }
            return str.Substring(num + text.Length);
        }

        public static string AfterLastOfAny(this string str, [Nullable(new byte[] { 2, 1 })] IEnumerable<string> texts, bool ignoreCase = false)
        {
            if (texts == null)
            {
                return str;
            }
            int num = -1;
            int num2 = 0;
            foreach (string text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    int num3 = str.LastIndexOfF(text, ignoreCase);
                    if (num3 >= 0 && (num < 0 || num < num3))
                    {
                        num = num3;
                        num2 = text.Length;
                    }
                }
            }
            if (num < 0)
            {
                return str;
            }
            return str.Substring(num + num2);
        }

        public static string Between(this string str, string after, string before, bool ignoreCase = false)
        {
            int num = (string.IsNullOrEmpty(after) ? (-1) : str.LastIndexOfF(after, ignoreCase));
            if (num >= 0)
            {
                num += after.Length;
            }
            else
            {
                num = 0;
            }
            int num2 = (string.IsNullOrEmpty(before) ? (-1) : str.IndexOfF(before, num, ignoreCase));
            if (num2 >= 0)
            {
                int num3 = num;
                return str.Substring(num3, num2 - num3);
            }
            if (num > 0)
            {
                return str.Substring(num);
            }
            return str;
        }

        public static string Replace(this string str, string oldValue, Func<string> newValueGetter)
        {
            if (!str.Contains(oldValue))
            {
                return str;
            }
            return str.Replace(oldValue, newValueGetter());
        }

        public static string ReplaceLineEndings(this string input, string newValue, bool mergeMultiple = false)
        {
            return (mergeMultiple ? StringExtensions.regexLineEndingAndMerge : StringExtensions.regexLineEnding).Replace(input, newValue.Replace("$", "$$"));
        }

        public static IEnumerable<string> RegexSearch(this string str, string pattern, RegexOptions options = RegexOptions.None)
        {
            IEnumerable<string> enumerable;
            try
            {
                enumerable = from Match m in Regex.Matches(str, pattern, options)
                    select m.Value;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "RegexSearch 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Extensions\\StringExtensions.cs");
                enumerable = Array.Empty<string>();
            }
            return enumerable;
        }

        [return: Nullable(2)]
        public static string RegexSeek(this string str, string pattern, RegexOptions options = RegexOptions.None)
        {
            string text;
            try
            {
                Match match = Regex.Match(str, pattern, options);
                text = (match.Success ? match.Value : null);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "RegexSeek 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Extensions\\StringExtensions.cs");
                text = null;
            }
            return text;
        }

        public static bool RegexCheck(this string str, string pattern, RegexOptions options = RegexOptions.None)
        {
            bool flag;
            try
            {
                flag = Regex.IsMatch(str, pattern, options);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "RegexCheck 失败（" + pattern + "）", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\MeloongCore\\Shared\\Extensions\\StringExtensions.cs");
                flag = false;
            }
            return flag;
        }

        public static string RegexReplace(this string allContents, string searchRegex, string replaceTo, RegexOptions options = RegexOptions.None)
        {
            return Regex.Replace(allContents, searchRegex, replaceTo, options);
        }

        public static string RegexReplace(this string allContents, string searchRegex, MatchEvaluator replaceTo, RegexOptions options = RegexOptions.None)
        {
            return Regex.Replace(allContents, searchRegex, replaceTo, options);
        }

        public static string ReadString(this Stream stream, [Nullable(2)] Encoding encoding = null)
        {
            string @string;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                if (stream.CanSeek && stream.Position != 0L)
                {
                    stream.Seek(0L, SeekOrigin.Begin);
                }
                stream.CopyTo(memoryStream);
                @string = memoryStream.ToArray().GetString(encoding);
            }
            return @string;
        }

        public static string GetString(this byte[] bytes, [Nullable(2)] Encoding encoding = null)
        {
            if (encoding != null)
            {
                return encoding.GetString(bytes);
            }
            int num = bytes.Length;
            if (num >= 3 && bytes[0] == 239 && bytes[1] == 187 && bytes[2] == 191)
            {
                return Encoding.UTF8.GetString(bytes, 3, num - 3);
            }
            if (num >= 2 && bytes[0] == 254 && bytes[1] == 255)
            {
                return Encoding.BigEndianUnicode.GetString(bytes, 2, num - 2);
            }
            if (num >= 2 && bytes[0] == 255 && bytes[1] == 254)
            {
                return Encoding.Unicode.GetString(bytes, 2, num - 2);
            }
            string text;
            try
            {
                text = new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                text = Encoding.GetEncoding("GB18030").GetString(bytes);
            }
            return text;
        }

        public static string Capitalize(this string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }
            return word.Substring(0, 1).Upper() + word.Substring(1).Lower();
        }

        public static string EnsureLength([Nullable(2)] this string str, char code, int length)
        {
            if (str == null)
            {
                str = "";
            }
            if (str.Length <= length)
            {
                return str.PadLeft(length, code);
            }
            return str.Substring(0, length);
        }

        public static bool IsAsciiOnly(this string input)
        {
            return input.All<char>((char c) => c < '\u0080');
        }

        public static ulong GetStableHashCode(this string str)
        {
            ulong num = 5381UL;
            foreach (char c in str)
            {
                num = (num << 5) ^ num ^ (ulong)c;
            }
            return num ^ 12218072394304324399UL;
        }

        [return: Nullable(2)]
        public static JToken DeserializeJson(this string data)
        {
            JToken jtoken;
            try
            {
                jtoken = JsonConvert.DeserializeObject<JToken>(data, new JsonSerializerSettings
                {
                    DateTimeZoneHandling = 0
                });
            }
            catch (Exception ex)
            {
                int length = (data ?? "").Length;
                throw new FormatException("JSON deserialize failed: " + ((length > 2000) ? string.Format("{0}...(total {1} characters)...{2}", data.Substring(0, 500), length, data.Substring(data.Length - 500)) : (data ?? "")), ex);
            }
            return jtoken;
        }

        [return: Nullable(2)]
        public static T DeserializeJson<[Nullable(0)] T>(this string data) where T : JToken
        {
            return (T)((object)data.DeserializeJson());
        }

        private static readonly Regex regexLineEndingAndMerge = new Regex("(?:\\r\\n|[\\n\\r\\f\\u0085\\u2028\\u2029])+", RegexOptions.Compiled);

        private static readonly Regex regexLineEnding = new Regex("\\r\\n|[\\n\\r\\f\\u0085\\u2028\\u2029]", RegexOptions.Compiled);
    }
}
