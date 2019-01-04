using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DBNeon.Models;
using Type = System.Type;

namespace DBNeon.Views
{
    // Получаю на выходе строку вида "откуда ⇒ куда" для журнала перемещений.
    // На входе два Id местоположений и коллекция местоположений.
    public class ConverterIdToName : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {            
            if (values[1] is ObservableCollection<Models.Type>)
                return ((ObservableCollection<Models.Type>)values[1]).Single(t => t.Id == (int)values[0]).Name;
            if (values[1] is ObservableCollection<Location>)
            {
                if ((int)values[0] == -1)
                    return String.Format($" ⇒ {((ObservableCollection<Location>)values[1]).Single(t => t.Id == (int)values[2]).Name}");
                if ((int)values[2] == -1)
                    return String.Format($"{((ObservableCollection<Location>)values[1]).Single(t => t.Id == (int)values[0]).Name} ⇒ ");
                return String.Format($"{((ObservableCollection<Location>)values[1]).Single(t => t.Id == (int)values[0]).Name} ⇒ " +
                                     $"{((ObservableCollection<Location>)values[1]).Single(t => t.Id == (int)values[2]).Name}");
            }
            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
