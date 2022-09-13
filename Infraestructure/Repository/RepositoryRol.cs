using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RepositoryRol : IRepositoryRol
    {
        public IEnumerable<Rol> GetRol()
        {
            IEnumerable<Rol> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Rol.ToList<Rol>();
            }
            return lista;
        }

        public Rol GetRolByID(long id)
        {
            throw new NotImplementedException();
        }

        public Rol Save(Rol rol)
        {
            throw new NotImplementedException();
        }
    }
}
