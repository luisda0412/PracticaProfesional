using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceCategoria
    {
        IEnumerable<Categoria> GetCategoria();
        Categoria GetCategoriaByID(long id);
        Categoria Save(Categoria categoria);
        IEnumerable<Categoria> GetCategoriaByNombre(String nombre);
    }
}
