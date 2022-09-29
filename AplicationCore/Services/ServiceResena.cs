using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceResena : IServiceResena
    {
        public void Eliminar(long id)
        {
            IRepositoryResena repository = new RepositoryResena();
            repository.Eliminar(id);
        }

        public IEnumerable<Resena> GetResena()
        {
            IRepositoryResena repository = new RepositoryResena();
            return repository.GetResena();
        }

        public Resena GetResenaByID(int id)
        {
            throw new NotImplementedException();
        }

        public Resena Save(Resena resena)
        {
            IRepositoryResena repository = new RepositoryResena();
            return repository.Save(resena);
        }
    }
}
