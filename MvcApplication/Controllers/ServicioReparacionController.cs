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
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Servicio_Reparacion ser)
        {
            ModelState.Remove("estado");
            if (ModelState.IsValid) {
                try
                {
                    _ServiceServicio.Save(ser);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "servicio guardado con éxito", SweetAlertMessageType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                    return RedirectToAction("Default", "Error");
                }
            }
            return RedirectToAction("Index");

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
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Servicio_Reparacion serv = cdt.Servicio_Reparacion.Where(x => x.id == id).FirstOrDefault();
                    serv.estado = !serv.estado;
                    cdt.Servicio_Reparacion.Add(serv);

                    cdt.Entry(serv).State = EntityState.Modified;
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

    }


}