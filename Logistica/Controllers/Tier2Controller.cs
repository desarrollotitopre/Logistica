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
        public ActionResult PlanEtiquetas()
        {
            if (Session["Tipo"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignOut", "Acceso");
            }
        }
        public ActionResult ModificarPlan()
        {
            if (Session["Tipo"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignOut", "Acceso");
            }
        }

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
                        registro.Proceso
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

        [HttpPost]
        public JsonResult datosPlanSemanalPrensas(List<PlanEtiquetas> datosXLSX, string nombreUsuario, FechasSemana fechasDias, string nombrePlan, string proceso)
        {
            try
            {

                var registrosParaInsertar = new List<RegistroPlanBD>();

                foreach (var plan in datosXLSX)
                {
                    System.Diagnostics.Debug.WriteLine($"Plan: {plan.linea}, NoParte: {plan.NoParte}, Lunes: {plan.Lunes}, Martes: {plan.Martes}, Miercoles: {plan.Miercoles}, Jueves: {plan.Jueves}, Viernes: {plan.Viernes}, Sabado: {plan.Sabado}");
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Lunes");
                    System.Diagnostics.Debug.WriteLine($"{plan.Lunes}");
                    if (plan.Lunes > 0)
                    {
                        // Verifica si los parámetros son válidos
                        if (string.IsNullOrEmpty(plan.linea) || string.IsNullOrEmpty(nombrePlan) ||
                            string.IsNullOrEmpty(plan.NoParte) || string.IsNullOrEmpty(fechasDias.FechaLunes) ||
                            string.IsNullOrEmpty(nombreUsuario) || plan.LunesParcial == null ||
                            string.IsNullOrEmpty(proceso))
                        {
                            System.Diagnostics.Debug.WriteLine("Uno o más parámetros son nulos o inválidos.");
                            System.Diagnostics.Debug.WriteLine($"linea: {plan.linea}, NoParte: {plan.NoParte}, FechaLunes: {fechasDias.FechaLunes}, nombreUsuario: {nombreUsuario}, plan.LunesParcial: {plan.LunesParcial}, proceso: {proceso}");
                            continue; // Salta este registro
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Los valores no son nulos");
                            System.Diagnostics.Debug.WriteLine($"linea: {plan.linea}, NoParte: {plan.NoParte}, FechaLunes: {fechasDias.FechaLunes}, nombreUsuario: {nombreUsuario}, plan.LunesParcial: {plan.LunesParcial}, proceso: {proceso}");
                        }

                        // Si todos los parámetros son válidos, agrega el registro
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Martes");
                    if (plan.Martes > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Miercoles");
                    if (plan.Miercoles > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Jueves");
                    if (plan.Jueves > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Viernes");
                    if (plan.Viernes > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine($"Comenzando validacion de Sabado");
                    if (plan.Sabado > 0)
                    {
                        registrosParaInsertar.Add(CrearRegistroPrensas(
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
                    System.Diagnostics.Debug.WriteLine("Termino paso en IF");
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

                foreach (var registro in registrosParaInsertar)
                {
                    System.Diagnostics.Debug.WriteLine("Entro al forEach de registro");
                    dataTable.Rows.Add(
                        registro.Linea,
                        registro.TituloPlan,
                        registro.NoParte,
                        registro.FechaPlan,
                        registro.CantidadEtiquetas,
                        registro.UsuarioCreacion,
                        registro.EtiquetaParcial,
                        registro.Proceso
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
                return Json(new { success = false, message = $"Error al procesar los datos: {ex.Message}" });
            }
        }
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