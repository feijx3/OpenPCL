using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateInteger : Validate
    {
        public int Min
        {
            [CompilerGenerated]
            get
            {
                return this._Min;
            }
            [CompilerGenerated]
            set
            {
                this._Min = value;
            }
        }

        public int Max
        {
            [CompilerGenerated]
            get
            {
                return this._Max;
            }
            [CompilerGenerated]
            set
            {
                this._Max = value;
            }
        }

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

        public override string Check(string Str)
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

        [CompilerGenerated]
        private int _Min;

        [CompilerGenerated]
        private int _Max;
    }
}
