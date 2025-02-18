using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class RegistroActualizacion
    {
        public int Id { get; set; }
        public string NoParte { get; set; }
        public int CantidadEtiquetas { get; set; }
        public int EtiquetaParcial { get; set; }

    }
}