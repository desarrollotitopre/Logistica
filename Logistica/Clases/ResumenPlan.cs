using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class ResumenPlan
    {
        public string Linea { get; set; }
        public string NoParte { get; set; }
        public string CantidadEtiquetas{ get; set; }
        public string EtiquetasParciales { get; set; }
        public string UsuarioRecibio { get; set; }
        public string FechaEntrega { get; set; }
        public string FechaRecibido { get; set; }
    }
}