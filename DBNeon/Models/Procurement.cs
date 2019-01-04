using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace DBNeon.Models
{
    public class Procurement : ViewModelBase
    {
        private string name;
        public int Id { get; set; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
    }
}
