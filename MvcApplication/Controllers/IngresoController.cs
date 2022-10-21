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
                         Detalle_Ingreso linea = new Detalle_Ingreso();
                         linea.articulo_id = (int)items.idArticulo;
                         linea.cantidad = items.cantidad;
                         linea.ingreso_id = ingreso.id;

                         //Voy llenando el monto del ingreso con el precio de cada linea de articulo
                         ingreso.monto_total += linea.Articulo.precio;
                     }
                    ingreso.usuario_id = user.id;


                    IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                    Ingreso ven = _ServiceIngreso.Save(ingreso);
                    return RedirectToAction("IndexIngreso");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                return RedirectToAction("Default", "Error");
            }
            return RedirectToAction("IndexIngreso");
        }


    }
}