using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    // Сообщение, оповещающее о том, какое окно надо открыть ("Типы", "Объекты" или "Поставки").
    public class OpenWindowMessage : MessageBase
    {
        public WindowType Type { get; set; }
    }
}
