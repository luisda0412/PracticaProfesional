using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Reflection;
using System.Web.Mvc;
using MvcApplication.Util;
using System.IO;
using System.Data.Entity;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        IServiceUsuario iserviceU = new ServiceUsuario();
        public ActionResult Index()
        {
            if (TempData["mensaje"] != null)
                ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            return View();      
        }

        public ActionResult IndexAdmin()
        {
            if (TempData["mensaje"] != null)
                ViewBag.NotificationMessage = TempData["mensaje"].ToString();

            try
            {
                    if (DateTime.Now.Day == 1)
                    {
                        string path = @"C:\RespaldosVYCUZ";
                        try
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string url = @"'C:\RespaldosVYCUZ\VYCUZ_" + DateTime.Now.ToString("dd-MMMM-yyyy HH-mm") + ".bak'";
                            using (MyContext ctx = new MyContext())
                            {
                                ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "backup database Registro_Inventario_VYCUZ to disk = " + url);
                            }
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

            }
            catch (Exception dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }

            return View();
        }

        public ActionResult Loguearse()
        {
            
            return View();
        }
        public ActionResult recuperar()
        {
            UsuarioController ctl = new UsuarioController();
            return ctl.IniciarRecuperacion();
           
        }


        public ActionResult Login(Usuario usuario)
        {
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            Usuario oUsuario = null;
            

            ModelState.Remove("estado");
            ModelState.Remove("telefono");
            ModelState.Remove("nombre");
            ModelState.Remove("apellidos");
            ModelState.Remove("rol_id");
            try
            {
                if (ModelState.IsValid)
                {
                    oUsuario = _ServiceUsuario.LogIn(usuario.correo_electronico, usuario.clave);

                    if (oUsuario != null)
                    {
                        if (oUsuario.rol_id == 1)
                        {
                            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Bienvenido a VYCUZ", "Un gusto tenerte por acá " + oUsuario.nombre, SweetAlertMessageType.info);
                            Session["User"] = oUsuario;


                            Infraestructure.Util.Log.Info("\n");
                            string msj = "Nuevo inicio de sesión (administrador) por: ";                          
                            Infraestructure.Util.Log.Info(msj + usuario.clave + " " + usuario.correo_electronico);
                            return RedirectToAction("IndexAdmin");
                        }
                        Session["User"] = oUsuario;
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Bienvenido a VYCUZ", "Un gusto tenerte por acá " + oUsuario.nombre, SweetAlertMessageType.info);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Ups! no se ha podido crear su sesión", "revise sus credenciales e intente de nuevo", SweetAlertMessageType.warning);
                    }
                }
                return View("Loguearse");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;         
                return RedirectToAction("Default", "Error");
            }
        }
        public ActionResult Logout()
        {
            try
            {
                Infraestructure.Util.Log.Info("Usuario desconectado!");
                Session["User"] = null;
                int id = Convert.ToInt32(TempData["idUser"]);
                TempData["idUser"] = null;
                id = Convert.ToInt32(TempData["idUser"]);
                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
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
                    Infraestructure.Util.Log.Warn($"El usuario {oUsuario.nombre}");
                }

                return View();
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData["Redirect"] = "Login";
                TempData["Redirect-Action"] = "Index";
                return RedirectToAction("Default", "Error");
            }
        }

        //Vista para crear una nueva cuenta de usuario
        public ActionResult Registro()
        {
            return View();
        }

        //Metodo para registrar la cuenta del nuevo cliente
        public ActionResult SaveCliente(Usuario user)
        {
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            try
            {

                user.rol_id = 2;
                Usuario oUser = _ServiceUsuario.Save(user);

                return RedirectToAction("Loguearse");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

    }
}