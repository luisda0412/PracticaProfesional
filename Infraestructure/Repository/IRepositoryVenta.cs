using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryVenta
    {
        IEnumerable<Venta> GetVentas();
        Venta GetVentaByID(long id);
        Venta Save(Venta  venta);
        void GetVentaCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal);
    }
}
