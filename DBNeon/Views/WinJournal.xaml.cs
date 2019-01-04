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
    /// Логика взаимодействия для WinJournal.xaml
    /// </summary>
    public partial class WinJournal : Window
    {
        public WinJournal()
        {
            InitializeComponent();
            Closing += (s, e) => Cleanup();
            Messenger.Default.Register<CloseWindow>(this, otvet => Close());
            Messenger.Default.Register<MessDateForScroll>(this, data => lbxMove.ScrollIntoView(data.MovingItem));
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<CloseWindow>(this);
            Messenger.Default.Unregister<MessDateForScroll>(this);
        }
        
    }
}
