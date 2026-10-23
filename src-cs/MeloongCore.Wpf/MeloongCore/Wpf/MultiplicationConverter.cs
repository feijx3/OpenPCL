using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace MeloongCore.Wpf
{
    public class MultiplicationConverter : IValueConverter
    {
        [NullableContext(2)]
        public object Convert(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            double num;
            if (!double.TryParse(value.ToString(), out num))
            {
                return 0;
            }
            double num2 = 1.0;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out num2);
            }
            return num * num2;
        }

        [NullableContext(2)]
        public object ConvertBack(object value, [Nullable(1)] Type targetType, object parameter, [Nullable(1)] CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }
            double num;
            if (!double.TryParse(value.ToString(), out num))
            {
                return Binding.DoNothing;
            }
            double num2 = 1.0;
            if (parameter != null)
            {
                double.TryParse(parameter.ToString(), out num2);
            }
            if (num2 == 0.0)
            {
                return Binding.DoNothing;
            }
            return num / num2;
        }
    }
}
