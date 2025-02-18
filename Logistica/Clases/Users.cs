using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class Users
    {
        public string PathPerfil { get; set; }
        public int Nomina { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public int Cantidad { get; set; }
        public List<int> VistasPermitidas { get; set; } = new List<int>();
        public List<string> IDDepartamentos { get; set; } = new List<string>();

    }
}