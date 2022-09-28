using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AplicationCore.Services;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Infraestructure.Models;
using Web.Utils;
using System.IO;


namespace MvcApplication.Controllers
{
    public class ReparacionesController : Controller
    {
        //Para guardadr la reparacion que se clickee
        int? codigo = 0;

        //private MyContext db = new MyContext();

        // GET: Reparaciones
        public ActionResult Index()
        {
            IEnumerable<Reparaciones> lista = null;
            try
            {
                IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
                lista = _ServiceReparaciones.GetReparacion();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }
        


        //Reportes Tecnicos--------------------------------------------------
        //-------------------------------------------------------------------
        //-------------------------------------------------------------------
        public ActionResult ReportesTecnicos(int? id)
        {
            this.codigo = id;

             IEnumerable<Reportes_Tecnicos> lista = null;
            try
            {
                IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
                lista = _ServiceRTecnico.GetReportesByID((long)id);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult CreateReporte()
        {
            
            return View();
        }

        public ActionResult SaveReporte(Reportes_Tecnicos repo)
        {
            MemoryStream target = new MemoryStream();
            IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
            try
            {
                repo.reparacion_id = (Convert.ToInt32(TempData["idReporte"])); 
                _ServiceRTecnico.Save(repo);

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

        public ActionResult EliminarReporte(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
            try
            {

                _ServiceRTecnico.Eliminar((long)id);
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

 


    }

}
