﻿using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Web.Utils;

namespace Infraestructure.Repository
{
    public class RepositoryReparaciones : IRepositoryReparaciones
    {
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
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
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
                    lista = ctx.Reparaciones.Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).ToList<Reparaciones>();
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
                lista = ctx.Reparaciones.Include(x => x.Reportes_Tecnicos).Include(x => x.Servicio_Reparacion).Include(x => x.Usuario).ToList().
                    FindAll(l => l.cliente_id.Equals(Convert.ToInt32(nombre)));
            }
            return lista;
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
                    Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }

        }
    }
}
