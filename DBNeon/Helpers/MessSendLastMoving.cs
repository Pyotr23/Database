using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    public class MessSendLastMoving : MessageBase
    {
        public DateTime Date { get; set; }
        public string Number { get; set; }
    }
}
