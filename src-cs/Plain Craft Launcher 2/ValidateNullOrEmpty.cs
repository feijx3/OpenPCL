using System;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateNullOrEmpty : Validate
    {
        public override string Validate(string Str)
        {
            string text;
            if (Information.IsNothing(Str) || string.IsNullOrEmpty(Str))
            {
                text = "输入内容不能为空！";
            }
            else
            {
                text = "";
            }
            return text;
        }
    }
}
