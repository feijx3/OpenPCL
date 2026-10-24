using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;
using Microsoft.VisualBasic;

namespace PCL
{
    public class ValidateExcept : Validate
    {
        public Collection<string> Excepts
        {
            [CompilerGenerated]
            get
            {
                return this._Excepts;
            }
            [CompilerGenerated]
            set
            {
                this._Excepts = value;
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
            : this(new Collection<string>(Excepts.Select<char, string>((ValidateExcept._Closure___._I11_0 == null) ? (ValidateExcept._Closure___._I11_0 = new Func<char, string>(ValidateExcept._Closure___._I._Lambda___11_0)) : ValidateExcept._Closure___._I11_0).ToList<string>()), ErrorMessage)
        {
        }

        public override string Check(string Str)
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
                IEnumerator<string> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            return "";
        }

        [CompilerGenerated]
        private Collection<string> _Excepts;

        [CompilerGenerated]
        private string _ErrorMessage;

        [CompilerGenerated]
        [Serializable]
        private sealed class _Closure___
        {
            public _Closure___()
            {
            }

            static _Closure___()
            {
            }

            internal string _Lambda___11_0(char c)
            {
                return c.ToString();
            }

            public static readonly ValidateExcept._Closure___ _I = new ValidateExcept._Closure___();

            public static Func<char, string> _I11_0;
        }
    }
}
