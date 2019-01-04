using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBNeon.Models;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    public class MessAddNewBlock : MessageBase
    {
        public Block NewBlock { get; set; }
        public bool IsEdit { get; set; }
    }
}
