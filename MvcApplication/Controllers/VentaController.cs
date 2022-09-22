using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class VentaController: Controller
    {
        IServiceVenta _Serviceventa = new ServiceVenta();
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
    }
}