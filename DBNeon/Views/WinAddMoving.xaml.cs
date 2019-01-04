using DBNeon.Helpers;
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

namespace DBNeon.Views
{
    /// <summary>
    /// Логика взаимодействия для WinAddMoving.xaml
    /// </summary>
    public partial class WinAddMoving : Window
    {
        public WinAddMoving()
        {
            InitializeComponent();
            Closing += (s, e) => Cleanup();
            Messenger.Default.Register<CloseWindow>(this, otvet => Close());
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<CloseWindow>(this);
        }
    }
}
