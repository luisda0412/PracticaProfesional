using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class ResenaController : Controller
    {
        // GET: Resena
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index(int? id)
        {
            IEnumerable<Resena> lista = null;
            try
            {
                IServiceResena _ServiceResena = new ServiceResena();
                id = Convert.ToInt32(TempData["idArticulo"]);
                lista = _ServiceResena.GetResenaByIDArticulo((long)id);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Resena resena)
        {
            MemoryStream target = new MemoryStream();
            IServiceResena _ServiceResena = new ServiceResena();
            try
            {
                resena.usuario_id = Convert.ToInt32(TempData["idUser"]);
                resena.articulo_id = Convert.ToInt32(TempData["idArticulo"]);
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

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Eliminar(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceResena _ServiceResena = new ServiceResena();
            try
            {

                _ServiceResena.Eliminar((long)id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Resena";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult IndexArticulos()
        {
            IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                lista = _ServiceArticulo.GetArticulo();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            IServiceResena _ServiceResenas = new ServiceResena();
            Resena rese = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                rese = _ServiceResenas.GetResenaByID(id.Value);

                if (rese == null)
                {
                    return RedirectToAction("Index");
                }
                return View(rese);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("Index");
            }
        }

    }
}