using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace MeloongCore.Wpf
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        [NullableContext(2)]
        public object Convert(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            bool flag;
            return bool.TryParse(value.ToString(), out flag) ? (flag ? Visibility.Collapsed : Visibility.Visible) : Visibility.Visible;
        }

        [NullableContext(2)]
        public object ConvertBack(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            bool flag;
            if (value is Visibility)
            {
                Visibility visibility = (Visibility)value;
                flag = visibility > Visibility.Visible;
            }
            else
            {
                flag = false;
            }
            return flag;
        }
    }
}
