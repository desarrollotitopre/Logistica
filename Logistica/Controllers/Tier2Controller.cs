using Logistica.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TruckyV2.Clases;

namespace Logistica.Controllers
{
    public class Tier2Controller : Controller
    {

        [AuthorizeModule(1)]
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

        /////////   Vistas
        [AuthorizeModule(1)]
        public ActionResult PlanEtiquetas()
        {
            return View();

        }
        [AuthorizeModule(1)]
        public ActionResult ModificarPlan()
        {
            return View();

        }
        [AuthorizeModule(1)]
        public ActionResult ConsultarPlanes()
        {
            return View();
            
        }
        [AuthorizeModule(1)]
        public ActionResult CatalogoEtiquetas()
        {
            return View();
        }

        ////// Plan etiquetas
        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult datosPlanSemanalPrensas(List<PlanPrensas> datosXLSX, string nombreUsuario, FechasSemana fechasDias, string nombrePlan, string proceso)
        {
            try
            {
                if (fechasDias == null || string.IsNullOrEmpty(fechasDias.FechaLunes))
                {
                    throw new Exception("El objeto fechasDias es nulo o no contiene la fecha del Lunes.");
                }

                var primeraFecha = fechasDias.FechaLunes;
                Console.WriteLine("Fecha del Lunes recibida: " + primeraFecha);

                var registrosParaInsertar = new List<RegistroPlanBD>();

                foreach (var plan in datosXLSX)
                {
                    registrosParaInsertar.Add(CrearRegistroPrensas(
                            plan.Linea,
                            nombrePlan,
                            plan.NoParte,
                            primeraFecha,
                            plan.CantidadEtiquetas,
                            nombreUsuario,
                            plan.EsParcial,
                            proceso
                    ));
                }


                Conexion conexion = new Conexion();
                bool result = false;
                string Message = string.Empty;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Linea", typeof(string));
                dataTable.Columns.Add("TituloPlan", typeof(string));
                dataTable.Columns.Add("NoParte", typeof(string));
                dataTable.Columns.Add("FechaPlan", typeof(DateTime));
                dataTable.Columns.Add("CantidadEtiquetas", typeof(int));
                dataTable.Columns.Add("UsuarioCreacion", typeof(string));
                dataTable.Columns.Add("EtiquetaParcial", typeof(int));
                dataTable.Columns.Add("Proceso", typeof(string));
                dataTable.Columns.Add("CantEtqSoli", typeof(int));

                foreach (var registro in registrosParaInsertar)
                {
                    System.Diagnostics.Debug.WriteLine("Entro al forEach de registro");

                    System.Diagnostics.Debug.WriteLine("Fecha de la tabla: " + primeraFecha);
                    dataTable.Rows.Add(
                        registro.Linea,
                        registro.TituloPlan,
                        registro.NoParte,
                        primeraFecha,
                        registro.CantidadEtiquetas,
                        registro.UsuarioCreacion,
                        registro.EtiquetaParcial,
                        registro.Proceso,
                        registro.CantidadEtiquetas
                    );
                }

                System.Diagnostics.Debug.WriteLine("Procesando dateTable a InsertarDatosPlanXLSX");

                var resultado = conexion.InsertarDatosPlanXLSX(dataTable);

                if (!resultado)
                {
                    throw new Exception("Error al insertar los registros.");
                }

                return Json(new { success = true, result = result, Message = "Datos guardados existosamente" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al procesar los datos: " + ex.Message);
                return Json(new { success = false, message = $"Error al procesar los datos: {ex.Message}" });

            }
        }

        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult datosPlanSemanal(List<PlanEtiquetas> datosXLSX, string nombreUsuario, FechasSemana fechasDias, string nombrePlan, string proceso)
        {
            try
            {
                var registrosParaInsertar = new List<RegistroPlanBD>();

                foreach (var plan in datosXLSX)
                {
                    if (plan.Lunes > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaLunes,
                            plan.Lunes,
                            nombreUsuario,
                            plan.LunesParcial,
                            proceso
                        ));
                    }

                    if (plan.Martes > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaMartes,
                            plan.Martes,
                            nombreUsuario,
                            plan.MartesParcial,
                            proceso
                        ));
                    }

                    if (plan.Miercoles > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaMiercoles,
                            plan.Miercoles,
                            nombreUsuario,
                            plan.MiercolesParcial,
                            proceso
                        ));
                    }

                    if (plan.Jueves > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaJueves,
                            plan.Jueves,
                            nombreUsuario,
                            plan.JuevesParcial,
                            proceso
                        ));
                    }

                    if (plan.Viernes > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaViernes,
                            plan.Viernes,
                            nombreUsuario,
                            plan.ViernesParcial,
                            proceso
                        ));
                    }

                    if (plan.Sabado > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaSabado,
                            plan.Sabado,
                            nombreUsuario,
                            plan.SabadoParcial,
                            proceso
                        ));
                    }

                    if (plan.Domingo > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistro(
                            plan.linea,
                            nombrePlan,
                            plan.NoParte,
                            fechasDias.FechaDomingo,
                            plan.Domingo,
                            nombreUsuario,
                            plan.DomingoParcial,
                            proceso
                        ));
                    }
                }


                Conexion conexion = new Conexion();
                bool result = false;
                string Message = string.Empty;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Linea", typeof(string));
                dataTable.Columns.Add("TituloPlan", typeof(string));
                dataTable.Columns.Add("NoParte", typeof(string));
                dataTable.Columns.Add("FechaPlan", typeof(DateTime));
                dataTable.Columns.Add("CantidadEtiquetas", typeof(int));
                dataTable.Columns.Add("UsuarioCreacion", typeof(string));
                dataTable.Columns.Add("EtiquetaParcial", typeof(int));
                dataTable.Columns.Add("Proceso", typeof(string));
                dataTable.Columns.Add("CantEtqSoli", typeof(int));

                foreach (var registro in registrosParaInsertar)
                {
                    dataTable.Rows.Add(
                        registro.Linea,
                        registro.TituloPlan,
                        registro.NoParte,
                        registro.FechaPlan,
                        registro.CantidadEtiquetas,
                        registro.UsuarioCreacion,
                        registro.EtiquetaParcial,
                        registro.Proceso,
                        registro.CantidadEtiquetas
                    );

                }
                var resultado = conexion.InsertarDatosPlanXLSX(dataTable);

                if (!resultado)
                {
                    throw new Exception("Error al insertar los registros.");
                }

                return Json(new { success = true, result = result, Message = "Datos guardados existosamente" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al procesar los datos: {ex.Message}" });
            }
        }

        ////// Modificar Plan
        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult consultarPlan(string proceso, string fecha)
        {
            try
            {
                Conexion conexion = new Conexion();
                var registrosConsultados = conexion.consultarPlanes(proceso, fecha);
                if (registrosConsultados != null && registrosConsultados.Any())
                {
                    return Json(new { success = true, data = registrosConsultados }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = $"No se encontraron registros de {proceso} y {fecha}" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consultar los datos: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult actualizarPlan(string proceso, string fecha, List<RegistroActualizacion> registros)
        {
            try
            {
                Conexion conexion = new Conexion();
                bool actualizado = conexion.actualizarMultiplesRegistros(proceso, fecha, registros);
                return Json(new { success = actualizado });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //////   Consultar Planes
        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult consultarListaLineas(string planSeleccionado)
        {
            try
            {
                if (string.IsNullOrEmpty(planSeleccionado))
                {
                    return Json(new { success = false, message = "El plan seleccionado no puede estar vacío." });
                }
                Conexion conexion = new Conexion();
                List<String> listaLineas = conexion.cargarLineas(planSeleccionado);
                var data = listaLineas.Select(p => new { nombre = p }).ToList();
                return Json(new { success = true, data = listaLineas });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult obtenerResumen(string plan, string linea, string fecha, bool planCompleto)
        {
            try
            {
                if (string.IsNullOrEmpty(plan) || string.IsNullOrEmpty(fecha))
                {
                    return Json(new { success = false, message = "Datos incompletos." });
                }
                Conexion conexion = new Conexion();
                List<ResumenPlan> datosResumen = null;

                if (planCompleto)
                {
                    datosResumen = conexion.ObtenerResumenCompleto(plan, fecha);
                }
                else
                {
                    datosResumen = conexion.ObtenerResumenPorLinea(plan, fecha, linea);
                }

                return Json(new { success = true, data = datosResumen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult obtenerDetalleEtiquetas(string noParte, string proceso, string fecha)
        {
            try
            {
                if (string.IsNullOrEmpty(noParte))
                {
                    return Json(new { success = false, message = "No se ha proporcionado el número de parte." });
                }
                Conexion conexion = new Conexion();
                List<string> listaDetalles = conexion.consultarDetalleEtiquetasEntrega(noParte, proceso, fecha);
                return Json(new { success = true, data = listaDetalles });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //////   Catalogo Etiquetas
        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult consultarEtiquetas()
        {
            try
            {
                Conexion conexion = new Conexion();
                List<object> listaPlanes = conexion.consultarEtiquetas();
                return Json(new { success = true, data = listaPlanes }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeModule(1)]
        [HttpPost]
        public JsonResult consultarDetalleEtiquetas(string noParte)
        {
            try
            {
                Conexion conexion = new Conexion();
                List<object> listaDetalles = conexion.consultarDetalleEtiquetas(noParte);
                return Json(new { success = true, data = listaDetalles }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        //////   Funciones adicionales
        [AuthorizeModule(1)]
        private RegistroPlanBD CrearRegistro(string linea, string nombrePlan, string noParte, string fecha, int cantidad, string usuario, bool esParcial, string proceso)
        {
            return new RegistroPlanBD
            {
                Linea = linea,
                TituloPlan = nombrePlan,
                NoParte = noParte,
                FechaPlan = DateTime.ParseExact(fecha, "yyyy/MM/dd", null),
                CantidadEtiquetas = cantidad,
                UsuarioCreacion = usuario,
                EtiquetaParcial = esParcial ? 1 : 0,
                Proceso = proceso
            };
        }
        [AuthorizeModule(1)]
        private RegistroPlanBD CrearRegistroPrensas(string linea, string nombrePlan, string noParte, string fecha, int cantidad, string usuario, bool esParcial, string proceso)
        {
            return new RegistroPlanBD
            {
                Linea = linea,
                TituloPlan = nombrePlan,
                NoParte = noParte,
                FechaPlan = DateTime.ParseExact(fecha, "yyyy/MM/dd", null),
                CantidadEtiquetas = cantidad,
                UsuarioCreacion = usuario,
                EtiquetaParcial = 0,
                Proceso = proceso
            };
        }

        private int ConvertToInt(String value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return int.TryParse(value, out int result) ? result : 0;
        }
    }
}