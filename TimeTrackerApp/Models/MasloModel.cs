using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.ModelMasla
{
    public class RodzajMasla
    {
        public string Nazwa { get; set; }
        public int ProcentTluszczyku { get; set; }
    }

    public class MasloModel
    {
        public List<RodzajMasla> masla { get; set; } = new List<RodzajMasla>() 
        {
            new RodzajMasla() { Nazwa = "Łaciate", ProcentTluszczyku = 82 },
            new RodzajMasla() { Nazwa = "Smarowidło Mix", ProcentTluszczyku = 53 }
        };
    }
}
