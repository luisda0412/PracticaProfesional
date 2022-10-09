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
             IServiceUsuario serviceUsuario = new ServiceUsuario();
            try
            {
                if (Carrito.Instancia.Items.Count() <= 0)
                {
                    return RedirectToAction("");
                }
                else
                {
                    Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));
                    venta.nombre_cliente = user.nombre;
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
                            venta.impuesto = 0.13;
                            venta.tipoventa= user.rol_id == 2 ? venta.tipoventa = true : venta.tipoventa = false;
                            venta.usuario_id = Convert.ToInt32(TempData["idUser"]);
                            venta.estado = true;
                            linea.venta_id = venta.id;
                            linea.descuento = 0;
                            linea.venta_id = venta.id;
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal() + ((double?)Carrito.Instancia.GetTotal() * venta.impuesto)- linea.descuento;
                            linea.precio = venta.monto_total;
                            venta.Detalle_Venta.Add(linea);
                        }

                        IServiceVenta _ServiceVenta = new ServiceVenta();
                        Venta ven = _Serviceventa.Save(venta);
                    }
                    return RedirectToAction("Index");
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
            if (TempData.ContainsKey("NotificationMessage"))
            {
                ViewBag.NotificationMessage = TempData["NotificationMessage"];
            }

            ViewBag.DetalleOrden = Carrito.Instancia.Items;
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

        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleOrden = Carrito.Instancia.Items;
            TempData["NotiCarrito"] = Carrito.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("Detalle", Carrito.Instancia.Items);

        }

        //Actualizar solo la cantidad de libros que se muestra en el menú
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult actualizarOrdenCantidad()
        {
            if (TempData.ContainsKey("NotiCarrito"))
            {
                ViewBag.NotiCarrito = TempData["NotiCarrito"];
            }
            int cantidadLibros = Carrito.Instancia.Items.Count();
            return PartialView("Detalle");

        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult eliminarProducto(long? idArticulo)
        {
            try
            {
                ViewBag.NotificationMessage = Carrito.Instancia.EliminarItem((long)idArticulo);
                return PartialView("Detalle", Carrito.Instancia.Items);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return PartialView("Detalle", Carrito.Instancia.Items);
        }
    }
}