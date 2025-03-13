using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //luokka varauksen palveluille
    public class Varauksen_palvelut
    {
        public Varaus Varaus { get; set; }
        public Palvelu Palvelu { get; set; }
        public int lukumaara { get; set; }

        public Varauksen_palvelut(Varaus varaus, Palvelu palvelu, int lukumaara)
        {
            Varaus = varaus;
            Palvelu = palvelu;
            this.lukumaara = lukumaara;
        }
    }
}
