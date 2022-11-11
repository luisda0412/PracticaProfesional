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
        IEnumerable<Articulo> GetArticuloMante();
        IEnumerable<Articulo> GetArticulo();
        Articulo GetArticuloByID(int id);
        IEnumerable<Articulo> GetArticuloByNombre(String nombre);
        void Save(Articulo articulo);
        IEnumerable<Articulo> GetArticuloByProveedor(long id);
        void Eliminar(long id);
        void actualizarCantidad(int id, int cantidad, bool tipoMovimiento);
    }
}
