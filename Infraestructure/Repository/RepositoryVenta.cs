using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Web.Utils;
using System.Xml;

namespace Infraestructure.Repository
{
    public class RepositoryVenta : IRepositoryVenta
    {
        public Venta GetVentaByID(long id)
        {
            Venta venta = null;
            try
            {
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    venta = ctx.Venta.
                               Include("Detalle_Venta").
                               Where(p => p.id == id).
                               FirstOrDefault<Venta>();

                }
                return venta;

            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
        }

        public void GetVentaCountDate(out string etiquetas, out string valores, DateTime fechainicial, DateTime fechafinal)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venta> GetVentas()
        {
            IEnumerable<Venta> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Venta.Include(x => x.Detalle_Venta).ToList<Venta>();
            }
            return lista;
        }

        public Venta Save(Venta venta)
        {
            int resultado = 0;
            Venta detalle = null;
            try
            {
                using (MyContext cdt = new MyContext())
                {
                    using (var transaccion = cdt.Database.BeginTransaction())
                    {
                        cdt.Venta.Add(venta);
                        resultado = cdt.SaveChanges();

                        foreach (var linea in venta.Detalle_Venta)
                        {
                            linea.venta_id = venta.id;
                        }
                        foreach (var item in venta.Detalle_Venta)
                        {
                            Articulo oArticulo = cdt.Articulo.Find(item.articulo_id);
                            cdt.Entry(oArticulo).State = EntityState.Modified;
                            resultado = cdt.SaveChanges();
                        }

                        transaccion.Commit();
                    }
                }

                if (resultado >= 0)

                    detalle = GetVentaByID(venta.id);
                return detalle;


            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }

        }

        public void aplicarDescuento(int idArticulo, double descuento)
        {
            IRepositoryArticulo repoA = new RepositoryArticulo();
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                try
                {

                    Articulo oArticulo = repoA.GetArticuloByID(idArticulo);
                    descuento = descuento / 100;
                    oArticulo.precio = oArticulo.precio - (oArticulo.precio * descuento);
                    cdt.Articulo.Add(oArticulo);
                    cdt.Entry(oArticulo).State = EntityState.Modified;

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }
    }
}
