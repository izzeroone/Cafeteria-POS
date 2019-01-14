using System;
using System.Globalization;
using System.Windows.Data;

namespace MaterialDesignThemes.Wpf.Converters
{
    public class NonPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = "";
            if (value != null) val = value.ToString();
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}