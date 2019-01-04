using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBNeon.Models;
using Type = DBNeon.Models.Type;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    public class MessForChangeBlock : MessageBase
    {
        public bool IsEdit { get; set; }
        public string Number { get; set; }
        public int? TypeId { get; set; }
        public int? PlaceId { get; set; }
        public string Date { get; set; }
        public int? OwnerId { get; set; }
        public int? ProcurementId { get; set; }
        public bool Decommissioned { get; set; }
        public string Description { get; set; }
        public List<string> AllBlockNumbers { get; set; }
        public Type[] TypesArray { get; set; }
        public Location[] LocationsArray { get; set; }
        public Procurement[] ProcurementsArray { get; set; }
    }
}
