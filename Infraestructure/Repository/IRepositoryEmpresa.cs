using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryEmpresa
    {
        IEnumerable<Empresa> GetEmpresa();
        void Save(Empresa empresa);
        Empresa GetEmpresaByID(int id);
   

    }
}
