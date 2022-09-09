using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RepositoryProveedor : IRepositoryProveedor
    {
        public IEnumerable<Proveedor> GetProveedor()
        {
            IEnumerable<Proveedor> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Proveedor.ToList<Proveedor>();
            }
            return lista;
        }

        public Proveedor GetProveedorByID(long id)
        {
            Proveedor oProveedor = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oProveedor = ctx.Proveedor.Include(x=> x.Articulo).Where(x => x.id == id).FirstOrDefault();
            }
            return oProveedor;
        }

        public IEnumerable<Proveedor> GetProveedorByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public void Save(Proveedor prov)
        {
            throw new NotImplementedException();
        }
    }
}
