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

        //[AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]
        public ActionResult Perfil()
        {

            return View();
        }
        //[AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]

        //[HttpPost]
        //public ActionResult ActualizarPerfil(HttpPostedFileBase nuevaFotoPerfil, HttpPostedFileBase nuevaFirma, string Nombre, string Apellidos, string PathPerfilOld, string PathFirmaOld)
        //{
        //    Conexion conexion = new Conexion();
        //    var Nomina = Session["Nomina"].ToString();

        //    var rutaFotoPerfil = conexion.GuardarImagen(nuevaFotoPerfil, Nomina, "perfil", PathPerfilOld);
        //    var rutaFirma = conexion.GuardarImagen(nuevaFirma, Nomina, "firma", PathFirmaOld);

        //    var (result, message) = conexion.ActualizarUsuario(Nomina, Nombre, Apellidos, rutaFotoPerfil, rutaFirma);

        //    if (result == 1)
        //    {
        //        if (!string.IsNullOrEmpty(rutaFotoPerfil)) Session["PathPerfil"] = rutaFotoPerfil;
        //        if (!string.IsNullOrEmpty(rutaFirma)) Session["PathFirma"] = rutaFirma;
        //        Session["nombre"] = Nombre;
        //        Session["apellidos"] = Apellidos;

        //        TempData["Message"] = message;
        //        return RedirectToAction("Perfil");
        //    }
        //    else
        //    {
        //        TempData["Message"] = message;
        //        return RedirectToAction("Perfil");
        //    }
        //}

        //[AuthorizeModule(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)]
        public ActionResult Dashboard()
        {
            return View();
        }

    }
}