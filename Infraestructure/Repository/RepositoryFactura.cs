using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infraestructure.Repository
{
    public class RepositoryFactura : IRepositoryFactura
    {
        public IEnumerable<Facturas> GetFactura()
        {
            throw new NotImplementedException();
        }

        public Facturas GetFacturaByID(int id)
        {
            Facturas factura = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    factura = ctx.Facturas.Include(f => f.Venta)
                         .Include(f => f.Venta.Usuario)
                         .Include(f => f.Venta.Detalle_Venta.Select(dv => dv.Articulo))
                         .FirstOrDefault(f => f.id == id);
                }

                return factura;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
        }

        public IEnumerable<Facturas> GetListaFacturaID(int id)
        {
            IEnumerable< Facturas> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Facturas.Where(x => x.id == id).Include(x => x.Venta).ToList();
                  
            }
            return lista;
        }

        public Facturas Save(Facturas factura)
        {
            int retorno = 0;
            Facturas oFacturas = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oFacturas = GetFacturaByID((int)factura.id);

                if (oFacturas == null)
                {
                    ctx.Facturas.Add(factura);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Facturas.Add(factura);
                    ctx.Entry(factura).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oFacturas = GetFacturaByID((int)factura.id);

            return oFacturas;
        }
    }
}
