using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace MaterialDesignThemes.Wpf.Converters
{
    public class IngredientMaxProgressBarConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                var contain = (double) values[0];
                var std_contain = (double) values[1];
                if (contain < std_contain) return Brushes.Red;

                return Brushes.Chartreuse;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}