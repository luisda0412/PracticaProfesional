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
        
        [CustomAuthorize((int)Roles.Administrador)]
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
                if (TempData["mensaje2"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje2"].ToString();
            }
            catch (Exception e)
            {
                TempData["mensaje2"] = "Error al procesar los datos! " + e.Message;
            }
            return View(lista);
        }


        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult CerrarCaja()
        {
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja caja = new Arqueos_Caja();
                    
            try
             {

                caja.usuario_id = Convert.ToInt32(TempData["idUser"]);
                caja.fecha = DateTime.Now;
                 _ServiceCaja.SaveArqueo(caja);
                 TempData["mensaje2"] = Util.SweetAlertHelper.Mensaje("Acción en Caja", "Caja Chica cerrada", SweetAlertMessageType.info);
             }
             catch (Exception ex)
             {
                 TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                 return RedirectToAction("Default", "Error");
             }
            
            return RedirectToAction("IndexArqueos");
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult AbrirCaja(string monto)
        {
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja arqueito = new Arqueos_Caja();
            Caja_Chica cajaChica = new Caja_Chica();

            if (Convert.ToDouble(monto) == 0)
            {
                TempData["mensaje2"] = Util.SweetAlertHelper.Mensaje("Acción inválida", "Digite un monto mayor a 0", SweetAlertMessageType.error);
                return RedirectToAction("IndexArqueos");
            }

            try
            {
                double dinero = Convert.ToDouble(monto);
                arqueito.saldo = dinero;
                arqueito.usuario_id = Convert.ToInt32(TempData["idUser"]);
                arqueito.fecha = DateTime.Now;
                _ServiceCaja.AbrirArqueo(arqueito);
                TempData["mensaje2"] = Util.SweetAlertHelper.Mensaje("Acción en Caja", "Caja chica abierta", SweetAlertMessageType.info);

                cajaChica.fecha = DateTime.Now;
                cajaChica.entrada = dinero;
                cajaChica.salida = 0.0;
                cajaChica.saldo = dinero;

                IServiceCajaChica caja = new ServiceCajaChica();
                caja.Save(cajaChica);

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