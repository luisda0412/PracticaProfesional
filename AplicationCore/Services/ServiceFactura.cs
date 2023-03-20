using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceFactura : IServiceFactura
    {
        public IEnumerable<Facturas> GetFactura()
        {
            throw new NotImplementedException();
        }

        public Facturas GetFacturaByID(int id)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetFacturaByID(id);
        }

        public IEnumerable<Facturas> GetListaFacturaID(int id)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.GetListaFacturaID(id);
        }

        public Facturas Save(Facturas factura)
        {
            IRepositoryFactura repository = new RepositoryFactura();
            return repository.Save(factura);
        }
    }
}
