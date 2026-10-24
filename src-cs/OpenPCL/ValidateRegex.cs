using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MeloongCore.Extensions;

namespace PCL
{
    public class ValidateRegex : Validate
    {
        public string Regex
        {
            [CompilerGenerated]
            get
            {
                return this._Regex;
            }
            [CompilerGenerated]
            set
            {
                this._Regex = value;
            }
        }

        public string ErrorDescription
        {
            [CompilerGenerated]
            get
            {
                return this._ErrorDescription;
            }
            [CompilerGenerated]
            set
            {
                this._ErrorDescription = value;
            }
        }

        public ValidateRegex()
        {
            this.ErrorDescription = "正则检查失败！";
        }

        public ValidateRegex(string Regex, string ErrorDescription = "正则检查失败！")
        {
            this.ErrorDescription = "正则检查失败！";
            this.Regex = Regex;
            this.ErrorDescription = ErrorDescription;
        }

        public override string Check(string Str)
        {
            string text;
            if (!Str.RegexCheck(this.Regex, RegexOptions.None))
            {
                text = this.ErrorDescription;
            }
            else
            {
                text = "";
            }
            return text;
        }

        [CompilerGenerated]
        private string _Regex;

        [CompilerGenerated]
        private string _ErrorDescription;
    }
}
