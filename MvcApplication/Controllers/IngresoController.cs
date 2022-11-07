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
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();

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
            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Aviso", "Artículo agregado a la orden", SweetAlertMessageType.success);
            return RedirectToAction("IndexIngreso");

        }
        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleIngreso = Comprita.Instancia.Items;
            TempData["NotiCarrito"] = Comprita.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("_PartialViewDetalle", Comprita.Instancia.Items);

        }

        public ActionResult eliminarProducto(long? id)
        {
            try
            {
                TempData["mensaje"] = Comprita.Instancia.EliminarItem((long)id);
                return RedirectToAction("IndexIngreso");
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return RedirectToAction("IndexIngreso");
        }

        public ActionResult Save(Ingreso ingreso)
        {
            IServiceUsuario serviceUsuario = new ServiceUsuario();
            IServiceArticulo serviceArticulo = new ServiceArticulo();
            try
            {
               
                if (ModelState.IsValid)
                {
                    var listaLinea = Comprita.Instancia.Items;

                    if (listaLinea.Count != 0)
                    {
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
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Compra Registrada!", "El ingreso de inventario a la tienda se ha realizado con éxito!", SweetAlertMessageType.warning);
                        return RedirectToAction("IndexIngreso");
                    }
                    else
                    {
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Sin Artículos", "Primero seleccione los artículos de la compra realizada, diríjase a la sección de abajo", SweetAlertMessageType.warning);
                    }
                       
                }
                else
                {
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Comentario vacío", "Digíte un comentario acorde a la compra", SweetAlertMessageType.warning);
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