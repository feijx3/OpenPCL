using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace PCL
{
    [StandardModule]
    public static class ModValidate
    {
        public static string Validate(string Text, IEnumerable<Validate> ValidateRules)
        {
            string text = "";
            try
            {
                foreach (Validate validate in ValidateRules)
                {
                    text = validate.Check(Text);
                    if (text == null)
                    {
                        return "";
                    }
                    if (Operators.CompareString(text, "", false) != 0)
                    {
                        return text;
                    }
                }
            }
            finally
            {
                IEnumerator<Validate> enumerator = default;
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            return text;
        }
    }
}
