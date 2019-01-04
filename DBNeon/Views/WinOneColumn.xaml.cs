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
    /// Логика взаимодействия для WinOneColumn.xaml
    /// </summary>
    public partial class WinOneColumn : Window
    {
        public WinOneColumn()
        {
            InitializeComponent();
            Closing += (s, e) => this.Cleanup();
            

            // Принимается сообщение открытия окна "Добавить/изменить" со списком имён коллекции.
            Messenger.Default.Register<MessOpenAddWin>(this, message =>
            {
                var uniqueKey = System.Guid.NewGuid().ToString();
                var addWinVM = SimpleIoc.Default.GetInstance<VMAddRename>(uniqueKey);
                addWinVM.Collection = message.Names;
                addWinVM.IsEdit = message.IsEdit;
                if (message.IsEdit)
                    addWinVM.Name = message.Name;
                else
                    addWinVM.Name = "";
                var winAddRename = new WinAddRename(){ DataContext = addWinVM };
                winAddRename.Closed += (sender, args) => SimpleIoc.Default.Unregister(uniqueKey);
                winAddRename.Show();
            });

            // Принимается сообщение для вызова диалогового окна на подтверждение удаления.
            Messenger.Default.Register<MessDelete>(this, message =>
            {
                MessageBoxResult result = MessageBox.Show($"{message.Phrase}", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                // Если точно нужно удалить, то отправляется в VM сообщение.
                if (result == MessageBoxResult.Yes)
                    Messenger.Default.Send(new MessDelItemFromDB());
            });
        }

        public void Cleanup()
        {
            Messenger.Default.Unregister<MessOpenAddWin>(this);
            Messenger.Default.Unregister<MessDelete>(this);
        }
    }
}
