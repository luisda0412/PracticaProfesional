using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryNotas
    {
        IEnumerable<NotasDeCreditoYDebito> GetNota();
        IEnumerable<NotasDeCreditoYDebito> GetListaNotasFecha(DateTime date);
        NotasDeCreditoYDebito GetNotaByID(int id);
        NotasDeCreditoYDebito Save(NotasDeCreditoYDebito nota);
        
        void Desabilitar(int id);

    }
}
