using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Web.Utils;



namespace Infraestructure.Repository
{
    public class RepositoryIngreso : IRepositoryIngreso
    {
        public Ingreso GetCompraByID(long id)
        {
            Ingreso ingreso = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    ingreso = ctx.Ingreso.
                               Include("Detalle_Ingreso").
                               Where(p => p.id == id).
                               FirstOrDefault<Ingreso>();

                }
                return ingreso;

            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public void GetIngresoCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ingreso> GetIngresos()
        {
            IEnumerable<Ingreso> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Ingreso.Include(x => x.Detalle_Ingreso).ToList<Ingreso>();

            }
            return lista;
        }

        public Ingreso Save(Ingreso ingreso)
        {
            int resultado = 0;
            Ingreso detalle = null;
            try
            {
                using (MyContext cdt = new MyContext())
                {
                    using (var transaccion = cdt.Database.BeginTransaction())
                    {
                        cdt.Ingreso.Add(ingreso);
                        resultado = cdt.SaveChanges();

                        foreach (var linea in ingreso.Detalle_Ingreso)
                        {
                            linea.ingreso_id = ingreso.id;
                        }
                        foreach (var item in ingreso.Detalle_Ingreso)
                        {
                            Articulo oArticulo = cdt.Articulo.Find(item.articulo_id);
                            cdt.Entry(oArticulo).State = EntityState.Modified;
                            resultado = cdt.SaveChanges();
                        }

                        transaccion.Commit();
                    }
                }

                if (resultado >= 0)

                    detalle = GetCompraByID(ingreso.id);
                return detalle;


            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }
    }
}
