using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DBNeon.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Type = DBNeon.Models.Type;
using System.Windows.Media;

namespace DBNeon.ViewModels
{
    public class VMAddRename : ViewModelBase
    {
        private string name;
        private string[] typesCollection;
        private bool enableButton;
        private Brush color;

        public bool IsEdit { get; set; }

        // Свойство управления цвета набираемого названия. Красный при повторяющемся названии.
        public Brush Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChanged(nameof(Color));
            }
        }

        // Свойство отключения кнопки "Подтвердить" при повторе названия.
        public bool EnableButton
        {
            get => enableButton;
            set
            {
                enableButton = value;
                if (value)
                    Color = Brushes.Black;
                else
                    Color = Brushes.Red;
                RaisePropertyChanged(nameof(EnableButton));
            }
        }

        // Коллекция имён.
        public string[] Collection
        {
            get => typesCollection;
            set
            {
                typesCollection = value;
                RaisePropertyChanged(nameof(Collection));
            }
        }

        public RelayCommand YesCommand { get; set; }
        public RelayCommand NoCommand { get; set; }

        // Вводимое название.
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                EnableButton = !Collection.Contains(name);
                RaisePropertyChanged(() => Name);
            }
        }


        public VMAddRename()
        {
            YesCommand = new RelayCommand(() =>
            {
                // Отправка сообщения о закрытии окна.
                Messenger.Default.Send(new CloseWindow());
                // Сообщение с именем нового элемента.
                Messenger.Default.Send(new MessNewName(){ IsEdit = this.IsEdit, Name = this.Name });
            });
            NoCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new CloseWindow());
            });
        }

    }
}
