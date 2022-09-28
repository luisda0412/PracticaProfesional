
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Security
{
    //Especifica que el acceso a un controlador o método de acción está restringido
    //a los usuarios que cumplen con el requisito de autorización.
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //Roles permitidos
        private readonly int[] allowedroles;
        public CustomAuthorizeAttribute(params int[] roles)
        {
            //roles Obtiene los roles de usuario autorizados
            //para acceder al controlador o al método de acción.
            this.allowedroles = roles;
        }

        //Verificaciones de autorización personalizadas
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var oUsuario = (Usuario)httpContext.Session["User"];

            if (oUsuario != null)
            {
                foreach (var rol in allowedroles)
                {
                    if (rol == oUsuario.rol_id)
                        return true;
                }
            }
            return authorize;
        }

        //Procesa solicitudes HTTP que fallan en la autorización.
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Si hubo un error redireccione a el siguiente Controller y View
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}
