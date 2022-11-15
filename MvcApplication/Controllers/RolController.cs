using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;


namespace MvcApplication.Controllers
{
    public class RolController : Controller
    {
        // GET: Rol

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Rol> lista = null;
            try
            {
                IServiceRol _ServiceRol = new ServiceRol();
                lista = _ServiceRol.GetRol();
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Rol rol)
        {
            MemoryStream target = new MemoryStream();
            IServiceRol _ServiceRol = new ServiceRol();
            try
            {

                _ServiceRol.Save(rol);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Rol";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceRol _ServiceRol = new ServiceRol();
            Rol rol = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                rol = _ServiceRol.GetRolByID(id.Value);

                if (rol == null)
                {
                    TempData["Message"] = "No existe el rol solicitado";
                    TempData["Redirect"] = "Rol";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Rol";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            ServiceRol _ServiceRol = new ServiceRol();
            Rol rol = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                rol = _ServiceRol.GetRolByID(id.Value);

                if (rol == null)
                {
                    return RedirectToAction("Index");
                }
                return View(rol);

            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        public void desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Rol rol= cdt.Rol.Where(x => x.id == id).FirstOrDefault();
                    rol.estado = !rol.estado;
                    cdt.Rol.Add(rol);

                    cdt.Entry(rol).State = EntityState.Modified;
                    cdt.SaveChanges();

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }
    }
}