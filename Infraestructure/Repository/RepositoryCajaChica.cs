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
    public class RepositoryCajaChica : IRepositoryCajaChica
    {
        IRepositoryUsuario repoU = new RepositoryUsuario();
        public IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fecha)
        {
            IEnumerable<Caja_Chica> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Caja_Chica.ToList().
                    FindAll(l => l.fecha.Equals(fecha.Date));
            }
            return lista;
        }

        public void Eliminar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Caja_Chica caja = cdt.Caja_Chica.Find(id);
                    cdt.Caja_Chica.Remove(caja);
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
        public IEnumerable<Caja_Chica> GetCajaChica()
        {
            try
            {
                IEnumerable<Caja_Chica> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Caja_Chica.ToList<Caja_Chica>();
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

        public Caja_Chica GetCajaChicaByID(int id)
        {
            Caja_Chica oCaja = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oCaja = ctx.Caja_Chica.Where(a => a.id == id).FirstOrDefault();
            }
            return oCaja;
        }

        public void Save(Caja_Chica caja)
        {
            Caja_Chica cajaexist = GetCajaChicaByID(caja.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (cajaexist == null)
                    {

                        cdt.Caja_Chica.Add(caja);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Caja_Chica.Add(caja);
                        cdt.Entry(caja).State = EntityState.Modified;
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
