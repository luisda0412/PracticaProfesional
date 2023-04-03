using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceReparaciones
    {
        IEnumerable<Reparaciones> GetReparacion();

        IEnumerable<Reparaciones> GetReparacionesCobradas();
        Reparaciones GetReparacionByID(int id);
        IEnumerable<Reparaciones> GetReparacionByNombre(String nombre);
        void Save(Reparaciones reparacion);
        void Eliminar(int id);

        void Desabilitar(int id);
        IEnumerable<Reparaciones> GetReparacionPorUsuario(int idUsuario);
    }
}
