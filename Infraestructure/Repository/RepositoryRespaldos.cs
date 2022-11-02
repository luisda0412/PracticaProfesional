using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryRespaldos : IRepositoryRespaldos
    {
        public void guardarRespaldo()
        {
            try
            {
                string path = @"C:\RespaldosSCAP";
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string url = @"'C:\RespaldosVYCUZ\VYCUZ_" + DateTime.Now.ToString("dd-MMMM-yyyy HH-mm") + ".bak'";
                    using (MyContext ctx = new MyContext())
                    {
                        ctx.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "backup database Registro_Inventario_VYCUZ to disk = " + url);
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public void restaurarRespaldo(string ruta)
        {
            try
            {
                SqlConnection con = new SqlConnection("data source=localhost;initial catalog=Registro_Inventario_VYCUZ;user id=sa;password=123456;MultipleActiveResultSets=True;");

                con.Open();

                string sqlStmt2 = string.Format("ALTER DATABASE Registro_Inventario_VYCUZ SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                SqlCommand bu2 = new SqlCommand(sqlStmt2, con);
                bu2.ExecuteNonQuery();

                string sqlStmt3 = "USE MASTER RESTORE DATABASE Registro_Inventario_VYCUZ FROM DISK='" + ruta + "'WITH REPLACE;";
                SqlCommand bu3 = new SqlCommand(sqlStmt3, con);
                bu3.ExecuteNonQuery();

                string sqlStmt4 = string.Format("ALTER DATABASE Registro_Inventario_VYCUZ SET MULTI_USER");
                SqlCommand bu4 = new SqlCommand(sqlStmt4, con);
                bu4.ExecuteNonQuery();

                con.Close();

            }
            catch (Exception dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }
    }
}
