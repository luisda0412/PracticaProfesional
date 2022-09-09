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
        public IEnumerable<Articulo> GetArticulo()
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            return repository.GetArticulo();
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

        public void Save(Articulo articulo, string[] proveedor)
        {
            IRepositoryArticulo repository = new RepositoryArticulo();
            repository.Save(articulo, proveedor);
        }
    }
}
