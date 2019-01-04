using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    public class MessHideColumns : MessageBase
    {
        public bool Hide { get; set; }
    }
}
