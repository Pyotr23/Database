using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.ViewModels
{
    public class VMBlockChange : ViewModelBase
    {
        private string name;
        private string date;             
        private int startPlaceId = -1;
        private string startDescription = "Новый блок";
        //private bool withStart;

        // Описание для самого первого перемещения.
        public string StartDescription
        {
            get => startDescription;
            set
            {
                startDescription = value;
                RaisePropertyChanged(nameof(StartDescription));
            }
        }

        // Id местоположения, из которого происходит первое перемещение.
        public int StartPlaceId
        {
            get => startPlaceId;
            set
            {
                startPlaceId = value;
                RaisePropertyChanged(nameof(startPlaceId));
            }
        }
        
        // Команды OK и Cancel.
        public RelayCommand OkCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }            
       
        // Параметры блока.
        public int Id { get; set; }
        public string Number
        {
            get => name;
            set
            {
                name = value;                
                RaisePropertyChanged(nameof(Number));
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public int? TypeId { get; set; }
        public int? PlaceId { get; set; }

        public string Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public int? OwnerId { get; set; }
        public int? ProcurementId { get; set; }
        public bool Decommissioned { get; set; }
        public string Description { get; set; }

        // Список номеров блоков.
        public List<string> Numbers { get; set; }

        // Свойство, указывающее требуется ли редактирование (true) или добавление нового элемента (false)
        public bool IsEdit { get; set; }         
        
        // Массивы типов, мест и поставок.
        public Models.Type[] Types { get; set; }
        public Location[] Locations { get; set; }
        public Procurement[] Procurements { get; set; }     
        public bool WithoutMoving { get; set; }

        public VMBlockChange()
        {
            Messenger.Default.Register<MessForChangeBlock>(this, ReceiveParamsForAddEdit);

            OkCommand = new RelayCommand(TapOnOk, () => { return Numbers == null ? false : !Numbers.Contains(Number); });

            CancelCommand = new RelayCommand(() => Messenger.Default.Send(new CloseWindow()));
        }

        private void ReceiveParamsForAddEdit(MessForChangeBlock mess)
        {
            IsEdit = mess.IsEdit;
            Numbers = mess.AllBlockNumbers;

            Types = mess.TypesArray.OrderBy(x => x.Name).ToArray();
            Locations = mess.LocationsArray.OrderBy(x => x.Name).ToArray();
            Procurements = mess.ProcurementsArray.OrderBy(x => x.Name).ToArray();

            TypeId = mess.TypeId;
            PlaceId = mess.PlaceId;
            Date = DateTime.Now.ToLongDateString();

            if (IsEdit)
            {
                Number = mess.Number;
                Date = mess.Date;
                OwnerId = mess.OwnerId;
                ProcurementId = mess.ProcurementId;
                Decommissioned = mess.Decommissioned;
                Description = mess.Description;
            }
        }

        private void TapOnOk()
        {
            Messenger.Default.Send(new MessAddNewBlock() { IsEdit = this.IsEdit, NewBlock = new Block()
            {
                Number = Number,
                TypeId = TypeId,
                PlaceId = PlaceId,
                Date = Date,
                OwnerId = OwnerId,
                ProcurementId = ProcurementId,
                Decommissioned = Decommissioned ? 1 : 0,
                Description = Description
            }
            });

            // Если окно открывалось для редактирования, то закрываю после нажатия на ОК.
            if (IsEdit)
                Messenger.Default.Send(new CloseWindow() { Explicite = true });

            // Если добавляется новый блок, то, возможно, заношу его перемещение, как последнее. Добавляю имя его в список номеров.
            else
            {
                Numbers.Add(Number);
                RaisePropertyChanged(nameof(Numbers));
                OkCommand.RaiseCanExecuteChanged();

                // Добавляю новое перемещение в соответсвующую таблицу.
                NeonContext nc = new NeonContext();
                nc.Movings.Load();
                nc.Movings.Add(new Moving()
                {
                    Number = Number,
                    TypeId = TypeId,
                    LocationFromId = StartPlaceId,
                    LocationToId = PlaceId,
                    Date = Date,
                    Explanation = StartDescription
                });
                nc.SaveChanges();

                Messenger.Default.Send(new NotificationMessage("Обновить последнее перемещение"));
            }
        }
    }
}
