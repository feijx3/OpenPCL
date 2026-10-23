using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateExcept : Validate
    {
        public Collection<string> Excepts { get; set; }

        public string ErrorMessage { get; set; }

        public ValidateExcept()
        {
            this.Excepts = new Collection<string>();
            this.ErrorMessage = "输入内容不能包含 %";
        }

        public ValidateExcept(Collection<string> Excepts, string ErrorMessage = "输入内容不能包含 %")
        {
            this.Excepts = new Collection<string>();
            this.Excepts = Excepts;
            this.ErrorMessage = ErrorMessage;
        }

        public ValidateExcept(IEnumerable<string> Excepts, string ErrorMessage = "输入内容不能包含 %")
            : this(new Collection<string>(new List<string>(Excepts)), ErrorMessage)
        {
        }

        public ValidateExcept(IEnumerable<char> Excepts, string ErrorMessage = "输入内容不能包含 %")
            : this(new Collection<string>(Excepts.Select<char, string>((ValidateExcept._Closure$__.$I11-0 == null) ? (ValidateExcept._Closure$__.$I11-0 = (char c) => c.ToString()) : ValidateExcept._Closure$__.$I11-0).ToList<string>()), ErrorMessage)
        {
        }

        public override string Validate(string Str)
        {
            try
            {
                foreach (string text in this.Excepts)
                {
                    if (Str.IndexOfF(text, true) >= 0)
                    {
                        if (Information.IsNothing(this.ErrorMessage))
                        {
                            this.ErrorMessage = "";
                        }
                        return this.ErrorMessage.Replace("%", text);
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
            return "";
        }
    }
}
