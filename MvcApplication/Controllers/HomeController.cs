using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Reflection;
using System.Web.Mvc;
using Web.Utils;
using MvcApplication.Util;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        IServiceUsuario iserviceU = new ServiceUsuario();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Loguearse()
        {
            return View();
        }

        public ActionResult Login(Usuario usuario)
        {
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            Usuario oUsuario = null;
            try
            {
                if (ModelState.IsValid)
                {
                    oUsuario = _ServiceUsuario.LogIn(usuario.correo_electronico, usuario.clave);

                    if (oUsuario != null)
                    {
                        Session["User"] = oUsuario;
                        Log.Info($"Accede {oUsuario.nombre}");
                        ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Bienvenido a VYCUZ", "un gusto tenerte de vuelta", SweetAlertMessageType.success);
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        Log.Warn($"{usuario.correo_electronico} se intentó conectar  y falló");
                        ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Ups! no se ha podido crear su sesión", "revise sus credenciales e intente de nuevo", SweetAlertMessageType.warning);

                    }
                }

                return View("Loguearse");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }
        public ActionResult Logout()
        {
            try
            {
                Log.Info("Usuario desconectado!");
                Session["User"] = null;
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData["Redirect"] = "Login";
                TempData["Redirect-Action"] = "Index";
                return RedirectToAction("Default", "Error");
            }
        }

         public ActionResult UnAuthorized()
        {
            try
            {
                ViewBag.Message = "No autorizado";

                if (Session["User"] != null)
                {
                    Usuario oUsuario = Session["User"] as Usuario;
                    Log.Warn($"El usuario {oUsuario.nombre}");
                }

                return View();
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData["Redirect"] = "Login";
                TempData["Redirect-Action"] = "Index";
                return RedirectToAction("Default", "Error");
            }
        }

    }
}