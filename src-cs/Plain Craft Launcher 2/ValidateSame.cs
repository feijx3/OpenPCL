using System;

namespace PCL
{
    public class ValidateSame : Validate
    {
        public string Required { get; set; }

        public string ErrorMessage { get; set; }

        public bool IgnoreCase { get; set; }

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

        public override string Validate(string Str)
        {
            if (string.Compare(Str, this.Required, this.IgnoreCase) != 0)
            {
                return this.ErrorMessage;
            }
            return "";
        }
    }
}
