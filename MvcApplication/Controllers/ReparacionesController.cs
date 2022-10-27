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
using MvcApplication.Util;

namespace MvcApplication.Controllers
{
    public class ReparacionesController : Controller
    {
        Reportes_Tecnicos re = new Reportes_Tecnicos();
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
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult IndexUsuario()
        {
            IEnumerable<Reparaciones> lista = null;
            try
            {
                IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
                int idUsuario = Convert.ToInt32(TempData["idUser"]);
                lista = _ServiceReparaciones.GetReparacionPorUsuario(idUsuario);
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult ReportesTecnicosUsuario(int? id)
        {

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

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Reparaciones repa)
        {
        
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            ModelState.Remove("imagen");
            if (ModelState.IsValid)
            {
                try
                {
                    repa.usuario_id = Convert.ToInt32(TempData["idUser"]);
                    repa.fecha = DateTime.Now;
                    _ServiceReparaciones.Save(repa);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "reparación guardada con éxito", SweetAlertMessageType.success);
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
            ViewBag.ServiciosLista = listaServicios();
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
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
                    TempData["Message"] = "No existe el proveedor solicitado";
                    TempData["Redirect"] = "Proveedor";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                ViewBag.ServiciosLista = listaServicios();
                return View(repa);
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
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();       
                try
                {
                    _ServiceReparaciones.Eliminar((int)id);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Reparación eliminada", "Datos eliminados de la base", SweetAlertMessageType.success);                  
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                }
            return RedirectToAction("Index");
        }

        public ActionResult buscarReparacionxCedula(string filtro)
        {
            IEnumerable<Reparaciones> lista = null;
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceReparaciones.GetReparacion();
            }
            else
            {
                lista = _ServiceReparaciones.GetReparacionByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxCedula", lista);
        }



        //Reportes Tecnicos--------------------------------------------------
        //-------------------------------------------------------------------
        //-------------------------------------------------------------------

        //VARIABLE QUE LLENA EL CODIGO DEL SERVICIO PARA EL REPORTE
        public static int codigoSer { get; set; }
        public ActionResult ReportesTecnicos(int? id)
        {
            this.codigo = id;
            codigoSer = (int)id;

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
                repo.reparacion_id = codigoSer; 
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
