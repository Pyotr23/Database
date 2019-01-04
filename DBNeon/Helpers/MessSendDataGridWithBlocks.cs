using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    public class MessSendDataGridWithBlocks : MessageBase
    {
        public DataGrid DtGrid { get; set; }
    }
}
