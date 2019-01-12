using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Cafocha.BusinessContext.WarehouseWorkspace;

namespace Cafocha.GUI.Converter
{

    [ValueConversion(typeof(string), typeof(string))]
    public class StockGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (from types in WarehouseModule.StockTypes
                where types.StId.Equals(value)
                select types).FirstOrDefault()
                ?.Name;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
