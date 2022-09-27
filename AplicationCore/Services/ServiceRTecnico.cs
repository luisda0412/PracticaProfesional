using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceRTecnico : IServiceRTecnico
    {
        public void Eliminar(long id)
        {
            IRepositoryRTecnico repository = new RepositoryRTecnico();
            repository.Eliminar(id);
        }

        public IEnumerable<Reportes_Tecnicos> GetReportesByID(long id)
        {
            IRepositoryRTecnico repository = new RepositoryRTecnico();
            return repository.GetReportesByID(id);
        }

        public void Save(Reportes_Tecnicos reportes)
        {
            IRepositoryRTecnico repository = new RepositoryRTecnico();
            repository.Save(reportes);
        }
    }
}
