using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TruckyV2.Clases
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                // Verificar si el usuario está autenticado
                if (!controller.HttpContext.User.Identity.IsAuthenticated)
                {
                    // Si el usuario no está autenticado, redirigir a la página de cierre de sesión
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessSystem", action = "SignOut" }));
                    return;
                }

                // Verificar si existe la cookie de autenticación
                HttpCookie authCookie = controller.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                {
                    // Si no hay cookie de autenticación, redirigir a la página de cierre de sesión
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessSystem", action = "SignOut" }));
                    return;
                }

                // Intentar descifrar el ticket de autenticación
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (Exception)
                {
                    // Si hay un error al descifrar el ticket, redirigir a la página de cierre de sesión
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessSystem", action = "SignOut" }));
                    return;
                }

                // Verificar si el ticket de autenticación es válido y no ha expirado
                if (authTicket == null || authTicket.Expired)
                {
                    // Si el ticket ha expirado, redirigir a la página de cierre de sesión
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "AccessSystem", action = "SignOut" }));
                    return;
                }
            }

            // Continuar con la ejecución de la acción si todas las verificaciones son válidas
            base.OnActionExecuting(filterContext);
        }
    }
}