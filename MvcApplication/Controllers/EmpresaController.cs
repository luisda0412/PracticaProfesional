using AplicationCore.Services;
using Infraestructure.Models;
using MvcApplication.Util;
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
    public class EmpresaController : Controller
    {
        // GET: Empresa
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Empresa> lista = null;
            try
            {
                IServiceEmpresa _ServiceEmpresa = new ServiceEmpresa();
                lista = _ServiceEmpresa.GetEmpresa();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
            }
            return View(lista);
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult Save(Empresa emp)
        {
      
            IServiceEmpresa _ServiceEmpresa = new ServiceEmpresa();
            if (ModelState.IsValid) {
                try
                {

                    _ServiceEmpresa.Save(emp);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "infomación guardada con éxito", SweetAlertMessageType.success);

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
        public ActionResult Edit(int? id)
        {
            IServiceEmpresa _ServiceEmpresa = new ServiceEmpresa();
            Empresa emp = null;

            try
            {
                // Si va null
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                emp = _ServiceEmpresa.GetEmpresaByID(id.Value);
                if (emp == null)
                {
                    TempData["Message"] = "No existe la la empresa solicitada";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }
                return View(emp);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }


        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            IServiceEmpresa _ServiceEmpresa = new ServiceEmpresa();
            Empresa emp = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                emp = _ServiceEmpresa.GetEmpresaByID(id.Value);

                if (emp == null)
                {
                    return RedirectToAction("Index");
                }
                return View(emp);

            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("Index");
            }
        }
    }
}