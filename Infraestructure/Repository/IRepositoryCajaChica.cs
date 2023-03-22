using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryCajaChica
    {
        IEnumerable<Arqueos_Caja> GetArqueos();
        IEnumerable<Caja_Chica > GetCajaChica();
        Caja_Chica GetCajaChicaLast();
        IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fecha);
        void Save(Caja_Chica caja);
        void SaveArqueo(Arqueos_Caja arq);
        void Eliminar(int id);
        Arqueos_Caja GetArqueoLast();

    }
}
