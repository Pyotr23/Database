using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.ViewModels
{
    public class VMAddMoving : ViewModelBase
    {
        private NeonContext nc;
        private Block selectedBlock;
        private string type;
        private string locationFrom;
        private Location locationTo;
        private bool isEnable;
        private DateTime date = DateTime.Now;
        private ObservableCollection<Block> blocks;
        private string success;

        public string Success
        {
            get => success;
            set
            {
                success = value;
                RaisePropertyChanged(nameof(Success));
            }
        }
        public bool IsEnable
        {
            get => isEnable;
            set
            {
                isEnable = value;
                Success = "";
                RaisePropertyChanged(nameof(IsEnable));
            }
        }
        public ObservableCollection<Block> Blocks
        {
            get => blocks;
            set
            {
                blocks = value;
                RaisePropertyChanged(nameof(Blocks));
            } 
        }
        public ObservableCollection<Models.Type> Types { get; set; }
        public ObservableCollection<Location> Locations { get; set; }

        public RelayCommand Relocate { get; set; }
        public RelayCommand Close { get; set; }

        public string Description { get; set; }

        public Location LocationTo
        {
            get => locationTo;
            set
            {
                locationTo = value;
                IsEnable = locationFrom != LocationTo.Name;
                RaisePropertyChanged(nameof(LocationTo));
            }
        }

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public string LocationFrom
        {
            get => locationFrom;
            set
            {
                locationFrom = value;
                if (LocationTo != null)
                    IsEnable = locationFrom != LocationTo.Name;
                else
                    IsEnable = false;
                RaisePropertyChanged(nameof(LocationFrom));
            }
        } 

        public string Type {
            get => type;
            set
            {
                type = value;
                RaisePropertyChanged(nameof(Type));
            }
        }
        
        public Block SelectedBlock
        {
            get => selectedBlock;
            set
            {
                selectedBlock = value;
                Type = Types.SingleOrDefault(t => t.Id == SelectedBlock.TypeId).Name;
                LocationFrom = Locations.SingleOrDefault(l => l.Id == SelectedBlock.PlaceId).Name;
                Success = "";
                RaisePropertyChanged(nameof(SelectedBlock));
            }
        }
        
        public VMAddMoving()
        {
            nc = new NeonContext();
            nc.Blocks.Load();
            Blocks = nc.Blocks.Local;
            nc.Types.Load();
            Types = nc.Types.Local;
            nc.Locations.Load();
            Locations = nc.Locations.Local;

            Relocate = new RelayCommand(() =>
            {
                nc = new NeonContext();
                nc.Movings.Load();
                nc.Movings.Add(new Moving()
                {
                    Number = SelectedBlock.Number,
                    TypeId = SelectedBlock.TypeId,
                    LocationFromId = SelectedBlock.PlaceId,
                    LocationToId = LocationTo.Id,
                    Date = this.Date.ToShortDateString(),
                    Explanation = Description
                });

                Messenger.Default.Send(new MessSendLastMoving() { Date = Date, Number = SelectedBlock.Number});
                
                SelectedBlock.PlaceId = LocationTo.Id;
                SelectedBlock.Date = Date.ToShortDateString();
                nc.Entry(SelectedBlock).State = EntityState.Modified;
                nc.SaveChanges();

                LocationFrom = LocationTo.Name;
                Success = "Успех!";
            });
            Close = new RelayCommand(() => Messenger.Default.Send(new CloseWindow()));
        }
    }
}
