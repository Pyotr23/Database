using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Input;
using DBNeon.Helpers;
using DBNeon.Models;
using DBNeon.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //private NeonContext nc = new NeonContext();
        private Moving lastMoving;
        private string lastMovingText;
        public RelayCommand OpenTypeWin { get; private set; }
        public RelayCommand OpenLocWin { get; private set; }
        public RelayCommand OpenProcWin { get; private set; }
        public RelayCommand OpenBlockWin { get; private set; }
        public RelayCommand OpenAddMovWin { get; private set; }
        public RelayCommand OpenJournalWin { get; private set; }
        public RelayCommand OpenTableWin { get; private set; }
        

        // �������� ��� ����������� ���� � ������ ����� ���������� �� ���� �����������.
        public string LastMovingText
        {
            get => lastMovingText;
            set
            {
                lastMovingText = value;
                RaisePropertyChanged(nameof(LastMovingText));
            }
        }

        public MainViewModel()
        {
            // ��������� ���������� (����, ����� �����) � ��������� �����������.
            GetFirstMoving();

            // ��� ����� ����������� ������� ��� ���������� ������ � ��������� �� ���� �����������.
            Messenger.Default.Register<NotificationMessage>(this, Refresh);

            OpenTypeWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.types }));
            OpenLocWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.locations }));
            OpenProcWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.procurements }));

            // ������� �������� ���� �� ������� ���� ������.
            OpenBlockWin = new RelayCommand(ShowWindowWithAllBlocks);

            OpenAddMovWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.addMoving }));
            OpenJournalWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.journal }));
            OpenTableWin = new RelayCommand(() => Messenger.Default.Send(new OpenWindowMessage() { Type = WindowType.table }));
        }

        public void Refresh(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "�������� ��������� �����������")
            {
                GetFirstMoving();
                RaisePropertyChanged(nameof(LastMovingText));
            }
        }

        public void GetFirstMoving()
        {
            NeonContext nc = new NeonContext();
            nc.Movings.Load();
            var movings = nc.Movings.Local;
            DateTime lastDate = movings.Select(d => DateTime.Parse(d.Date)).OrderByDescending(z => z).FirstOrDefault();
            Moving lastMoving = movings.FirstOrDefault(x => x.Date == lastDate.ToShortDateString());
            if (lastMoving != null)
                LastMovingText = $"{lastDate.ToLongDateString()}, ���� �{lastMoving.Number}";
        }

        public void ShowWindowWithAllBlocks()
        {
            Messenger.Default.Send(new NotificationMessage("ShowWinBlocks"));
        }
    }
}