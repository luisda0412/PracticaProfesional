using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RepositoryEmpresa : IRepositoryEmpresa
    {
        public IEnumerable<Empresa> GetEmpresa()
        {
            IEnumerable<Empresa> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Empresa.ToList<Empresa>();
            }
            return lista;
        }

        public void Save(Empresa empresa)
        {
            throw new NotImplementedException();
        }
    }
}
