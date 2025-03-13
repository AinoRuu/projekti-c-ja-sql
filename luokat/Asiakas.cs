using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //asiakas luokka jossa asiakastiedot
    public class Asiakas
    {
        public int Asiakas_id { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Osoite { get; set; }
        public Posti Postinumero { get; set; }
        public string Sahkoposti { get; set; }
        public string PuhelinNumero { get; set; }

        //muodostin kaikilla luokan attribuuteilla
        public Asiakas(int asiakas_id, string etunimi, string sukunimi, string osoite, Posti postinumero, string sahkoposti, string puhelinnumero) 
        {
            Asiakas_id = asiakas_id;
            Etunimi = etunimi;
            Sukunimi = sukunimi;
            Osoite = osoite;
            Postinumero = postinumero;
            Sahkoposti = sahkoposti;
            PuhelinNumero = puhelinnumero;
        }

        public Asiakas()
        {
        }
    }
}
