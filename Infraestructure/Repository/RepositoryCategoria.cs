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
    public class RepositoryCategoria : IRepositoryCategoria
    {
        public void Eliminar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                try
                {

                    Categoria cat = cdt.Categoria.Where(a => a.id == id).FirstOrDefault();
                    cdt.Categoria.Remove(cat);
                    cdt.SaveChanges();

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }

        public IEnumerable<Categoria> GetCategoria()
        {
            IEnumerable<Categoria> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Categoria.ToList<Categoria>();
            }
            return lista;
        }

        public Categoria GetCategoriaByID(int id)
        {
            Categoria prod = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    prod = ctx.Categoria.Find(id);
                }

                return prod;
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

        public IEnumerable<Categoria> GetCategoriaByNombre(string nombre)
        {
            IEnumerable<Categoria> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Categoria.ToList().
                    FindAll(l => l.nombre.ToLower().Contains(nombre.ToLower()));
            }
            return lista;
        }

        public void Save(Categoria categoria)
        {
            Categoria catExiste = GetCategoriaByID(categoria.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (catExiste == null)
                    {
                        categoria.estado = true;
                        cdt.Categoria.Add(categoria);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Categoria.Add(categoria);
                        cdt.Entry(categoria).State = EntityState.Modified;
                        cdt.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }
    }
}

