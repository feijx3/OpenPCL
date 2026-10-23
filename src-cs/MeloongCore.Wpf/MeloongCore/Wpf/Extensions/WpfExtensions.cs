using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MeloongCore.Wpf.Extensions
{
    public static class WpfExtensions
    {
        [NullableContext(2)]
        public static bool Any(this UIElementCollection arr)
        {
            return arr != null && arr.Count > 0;
        }
    }
}
