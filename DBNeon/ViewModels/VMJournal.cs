using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DBNeon.Helpers;
using DBNeon.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Block = System.Windows.Documents.Block;

namespace DBNeon.ViewModels
{
    public class VMJournal : ViewModelBase
    {
        private List<MovingForJournal> listOfNewMovings;
        private ObservableCollection<Models.Type> types;
        private ObservableCollection<Location> locations;
        private string selectedBlock;
        private Location selectedLoc;
        private DateTime dateForSearch = DateTime.Now;
        private Visibility enableButton = Visibility.Hidden;
        private bool enabledBlock = true;
        private bool enabledPlace = true;
        private bool enabledDate = true;

        private NeonContext nc;

        public bool EnabledBlock
        {
            get => enabledBlock;
            set
            {
                enabledBlock = value;
                RaisePropertyChanged(nameof(EnabledBlock));
            }
        }

        public bool EnabledPlace
        {
            get => enabledPlace;
            set
            {
                enabledPlace = value;
                RaisePropertyChanged(nameof(EnabledPlace));
            }
        }

        public bool EnabledDate
        {
            get => enabledDate;
            set
            {
                enabledDate = value;
                RaisePropertyChanged(nameof(EnabledDate));
            }
        }

        public Visibility EnableButton
        {
            get => enableButton;
            set
            {
                enableButton = value;
                RaisePropertyChanged(nameof(EnableButton));
            }
        }
        public RelayCommand ResetCommand { get; set; }
        public DateTime DateForSearch
        {
            get => dateForSearch;
            set
            {
                dateForSearch = value;
                if (value != DateTime.Now)
                {
                    EnableButton = Visibility.Visible;
                    EnabledBlock = false;
                    EnabledPlace = false;
                }
                Messenger.Default.Send(new MessDateForScroll() { MovingItem = GetNearestMoving(value) });                
                RaisePropertyChanged(nameof(DateForSearch));
            }
        }
        public List<Moving> Movings { get; set; }

        public Location SelectedLoc
        {
            get => selectedLoc;
            set
            {
                selectedLoc = value;
                if (value != null)
                {
                    ListOfNewMovings =
                        ConvertMovings(Movings.Where(m => m.LocationFromId == selectedLoc.Id | m.LocationToId == selectedLoc.Id).ToList());
                    EnableButton = Visibility.Visible;
                    EnabledBlock = false;
                    EnabledDate = false;
                }
                    
                RaisePropertyChanged(nameof(SelectedLoc));
            }
        }
        public string SelectedBlock
        {
            get => selectedBlock;
            set
            {
                selectedBlock = value;
                if (value != null)
                {
                    ListOfNewMovings = ConvertMovings(Movings.Where(n => n.Number == value).ToList());
                    EnableButton = Visibility.Visible;
                    EnabledDate = false;
                    EnabledPlace = false;
                }
                RaisePropertyChanged(nameof(SelectedBlock));
            }
        }
        public string[] BlocksName { get; set; }
        public ObservableCollection<Location> Locations
        {
            get => locations;
            set
            {
                locations = value;
                RaisePropertyChanged(nameof(Locations));
            }
        }
        public ObservableCollection<Models.Type> Types
        {
            get => types;
            set
            {
                types = value;
                RaisePropertyChanged(nameof(Types));
            }
        }
        public List<MovingForJournal> ListOfNewMovings
        {
            get => listOfNewMovings;
            set
            {
                listOfNewMovings = value;
                RaisePropertyChanged(nameof(ListOfNewMovings));
            }
        }

        // Конструкторы
        public VMJournal()
        {
            nc = new NeonContext(); 
            nc.Movings.Load();
            Movings = nc.Movings.ToList();
            ListOfNewMovings = ConvertMovings(Movings);
            nc.Types.Load();
            Types = nc.Types.Local;
            nc.Locations.Load();
            Locations = nc.Locations.Local;
            nc.Blocks.Load();
            BlocksName = nc.Blocks.Local.Select(x => x.Number).ToArray();

            ResetCommand = new RelayCommand(() =>
            {
                ListOfNewMovings = ConvertMovings(Movings);
                DateForSearch = DateTime.Now;
                SelectedLoc = null;
                SelectedBlock = null;
                EnableButton = Visibility.Hidden;
                EnabledDate = true;
                EnabledPlace = true;
                EnabledBlock = true;
            });
        }

        // Методы
        public List<MovingForJournal> ConvertMovings(List<Moving> simplyMovings)
        {
            List<MovingForJournal> newList = new List<MovingForJournal>();
            var distinctDate = simplyMovings.Select(d => DateTime.Parse(d.Date)).Distinct().OrderByDescending(d => d).ToList();
            foreach (DateTime date in distinctDate)
            {
                List<Moving> movingsThisDate = simplyMovings.Where(m => m.Date == date.ToShortDateString()).OrderByDescending(u => u.Id).ToList();
                newList.Add(new MovingForJournal(){ Date = date.ToLongDateString(), NewMovings = movingsThisDate});
            }
            return newList.ToList();
        }

        public MovingForJournal GetNearestMoving(DateTime data)
        {
            var movingUnderData = ListOfNewMovings.FirstOrDefault(d => Convert.ToDateTime(d.Date) <= DateForSearch);
            if (movingUnderData == null)
                return ListOfNewMovings.ElementAtOrDefault(ListOfNewMovings.Count() - 1);
            return movingUnderData;
        }
    }
}
