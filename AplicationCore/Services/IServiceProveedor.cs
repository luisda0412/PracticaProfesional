using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceProveedor
    {
        IEnumerable<Proveedor> GetProveedor();
        Proveedor GetProveedorByID(long id);
        IEnumerable<Proveedor> GetProveedorByNombre(String nombre);
        void Save(Proveedor prov);
    }
}
