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
    public class ArticuloController : Controller
    {
        IServiceArticulo _ServiceArticulo = new ServiceArticulo();
        // GET: Articulo
        public ActionResult Index()
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

        private SelectList listaArticulos(int idArticulo = 0)
        {
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();
            IEnumerable<Articulo> listaArticulos = _ServiceArticulo.GetArticulo();
            return new SelectList(listaArticulos, "id", "nombre", idArticulo);
        }

        private SelectList listaCategoria(long idCat = 0)
        {
            IServiceCategoria _ServiceCategoria = new ServiceCategoria();
            IEnumerable<Categoria> listaCategoria = _ServiceCategoria.GetCategoria();
            return new SelectList(listaCategoria, "id", "nombre", idCat);
        }

        public ActionResult buscarArticuloxNombre(string filtro)
        {
            IEnumerable<Articulo> lista = null;
            IServiceArticulo _ServiceArticulo= new ServiceArticulo();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceArticulo.GetArticulo();
            }
            else
            {
                lista = _ServiceArticulo.GetArticuloByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewMantenimiento", lista);
        }
        public ActionResult Save(Articulo art, string[] categoria, HttpPostedFileBase ImageFile)
        {
            MemoryStream target = new MemoryStream();

            if (art.imagen == null)
            {
                if (ImageFile != null)
                {
                    ImageFile.InputStream.CopyTo(target);
                    art.imagen = target.ToArray();
                    ModelState.Remove("Imagen");

                }

            }
            art.categoria_id = int.Parse(categoria[0]);
            _ServiceArticulo.Save(art);
            return RedirectToAction("Index");
        }
        public ActionResult Create()
        {
            ViewBag.categoria_id = listaCategoria();
            return View();
        }
    }
}