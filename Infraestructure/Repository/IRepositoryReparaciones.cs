using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryReparaciones
    {
        IEnumerable<Reparaciones> GetReparacion();
        Reparaciones GetReparacionByID(int id);
        IEnumerable<Reparaciones> GetReparacionByNombre(String nombre);
        void Save(Reparaciones reparacion);
        void Eliminar(int id);

        void Desabilitar(int id);
        IEnumerable<Reparaciones> GetReparacionPorUsuario(int idUsuario);
    }
}
