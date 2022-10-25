using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Utils;

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
                    factura = ctx.Facturas.Find(id);
                }

                return factura;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
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
