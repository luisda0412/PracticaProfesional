using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceArticulo
    {
        IEnumerable<Articulo> GetArticulo();
        Articulo GetArticuloByID(int id);
        void Save(Articulo articulo);
    }
}
