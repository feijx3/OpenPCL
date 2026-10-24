using System;
using System.Runtime.CompilerServices;

namespace PCL
{
    public class ValidateSame : Validate
    {
        public string Required
        {
            [CompilerGenerated]
            get
            {
                return this._Required;
            }
            [CompilerGenerated]
            set
            {
                this._Required = value;
            }
        }

        public string ErrorMessage
        {
            [CompilerGenerated]
            get
            {
                return this._ErrorMessage;
            }
            [CompilerGenerated]
            set
            {
                this._ErrorMessage = value;
            }
        }

        public bool IgnoreCase
        {
            [CompilerGenerated]
            get
            {
                return this._IgnoreCase;
            }
            [CompilerGenerated]
            set
            {
                this._IgnoreCase = value;
            }
        }

        public ValidateSame()
        {
            this.IgnoreCase = false;
        }

        public ValidateSame(string Required, string ErrorMessage = "输入内容有误", bool IgnoreCase = false)
        {
            this.IgnoreCase = false;
            this.Required = Required;
            this.ErrorMessage = ErrorMessage;
            this.IgnoreCase = IgnoreCase;
        }

        public override string Check(string Str)
        {
            if (string.Compare(Str, this.Required, this.IgnoreCase) != 0)
            {
                return this.ErrorMessage;
            }
            return "";
        }

        [CompilerGenerated]
        private string _Required;

        [CompilerGenerated]
        private string _ErrorMessage;

        [CompilerGenerated]
        private bool _IgnoreCase;
    }
}
