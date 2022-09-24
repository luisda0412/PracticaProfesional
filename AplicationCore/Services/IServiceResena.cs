using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceResena
    {
        IEnumerable<Resena> GetResena();
        Resena GetResenaByID(int id);
        Resena Save(Resena resena);
    }
}
