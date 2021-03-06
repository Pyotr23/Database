﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using DBNeon.Models;
using System.Collections.ObjectModel;
using System.Data.Entity;
using DBNeon.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.ViewModels
{
    public class VMLocations : ViewModelBase
    {
        private NeonContext nc;                         // контекст
        private ObservableCollection<Location> collection;
        private Location selectedItem;
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
        public Location SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                IsSelect = true;        // Если элемент выбран, то IsSelect становится true.
                RaisePropertyChanged(nameof(selectedItem));
            }
        }

        // Коллекция объектов.
        public ObservableCollection<Location> Collection
        {
            get => collection;
            set
            {
                collection = value;
                RaisePropertyChanged(nameof(Collection));
            }
        }

        // Конструктор View-Model.
        public VMLocations()
        {
            // В коллекцию загружаются объекты из БД.
            nc = new NeonContext();
            nc.Locations.Load();
            Collection = nc.Locations.Local;

            // При нажатии на кнопку, реализующую эту команду, отправляется массив имён коллекции. 
            // Это необходимо для исключения повтора названия.
            AddCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessOpenAddWin() { Names = Collection.Select(n => n.Name).ToArray() });
            });

            // При нажатии на "кнопку" "Удалить" отправляется строка типа "Точно удалить объект такой-то?"
            DelCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessDelete() { Phrase = $"объект \"{SelectedItem.Name}\"?" });
            });

            // Если пользователь соглашается с удалением, то принимается команда и выполняется удаление из БД.
            Messenger.Default.Register<MessDelItemFromDB>(this, x =>
            {
                nc.Locations.Remove(SelectedItem);
                nc.SaveChanges();
            });

            // Обработка нажатия кнопки изменения.
            EditCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new MessOpenAddWin() { Names = Collection.Select(n => n.Name).ToArray(), Name = SelectedItem.Name, IsEdit = true });
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
                    nc.Locations.Add(new Location() { Name = x.Name });
                nc.SaveChanges();
            });
        }
    }
}
