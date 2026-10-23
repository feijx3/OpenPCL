using System;
using System.Text.RegularExpressions;
using MeloongCore.Extensions;

namespace PCL
{
    public class ValidateRegex : Validate
    {
        public string Regex { get; set; }

        public string ErrorDescription { get; set; }

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

        public override string Validate(string Str)
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
    }
}
