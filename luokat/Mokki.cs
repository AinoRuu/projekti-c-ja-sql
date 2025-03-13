using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //luokka jossa mökin tiedot
    public class Mokki
    {
        public int Mokki_id { get; set; }
        public Alue Alue { get; set; }
        public Posti Postinumero { get; set; }
        public string Nimi {  get; set; }
        public string Osoite { get; set; }
        public double Hinta { get; set; }
        public string Kuvaus { get; set; }
        public int Henkilomaara { get; set; }
        public string Varustelu { get; set; }

         public Mokki()
        {

        }
        public Mokki( int mokki_id, Alue alue, Posti postinumero, string nimi, string osoite, double hinta, string kuvaus, int henkiloMaara, string varustelu) 
        {
            Mokki_id = mokki_id;
            Alue = alue;
            Postinumero = postinumero;
            Nimi = nimi;
            Osoite = osoite;
            Hinta = hinta;
            Kuvaus = kuvaus;
            Henkilomaara = henkiloMaara;
            Varustelu = varustelu;
        }
    }
}
