using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MeloongCore;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateFileName : Validate
    {
        public string Name
        {
            [CompilerGenerated]
            get
            {
                return this._Name;
            }
            [CompilerGenerated]
            set
            {
                this._Name = value;
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

        public bool IgnoreCase
        {
            [CompilerGenerated]
            get
            {
                return this._IgnoreCase;
            }
            [CompilerGenerated]
            set
            {
                this._IgnoreCase = value;
            }
        }

        public string ParentFolder
        {
            [CompilerGenerated]
            get
            {
                return this._ParentFolder;
            }
            [CompilerGenerated]
            set
            {
                this._ParentFolder = value;
            }
        }

        public object RequireParentFolderExists
        {
            [CompilerGenerated]
            get
            {
                return this._RequireParentFolderExists;
            }
            [CompilerGenerated]
            set
            {
                this._RequireParentFolderExists = RuntimeHelpers.GetObjectValue(value);
            }
        }

        public ValidateFileName()
        {
            this.UseMinecraftCharCheck = true;
            this.IgnoreCase = true;
            this.ParentFolder = null;
            this.RequireParentFolderExists = true;
        }

        public ValidateFileName(string Name, bool UseMinecraftCharCheck = true, bool IgnoreCase = true)
        {
            this.UseMinecraftCharCheck = true;
            this.IgnoreCase = true;
            this.ParentFolder = null;
            this.RequireParentFolderExists = true;
            this.Name = Name;
            this.IgnoreCase = IgnoreCase;
            this.UseMinecraftCharCheck = UseMinecraftCharCheck;
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
                    text2 = "文件名不能以空格开头！";
                }
                else if (Str.EndsWithF(" ", false))
                {
                    text2 = "文件名不能以空格结尾！";
                }
                else
                {
                    text = new ValidateLength(1, 253).Check(Str + this.ParentFolder);
                    if (Operators.CompareString(text, "", false) != 0)
                    {
                        text2 = text;
                    }
                    else if (Str.EndsWithF(".", false))
                    {
                        text2 = "文件名不能以小数点结尾！";
                    }
                    else
                    {
                        string text3 = new ValidateExcept(new string(Path.GetInvalidFileNameChars()) + (this.UseMinecraftCharCheck ? "!;" : ""), "文件名不可包含 % 字符！").Check(Str);
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
                            }, "文件名不可为 %！", true).Check(Str);
                            if (Operators.CompareString(text4, "", false) != 0)
                            {
                                text2 = text4;
                            }
                            else if (Str.RegexCheck(".{2,}~\\d", RegexOptions.None))
                            {
                                text2 = "文件名不能包含这一特殊格式！";
                            }
                            else
                            {
                                if (this.ParentFolder != null)
                                {
                                    DirectoryInfo info = DirectoryUtils.GetInfo(this.ParentFolder);
                                    if (info.Exists)
                                    {
                                        string text5 = new ValidateExceptSame(info.EnumerateFiles("*").Select<FileInfo, string>((ValidateFileName._Closure___._I22_0 == null) ? (ValidateFileName._Closure___._I22_0 = new Func<FileInfo, string>(ValidateFileName._Closure___._I._Lambda___22_0)) : ValidateFileName._Closure___._I22_0), "不可与现有文件重名！", this.IgnoreCase).Check(Str);
                                        if (Operators.CompareString(text5, "", false) != 0)
                                        {
                                            return text5;
                                        }
                                    }
                                    else if (Conversions.ToBoolean(this.RequireParentFolderExists))
                                    {
                                        return string.Format("父文件夹不存在：{0}", this.ParentFolder);
                                    }
                                }
                                text2 = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "检查文件名出错", LogBehavior.ToastIfDebug, "E:\\Projects\\PCL\\本体代码\\Plain Craft Launcher 2\\Modules\\Base\\ModValidate.vb");
                text2 = "错误：" + ex.Message;
            }
            return text2;
        }

        [CompilerGenerated]
        private string _Name;

        [CompilerGenerated]
        private bool _UseMinecraftCharCheck;

        [CompilerGenerated]
        private bool _IgnoreCase;

        [CompilerGenerated]
        private string _ParentFolder;

        [CompilerGenerated]
        private object _RequireParentFolderExists;

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

            internal string _Lambda___22_0(FileInfo f)
            {
                return f.Name;
            }

            public static readonly ValidateFileName._Closure___ _I = new ValidateFileName._Closure___();

            public static Func<FileInfo, string> _I22_0;
        }
    }
}
