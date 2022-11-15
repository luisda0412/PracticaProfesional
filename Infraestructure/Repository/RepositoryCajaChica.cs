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
    public class RepositoryCajaChica : IRepositoryCajaChica
    {
        IRepositoryUsuario repoU = new RepositoryUsuario();
        public IEnumerable<Caja_Chica> GetCajaByFecha(DateTime fechaparam)
        {
            IEnumerable<Caja_Chica> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Caja_Chica.ToList().FindAll(l => l.fecha.Value.Date.Equals(fechaparam.Date));
                    
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
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
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
                    lista = ctx.Caja_Chica.OrderByDescending(x => x.fecha).ToList<Caja_Chica>();
                }
                return lista;
            }
            catch (Exception e)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Caja_Chica GetCajaChicaLast()
        {
            Caja_Chica oCaja = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oCaja = ctx.Caja_Chica.OrderByDescending(x => x.fecha).First();
            }
            return oCaja;
        }

        public void Save(Caja_Chica caja)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {         
                    
                    cdt.Caja_Chica.Add(caja);
                    Infraestructure.Util.Log.Info("Acción de Salvar Caja, entrada de efectivo: " + caja.entrada +"salida de efectivo: "+caja.salida);
                    cdt.SaveChanges();
                }
                catch (Exception e)
                {
                    string mensaje = "Error" + e.Message;
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }

        public IEnumerable<Arqueos_Caja> GetArqueos()
        {
            try
            {
                IEnumerable<Arqueos_Caja> lista = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Arqueos_Caja.OrderByDescending(x => x.fecha).Include(x => x.Usuario).ToList<Arqueos_Caja>();
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

        public Arqueos_Caja GetArqueoLast()
        {
            Arqueos_Caja oCaja = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oCaja = ctx.Arqueos_Caja.OrderByDescending(x => x.fecha).First();
            }
            return oCaja;
        }

        public void SaveArqueo(Arqueos_Caja caja)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                Caja_Chica chica = GetCajaChicaLast();
                Arqueos_Caja ultima = GetArqueoLast();
                string state;
                try
                {
                    if(ultima.estado == true)
                    {
                        caja.estado = false;
                        state = "Cerrada";
                    }
                    else
                    {
                        caja.estado = true;
                        state = "Abierta";
                    }
                    caja.saldo = chica.saldo;
                    cdt.Arqueos_Caja.Add(caja);

                    Infraestructure.Util.Log.Info("Arqueo de caja, estado: " + state + " Saldo: ₡" + caja.saldo);
                    cdt.SaveChanges();
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
