using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public interface IRepositoryIngreso
    {

        IEnumerable<Ingreso> GetIngresos();
        Ingreso GetCompraByID(long id);
        Ingreso  Save(Ingreso  ingreso );
        void GetIngresoCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal);
    }
}
