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
    public class ValidateFolderName : Validate
    {
        public string Folder
        {
            [CompilerGenerated]
            get
            {
                return this._Folder;
            }
            [CompilerGenerated]
            set
            {
                this._Folder = value;
            }
        }

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

        public ValidateFolderName()
        {
            this.UseMinecraftCharCheck = true;
        }

        public ValidateFolderName(string Folder, bool UseMinecraftCharCheck = true, List<string> IgnoreList = null)
        {
            this.UseMinecraftCharCheck = true;
            int num2 = 0;
            int num4;
            object obj;
            int num = 0;
            int __onErrorFrom_IL_00AA = -1;
            ONERR_IL_00AA:
            try
            {
                if (__onErrorFrom_IL_00AA >= 0)
                {
                    if (__onErrorFrom_IL_00AA < 1) goto IL_000D;
                    if (__onErrorFrom_IL_00AA < 2) goto IL_0016;
                    if (__onErrorFrom_IL_00AA < 4) goto IL_0026;
                    if (__onErrorFrom_IL_00AA < 5) goto IL_003A;
                    goto IL_00AA;
                }
                IL_000D:
                num = 1;
                this.Folder = Folder;
                IL_0016:
                num = 2;
                this.UseMinecraftCharCheck = UseMinecraftCharCheck;
                IL_001F:
                ProjectData.ClearProjectError();
                num2 = 1;
                IL_0026:
                num = 4;
                this.PathIgnore = DirectoryUtils.EnumerateDirectories(Folder, false, "*");
                IL_003A:
                num = 5;
                this.IgnoreList = IgnoreList;
                IL_0043:
                goto IL_00AA;
            }
            catch (Exception) when (num2 != 0)
            {
                __onErrorFrom_IL_00AA = num;
                goto ONERR_IL_00AA;
            }
            IL_00AA:;
        }

        public override string Check(string Str)
        {
            string text2;
            try
            {
                string text = new ValidateNullOrWhiteSpace().Check(Str);
                if (Operators.CompareString(text, "", false) != 0)
                {
                    text2 = text;
                }
                else if (Str.StartsWithF(" ", false))
                {
                    text2 = "文件夹名不能以空格开头！";
                }
                else if (Str.EndsWithF(" ", false))
                {
                    text2 = "文件夹名不能以空格结尾！";
                }
                else
                {
                    text = new ValidateLength(1, 100).Check(Str);
                    if (Operators.CompareString(text, "", false) != 0)
                    {
                        text2 = text;
                    }
                    else if (Str.EndsWithF(".", false))
                    {
                        text2 = "文件夹名不能以小数点结尾！";
                    }
                    else
                    {
                        string text3 = new ValidateExcept(new string(Path.GetInvalidFileNameChars()) + (this.UseMinecraftCharCheck ? "!;" : ""), "文件夹名不可包含 % 字符！").Check(Str);
                        if (Operators.CompareString(text3, "", false) != 0)
                        {
                            text2 = text3;
                        }
                        else
                        {
                            string text4 = new ValidateExceptSame(new string[]
                            {
                                "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
                                "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
                                "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                            }, "文件夹名不可为 %！", true).Check(Str);
                            if (Operators.CompareString(text4, "", false) != 0)
                            {
                                text2 = text4;
                            }
                            else if (Str.RegexCheck(".{2,}~\\d", RegexOptions.None))
                            {
                                text2 = "文件夹名不能包含这一特殊格式！";
                            }
                            else
                            {
                                List<string> list = new List<string>();
                                if (this.PathIgnore != null)
                                {
                                    try
                                    {
                                        foreach (string text5 in this.PathIgnore)
                                        {
                                            string lastPart = PathUtils.GetLastPart(text5);
                                            if (this.IgnoreList == null || !this.IgnoreList.Contains(lastPart, StringComparer.OrdinalIgnoreCase))
                                            {
                                                list.Add(lastPart);
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
                                }
                                string text6 = new ValidateExceptSame(list, "不可与现有文件夹重名！", true).Check(Str);
                                if (Operators.CompareString(text6, "", false) != 0)
                                {
                                    text2 = text6;
                                }
                                else
                                {
                                    text2 = "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "检查文件夹名出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModValidate.vb");
                text2 = "错误：" + ex.Message;
            }
            return text2;
        }

        [CompilerGenerated]
        private string _Folder;

        [CompilerGenerated]
        private bool _UseMinecraftCharCheck;

        private readonly IEnumerable<string> PathIgnore;

        private readonly List<string> IgnoreList;
    }
}
