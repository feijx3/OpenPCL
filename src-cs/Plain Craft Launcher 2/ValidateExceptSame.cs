using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateExceptSame : Validate
    {
        public Collection<string> Excepts { get; set; }

        public string ErrorMessage { get; set; }

        public bool IgnoreCase { get; set; }

        public ValidateExceptSame()
        {
            this.Excepts = new Collection<string>();
            this.IgnoreCase = false;
        }

        public ValidateExceptSame(Collection<string> Excepts, string ErrorMessage = "输入内容不能为 %", bool IgnoreCase = false)
        {
            this.Excepts = new Collection<string>();
            this.IgnoreCase = false;
            this.Excepts = Excepts;
            this.ErrorMessage = ErrorMessage;
            this.IgnoreCase = IgnoreCase;
        }

        public ValidateExceptSame(IEnumerable<string> Excepts, string ErrorMessage = "输入内容不能为 %", bool IgnoreCase = false)
            : this(new Collection<string>(new List<string>(Excepts)), ErrorMessage, IgnoreCase)
        {
        }

        public ValidateExceptSame(string Except, string ErrorMessage = "输入内容不能为 %", bool IgnoreCase = false)
            : this(new Collection<string> { Except }, ErrorMessage, IgnoreCase)
        {
        }

        public override string Validate(string Str)
        {
            string text;
            if (Str == null)
            {
                text = this.ErrorMessage.Replace("%", "null");
            }
            else
            {
                try
                {
                    foreach (string text2 in this.Excepts)
                    {
                        if (this.IgnoreCase)
                        {
                            if (Operators.CompareString(Str.Lower(), text2.Lower(), false) == 0)
                            {
                                return this.ErrorMessage.Replace("%", text2);
                            }
                        }
                        else if (Str.Equals(text2))
                        {
                            return this.ErrorMessage.Replace("%", text2);
                        }
                    }
                }
                finally
                {
                    IEnumerator<string> enumerator;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                text = "";
            }
            return text;
        }
    }
}
