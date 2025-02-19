using Logistica.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TruckyV2.Clases;

namespace Logistica.Controllers
{
    public class HomeController : Controller
    {
        /* ============================================= MODULO PERFIL GENERAL============================================ */
        /* =============================================================================================================== */

        [AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]
        public ActionResult Perfil()
        {

            return View();
        }
        [AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]

        [HttpPost]
        public ActionResult ActualizarPerfil(HttpPostedFileBase nuevaFotoPerfil, string Nombre, string Apellidos, string PathPerfilOld)
        {
            Conexion conexion = new Conexion();
            var Nomina = Session["Nomina"].ToString();

            var rutaFotoPerfil = conexion.GuardarImagen(nuevaFotoPerfil, Nomina, PathPerfilOld);

            var (result, message) = conexion.ActualizarUsuario(Nomina, Nombre, Apellidos, rutaFotoPerfil);

            if (result == 1)
            {
                if (!string.IsNullOrEmpty(rutaFotoPerfil)) Session["PathPerfil"] = rutaFotoPerfil;
                Session["nombre"] = Nombre;
                Session["apellidos"] = Apellidos;

                TempData["Message"] = message;
                return RedirectToAction("Perfil");
            }
            else
            {
                TempData["Message"] = message;
                return RedirectToAction("Perfil");
            }
        }

        [AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]
        public ActionResult Dashboard()
        {
            return View();
        }

    }
}