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

        public Categoria GetCategoriaByID(int id)
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            return repository.GetCategoriaByID(id);
        }

        public IEnumerable<Categoria> GetCategoriaByNombre(string nombre)
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            return repository.GetCategoriaByNombre(nombre);
        }

        public void Save(Categoria categoria)
        {
            IRepositoryCategoria repository = new RepositoryCategoria();
            repository.Save(categoria);
        }
    }
}
