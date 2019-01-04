using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers 
{
    public class MessNewName : MessageBase
    {
        public string Name { get; set; }
        public bool IsEdit { get; set; }
    }
}
