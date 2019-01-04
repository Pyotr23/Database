using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBNeon.Helpers
{
    // Для сообщения, предназначенного для закрытия окна.
    public class CloseWindow : MessageBase
    {
        public bool Explicite { get; set; }     // Если флаг становлен, то будет осуществляться привязка жлементов окна перед закрытием.
    }
}
