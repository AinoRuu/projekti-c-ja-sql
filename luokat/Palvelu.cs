using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //palvelu luokka 
    public class Palvelu
    {
        public int Palvelu_id {  get; set; }
        public Alue Alue { get; set; }
        public string Nimi { get; set; }
        public string Kuvaus { get; set; }
        public double Hinta { get; set; }
        public double ALV { get; set; }

        //muodostin kaikilla attribuuteilla
        public Palvelu(int palvelu_id, Alue alue, string nimi, string kuvaus, double hinta, double alv) 
        {
            Palvelu_id = palvelu_id;
            Alue = alue;
            Nimi = nimi;
            Kuvaus = kuvaus;
            Hinta = hinta;
            ALV = alv;
        }
    }
}
