using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class PlanEtiquetas
    {
        public string linea { get; set; }
        public string NoParte { get; set; }
        public int Lunes { get; set; }
        public bool LunesParcial { get; set; }
        public int Martes { get; set; }
        public bool MartesParcial { get; set; }
        public int Miercoles { get; set; }
        public bool MiercolesParcial { get; set; }
        public int Jueves { get; set; }
        public bool JuevesParcial { get; set; }
        public int Viernes { get; set; }
        public bool ViernesParcial { get; set; }
        public int Sabado { get; set; }
        public bool SabadoParcial { get; set; }
        public int Domingo { get; set; }
        public bool DomingoParcial { get; set; }
    }
}