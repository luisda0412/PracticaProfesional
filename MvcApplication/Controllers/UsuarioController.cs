using AplicationCore.Services;
using Infraestructure.Models;
using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;

using Web.ViewModel;

namespace MvcApplication.Controllers
{
    public class UsuarioController : Controller
    {
        private static Usuario aux;
        // GET: Usuario
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Usuario> lista = null;
            try
            {
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                lista = _ServiceUsuario.GetUsuario();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Usuario user)
        {
            MemoryStream target = new MemoryStream();
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            if(user.estado != null)
            {
                ModelState.Remove("estado");
                ModelState.Remove("clave");
                ModelState.Remove("id");
                if (ModelState.IsValid)
                {
                 try
                    {
                        Usuario oUser = _ServiceUsuario.Save(user);
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "usuario editado con éxito", SweetAlertMessageType.success);
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                        return RedirectToAction("Default", "Error");
                    }
                }
            }
            else
            {
                ModelState.Remove("estado");
                if (ModelState.IsValid)
                {
                    try
                    {
                        Usuario oUser = _ServiceUsuario.Save(user);
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "usuario guardado con éxito", SweetAlertMessageType.success);
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                        return RedirectToAction("Default", "Error");
                    }
                }
            }
            return RedirectToAction("Index");

        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            ViewBag.RolesLista = listaRol();
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceUsuario _ServiceProducto = new ServiceUsuario();
            Usuario user = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                user = _ServiceProducto.GetUsuarioByID(id.Value);
                if (user == null)
                {
                    TempData["Message"] = "No existe el libro solicitado";
                    TempData["Redirect"] = "Usuario";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }
                ViewBag.RolesLista = listaRol();
                return View(user);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            ServiceUsuario _ServiceUsuario = new ServiceUsuario();
            Usuario user = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                user = _ServiceUsuario.GetUsuarioByID(id.Value);

                if (user == null)
                {
                    return RedirectToAction("Index");
                }
                return View(user);

            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        private SelectList listaRol(long idRol = 0)
        {
            IServiceUsuario _ServiceUsuario = new ServiceUsuario();
            IEnumerable<Rol> listaTipo = _ServiceUsuario.GetRol();
            return new SelectList(listaTipo, "id", "tipo", idRol);
        }

        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Usuario usu = cdt.Usuario.Where(x => x.id == id).FirstOrDefault();
                    usu.estado = !usu.estado;
                    cdt.Usuario.Add(usu);

                    cdt.Entry(usu).State = EntityState.Modified;
                    cdt.SaveChanges();
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }

        #region RecuperarContraseña
        //Vista para solicitar al cliente los datos necesarios para recuperar la contraseña
        [HttpPost]
        public ActionResult IniciarRecuperacion(LoginViewModel empleado)
        {
            try
            {
                IServiceUsuario service = new ServiceUsuario();
                Usuario oEmpleado = service.VerificarUsuario(empleado.Email);

                if (oEmpleado != null)
                {
                    service.Save(oEmpleado);
                }

                return View("NotificacionCorreo");

            }
            catch (Exception ex)
            {

                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }

        }

        public ActionResult NotificacionCorreo()
        {
            return View();
        }


        [HttpGet]
        public ActionResult IniciarRecuperacion()
        {
            return View();
        }

        [HttpGet]
        //Vista para solicitar al cliente la nueva contraseña
        public ActionResult Recuperacion(string token)
        {
            IServiceUsuario service = new ServiceUsuario();
            try
            {
                if (token == null || token.Trim().Equals(""))
                {
                    return View("Index");
                }

                Usuario oEmpleado = service.GetUsuarioByToken(token);
                if (oEmpleado == null)
                {
                    ViewBag.Error = "Tú token ha expirado.";
                    return View("Index");
                }
                aux = oEmpleado;
                return View();
            }
            catch (Exception ex)
            {

                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }

        [HttpPost]
        public ActionResult Recuperacion(Usuario pUsuario)
        {
            aux.clave = pUsuario.clave;
            IServiceUsuario service = new ServiceUsuario();
            try
            {
                if (aux != null)
                {
                    aux.tokenRecuperacion = null;
                    service.Save(aux);
                }
                return View("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                // Pasar el Error a la página que lo muestra
                TempData["Message"] = ex.Message;
                TempData.Keep();
                return RedirectToAction("Default", "Error");
            }
        }
        #endregion


    }
}