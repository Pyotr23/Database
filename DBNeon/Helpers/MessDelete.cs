using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBNeon.Models;
using GalaSoft.MvvmLight.Messaging;

namespace DBNeon.Helpers
{
    // Сообщение для вызова диалогового окна для подтверждения удаления.
    // Пример параметра - "Точно удалить тип "АВВС"?"
    public class MessDelete : MessageBase
    {
        const string del = "Точно удалить ";
        private string phrase;

        public string Phrase
        {
            get => phrase;
            set { phrase = del + value; }
        }
    }
}
