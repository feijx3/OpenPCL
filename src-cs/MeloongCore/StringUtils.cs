using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using MeloongCore.Extensions;

namespace MeloongCore
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class StringUtils
    {
        public static string XmlEscape(string str)
        {
            if (str.StartsWithF("{", false))
            {
                str = "{}" + str;
            }
            return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;")
                .ReplaceLineEndings("&#xa;", false);
        }

        public static string LikePatternEscape(string input)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            while (i < input.Length)
            {
                char c = input[i];
                if (c <= '*')
                {
                    if (c != '#' && c != '*')
                    {
                        goto IL_0039;
                    }
                    goto IL_0034;
                }
                else
                {
                    if (c == '?' || c == '[' || c == ']')
                    {
                        goto IL_0034;
                    }
                    goto IL_0039;
                }
                IL_003C:
                bool flag;
                if (flag)
                {
                    stringBuilder.Append('[').Append(c).Append(']');
                }
                else
                {
                    stringBuilder.Append(c);
                }
                i++;
                continue;
                IL_0034:
                flag = true;
                goto IL_003C;
                IL_0039:
                flag = false;
                goto IL_003C;
            }
            return stringBuilder.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RegexEscape(string value)
        {
            return Regex.Escape(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RegexUnescape(string value)
        {
            return Regex.Unescape(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UrlEscape(string value)
        {
            return Uri.EscapeDataString(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UrlUnescape(string value)
        {
            return Uri.UnescapeDataString(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormUrlEscape(string value)
        {
            return WebUtility.UrlEncode(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormUrlUnescape(string value)
        {
            return WebUtility.UrlDecode(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string HtmlEscape(string value)
        {
            return WebUtility.HtmlEncode(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string HtmlUnescape(string value)
        {
            return WebUtility.HtmlDecode(value);
        }

        public static string FormatByteSize(long byteSize)
        {
            string text = ((byteSize < 0L) ? "-" : "");
            if (byteSize < 0L)
            {
                byteSize *= -1L;
            }
            if (byteSize < 1000L)
            {
                return string.Format("{0}{1} B", text, byteSize);
            }
            if (byteSize < 1024000L)
            {
                string text2 = Math.Round((double)byteSize / 1024.0).ToString(CultureInfo.InvariantCulture);
                return string.Format("{0}{1} K", text, Math.Round((double)byteSize / 1024.0, (3 - text2.Length).Clamp(0, 2)));
            }
            if (byteSize < 1048576000L)
            {
                string text3 = Math.Round((double)byteSize / 1024.0 / 1024.0).ToString(CultureInfo.InvariantCulture);
                return string.Format("{0}{1} M", text, Math.Round((double)byteSize / 1024.0 / 1024.0, (3 - text3.Length).Clamp(0, 2)));
            }
            string text4 = Math.Round((double)byteSize / 1024.0 / 1024.0 / 1024.0).ToString(CultureInfo.InvariantCulture);
            return string.Format("{0}{1} G", text, Math.Round((double)byteSize / 1024.0 / 1024.0 / 1024.0, (3 - text4.Length).Clamp(0, 2)));
        }

        public static string FormatTimeSpan(TimeSpan span, bool isShort)
        {
            string text = ((span.TotalMilliseconds > 0.0) ? "后" : "前");
            if (span.TotalMilliseconds < 0.0)
            {
                span = -span;
            }
            int num = (int)Math.Floor((double)span.Days / 30.0);
            if (isShort)
            {
                if (num >= 12)
                {
                    return string.Format("{0} 年{1}", Math.Floor((double)num / 12.0), text);
                }
                if (num >= 2)
                {
                    return string.Format("{0} 个月{1}", num, text);
                }
                if (span.TotalDays >= 2.0)
                {
                    return string.Format("{0} 天{1}", span.Days, text);
                }
                if (span.TotalHours >= 1.0)
                {
                    return string.Format("{0} 小时{1}", span.Hours, text);
                }
                if (span.TotalMinutes >= 1.0)
                {
                    return string.Format("{0} 分钟{1}", span.Minutes, text);
                }
                if (span.TotalSeconds >= 1.0)
                {
                    return string.Format("{0} 秒{1}", span.Seconds, text);
                }
                return "1 秒" + text;
            }
            else
            {
                if (num >= 61)
                {
                    return string.Format("{0} 年{1}", Math.Floor((double)num / 12.0), text);
                }
                if (num >= 12)
                {
                    return string.Format("{0} 年{1}{2}", Math.Floor((double)num / 12.0), (num % 12 > 0) ? string.Format(" {0} 个月", num % 12) : "", text);
                }
                if (num >= 4)
                {
                    return string.Format("{0} 个月{1}", num, text);
                }
                if (num >= 1)
                {
                    return string.Format("{0} 月{1}{2}", num, (span.Days % 30 > 0) ? string.Format(" {0} 天", span.Days % 30) : "", text);
                }
                if (span.TotalDays >= 4.0)
                {
                    return string.Format("{0} 天{1}", span.Days, text);
                }
                if (span.TotalDays >= 1.0)
                {
                    return string.Format("{0} 天{1}{2}", span.Days, (span.Hours > 0) ? string.Format(" {0} 小时", span.Hours) : "", text);
                }
                if (span.TotalHours >= 10.0)
                {
                    return string.Format("{0} 小时{1}", span.Hours, text);
                }
                if (span.TotalHours >= 1.0)
                {
                    return string.Format("{0} 小时{1}{2}", span.Hours, (span.Minutes > 0) ? string.Format(" {0} 分钟", span.Minutes) : "", text);
                }
                if (span.TotalMinutes >= 10.0)
                {
                    return string.Format("{0} 分钟{1}", span.Minutes, text);
                }
                if (span.TotalMinutes >= 1.0)
                {
                    return string.Format("{0} 分{1}{2}", span.Minutes, (span.Seconds > 0) ? string.Format(" {0} 秒", span.Seconds) : "", text);
                }
                if (span.TotalSeconds >= 1.0)
                {
                    return string.Format("{0} 秒{1}", span.Seconds, text);
                }
                return "1 秒" + text;
            }
        }

        public static Version ParseVersionWithDefaults(string value)
        {
            for (;;)
            {
                if (value.Count<char>((char c) => c == '.') >= 3)
                {
                    break;
                }
                value += ".0";
            }
            return Version.Parse(value);
        }
    }
}
