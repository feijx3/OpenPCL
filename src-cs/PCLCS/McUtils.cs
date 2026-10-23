using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MeloongCore.Extensions;

namespace PCLCS
{
    [NullableContext(1)]
    [Nullable(0)]
    public static class McUtils
    {
        public static int CompareVersion(string left, string right)
        {
            if (left == right)
            {
                return 0;
            }
            if (!(left == "未知版本") && !(right == "未知版本"))
            {
                left = left.Lower();
                right = right.Lower();
                List<string> list = left.Replace("快照", "snapshot").Replace("预览版", "pre").RegexSearch("[a-z]+|[0-9]+", RegexOptions.None)
                    .ToList<string>();
                List<string> list2 = right.Replace("快照", "snapshot").Replace("预览版", "pre").RegexSearch("[a-z]+|[0-9]+", RegexOptions.None)
                    .ToList<string>();
                int num = 0;
                while (num < list.Count || num < list2.Count)
                {
                    string text = ((num < list.Count) ? list[num] : "0");
                    string text2 = ((num < list2.Count) ? list2[num] : "0");
                    if (!(text == text2))
                    {
                        int num2;
                        if (!(text == "rc"))
                        {
                            if (!(text == "pre"))
                            {
                                if (!(text == "snapshot"))
                                {
                                    if (!(text == "experimental"))
                                    {
                                        int num3;
                                        num2 = (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num3) ? num3 : 0);
                                    }
                                    else
                                    {
                                        num2 = -4;
                                    }
                                }
                                else
                                {
                                    num2 = -3;
                                }
                            }
                            else
                            {
                                num2 = -2;
                            }
                        }
                        else
                        {
                            num2 = -1;
                        }
                        int num4 = num2;
                        if (!(text2 == "rc"))
                        {
                            if (!(text2 == "pre"))
                            {
                                if (!(text2 == "snapshot"))
                                {
                                    if (!(text2 == "experimental"))
                                    {
                                        int num5;
                                        num2 = (int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out num5) ? num5 : 0);
                                    }
                                    else
                                    {
                                        num2 = -4;
                                    }
                                }
                                else
                                {
                                    num2 = -3;
                                }
                            }
                            else
                            {
                                num2 = -2;
                            }
                        }
                        else
                        {
                            num2 = -1;
                        }
                        int num6 = num2;
                        if (num4 == 0 && num6 == 0)
                        {
                            return string.Compare(text, text2, StringComparison.Ordinal);
                        }
                        if (num4 != num6)
                        {
                            return num4.CompareTo(num6);
                        }
                    }
                    num++;
                }
                return string.Compare(left, right, StringComparison.Ordinal);
            }
            if (!(left == "未知版本"))
            {
                return -1;
            }
            return 1;
        }

        public static bool CompareVersionGE(string left, string right)
        {
            return McUtils.CompareVersion(left, right) >= 0;
        }
    }
}
