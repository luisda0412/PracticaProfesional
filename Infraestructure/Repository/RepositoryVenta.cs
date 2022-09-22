using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Infraestructure.Repository
{
    public class RepositoryVenta : IRepositoryVenta
    {
        public Venta GetVentaByID(long id)
        {
            throw new NotImplementedException();
        }

        public void GetVentaCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venta> GetVentas()
        {
            IEnumerable<Venta> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Venta.Include(x => x.Detalle_Venta).ToList<Venta>();
            }
            return lista;
        }

        public Venta Save(Venta venta)
        {
            throw new NotImplementedException();
        }
    }
}
