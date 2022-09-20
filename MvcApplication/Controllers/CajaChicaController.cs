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
    public class CajaChicaController: Controller
    {
        public ActionResult Index()
        {
            IEnumerable<Caja_Chica> lista = null;
            try
            {
                IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
                lista = _ServiceCaja.GetCajaChica();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }
    }
}