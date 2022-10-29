﻿using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceEmpresa : IServiceEmpresa
    {
        public IEnumerable<Empresa> GetEmpresa()
        {
            IRepositoryEmpresa repository = new RepositoryEmpresa();
            return repository.GetEmpresa();
        }

        public Empresa GetEmpresaByID(int id)
        {
            IRepositoryEmpresa repository = new RepositoryEmpresa();
            return repository.GetEmpresaByID(id);
        }

        public void Save(Empresa empresa)
        {
            IRepositoryEmpresa repository = new RepositoryEmpresa();
            repository.Save(empresa);
        }
    }
}