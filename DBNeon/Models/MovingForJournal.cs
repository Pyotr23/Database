using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBNeon.Models
{
    public class MovingForJournal
    {
        public string Date { get; set; }
        public List<Moving> NewMovings { get; set; }
    }
}
