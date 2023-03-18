using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class NotasController : Controller
    {
        public static string idFactura { get; set; }
        public static string nuevoMonto { get; set; }
        public static string motivo { get; set; }
        public ActionResult obtenerDatosFormNota()
        {
            idFactura = Request.Form["id"];
            nuevoMonto = Request.Form["monto"];
            motivo = Request.Form["motivo"];

            return RedirectToAction("CrearNotaDeCredito");
        }

        public ActionResult CrearNotaCredito()
        {
            obtenerDatosFormNota();

            Venta venta = new Venta();

            IServiceFactura serviceFactura = new ServiceFactura();
            IServiceVenta serviceVenta = new ServiceVenta();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(idFactura));
            venta = serviceVenta.GetVentaByID((long)factura.venta_id);


            return View();
        }
    }
}