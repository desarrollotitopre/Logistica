using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;


namespace Logistica.Models
{
    public class AuthorizeModuleAttribute : ActionFilterAttribute
    {
        //private readonly string _vista;
        private readonly int[] _vista;

        public AuthorizeModuleAttribute(params int[] vista)
        {
            _vista = vista;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var vistasPermitidas = (List<int>)filterContext.HttpContext.Session["VistasPermitidas"];


            // Verifica si la sesión ha expirado
            if (filterContext.HttpContext.Session == null || vistasPermitidas == null)
            {
                // Redirige al login si la sesión ha expirado
                filterContext.Result = new RedirectResult("~/Acceso/Login");
                return;
            }
            // Verifica si el usuario tiene permiso para al menos uno de los módulos especificados
            if (vistasPermitidas == null || !_vista.Any(vista => vistasPermitidas.Contains(vista)))
            {
                //filterContext.Result = new RedirectResult("~/AccessSystem/Login");
                filterContext.Result = new PartialViewResult
                {
                    ViewName = "~/Views/Shared/_AccesoDenegado.cshtml"
                };
            }


            base.OnActionExecuting(filterContext);
        }
    }
}