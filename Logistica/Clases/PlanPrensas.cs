using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class PlanPrensas
    {
        public string Linea { get; set; }
        public string NoParte { get; set; }
        public int CantidadEtiquetas { get; set; }
        public bool EsParcial { get; set; }
        public string UsuarioCreacion { get; set; }
        public string Proceso { get; set; }
        public DateTime FechaPlan { get; set; }
    }
}