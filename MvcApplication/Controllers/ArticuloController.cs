﻿using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class ArticuloController : Controller
    {
        IServiceArticulo _ServiceArticulo = new ServiceArticulo();
        // GET: Articulo
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
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

        //Listas para los combos de cada articulo que se edite
        private SelectList listaCategoria(long idCat = 0)
        {
            IServiceCategoria _ServiceCategoria = new ServiceCategoria();
            IEnumerable<Categoria> listaCategoria = _ServiceCategoria.GetCategoria();
            return new SelectList(listaCategoria, "id", "nombre", idCat);
        }
        private SelectList listaProveedor(long idProv = 0)
        {
            IServiceProveedor _ServiceProveedor = new ServiceProveedor();
            IEnumerable<Proveedor> listaProveedor = _ServiceProveedor.GetProveedor();
            return new SelectList(listaProveedor, "id", "descripcion", idProv);
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
            return PartialView("_PartialViewVistaxNombre", lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Articulo art, string[] categoria, string[] proveedor, HttpPostedFileBase ImageFile)
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
            
            _ServiceArticulo.Save(art, proveedor);
            return RedirectToAction("Index");
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            ViewBag.categoria_id = listaCategoria();
            ViewBag.idProveedor = listaProveedor();
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            ServiceArticulo _ServiceArticulo = new ServiceArticulo();
            Articulo articulo = null;

            try
            {
                if (id ==null)
                {
                    return RedirectToAction("Index");
                }

                articulo= _ServiceArticulo.GetArticuloByID(id.Value);

                if (articulo==null)
                {
                    return RedirectToAction("Index");
                }
                return View(articulo);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("Index");
            }
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceArticulo _ServiceArticulo = new ServiceArticulo();
            Articulo articulo = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                articulo = _ServiceArticulo.GetArticuloByID(id.Value);

                if (articulo == null)
                {
                    TempData["Message"] = "No existe el articulo solicitado";
                    TempData["Redirect"] = "Articulo";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                ViewBag.idCategoria = listaCategoria((long)articulo.categoria_id);
                ViewBag.idProveedor = listaProveedor(articulo.id);
                return View(articulo);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Articulo";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        /*public ActionResult EliminarArticulo(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();
            try
            {

                _ServiceArticulo.Eliminar((long)id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }*/


        public ActionResult IndexCatalogo()
        {
            IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                lista = _ServiceArticulo.GetArticulo();

                //enviar la lista de productos al viewBag
                IServiceProveedor _ServiceProveedor = new ServiceProveedor();
                ViewBag.listaProveedores = _ServiceProveedor.GetProveedor();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public PartialViewResult ArticulosxProveedor(long? id)
        {
            IEnumerable<Articulo> lista = null;
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();

            if (id != null)
            {
                lista = _ServiceArticulo.GetArticuloByProveedor((long)id);
            }
            return PartialView("_PartialViewArticulo", lista);
        }

        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Articulo art = cdt.Articulo.Where(x => x.id == id).FirstOrDefault();
                    art.estado = !art.estado;
                    cdt.Articulo.Add(art);
                    cdt.Entry(art).State = EntityState.Modified;
                    cdt.SaveChanges();
                    return RedirectToAction("Index");


                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }


    }
}
