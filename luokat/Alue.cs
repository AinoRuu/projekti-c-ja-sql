using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokinVuokrausApp.luokat
{
    //luokka Alueelle joissa luokan muuttujinan alueen ID numero ja nimi
    public class Alue
    {
        public int Alue_Id { get; set; }
        public string Nimi { get; set; }

        public Alue()
        {

        }
        //muodostin alueen kaikilla attribuuteilla
        public Alue(int alue_Id, String nimi) 
        {
            Alue_Id = alue_Id;
            Nimi = nimi;

        }
    }
}
