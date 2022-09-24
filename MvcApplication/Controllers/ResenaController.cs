using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult Save(Resena resena)
        {
            MemoryStream target = new MemoryStream();
            IServiceResena _ServiceResena = new ServiceResena();
            try
            {
                resena.usuario_id = Convert.ToInt32(TempData["idUser"]);
                Resena oResena = _ServiceResena.Save(resena);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}