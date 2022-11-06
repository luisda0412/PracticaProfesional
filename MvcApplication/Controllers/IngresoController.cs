using AplicationCore.Services;
using Infraestructure.Models;
using MvcApplication.Util;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Utils;
using Web.ViewModel;

namespace MvcApplication.Controllers
{
    public class IngresoController: Controller
    {
        public ActionResult IndexIngreso()
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
            ViewBag.DetalleIngreso = Comprita.Instancia.Items;
            return View();
        }

        public ActionResult Comprar(int? id)
        {
          
            int cantidadCompra = Comprita.Instancia.Items.Count();
            ViewBag.NotificationMessage= Comprita.Instancia.AgregarItem((int)id);
            ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Compras", "Artículo agregado a la orden", SweetAlertMessageType.success);
            return RedirectToAction("IndexIngreso");

        }
        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleIngreso = Comprita.Instancia.Items;
            TempData["NotiCarrito"] = Comprita.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("_PartialViewDetalle", Comprita.Instancia.Items);

        }

        public ActionResult eliminarProducto(long? idArticulo)
        {
            try
            {
                ViewBag.NotificationMessage = Comprita.Instancia.EliminarItem((long)idArticulo);
                return PartialView("_PartialViewDetalle", Comprita.Instancia.Items);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return PartialView("_PartialViewDetalle", Comprita.Instancia.Items);
        }

        public ActionResult Save(Ingreso ingreso)
        {
            IServiceUsuario serviceUsuario = new ServiceUsuario();
            IServiceArticulo serviceArticulo = new ServiceArticulo();
            try
            {
                Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));
                if (user != null)
                {

                    //Lista que trae provisionalmente cada detalle del ingreso, osea cada linea de cada articulo
                    var listaLinea = Comprita.Instancia.Items;
                     
                    foreach (var items in listaLinea)
                     {
                        //Se van llenando en BD cada linea del detalle
                         Detalle_Ingreso detalle = new Detalle_Ingreso();
                         detalle.articulo_id = (int)items.idArticulo;
                         detalle.cantidad = items.cantidad;
                        //ingreso.monto_total += linea.Articulo.precio;
                        ingreso.monto_total = (double)Comprita.Instancia.GetTotal();
                        ingreso.Detalle_Ingreso.Add(detalle);
                        serviceArticulo.actualizarCantidad((int)detalle.articulo_id, (int)detalle.cantidad, true);
                    }
                    ingreso.fecha = DateTime.Now;
                    ingreso.usuario_id = Convert.ToInt32(TempData["idUser"]);
                    IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                    Ingreso compra = _ServiceIngreso.Save(ingreso);

                    Comprita.Instancia.eliminarCarrito();
                    return RedirectToAction("IndexIngreso");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                return RedirectToAction("Default", "Error");
            }
            return RedirectToAction("IndexIngreso");
        }


    }
}