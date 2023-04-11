using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace Infraestructure.Repository
{
    public class RepositoryReparaciones : IRepositoryReparaciones
    {
        public void Desabilitar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Reparaciones repo = cdt.Reparaciones.Find(id);
                    repo.estado = false;
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

    public void Eliminar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {


                    Reparaciones repo = cdt.Reparaciones.Find(id);
                    cdt.Reparaciones.Remove(repo);
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

        public IEnumerable<Reparaciones> GetReparacion()
        {
            try
            {
                IEnumerable<Reparaciones> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Reparaciones
                    .Where(x => x.estado == true)
                    .Include(x => x.Servicio_Reparacion)
                    .Include(x => x.Usuario)
                    .OrderByDescending(x => x.fecha)
                    .ToList<Reparaciones>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Reparaciones GetReparacionByID(int id)
        {
            Reparaciones oReparacion = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oReparacion = ctx.Reparaciones.Where(a => a.id == id).Include(x => x.Reportes_Tecnicos).Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).FirstOrDefault();
            }
            return oReparacion;
        }

        public IEnumerable<Reparaciones> GetReparacionByNombre(string nombre)
        {
            IEnumerable<Reparaciones> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Reparaciones
                            .Include(x => x.Reportes_Tecnicos)
                            .Include(x => x.Servicio_Reparacion)
                            .Include(x => x.Usuario).ToList()
                            .FindAll(l => l.cliente_id == Convert.ToInt32(nombre) && l.estado == true);                  
            }
            return lista;
        }

        public IEnumerable<Reparaciones> GetReparacionesCobradas()
        {
            try
            {
                IEnumerable<Reparaciones> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Reparaciones.Where(x => x.estado == false).Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).ToList<Reparaciones>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public IEnumerable<Reparaciones> GetReparacionPorUsuario(int idUsuario)
        {
            try
            {
                IEnumerable<Reparaciones> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Reparaciones.Where(x=> x.cliente_id==idUsuario && x.estado ==true).Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).ToList<Reparaciones>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
            catch (Exception e)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public void Save(Reparaciones reparacion)
        {
            Reparaciones reparacionExist = GetReparacionByID(reparacion.id);
         

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (reparacionExist == null)
                    {

                       
                        reparacion.estado = true;
                        cdt.Reparaciones.Add(reparacion);
                        cdt.SaveChanges();
                        string msj = "Se ha registrado una nueva reparación bajo la cédula: ";
                        Infraestructure.Util.Log.Info(msj + reparacion.cliente_id);
                    }
                    else
                    {
                        cdt.Reparaciones.Add(reparacion);
                        cdt.Entry(reparacion).State = EntityState.Modified;
                        cdt.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    string mensaje = "Error" + e.Message;
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }

        }
    }
}
