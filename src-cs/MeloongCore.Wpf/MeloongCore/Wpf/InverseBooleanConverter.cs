using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace MeloongCore.Wpf
{
    public class InverseBooleanConverter : IValueConverter
    {
        [NullableContext(2)]
        public object Convert(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            bool flag;
            return bool.TryParse(value.ToString(), out flag) && !flag;
        }

        [NullableContext(2)]
        public object ConvertBack(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            bool flag;
            return bool.TryParse(value.ToString(), out flag) && !flag;
        }
    }
}
