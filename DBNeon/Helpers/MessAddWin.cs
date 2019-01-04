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
    // Для сообщения, открывающего окно добавления,изменения объектов и пр. Параметр - массив имён.
    public class MessOpenAddWin  : MessageBase
    {
        public string[] Names { get; set; }
        public string Name { get; set; }

        public bool IsEdit { get; set; }
    }
}
