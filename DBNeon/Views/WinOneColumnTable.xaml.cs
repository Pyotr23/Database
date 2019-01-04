using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _5_DBNeon.ViewModels;
using Type = _5_DBNeon.Models.Type;

namespace _5_DBNeon
{
    /// <summary>
    /// Логика взаимодействия для WinOneColumnTable.xaml
    /// </summary>
    public partial class WinOneColumnTable : Window
    {
        public WinOneColumnTable(object type)
        {
            InitializeComponent();            
            this.DataContext = new AppViewModel(type);
        }        
    }
}
