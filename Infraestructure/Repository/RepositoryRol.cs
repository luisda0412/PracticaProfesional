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

        public Rol GetRolByID(int id)
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

        public void Save(Rol rol)
        {
            Rol rolExiste = GetRolByID(rol.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (rolExiste == null)
                    {
                        rol.estado = true;
                        cdt.Rol.Add(rol);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Rol.Add(rol);
                        cdt.Entry(rol).State = EntityState.Modified;
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
