using System;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateNullOrEmpty : Validate
    {
        public ValidateNullOrEmpty()
        {
        }

        public override string Check(string Str)
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
