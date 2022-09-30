using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AplicationCore.Services
{
    public interface IServiceEmpresa
    {
        IEnumerable<Empresa> GetEmpresa();
        void Save(Empresa empresa);

        Empresa GetEmpresaByID(int id);
    }
}
