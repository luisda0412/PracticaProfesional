using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class ContactoController : Controller
    {

        //Index del contato
        public ActionResult Index()
        {
            return View();
        }


        //Metodo para enviar el correo
        public ActionResult EnviarCorreo()
        {

            ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Mensaje enviado con éxito", "Trataremos responderte los más pronto posible", SweetAlertMessageType.success);
            return RedirectToAction("Index");
        }

    }
}