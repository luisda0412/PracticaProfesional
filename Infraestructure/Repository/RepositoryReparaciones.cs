using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryReparaciones : IRepositoryReparaciones
    {
        public void Eliminar(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Reparaciones> GetReparacion()
        {
            try
            {
                IEnumerable<Reparaciones> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Reparaciones.Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).ToList<Reparaciones>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
            catch (Exception e)
            {
                string mensaje = "";
                Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Reparaciones GetReparacionByID(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Reparaciones> GetReparacionByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public void Save(Reparaciones reparacion)
        {
            throw new NotImplementedException();
        }
    }
}
