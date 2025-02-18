using Logistica.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TruckyV2.Clases;

namespace Logistica.Controllers
{
    public class TransportesController : Controller
    {
        [AuthorizeModule(1, 2, 3, 4, 5)]
        public ActionResult Dashboard()
        {
            return View();
        }
        [AuthorizeModule(1, 2, 3, 4, 5)]
        public ActionResult CargaPlan()
        {
            return View();
        }
        [AuthorizeModule(1, 2, 3, 4, 5)]

        public ActionResult gestionFolio()
        {
            return View();
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        public bool IsSessionActive()
        {
            try
            {
                // Verifica si el usuario está autenticado
                if (!User.Identity.IsAuthenticated)
                {
                    return false; // El usuario no está autenticado
                }

                // Verifica si el ticket de autenticación existe
                HttpCookie authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                {
                    return false; // No hay cookie de autenticación
                }

                // Intenta descifrar el ticket de autenticación
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket == null)
                {
                    return false; // El ticket no es válido
                }

                if (authTicket.Expired)
                {
                    Debug.WriteLine("============== Ticket expirado ===============");
                    return false; // El ticket ha expirado
                }

                if (authTicket.Expiration <= DateTime.Now.AddSeconds(15))
                {
                    return false; // La fecha de expiración es anterior a la fecha actual
                }

                return true; // La sesión está activa
            }
            catch (Exception ex)
            {
                Debug.WriteLine("============== Error en IsSessionActive: " + ex.Message + " ===============");
                return false; // Error al descifrar el ticket de autenticación
            }
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult getTablePlan(string dateFind)
        {
            Conexion conexion = new Conexion();
            List<PlanTransporte> listPlanLog = conexion.getTablePlan(dateFind);
            if (IsSessionActive() == false)
            {
                return Json(new { SessionActive = false });
            }

            return Json(new { data = listPlanLog, SessionActive = true }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult getDetailFolio(int Folio)
        {
            Conexion conexion = new Conexion();
            List<PlanTransporte> getDetailFolio = conexion.getDetailFolio(Folio);

            return Json(new { data = getDetailFolio }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult getDataTransportista(int Folio)
        {
            Conexion conexion = new Conexion();
            List<Transportistas> listPlanLog = conexion.getDataTransportista(Folio);

            return Json(new { data = listPlanLog }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult changeStatusPlan(int Folio, int Status, string Rampa, string AutorizedBy)
        {
            Conexion conexion = new Conexion();
            bool result = conexion.ChangeStatusAndInsert(Folio, Status, Rampa, AutorizedBy);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);

        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult inserDataRegsProvsT2(int Folio, string AutorizedBy, string transportista, string Empresa, string placas, string origen, string gafete, string contenedor, string tracto)
        {
            Conexion conexion = new Conexion();
            bool result = conexion.inserDataRegsProvsT2(Folio, AutorizedBy, transportista, Empresa, placas, origen, gafete, contenedor, tracto);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);

        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult InsertarDatosCSV(List<string[]> datosCSV, string nombreUsuario)
        {
            Conexion conexion = new Conexion();
            bool result = false;
            int insertados = 0;
            int total = 0;
            string Message = string.Empty;

            if (datosCSV == null)
            {
                return Json(new { result = false, error = "La lista de datos CSV es null" });
            }


            List<PlanTransporte> listaDatos = new List<PlanTransporte>();
            foreach (var fila in datosCSV)
            {
                listaDatos.Add(new PlanTransporte
                {
                    FechaPlan = fila[0],
                    Proveedor = fila[1],
                    Proyecto = fila[2],
                    Unidad = fila[3],
                    LineaTrans = fila[4],
                    Carga = fila[5],
                    HrLlegadaPlan = fila[6],
                    HrSalidaPlan = fila[7],
                    Proceso = fila[8],
                    ASN = fila[9],
                    Area = fila[10]
                });
            }

            if (listaDatos.Count > 0)
            {
                (insertados, total, Message) = conexion.InsertarDatosPlanCSV(nombreUsuario, listaDatos);


            }
            return Json(new { total = total, insertados = insertados, Message = Message }, JsonRequestBehavior.AllowGet);

            //return Json(new { result = result });
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult cancelFolio(int Folio, string AutorizedBy)
        {
            Conexion conexion = new Conexion();
            bool result = conexion.cancelFolio(Folio, AutorizedBy);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult resetFullFolio(int Folio, string AutorizedBy)
        {
            Conexion conexion = new Conexion();
            bool result = conexion.resetFolio(Folio, AutorizedBy);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);


        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult resetStepFolio(int Folio, int Tipo, string AutorizedBy)
        {
            Conexion conexion = new Conexion();
            int result = 0;
            string Message = string.Empty;
            result = conexion.resetStepFolio(Folio, Tipo, AutorizedBy, out Message);
            return Json(new { result = result, Message = Message }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult updateFolio(int Folio, string AutorizedBy, string FechaPlan, string HrLlegadaPlan, string HrSalidaPlan, string Proveedor, string Proyecto, string Unidad, string Carga, string LineaTrans, string ASN)
        {
            Conexion conexion = new Conexion();
            bool result = conexion.updateFolio(Folio, AutorizedBy, FechaPlan, HrLlegadaPlan, HrSalidaPlan, Proveedor, Proyecto, Unidad, Carga, LineaTrans, ASN);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);


        }

        [AuthorizeModule(1, 2, 3, 4, 5)]
        [HttpPost]
        public JsonResult getFilterCustomers(string dateFind)
        {
            Conexion conexion = new Conexion();
            List<PlanTransporte> listCustomer = conexion.getFilterCustomers(dateFind);
            return Json(new { data = listCustomer }, JsonRequestBehavior.AllowGet);
        }
    }
}