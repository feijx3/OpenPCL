using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateFolderPath : Validate
    {
        public bool UseMinecraftCharCheck
        {
            [CompilerGenerated]
            get
            {
                return this._UseMinecraftCharCheck;
            }
            [CompilerGenerated]
            set
            {
                this._UseMinecraftCharCheck = value;
            }
        }

        public ValidateFolderPath()
        {
            this.UseMinecraftCharCheck = true;
        }

        public ValidateFolderPath(bool UseMinecraftCharCheck)
        {
            this.UseMinecraftCharCheck = true;
            this.UseMinecraftCharCheck = UseMinecraftCharCheck;
        }

        public override string Check(string Str)
        {
            Str = Str.Replace("/", "\\");
            if (!PathUtils.RemoveSlashSuffix(Str).EndsWithF(":", false))
            {
                Str = PathUtils.RemoveSlashSuffix(Str);
            }
            string text = new ValidateNullOrWhiteSpace().Check(Str);
            checked
            {
                string text2;
                if (Operators.CompareString(text, "", false) != 0)
                {
                    text2 = text;
                }
                else
                {
                    text = new ValidateLength(1, 254).Check(Str);
                    if (Operators.CompareString(text, "", false) != 0)
                    {
                        text2 = text;
                    }
                    else
                    {
                        string text3 = Str.RegexSeek(".{2,}~\\d", RegexOptions.None);
                        if (text3 != null)
                        {
                            text2 = string.Format("文件夹名不能包含 {0} 这一特殊格式！", text3);
                        }
                        else
                        {
                            if (!Str.StartsWithF("\\\\Mac\\", false))
                            {
                                try
                                {
                                    foreach (string text4 in Environment.GetLogicalDrives().Where<string>((ValidateFolderPath._Closure___._I6_0 == null) ? (ValidateFolderPath._Closure___._I6_0 = new Func<string, bool>(ValidateFolderPath._Closure___._I._Lambda___6_0)) : ValidateFolderPath._Closure___._I6_0))
                                    {
                                        string text5 = Conversions.ToString(text4.Upper().First<char>()) + ":\\";
                                        if (Operators.CompareString(Str.Upper(), text5, false) == 0)
                                        {
                                            return "";
                                        }
                                        if (Str.StartsWithF(text5, true))
                                        {
                                            goto IL_0146;
                                        }
                                    }
                                }
                                finally
                                {
                                    IEnumerator<string> enumerator = default;
                                    if (enumerator != null)
                                    {
                                        enumerator.Dispose();
                                    }
                                }
                                return "文件夹路径头存在错误！";
                            }
                            IL_0146:
                            int num = (Str.StartsWithF("\\\\Mac\\", false) ? 2 : 1);
                            int num2 = Str.Split("\\", false).Count<string>() - 1;
                            for (int i = num; i <= num2; i++)
                            {
                                string text6 = Str.Split("\\", false)[i];
                                if (Operators.CompareString(new ValidateNullOrWhiteSpace().Check(text6), "", false) != 0)
                                {
                                    return "文件夹路径存在错误！";
                                }
                                string text7 = new ValidateExcept(new string(Path.GetInvalidFileNameChars()) + (this.UseMinecraftCharCheck ? "!;" : ""), "路径中存在无效字符！").Check(text6);
                                if (Operators.CompareString(text7, "", false) != 0)
                                {
                                    return text7;
                                }
                                if (text6.StartsWithF(" ", false))
                                {
                                    return "文件夹名不能以空格开头！";
                                }
                                if (text6.EndsWithF(" ", false))
                                {
                                    return "文件夹名不能以空格结尾！";
                                }
                                if (text6.EndsWithF(".", false))
                                {
                                    return "文件夹名不能以小数点结尾！";
                                }
                                string text8 = new ValidateExceptSame(new string[]
                                {
                                    "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                                    "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                                    "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                                }, "文件夹名不可为 %！", false).Check(text6);
                                if (Operators.CompareString(text8, "", false) != 0)
                                {
                                    return text8;
                                }
                            }
                            text2 = "";
                        }
                    }
                }
                return text2;
            }
        }

        [CompilerGenerated]
        private bool _UseMinecraftCharCheck;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal bool _Lambda___6_0(string p)
            {
                return DirectoryUtils.Exists(p);
            }

            public static readonly ValidateFolderPath._Closure___ _I = new ValidateFolderPath._Closure___();

            public static Func<string, bool> _I6_0;
        }
    }
}
