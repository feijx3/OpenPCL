using System;

namespace PCL
{
    public class ValidateFunc : Validate
    {
        public ValidateFunc(Func<string, string> F)
        {
            this._func = F;
        }

        public override string Check(string Str)
        {
            return this._func(Str);
        }

        private readonly Func<string, string> _func;
    }
}
