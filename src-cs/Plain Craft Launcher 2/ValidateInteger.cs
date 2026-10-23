using System;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateInteger : Validate
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public ValidateInteger()
        {
            this.Max = int.MaxValue;
        }

        public ValidateInteger(int Min, int Max)
        {
            this.Max = int.MaxValue;
            this.Min = Min;
            this.Max = Max;
        }

        public override string Validate(string Str)
        {
            string text;
            int num;
            if (Str.Length > 9)
            {
                text = "请输入一个大小合理的数字！";
            }
            else if (!int.TryParse(Str, out num))
            {
                text = "请输入一个整数！";
            }
            else if (num > this.Max)
            {
                text = "不可超过 " + Conversions.ToString(this.Max) + "！";
            }
            else if (num < this.Min)
            {
                text = "不可低于 " + Conversions.ToString(this.Min) + "！";
            }
            else
            {
                text = "";
            }
            return text;
        }
    }
}
