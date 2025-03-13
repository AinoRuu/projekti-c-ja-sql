using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{   //posti luokka jossa muuttujina postinumero ja postitoimipaikka 
    public class Posti
    {
        public string Postinumero { get; set; }
        public string Postitoimipaikka { get; set; }

        //muodostin kaikilla posti luokan attribuuteilla
        public Posti(string postinumero, string postitoimipaikka)
        {
            Postinumero = postinumero;
            Postitoimipaikka = postitoimipaikka;
        }

         public Posti(string postinumero)
        {
            Postinumero = postinumero;
        }
        public Posti()
        {
            
        }
    }

    
}
