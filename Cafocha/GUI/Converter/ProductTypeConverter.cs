using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;

namespace Cafocha.GUI.Converter
{

    [ValueConversion(typeof(int), typeof(string))]
    public class ProductTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (ProductType)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
