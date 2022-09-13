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

        public void  Save(Servicio_Reparacion service)
        {
            Servicio_Reparacion servicioexist = GetServicioByID(service.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (servicioexist == null)
                    {
                        service.estado = true;
                        cdt.Servicio_Reparacion.Add(service);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Servicio_Reparacion.Add(service);
                        cdt.Entry(service).State = EntityState.Modified;    
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
