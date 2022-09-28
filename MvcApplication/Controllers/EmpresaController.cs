using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
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
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }
    }
}