using System;
using System.Text.RegularExpressions;
using MeloongCore.Extensions;

namespace PCL
{
    public class ValidateHttp : Validate
    {
        public ValidateHttp()
        {
        }

        public override string Check(string Str)
        {
            if (Str.EndsWithF("/", false))
            {
                Str = Str.Substring(0, checked(Str.Length - 1));
            }
            string text;
            if (!Str.RegexCheck("^(http[s]?)\\://", RegexOptions.None))
            {
                text = "输入的网址无效！";
            }
            else
            {
                text = "";
            }
            return text;
        }
    }
}
