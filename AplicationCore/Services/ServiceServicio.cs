﻿using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceServicio : IServiceServicio
    {
        public IEnumerable<Servicio_Reparacion> GetServicio()
        {
            IRepositoryServicio repository = new RepositoryServicio();
            return repository.GetServicio();
        }

        public Servicio_Reparacion GetServicioByID(int id)
        {
            IRepositoryServicio repository = new RepositoryServicio();
            return repository.GetServicioByID(id);
        }

        public IEnumerable<Servicio_Reparacion> GetServicioByDescripcion(string nombre)
        {
            IRepositoryServicio repository = new RepositoryServicio();
            return repository.GetServicioByDescripcion(nombre);
        }

        public void Save(Servicio_Reparacion service)
        {
            IRepositoryServicio repository = new RepositoryServicio();
            repository.Save(service);
        }

        public void Eliminar(int id)
        {
            IRepositoryServicio repository = new RepositoryServicio();
            repository.Eliminar(id);
        }
    }
}
