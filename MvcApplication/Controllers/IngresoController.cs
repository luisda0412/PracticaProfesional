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
            IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                lista = _ServiceArticulo.GetArticulo();
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Compras", "Registre aquí sus nuevas compras", SweetAlertMessageType.info);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult Save()
        {
            return View();
        }

    }
}