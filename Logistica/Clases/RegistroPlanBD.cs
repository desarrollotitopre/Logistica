using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class RegistroPlanBD
    {
        public string Linea { get; set; }
        public string TituloPlan { get; set; }
        public string NoParte { get; set; }
        public DateTime FechaPlan { get; set; }
        public int CantidadEtiquetas { get; set; }
        public string UsuarioCreacion { get; set; }
        public int EtiquetaParcial { get; set; }
        public string Proceso { get; set; }
    }
}