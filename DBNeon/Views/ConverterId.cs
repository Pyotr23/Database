using DBNeon.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DBNeon.Views
{
    public class ConverterId : IMultiValueConverter
    {
        public object Convert(object[] values, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == null)
                return "";
            if (values[1] is Location[])
                return ((Location[])values[1]).Single(l => l.Id == (int)values[0]).Name;
            if (values[1] is Models.Type[])
                return ((Models.Type[])values[1]).Single(l => l.Id == (int)values[0]).Name;
            if (values[1] is Procurement[])
                return ((Procurement[])values[1]).Single(l => l.Id == (int)values[0]).Name;
            return "нз";
        }       

        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
