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
    public class RepositoryResena : IRepositoryResena
    {
        public void Eliminar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Resena res = cdt.Resena.Find(id);
                    cdt.Resena.Remove(res);
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

        public IEnumerable<Resena> GetResena()
        {
            try
            {
                IEnumerable<Resena> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Resena.Include(x => x.Articulo).Include(x => x.Usuario).ToList<Resena>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
            catch (Exception e)
            {
                string mensaje = "";
                Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Resena GetResenaByID(int id)
        {
            Resena oResena = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oResena = ctx.Resena.Where(p => p.id == id).
                    Include(x => x.Articulo).Include(x=>x.Usuario).FirstOrDefault();
            }
            return oResena;
        }

        public IEnumerable<Resena> GetResenaByIDArticulo(long id)
        {
            IEnumerable<Resena> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Resena.
                    Where(p => p.articulo_id == id).Include(p => p.Articulo).Include(p => p.Usuario).ToList();
            }
            return lista;
        }

        public Resena Save(Resena resena)
        {
            int retorno = 0;
            Resena oResena = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oResena = GetResenaByID((int)resena.id);

                if (oResena == null)
                {
                    //resena.estado = true;
                    ctx.Resena.Add(resena);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Resena.Add(resena);
                    ctx.Entry(resena).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oResena = GetResenaByID((int)resena.id);

            return oResena;
        }
    }
}
