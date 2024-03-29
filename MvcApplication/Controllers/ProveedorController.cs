﻿using AplicationCore.Services;
using Infraestructure.Models;
using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;


namespace MvcApplication.Controllers
{
    public class ProveedorController : Controller
    {
        // GET: Proveedor
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Proveedor> lista = null;
            try
            {
                IServiceProveedor _ServiceProveedor = new ServiceProveedor();
                lista = _ServiceProveedor.GetProveedor();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
            }
            return View(lista);
        }

        public ActionResult buscarProveedorxNombre(string filtro)
        {
            IEnumerable<Proveedor> lista = null;
            IServiceProveedor _ServiceProveedor= new ServiceProveedor();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceProveedor.GetProveedor();
            }
            else
            {
                lista = _ServiceProveedor.GetProveedorByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxNombre", lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Proveedor prov)
        {
          

            IServiceProveedor _ServiceProveedor = new ServiceProveedor();
            if (ModelState.IsValid)
            {
                try
                {
                    _ServiceProveedor.Save(prov);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "proveedor guardado con éxito", SweetAlertMessageType.success);
                   
                }
                catch (Exception ex)
                {

                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                    return RedirectToAction("Default", "Error");
                }
            }
            return RedirectToAction("Index");

        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            ServiceProveedor _ServiceProveedor = new ServiceProveedor();
            Proveedor prov= null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                prov = _ServiceProveedor.GetProveedorByID(id.Value);

                if (prov == null)
                {
                    TempData["Message"] = "No existe el proveedor solicitado";
                    TempData["Redirect"] = "Proveedor";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                return View(prov);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Rol";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            ServiceProveedor _ServiceProveedor = new ServiceProveedor();
            Proveedor prov = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                prov = _ServiceProveedor.GetProveedorByID(id.Value);

                if (prov == null)
                {
                    return RedirectToAction("Index");
                }
                return View(prov);

            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

       
        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Proveedor pro = cdt.Proveedor.Where(x => x.id == id).FirstOrDefault();
                    pro.estado = !pro.estado;
                    cdt.Proveedor.Add(pro);
                    cdt.Entry(pro).State = EntityState.Modified;
                    cdt.SaveChanges();
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }
    }
}