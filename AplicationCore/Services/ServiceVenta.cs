using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceVenta : IServiceVenta
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
            IRepositoryVenta repository = new RepositoryVenta();
            return repository.GetVentas();
        }

        public Venta Save(Venta venta)
        {
            IRepositoryVenta repository = new RepositoryVenta();
            return repository.Save(venta);
        }
    }
}
