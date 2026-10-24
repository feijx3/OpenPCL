using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using MeloongCore.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    public class ValidateExceptSame : Validate
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

        public override string Check(string Str)
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
                    IEnumerator<string> enumerator = default;
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
                text = "";
            }
            return text;
        }

        [CompilerGenerated]
        private Collection<string> _Excepts;

        [CompilerGenerated]
        private string _ErrorMessage;

        [CompilerGenerated]
        private bool _IgnoreCase;
    }
}
