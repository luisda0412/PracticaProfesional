using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class ServicioReparacionController: Controller
    {
        IServiceServicio _ServiceServicio = new ServiceServicio();

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

    }


}