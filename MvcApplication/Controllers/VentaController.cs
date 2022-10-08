using AplicationCore.Services;
using Infraestructure.Models;
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
    public class VentaController: Controller
    {
        IServiceVenta _Serviceventa = new ServiceVenta();

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Venta> lista = null;
            try
            {
                IServiceVenta _ServiceVenta = new ServiceVenta();
                lista = _ServiceVenta.GetVentas();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult Save(Venta venta)
        {
            try
            {
                if (Carrito.Instancia.Items.Count() <= 0)
                {
                    return RedirectToAction("");
                }
                else
                {
                    if (venta.nombre_cliente != null)
                    {
                        IServiceArticulo serviceArticulo = new ServiceArticulo();
                        Articulo articulo = new Articulo();
                        var listaLinea = Carrito.Instancia.Items;

                        foreach (var items in listaLinea)
                        {
                            Detalle_Venta linea = new Detalle_Venta();
                            linea.articulo_id = (int)items.idArticulo;
                            linea.cantidad = items.cantidad;
                            //linea.total_linea = items.total_linea;
                            //factura.porcentaje_descuento = factura.porcentaje_descuento / 100;
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal() - ((double?)Carrito.Instancia.GetTotal());
                            linea.venta_id = venta.id;
                            venta.Detalle_Venta.Add(linea);
                        }

                        IServiceVenta _ServiceVenta = new ServiceVenta();
                        Venta ven = _Serviceventa.Save(venta);
                    }
                    return RedirectToAction("IndexFactura");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Factura";
                TempData["Redirect-Action"] = "IndexFactura";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult IndexVenta()
        {
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult ordenarProducto(int? idArticulo)
        {
            idArticulo = Convert.ToInt32(TempData["idArticulo"]);
            int cantidadLibros = Carrito.Instancia.Items.Count();
            ViewBag.NotiCarrito = Carrito.Instancia.AgregarItem((int)idArticulo);
            return PartialView("MovimientoCantidad");

        }
    }
}