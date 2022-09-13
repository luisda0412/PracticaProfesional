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
    public class RepositoryCategoria : IRepositoryCategoria
    {
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

        public Categoria GetCategoriaByID(long id)
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

        public Categoria Save(Categoria categoria)
        {
            int retorno = 0;
            Categoria oCat = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oCat = GetCategoriaByID((int)categoria.id);

                if (oCat == null)
                {
                    categoria.estado = true;
                    ctx.Categoria.Add(categoria);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Categoria.Add(categoria);
                    ctx.Entry(categoria).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oCat = GetCategoriaByID((int)categoria.id);

            return oCat;
        }
    }
    }

