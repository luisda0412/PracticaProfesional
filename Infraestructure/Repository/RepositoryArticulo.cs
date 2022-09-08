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
    public class RepositoryArticulo : IRepositoryArticulo
    {
        public IEnumerable<Articulo> GetArticulo()
        {
            try
            {
                IEnumerable<Articulo> lista = null;
                using (MyContext ctx= new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Articulo.Include(x=> x.Categoria).ToList<Articulo>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref(mensaje));
                throw new Exception(mensaje);
            }
            catch (Exception e)
            {
                string mensaje = "";
                Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Articulo GetArticuloByID(int id)
        {
            Articulo oArticulo = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oArticulo = ctx.Articulo.Where(a => a.id == id).Include(x => x.Categoria).FirstOrDefault();
            }
            return oArticulo;
        }

        public void Save(Articulo articulo)
        {
            
                Articulo articuloExist = GetArticuloByID(articulo.id);

                using (MyContext cdt = new MyContext())
                {
                    cdt.Configuration.LazyLoadingEnabled = false;

                    try
                    {
                        if (articuloExist == null)
                        {
                            articulo.estado = true;

                            cdt.Articulo.Add(articulo);

                            cdt.SaveChanges();
                        }
                        else
                        {
                            cdt.Articulo.Add(articulo);
                            cdt.Entry(articulo).State = EntityState.Modified;
                            cdt.SaveChanges();

                        }
                    }
                    catch (Exception e)
                    {
                        string mensaje = "Error" + e.Message;
                        Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                        throw;
                    }
                }

            }
    }
}
