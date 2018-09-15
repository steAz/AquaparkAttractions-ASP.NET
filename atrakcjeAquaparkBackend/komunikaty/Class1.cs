using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace komunikaty
{ 
    public interface IKomunikat
    {
        string Tekst { get; }
        DateTime Dzien { get; }
    }

    public class Komunikat : IKomunikat
    {
        public string Tekst { get; set; }
        public DateTime Dzien { get; set; }
    }

    public class Auth : IKomunikat
    {
        public string Tekst { get; set; }
        public DateTime Dzien { get; set; }
    }
}
