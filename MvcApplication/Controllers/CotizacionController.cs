using AplicationCore.Services;
using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Web.Utils;
using Web.ViewModel;

namespace MvcApplication.Controllers
{
    public class CotizacionController : Controller
    {
        // GET: Cotizacion
        public ActionResult Index()
        {
            //IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                ViewBag.listaArticulos = _ServiceArticulo.GetArticulo();
                // ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Compras", "Registre aquí sus nuevas compras", SweetAlertMessageType.info);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            ViewBag.DetalleIngreso = Cotizacion.Instancia.Items;
            return View();
        }

        public ActionResult Cotizar(int? id)
        {

            int cantidadCompra = Cotizacion.Instancia.Items.Count();
            ViewBag.NotificationMessage = Cotizacion.Instancia.AgregarItem((int)id);
            ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Compras", "Artículo agregado a la orden", SweetAlertMessageType.success);
            return RedirectToAction("Index");

        }

        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleOrden = Cotizacion.Instancia.Items;
            TempData["NotiCarrito"] = Cotizacion.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("_PartialViewDetalle", Cotizacion.Instancia.Items);

        }

        //POR AQUI PUEDE ANDAR EL ERROR DE QUE SE ESCONDE EL CARRITO 
        public ActionResult actualizarOrdenCantidad()
        {
            if (TempData.ContainsKey("NotiCarrito"))
            {
                ViewBag.NotiCarrito = TempData["NotiCarrito"];
            }
            int cantidadLibros = Carrito.Instancia.Items.Count();
            return PartialView("_PartialViewDetalle");

        }

        public ActionResult eliminarProducto(long? idArticulo)
        {
            try
            {
                ViewBag.NotificationMessage = Cotizacion.Instancia.EliminarItem((long)idArticulo);
                return PartialView("_PartialViewDetalle", Cotizacion.Instancia.Items);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return PartialView("_PartialViewDetalle", Cotizacion.Instancia.Items);
        }
    }
}