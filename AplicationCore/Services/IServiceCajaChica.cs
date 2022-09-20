using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceCajaChica
    {
        IEnumerable<Caja_Chica> GetCajaChica();
        Caja_Chica GetCajaChicaByID(int id);
        IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fecha);
        void Save(Caja_Chica caja, string[] usuario);
    }
}
