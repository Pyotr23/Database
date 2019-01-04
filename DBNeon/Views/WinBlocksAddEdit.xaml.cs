using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DBNeon.Helpers;
using DBNeon.ViewModel;
using DBNeon.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Views
{
    /// <summary>
    /// Логика взаимодействия для WinBlocksAddEdit.xaml
    /// </summary>
    public partial class WinBlocksAddEdit : Window
    {
        public WinBlocksAddEdit()
        {
            InitializeComponent();
            Closing += (s, e) => Cleanup();

            Messenger.Default.Register<CloseWindow>(this, (x) => Close());
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<CloseWindow>(this);
        }

        private void winChangeBlock_Loaded(object sender, RoutedEventArgs e)
        {
            tbxNumber.Focus();
        }
    }
}
