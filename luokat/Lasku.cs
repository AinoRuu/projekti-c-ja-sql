using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //luokka laskulle
    public class Lasku
    {
        public int Lasku_id {  get; set; }
        public Varaus Varaus { get; set; }
        public double Summa { get; set; }
        public double ALV { get; set; }
        public bool Maksettu {  get; set; }
        public string MaksettuText { get; set; }

        //muodostin kaikilla attribuuteilla
        public Lasku(int lasku_id, Varaus varaus, double summa, double alv, bool maksettu, string maksettuText)
        {
            Lasku_id = lasku_id;
            Varaus = varaus;
            Summa = summa;
            ALV = alv;
            Maksettu = maksettu;    
            MaksettuText = maksettuText;       
        }

        public Lasku()
        {
        }
    }

}
