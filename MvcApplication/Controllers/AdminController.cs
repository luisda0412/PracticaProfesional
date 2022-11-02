using MvcApplication.Util;
using MySql.Data.MySqlClient;
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

            string cnx = "server=localhost;user=sa;pwd=123456;database=Registro_Inventario_VYCUZ;";
            cnx += "charset=utf8;convertzerodatetime=true;";

            string mibackup = "C:\\vycuzBackup.sql";

            using (MySqlConnection conn = new MySqlConnection(cnx))
            {
                using (MySqlCommand cmd= new MySqlCommand())
                {
                    using (MySqlBackup mb= new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(mibackup);
                        conn.Close();
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Respaldo Exitoso!", "El respado se ha creado en su carpeta local C", SweetAlertMessageType.info);
                    }

                }

            }
            return RedirectToAction("IndexAdmin", "Home");

        }
    }
}