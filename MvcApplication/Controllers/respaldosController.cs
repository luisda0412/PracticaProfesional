using AplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class respaldosController : Controller
    {

        public ActionResult guardarRespaldo()
        {
            IServiceRespaldos serviceRespaldos = new ServiceRespaldos();
            serviceRespaldos.guardarRespaldo();

            return View("respaldoExitoso");
        }

        public ActionResult restaurarRespaldo(string ruta)
        {
            ruta = ruta.Substring(12);

            if (ruta.Contains(".bak"))
            {
                ruta = @"C:\RespaldosVYCUZ\" + ruta;
                IServiceRespaldos serviceRespaldos = new ServiceRespaldos();
                serviceRespaldos.restaurarRespaldo(ruta);
            }

            return View("restauracionExitosa");
        }
    }
}
