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
    public class RepositoryRol : IRepositoryRol
    {
        public IEnumerable<Rol> GetRol()
        {
            IEnumerable<Rol> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Rol.ToList<Rol>();
            }
            return lista;
        }

        public Rol GetRolByID(long id)
        {
            Rol rol = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    rol = ctx.Rol.Find(id);
                }

                return rol;
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

        public Rol Save(Rol rol)
        {
            int retorno = 0;
            Rol oRol = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oRol = GetRolByID((int)rol.id);

                if (oRol == null)
                {
                    rol.estado = true;
                    ctx.Rol.Add(rol);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Rol.Add(rol);
                    ctx.Entry(rol).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oRol = GetRolByID((int)rol.id);

            return oRol;
        }
    }
}
