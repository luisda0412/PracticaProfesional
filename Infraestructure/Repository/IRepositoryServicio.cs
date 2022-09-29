using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryServicio
    {
        IEnumerable<Servicio_Reparacion> GetServicio();

        void Save(Servicio_Reparacion service);

        Servicio_Reparacion GetServicioByID(int id);

        IEnumerable<Servicio_Reparacion> GetServicioByDescripcion(String nombre);
        void Eliminar(int id);
    }
}
