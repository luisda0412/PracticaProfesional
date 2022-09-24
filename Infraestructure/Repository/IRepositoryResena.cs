using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryResena
    {
        IEnumerable<Resena> GetResena();
        Resena GetResenaByID(int id);
        Resena Save(Resena resena);
    }
}
