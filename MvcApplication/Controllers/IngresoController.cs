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
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Compras", "Registre aquí sus nuevas compras", SweetAlertMessageType.info);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View();
        }

        public ActionResult Save(Ingreso ingreso)
        {
            IServiceUsuario serviceUsuario = new ServiceUsuario();
            try
            {
              
                 Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));

                     IServiceArticulo serviceArticulo = new ServiceArticulo();

                     Articulo articulo = new Articulo();
                

                    /* foreach (var items in listaLinea)
                     {
                         Detalle_Venta linea = new Detalle_Venta();
                         linea.articulo_id = (int)items.idArticulo;
                         linea.cantidad = items.cantidad;
                         ingreso.impuesto = 0.13;
                         ingreso.tipoventa = user.rol_id == 2 ? ingreso.tipoventa = true : ingreso.tipoventa = false;
                         ingreso.usuario_id = Convert.ToInt32(TempData["idUser"]);
                         ingreso.estado = true;
                         linea.venta_id = ingreso.id;
                         linea.descuento = 0;
                         linea.venta_id = ingreso.id;
                         ingreso.monto_total = (double?)Carrito.Instancia.GetTotal() + ((double?)Carrito.Instancia.GetTotal() * ingreso.impuesto) - linea.descuento;
                         linea.precio = ingreso.monto_total;
                         ingreso.Detalle_Venta.Add(linea);
                     }*/

                     IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                     Ingreso ven = _ServiceIngreso.Save(ingreso);
                     return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                return RedirectToAction("Default", "Error");
            }
        }

        public void Comprar(int id)
        {

        }

    }
}