using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceProveedor : IServiceProveedor
    {
        public void Eliminar(int id)
        {
            IRepositoryProveedor repository = new RepositoryProveedor();
            repository.Eliminar(id);
        }

        public IEnumerable<Proveedor> GetProveedor()
        {
            IRepositoryProveedor repository = new RepositoryProveedor();
            return repository.GetProveedor();
        }

        public Proveedor GetProveedorByID(long id)
        {
            IRepositoryProveedor repository = new RepositoryProveedor();
            return repository.GetProveedorByID(id);
        }

        public IEnumerable<Proveedor> GetProveedorByNombre(string nombre)
        {
            IRepositoryProveedor repository = new RepositoryProveedor();
            return repository.GetProveedorByNombre(nombre);
        }

        public void Save(Proveedor prov)
        {
            IRepositoryProveedor repository = new RepositoryProveedor();
            repository.Save(prov);
        }
    }
}
