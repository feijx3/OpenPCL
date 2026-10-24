using System;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateNullOrWhiteSpace : Validate
    {
        public ValidateNullOrWhiteSpace()
        {
        }

        public override string Check(string Str)
        {
            string text;
            if (Information.IsNothing(Str) || string.IsNullOrWhiteSpace(Str))
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
