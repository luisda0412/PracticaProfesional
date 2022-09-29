using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceReparaciones : IServiceReparaciones
    {
        public void Eliminar(int id)
        {
            IRepositoryReparaciones repository = new RepositoryReparaciones();
            repository.Eliminar(id);
        }

        public IEnumerable<Reparaciones> GetReparacion()
        {
            IRepositoryReparaciones repository = new RepositoryReparaciones();
            return repository.GetReparacion();
        }

        public Articulo GetReparacionByID(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Reparaciones> GetReparacionByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public void Save(Reparaciones reparacion)
        {
            throw new NotImplementedException();
        }
    }
}
