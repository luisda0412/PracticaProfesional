using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryFactura
    {
        IEnumerable<Facturas> GetFactura();
        IEnumerable<Facturas> GetListaFacturaID(int id);
        Facturas GetFacturaByID(int id);
        Facturas Save(Facturas factura);
    }
}
