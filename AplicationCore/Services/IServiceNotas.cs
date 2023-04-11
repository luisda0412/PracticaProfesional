using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceNotas
    {
        IEnumerable<NotasDeCreditoYDebito> GetNota();
        IEnumerable<NotasDeCreditoYDebito> GetListaNotasByNombre(string filtro);
        NotasDeCreditoYDebito GetNotaByID(int id);
        NotasDeCreditoYDebito Save(NotasDeCreditoYDebito nota);
        void Desabilitar(int id);

    }
}
