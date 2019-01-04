using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace DBNeon.Models
{
    public class Block : ViewModelBase
    {
        private string number;
        private int? typeId;
        private int? placeId;
        private string date;
        private int? ownerId;
        private int? procurementId;
        private int decommissioned;
        private string description;
        public int Id { get; set; }

        public string Number
        {
            get => number;
            set
            {
                number = value;
                RaisePropertyChanged(nameof(Number));
            }
        }

        public int? TypeId
        {
            get => typeId;
            set
            {
                typeId = value;
                RaisePropertyChanged(nameof(TypeId));
            }
        }

        public int? PlaceId
        {
            get => placeId;
            set
            {
                placeId = value;
                RaisePropertyChanged(nameof(PlaceId));
            }
        }

        public string Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(date));
            }
        }

        public int? OwnerId
        {
            get => ownerId;
            set
            {
                ownerId = value;
                RaisePropertyChanged(nameof(OwnerId));
            }
        }
        public int? ProcurementId
        {
            get => procurementId;
            set
            {
                procurementId = value;
                RaisePropertyChanged(nameof(ProcurementId));
            }
        }

        public int Decommissioned
        {
            get => decommissioned;
            set
            {
                decommissioned = value;
                RaisePropertyChanged(nameof(Decommissioned));
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }
    }
}
