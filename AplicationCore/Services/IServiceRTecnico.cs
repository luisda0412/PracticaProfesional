using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceRTecnico
    {
        IEnumerable<Reportes_Tecnicos> GetReportesByID(long id);
        void Save(Reportes_Tecnicos reportes);
        void Eliminar(long id);
    }
}
