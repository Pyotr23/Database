using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Type = DBNeon.Models.Type;

namespace DBNeon.ViewModel
{
    public class VMTypes : ViewModelBase
    {
        private NeonContext nc;                         // контекст
        private ObservableCollection<Type> collection;  
        private Type selectedItem;                      
        private bool isSelect;
        
        public RelayCommand AddCommand { get; }         // команда нажатия на кнопку "добавить"
        public RelayCommand DelCommand { get; }         // команда нажатия на кнопку "удалить"
        public RelayCommand EditCommand { get; }        // команда нажатия на кнопку "изменить"

        // Свойство, указывающее выбран ли элемент.
        public bool IsSelect
        {
            get => isSelect;
            set
            {
                isSelect = value;
                RaisePropertyChanged(nameof(IsSelect));
            }
        }

        // Выбранный элемент.
        public Type SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                IsSelect = true;        // Если элемент выбран, то IsSelect становится true.
                RaisePropertyChanged(nameof(selectedItem));
            }
        }

        // Коллекция типов блоков.
        public ObservableCollection<Type> Collection
        {
            get => collection;
            set
            {
                collection = value;
                RaisePropertyChanged(nameof(Collection));
            }
        }

        // Конструктор View-Model.
        public VMTypes()
        {
            // В коллекцию загружаются типы блоков из БД.
            nc = new NeonContext();
            nc.Types.Load();
            Collection = nc.Types.Local;

            // При нажатии на кнопку, реализующую эту команду, отправляется массив имён коллекции. 
            // Это необходимо для исключения повтора названия.
            AddCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new MessOpenAddWin() { Names = Collection.Select(n => n.Name).ToArray(), IsEdit = false});
                });

            // При нажатии на "кнопку" "Удалить" отправляется строка типа "Точно удалить тип такой-то?"
            DelCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessDelete(){ Phrase = $"тип \"{SelectedItem.Name}\"?"});
            });

            // Обработка нажатия кнопки изменения.
            EditCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessOpenAddWin(){ Names = Collection.Select(n => n.Name).ToArray(), Name = SelectedItem.Name, IsEdit = true});
            });

            // Если пользователь соглашается с удалением, то принимается команда и выполняется удаление из БД.
            Messenger.Default.Register<MessDelItemFromDB>(this, x =>
            {
                nc.Types.Remove(SelectedItem);
                nc.SaveChanges();
            });

            // Если подтверждается добавление элемент, то принимается данная команда с именем элемента.
            Messenger.Default.Register<MessNewName>(this, x =>
            {
                if (x.IsEdit)
                {
                    SelectedItem.Name = x.Name;
                    nc.Entry(SelectedItem).State = EntityState.Modified;

                }
                else
                    nc.Types.Add(new Type { Name = x.Name });
                nc.SaveChanges();
            });
        }
    }
}
