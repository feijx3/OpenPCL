using System;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateNullable : Validate
    {
        public override string Validate(string Str)
        {
            string text;
            if (Information.IsNothing(Str) || string.IsNullOrEmpty(Str))
            {
                text = null;
            }
            else
            {
                text = "";
            }
            return text;
        }
    }
}
