﻿using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceArticulo : IServiceArticulo
    {
        public void actualizarCantidad(int id, int cantidad, bool tipoMovimiento)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            repository.actualizarCantidad(id, cantidad, tipoMovimiento);
        }

        public void Eliminar(long id)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            repository.Eliminar(id);
        }

        public IEnumerable<Articulo> GetArticulo()
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetArticulo();
        }
        public IEnumerable<Articulo> GetArticuloMante()
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetArticuloMante();
        }


        public Articulo GetArticuloByID(int id)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetArticuloByID(id);
        }

        public IEnumerable<Articulo> GetArticuloByNombre(string nombre)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetProductoByNombre(nombre);
        }

        public IEnumerable<Articulo> GetArticuloByProveedor(long id)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetArticuloByProveedor(id);
        }

        public void Save(Articulo articulo)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            repository.Save(articulo);
        }
    }
}
