using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryRTecnico : IRepositoryRTecnico
    {
        public void Eliminar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {


                    Reportes_Tecnicos repo = cdt.Reportes_Tecnicos.Find(id);
                    cdt.Reportes_Tecnicos.Remove(repo);              
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

        public IEnumerable<Reportes_Tecnicos> GetReportesByID(long id)
        {
            IEnumerable<Reportes_Tecnicos> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Reportes_Tecnicos.
                    Where(p => p.reparacion_id == id).ToList();
            }
            return lista;
        }

        public void Save(Reportes_Tecnicos reportes)
        {
            throw new NotImplementedException();
        }
    }
}
