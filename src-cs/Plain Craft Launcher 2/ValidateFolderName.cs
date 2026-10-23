using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateFolderName : Validate
    {
        public string Folder { get; set; }

        public bool UseMinecraftCharCheck { get; set; }

        public ValidateFolderName()
        {
            this.UseMinecraftCharCheck = true;
        }

        public ValidateFolderName(string Folder, bool UseMinecraftCharCheck = true, List<string> IgnoreList = null)
        {
            this.UseMinecraftCharCheck = true;
            int num2;
            int num4;
            object obj;
            try
            {
                IL_000D:
                int num = 1;
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
                IL_0045:
                int num3 = num4 + 1;
                num4 = 0;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num3);
                IL_006B:
                goto IL_009F;
                IL_006D:
                num4 = num;
                @switch(ICSharpCode.Decompiler.ILAst.ILLabel[], num2);
                IL_007D:;
            }
            catch when (endfilter((obj is Exception) & (num2 != 0) & (num4 == 0)))
            {
                Exception ex = (Exception)obj2;
                goto IL_006D;
            }
            IL_009F:
            throw ProjectData.CreateProjectError(-2146828237);
            IL_00AA:
            if (num4 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        public override string Validate(string Str)
        {
            string text2;
            try
            {
                string text = new ValidateNullOrWhiteSpace().Validate(Str);
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
                    text = new ValidateLength(1, 100).Validate(Str);
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
                        string text3 = new ValidateExcept(new string(Path.GetInvalidFileNameChars()) + (this.UseMinecraftCharCheck ? "!;" : ""), "文件夹名不可包含 % 字符！").Validate(Str);
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
                            }, "文件夹名不可为 %！", true).Validate(Str);
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
                                        IEnumerator<string> enumerator;
                                        if (enumerator != null)
                                        {
                                            enumerator.Dispose();
                                        }
                                    }
                                }
                                string text6 = new ValidateExceptSame(list, "不可与现有文件夹重名！", true).Validate(Str);
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

        private readonly IEnumerable<string> PathIgnore;

        private readonly List<string> IgnoreList;
    }
}
