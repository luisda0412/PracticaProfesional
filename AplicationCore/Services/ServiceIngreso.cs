using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceIngreso : IServiceIngreso
    {
        public Ingreso GetCompraByID(long id)
        {
            IRepositoryIngreso repository = new RepositoryIngreso();
            return repository.GetCompraByID(id);
        }

        public void GetIngresoCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ingreso> GetIngresos()
        {
            IRepositoryIngreso repository = new RepositoryIngreso();
            return repository.GetIngresos();
        }

        public Ingreso Save(Ingreso ingreso)
        {
            IRepositoryIngreso repository = new RepositoryIngreso();
            return repository.Save(ingreso);
        }
    }
}
