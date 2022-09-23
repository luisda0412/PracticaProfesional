using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryResena : IRepositoryResena
    {
        public IEnumerable<Resena> GetResena()
        {
            try
            {
                IEnumerable<Resena> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Resena.Include(x => x.Articulo).Include(x => x.Usuario).ToList<Resena>();
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

        public Resena GetResenaByID(int id)
        {
            throw new NotImplementedException();
        }

        public Resena Save(Resena rol)
        {
            throw new NotImplementedException();
        }
    }
}
