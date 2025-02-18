using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TruckyV2.Clases;

namespace Logistica.Controllers
{
    public class AccesoController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult resetPassword()
        {
            int loginResult = 0;
            int Nomina = 0;
            if (TempData["loginResult"] != null)
            {
                loginResult = (int)TempData["loginResult"];
                Nomina = (int)TempData["Nomina"];
            }

            ViewBag.loginResult = loginResult;
            ViewBag.Nomina = Nomina;

            return View();
        }

        [HttpPost]
        public ActionResult Login(string Nomina, string Password)
        {
            Conexion conexion = new Conexion();
            string nombre, apellidos, message, code;

            int loginResult = conexion.validateUser(Convert.ToInt32(Nomina), conexion.ConvertSHA256(Password), out nombre, out apellidos, out message, out code);

            if (loginResult != 0)
            {
                if (loginResult == 2)
                {

                    TempData["loginResult"] = loginResult;
                    TempData["Nomina"] = Convert.ToInt32(Nomina);
                    return RedirectToAction("resetPassword", "Acceso");
                }
                else
                {
                    if (loginResult == 3)
                    {
                        ViewBag.Error = message;
                        return View();
                    }
                    else
                    {
                        string fullName = nombre + " " + apellidos;
                        Users user = new Users();

                        user = conexion.getTypeUser().Where(u => u.Nomina == Convert.ToInt32(Nomina)).FirstOrDefault();

                        if (user == null)
                        {
                            ViewBag.Error = "Falta asignar perfil";
                            return View();
                        }
                        else
                        {
                            int tipo = user.Perfil;

                            Session["NombreUsuario"] = fullName;
                            Session["fullName"] = nombre + " " + apellidos;
                            Session["nombre"] = nombre;
                            Session["apellidos"] = apellidos;
                            Session["Tipo"] = tipo;

                            FormsAuthentication.SetAuthCookie(Nomina, false);
                            ViewBag.Error = null;
                            return RedirectToAction("Dashboard", "Home");
                        }

                    }

                }
            }
            else
            {

                ViewBag.Error = message;
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult> resetPassword(string Nomina)
        {
            Conexion conexion = new Conexion();
            Users user = new Users();
            user = conexion.getTypeUser().Where(u => u.Nomina == Convert.ToInt32(Nomina)).FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "El usuario no está registrado, favor de verificar";
                return View();
            }
            else
            {
                // Datos que quieres enviar a la API
                var data = new
                {
                    nomina = Convert.ToInt32(Nomina),
                    IDPlataforma = 5
                };

                // URL de la API
                string apiUrl = "http://192.168.40.240:8099/topre_api/users/reset_password";

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            var apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(responseData);

                            if (apiResponse.Result == 1)
                            {
                                ViewBag.Success = apiResponse.Message;
                                return RedirectToAction("Login", "Acceso");
                            }
                            else
                            {
                                ViewBag.Error = apiResponse.Message;
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Hubo un error al restablecer la contraseña.";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = $"Hubo un error al comunicarse con el servidor: {ex.Message}";
                    }
                }

                return View();
            }
        }


        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon(); // Asegura que se limpie completamente la sesión
            return RedirectToAction("Login", "Acceso");
        }
    }
}