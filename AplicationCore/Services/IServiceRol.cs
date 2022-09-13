using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceRol
    {
        IEnumerable<Rol> GetRol();
        Rol GetRolByID(long id);
        Rol Save(Rol rol);
    }
}
