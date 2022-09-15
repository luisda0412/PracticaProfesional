using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceRol : IServiceRol
    {
        public IEnumerable<Rol> GetRol()
        {
            IRepositoryRol repository = new RepositoryRol();
            return repository.GetRol();
        }

        public Rol GetRolByID(long id)
        {
            IRepositoryRol repository = new RepositoryRol();
            return repository.GetRolByID(id);
        }

        public Rol Save(Rol rol)
        {
            IRepositoryRol repository = new RepositoryRol();
            return repository.Save(rol);
        }
    }
}
