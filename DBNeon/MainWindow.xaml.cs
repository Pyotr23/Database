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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBNeon.Helpers;
using DBNeon.Views;
using DBNeon.ViewModel;
using DBNeon.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;


namespace DBNeon
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, ReceiveMessageForOpenWindow);

            Closing += (s, e) => ViewModelLocator.Cleanup();


            // Открываю окно для отображения типов(или объектов, или поставок) с нужным контекстом.
            Messenger.Default.Register<OpenWindowMessage>(this, message =>
            {
                var uniqueKey = Guid.NewGuid().ToString();
                Window newWin;
                if (message.Type == WindowType.addMoving)
                {
                    newWin = new WinAddMoving();
                    var addMovindVM = SimpleIoc.Default.GetInstance<VMAddMoving>(uniqueKey);
                    newWin.DataContext = addMovindVM;
                }
                else if (message.Type == WindowType.journal)
                {
                    newWin = new WinJournal();
                    var journalVM = SimpleIoc.Default.GetInstance<VMJournal>(uniqueKey);
                    newWin.DataContext = journalVM;
                }
                else if (message.Type == WindowType.table)
                {
                    newWin = new WinTable();
                    var tableVM = SimpleIoc.Default.GetInstance<VMTable>(uniqueKey);
                    newWin.DataContext = tableVM;
                }
                else
                {
                    newWin = new WinOneColumn();
                    if (message.Type == WindowType.types)
                    {
                        var typeWinVM = SimpleIoc.Default.GetInstance<VMTypes>(uniqueKey);
                        newWin.DataContext = typeWinVM;
                        newWin.Title = "Типы";            // Устанавливаю заголовок открываемого окна.
                        typeWinVM.IsSelect = false;             // Свойство IsSelect отвечает за доступность кнопок "Изменить", "Удалить".
                    }
                    else if (message.Type == WindowType.locations)
                    {
                        var locWinVM = SimpleIoc.Default.GetInstance<VMLocations>(uniqueKey);
                        newWin.DataContext = locWinVM;
                        newWin.Title = "Объекты";
                    }
                    else if (message.Type == WindowType.procurements)
                    {
                        var procWinVM = SimpleIoc.Default.GetInstance<VMProcurements>(uniqueKey);
                        newWin.DataContext = procWinVM;
                        newWin.Title = "Поставки";
                    }
                    this.Visibility = Visibility.Hidden;
                }

                newWin.Show();
                newWin.Closed += (sender, args) =>
                {
                    SimpleIoc.Default.Unregister(uniqueKey);
                    this.Visibility = Visibility.Visible;
                };

            });
        }

        public void ReceiveMessageForOpenWindow(NotificationMessage msg)
        {
            if (msg.Notification == "ShowWinBlocks")
            {
                var uniqueKey = Guid.NewGuid().ToString();
                var blocksWin = new WinBlocks();
                var blocksWinVM = SimpleIoc.Default.GetInstance<VMBlocks>(uniqueKey);
                blocksWin.DataContext = blocksWinVM;
                this.Visibility = Visibility.Hidden;
                blocksWin.Show();

                blocksWin.Closed += (sender, args) =>
                {
                    SimpleIoc.Default.Unregister(uniqueKey);
                    this.Visibility = Visibility.Visible;
                };
            }
        }      

        public void Cleanup()
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
            Messenger.Default.Unregister<OpenWindowMessage>(this);
            //Messenger.Default.Unregister<MessOpenBlocksWin>(this);
        }
    }
}
