using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceCategoria : IServiceCategoria
    {
        public IEnumerable<Categoria> GetCategoria()
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            return repository.GetCategoria();
        }

        public Categoria GetCategoriaByID(long id)
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            return repository.GetCategoriaByID(id);
        }

        public Categoria Save(Categoria categoria)
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            return repository.Save(categoria);
        }
    }
}
