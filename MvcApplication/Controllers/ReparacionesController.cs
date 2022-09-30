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
using Web.Security;

namespace MvcApplication.Controllers
{
    public class ReparacionesController : Controller
    {
        //Para guardadr la reparacion que se clickee
        int? codigo = 0;

        //private MyContext db = new MyContext();
        private SelectList listaServicios(long idSer = 0)
        {
            IServiceServicio _ServiceServicio = new ServiceServicio();
            IEnumerable<Servicio_Reparacion> listaServicios = _ServiceServicio.GetServicio();
            return new SelectList(listaServicios, "id", "descripcion", idSer);
        }


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

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Reparaciones repa)
        {
            MemoryStream target = new MemoryStream();
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            try
            {
                repa.usuario_id = Convert.ToInt32(TempData["idUser"]);
                repa.fecha = DateTime.Now;
                _ServiceReparaciones.Save(repa);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Proveedor";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            ViewBag.servicio_reparacion_id = listaServicios();
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            Reparaciones prov = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                prov = _ServiceReparaciones.GetReparacionByID(id.Value);

                if (prov == null)
                {
                    TempData["Message"] = "No existe el proveedor solicitado";
                    TempData["Redirect"] = "Proveedor";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                return View(prov);
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
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            Reparaciones repa = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                repa = _ServiceReparaciones.GetReparacionByID(id.Value);

                if (repa == null)
                {
                    return RedirectToAction("Index");
                }
                return View(repa);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Reparaciones repa = cdt.Reparaciones.Where(x => x.id == id).FirstOrDefault();
                    repa.estado = !repa.estado;
                    cdt.Reparaciones.Add(repa);
                    cdt.Entry(repa).State = EntityState.Modified;
                    cdt.SaveChanges();
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
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
