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
           
          
            return RedirectToAction("IndexAdmin", "Home");

        }
    }
}