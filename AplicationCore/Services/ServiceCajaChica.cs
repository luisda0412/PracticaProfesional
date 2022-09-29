using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceCajaChica : IServiceCajaChica
    {
        public void Eliminar(int id)
        {
            IRepositoryCajaChica repository = new RepositoryCajaChica();
            repository.Eliminar(id);
        }

        public IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fecha)
        {
            IRepositoryCajaChica repository = new RepositoryCajaChica();
            return repository.GetCajaByFecha(fecha);
        }

        public IEnumerable<Caja_Chica> GetCajaChica()
        {
            IRepositoryCajaChica repository = new RepositoryCajaChica();
            return repository.GetCajaChica();
        }

        public Caja_Chica GetCajaChicaByID(int id)
        {
            IRepositoryCajaChica repository = new RepositoryCajaChica();
            return repository.GetCajaChicaByID(id);
        }

        public void Save(Caja_Chica caja, string[] usuario)
        {
            IRepositoryCajaChica repository = new RepositoryCajaChica();
            repository.Save(caja, usuario);
        }
    }
}
