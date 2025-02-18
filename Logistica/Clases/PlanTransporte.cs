using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TruckyV2.Clases
{
    public class PlanTransporte
    {
        public int Folio { get; set; }
        public string ASN { get; set; }
        public string FechaPlan { get; set; }
        public string HrLlegadaPlan { get; set; }
        public string HrSalidaPlan { get; set; }
        public string Proveedor { get; set; }
        public string Proyecto { get; set; }
        public string Unidad { get; set; }
        public string LineaTrans { get; set; }
        public string Carga { get; set; }
        public string Proceso { get; set; }
        public string LlegadaReal { get; set; }
        public string EntRampaReal { get; set; }
        public string SalRampaReal { get; set; }
        public string SalidaReal { get; set; }
        public string Area { get; set; }
        public string Rampa { get; set; }
        public string AutorizoRampa { get; set; }
        public string CargadoPor { get; set; }
        public int Status { get; set; }
        public string FechaCarga { get; set; }
    }
}