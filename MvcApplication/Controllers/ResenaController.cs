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
    public class ResenaController : Controller
    {
        // GET: Resena
        public ActionResult Index()
        {
            IEnumerable<Resena> lista = null;
            try
            {
                IServiceResena _ServiceResena = new ServiceResena();
                lista = _ServiceResena.GetResena();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }
    }
}