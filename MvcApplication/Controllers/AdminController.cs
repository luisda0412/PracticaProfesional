using AplicationCore.Services;
using MvcApplication.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class AdminController: Controller
    {
        public ActionResult respaldoBD()
        {

            IServiceRespaldos serviceRespaldos = new ServiceRespaldos();
            serviceRespaldos.guardarRespaldo();
            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Respaldo Exitoso!", "El respaldo se ha creado!", SweetAlertMessageType.success);  
            return RedirectToAction("IndexAdmin", "Home");

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

            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Restauración Exitosa!", "La restauración se ha creado!", SweetAlertMessageType.success);
            return RedirectToAction("IndexAdmin", "Home");
        }
    }
}