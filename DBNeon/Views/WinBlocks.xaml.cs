using DBNeon.ViewModel;
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
using DBNeon.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Views
{
    /// <summary>
    /// Логика взаимодействия для WinBlocks.xaml
    /// </summary>
    public partial class WinBlocks : Window
    {
        public WinBlocks()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessageAction>(this, ReceiveMessageForOpenWindow);

            Closing += (s, e) => Cleanup();
        }
        
        private void ReceiveMessageForOpenWindow(NotificationMessageAction msg)
        {
            if (msg.Notification == "ShowWinBlocksAdd" | msg.Notification == "ShowWinBlocksEdit")
            {
                var uniqueKey = Guid.NewGuid().ToString();
                var winBlockChange = new WinBlocksAddEdit();
                var vmBlockChange = SimpleIoc.Default.GetInstance<VMBlockChange>(uniqueKey);
                winBlockChange.DataContext = vmBlockChange;
                winBlockChange.Title = msg.Notification == "ShowWinBlocksAdd" ? "Добавление блока" : "Изменение блока";
                msg.Execute();
                winBlockChange.Show();

                winBlockChange.Closed += (sender, args) =>
                {
                    SimpleIoc.Default.Unregister(uniqueKey);                    
                };
            }
           
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<NotificationMessageAction>(this);
            //Messenger.Default.Unregister<MessForChangeBlock>(this);
        }
    }
}
