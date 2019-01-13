using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Cafocha.BusinessContext.WarehouseWorkspace;

namespace Cafocha.GUI.Converter
{

    [ValueConversion(typeof(bool), typeof(string))]
    public class StockTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
            {
                return "+";
            }
            else
            {
                return "-";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
