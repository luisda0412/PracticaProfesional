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
    public class RepositoryServicio : IRepositoryServicio
    {
        public IEnumerable<Servicio_Reparacion> GetServicio()
        {
            IEnumerable<Servicio_Reparacion> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Servicio_Reparacion.ToList<Servicio_Reparacion>();
            }
            return lista;
        }

        public Servicio_Reparacion GetServicioByID(int id)
        {
            Servicio_Reparacion service = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    service = ctx.Servicio_Reparacion.Find(id);
                }

                return service;
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
        }

        public IEnumerable<Servicio_Reparacion> GetServicioByDescripcion(string nombre)
        {
            IEnumerable<Servicio_Reparacion> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Servicio_Reparacion.ToList().
                    FindAll(l => l.descripcion.ToLower().Contains(nombre.ToLower()));
            }
            return lista;
        }

        public void Save(Servicio_Reparacion serv)
        {
            Servicio_Reparacion provExist = GetServicioByID(serv.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (provExist == null)
                    {
                        serv.estado = true;
                        cdt.Servicio_Reparacion.Add(serv);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Servicio_Reparacion.Add(serv);
                        cdt.Entry(serv).State = EntityState.Modified;
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

        public void Eliminar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                try
                {

                    Servicio_Reparacion serv = cdt.Servicio_Reparacion.Where(a => a.id == id).FirstOrDefault();
                    cdt.Servicio_Reparacion.Remove(serv);
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
    }
}
