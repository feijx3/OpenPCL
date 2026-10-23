using System;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateLength : Validate
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public ValidateLength()
        {
            this.Min = 0;
            this.Max = int.MaxValue;
        }

        public ValidateLength(int Min, int Max = 2147483647)
        {
            this.Min = 0;
            this.Max = int.MaxValue;
            this.Min = Min;
            this.Max = Max;
        }

        public override string Validate(string Str)
        {
            string text;
            if (Strings.Len(Str) != this.Max && this.Max == this.Min)
            {
                text = string.Format("长度必须为 {0} 个字符！", this.Max);
            }
            else if (Strings.Len(Str) > this.Max)
            {
                text = string.Format("长度最长为 {0} 个字符！", this.Max);
            }
            else if (Strings.Len(Str) < this.Min)
            {
                text = string.Format("长度至少需 {0} 个字符！", this.Min);
            }
            else
            {
                text = "";
            }
            return text;
        }
    }
}
