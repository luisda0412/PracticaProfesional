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
    public class ServicioReparacionController: Controller
    {
        public ActionResult Index()
        {
            IEnumerable<Servicio_Reparacion> lista = null;
            try
            {
                IServiceServicio _ServiceServicio = new ServiceServicio();
                lista = _ServiceServicio.GetServicio();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }
    }
}