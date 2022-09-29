using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryProveedor : IRepositoryProveedor
    {
        public void Eliminar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                try
                {

                    Proveedor prov= cdt.Proveedor.Where(a => a.id == id).FirstOrDefault();
                    cdt.Proveedor.Remove(prov);
                    cdt.SaveChanges();

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }

        public IEnumerable<Proveedor> GetProveedor()
        {
            IEnumerable<Proveedor> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Proveedor.ToList<Proveedor>();
            }
            return lista;
        }

        public Proveedor GetProveedorByID(long id)
        {
            Proveedor oProveedor = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oProveedor = ctx.Proveedor.Include(x=> x.Articulo).Where(x => x.id == id).FirstOrDefault();
            }
            return oProveedor;
        }

        public IEnumerable<Proveedor> GetProveedorByNombre(string nombre)
        {
            throw new NotImplementedException();
        }

        public void Save(Proveedor prov)
        {
            Proveedor proveedorExiste = GetProveedorByID(prov.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (proveedorExiste == null)
                    {
                        prov.estado = true;
                        cdt.Proveedor.Add(prov);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Proveedor.Add(prov);
                        cdt.Entry(prov).State = EntityState.Modified;
                        cdt.SaveChanges();
                    }
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
