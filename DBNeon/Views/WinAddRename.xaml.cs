using GalaSoft.MvvmLight.Messaging;
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
using DBNeon.Helpers;
using DBNeon.ViewModel;

namespace DBNeon.Views
{
    /// <summary>
    /// Логика взаимодействия для WinAddRename.xaml
    /// </summary>
    public partial class WinAddRename : Window
    {
        public WinAddRename()
        {
            InitializeComponent();
            Closing += (s, e) => Cleanup();
            //Messenger.Default.Register<MessAddNewBlock>(this, send =>
            //{
            //    DialogResult = true;
            //    if (send.IsEdit)
            //        Close();
            //});
            Messenger.Default.Register<CloseWindow>(this, otvet =>
            {
                //DialogResult = false;
                Close();
            });
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<CloseWindow>(this);
            //Messenger.Default.Unregister<MessAddNewBlock>(this);
        }

        private void WinAddRename_OnLoaded(object sender, RoutedEventArgs e)
        {
            tbxNewName.Focus();
        }
    }
}
