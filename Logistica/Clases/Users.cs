using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class Users
    {
        public int Nomina { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public DateTime Reset { get; set; }
        public int Status { get; set; }
        public int Perfil { get; set; }

    }
}