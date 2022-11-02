using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceRespaldos : IServiceRespaldos
    {
        public void guardarRespaldo()
        {
            IRepositoryRespaldos repositoryRespaldos = new RepositoryRespaldos();
            repositoryRespaldos.guardarRespaldo();
        }

        public void restaurarRespaldo(string ruta)
        {
            IRepositoryRespaldos repositoryRespaldos = new RepositoryRespaldos();
            repositoryRespaldos.restaurarRespaldo(ruta);
        }
    }
}
