using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //luokka mökin varaukselle
    public class Varaus
    {
        public int Varaus_id { get; set; }
        public Asiakas Asiakas { get; set; }
        public Mokki Mokki { get; set; }
        public DateTime Varattu_pvm { get; set; }
        public DateTime Vahvistus_pvm { get; set; }
        public DateTime Varattu_AlkuPvm { get; set; }
        public DateTime Varattu_LoppuPvm { get; set; }


        //muodostin kaikilla varaus luokan attribuuteilla
        public Varaus (int varaus_id, Asiakas asiakas, Mokki mokki, DateTime varattu_pvm, DateTime vahvistus_pvm, DateTime varattu_AlkuPvm, DateTime varattu_LoppuPvm)
        {
            Varaus_id = varaus_id;
            Asiakas = asiakas;
            Mokki = mokki;
            Varattu_pvm = varattu_pvm;
            Vahvistus_pvm = vahvistus_pvm;
            Varattu_AlkuPvm = varattu_AlkuPvm;
            Varattu_LoppuPvm = varattu_LoppuPvm;
        }

        public Varaus()
        {
        }
    }
}
