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
using Web.Utils;

namespace MvcApplication.Controllers
{
    public class CategoriaController : Controller
    {
        // GET: Categoria
        public ActionResult Index()
        {
            IEnumerable<Categoria> lista = null;
            try
            {
                IServiceCategoria _ServiceCategoria = new ServiceCategoria();
                lista = _ServiceCategoria.GetCategoria();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult Save(Categoria cat)
        {
            MemoryStream target = new MemoryStream();
            IServiceCategoria _ServiceCategoria = new ServiceCategoria();
            try
            {

                _ServiceCategoria.Save(cat);

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
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            ServiceCategoria _ServiceCategoria = new ServiceCategoria();
            Categoria cat = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                cat = _ServiceCategoria.GetCategoriaByID(id.Value);
                if (cat == null)
                {
                    TempData["Message"] = "No existe la categoria solicitada";
                    TempData["Redirect"] = "Usuario";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }
                return View(cat);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Categoria";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult buscarCategoriaxNombre(string filtro)
        {
            IEnumerable<Categoria> lista = null;
            IServiceCategoria _ServiceCategoria = new ServiceCategoria();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceCategoria.GetCategoria();
            }
            else
            {
                lista = _ServiceCategoria.GetCategoriaByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxNombre", lista);
        }

        public ActionResult Details(int? id)
        {
            ServiceCategoria _ServiceCategoria = new ServiceCategoria();
            Categoria cat= null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                cat = _ServiceCategoria.GetCategoriaByID(id.Value);

                if (cat == null)
                {
                    return RedirectToAction("Index");
                }
                return View(cat);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        public void desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Categoria cat = cdt.Categoria.Where(x => x.id == id).FirstOrDefault();
                    cat.estado = !cat.estado;
                    cdt.Categoria.Add(cat);

                    cdt.Entry(cat).State = EntityState.Modified;
                    cdt.SaveChanges();

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