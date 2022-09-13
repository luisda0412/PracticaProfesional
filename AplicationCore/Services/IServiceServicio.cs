using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceServicio
    {
        IEnumerable<Servicio_Reparacion> GetServicio();

        void Save(Servicio_Reparacion service);

        Servicio_Reparacion GetServicioByID(int id);
    }
}
