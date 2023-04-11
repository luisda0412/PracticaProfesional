using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceNotas : IServiceNotas
    {
        public void Desabilitar(int id)
        {
            IRepositoryNotas repository = new RepositoryNotas();
            repository.Desabilitar(id);
        }

        public IEnumerable<NotasDeCreditoYDebito> GetListaNotasByNombre(string filtro)
        {
            IRepositoryNotas repository = new RepositoryNotas();
            return repository.GetListaNotasNombre(filtro);
        }

        public IEnumerable<NotasDeCreditoYDebito> GetNota()
        {
            IRepositoryNotas repository = new RepositoryNotas();
            return repository.GetNota();
        }

        public NotasDeCreditoYDebito GetNotaByID(int id)
        {
            IRepositoryNotas repository = new RepositoryNotas();
            return repository.GetNotaByID(id);
        }

        public NotasDeCreditoYDebito Save(NotasDeCreditoYDebito nota)
        {
            IRepositoryNotas repository = new RepositoryNotas();
            return repository.Save(nota);
        }
    }
}
