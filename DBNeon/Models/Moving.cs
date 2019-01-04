using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBNeon.Models
{
    public class Moving
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int? TypeId { get; set; }
        public int? LocationFromId { get; set; }
        public int? LocationToId { get; set; }
        public string Date { get; set; }
        public string Explanation { get; set; }
    }
}
