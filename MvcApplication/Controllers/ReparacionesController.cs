using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AplicationCore.Services;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Infraestructure.Models;
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class ReparacionesController : Controller
    {
        //private MyContext db = new MyContext();

        // GET: Reparaciones
        public ActionResult Index()
        {
            IEnumerable<Reparaciones> lista = null;
            try
            {
                IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
                lista = _ServiceReparaciones.GetReparacion();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

    }

}
