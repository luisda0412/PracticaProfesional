using AplicationCore.Services;
using Infraestructure.Models;
using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;

namespace MvcApplication.Controllers
{
    public class CajaChicaController: Controller
    {
        
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Caja_Chica> lista = null;
            try
            {
                IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
                lista = _ServiceCaja.GetCajaChica();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
               
            }
            return View(lista);
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Delete(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            try
            {

                _ServiceCaja.Eliminar((int)id);
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Registro eliminado", "Datos eliminados del sistema", SweetAlertMessageType.success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult buscarCajaxFecha(DateTime filtro)
        {
            IEnumerable<Caja_Chica> lista = null;
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();

            // Error porque viene en blanco 
            if (filtro==null)
            {
                lista = _ServiceCaja.GetCajaChica();
            }
            else
            {
                lista = _ServiceCaja.GetCajaByFecha(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxFecha", lista);
        }



        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Caja_Chica caja)
        {
            
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            ModelState.Remove("id");
            ModelState.Remove("fecha");
            if (ModelState.IsValid)
            {
                try
                {

                    caja.fecha = DateTime.Now;
                    _ServiceCaja.Save(caja);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "La información de la caja chica ha sido guardada con éxito", SweetAlertMessageType.success);
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                    return RedirectToAction("Default", "Error");
                }
            }
            return RedirectToAction("Index");

        }



        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult IndexArqueos()
        {
            IEnumerable<Arqueos_Caja> lista = null;
            try
            {
                IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
                lista = _ServiceCaja.GetArqueos();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                TempData["mensaje"] = "Error al procesar los datos! " + e.Message;
            }
            return View(lista);
        }


        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult ArquearCaja()
        {
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja caja = new Arqueos_Caja();
             try
             {

                caja.usuario_id = Convert.ToInt32(TempData["idUser"]);
                caja.fecha = DateTime.Now;
                 _ServiceCaja.SaveArqueo(caja);
                 TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Acción de Caja", "El estado de la caja ha cambiado", SweetAlertMessageType.info);
             }
             catch (Exception ex)
             {
                 TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                 return RedirectToAction("Default", "Error");
             }
            
            return RedirectToAction("IndexArqueos");
        }


    }
}