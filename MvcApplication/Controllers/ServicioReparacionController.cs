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
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class ServicioReparacionController: Controller
    {
        IServiceServicio _ServiceServicio = new ServiceServicio();

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Servicio_Reparacion> lista = null;
            try
            {
                IServiceServicio _ServiceServicio = new ServiceServicio();
                lista = _ServiceServicio.GetServicio();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Servicio_Reparacion ser)
        {
            MemoryStream target = new MemoryStream();
            try
            {

                _ServiceServicio.Save(ser);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult buscarServicioxDescripcion(string filtro)
        {
            IEnumerable<Servicio_Reparacion> lista = null;
            IServiceServicio _ServiceServicio = new ServiceServicio();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceServicio.GetServicio();
            }
            else
            {
                lista = _ServiceServicio.GetServicioByDescripcion(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxDescripcion", lista);
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceServicio _ServiceServicio = new ServiceServicio();
            Servicio_Reparacion servicio = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                servicio = _ServiceServicio.GetServicioByID(id.Value);

                if (servicio == null)
                {
                    TempData["Message"] = "No existe el proveedor solicitado";
                    TempData["Redirect"] = "Proveedor";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                return View(servicio);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
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
            ServiceServicio _ServiceServicio = new ServiceServicio();
            Servicio_Reparacion servicio = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                servicio = _ServiceServicio.GetServicioByID(id.Value);

                if (servicio == null)
                {
                    return RedirectToAction("Index");
                }
                return View(servicio);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        public ActionResult EliminarServicio(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceServicio _ServiceServicio = new ServiceServicio();
            try
            {

                _ServiceServicio.Eliminar((int)id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Servicio_Reparacion";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

    }


}