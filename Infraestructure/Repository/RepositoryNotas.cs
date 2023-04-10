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
    public class RepositoryNotas : IRepositoryNotas
    {
        public void Desabilitar(int id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    NotasDeCreditoYDebito  repo = cdt.NotasDeCreditoYDebito.Find(id);
                    repo.estado = true;
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

        public IEnumerable<NotasDeCreditoYDebito> GetListaNotasFecha(DateTime date)
        {
            IEnumerable<NotasDeCreditoYDebito> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.NotasDeCreditoYDebito
                           .ToList()
                           .FindAll(l => l.fecha.Value.Date.Equals(date.Date))
                           .OrderByDescending(l => l.fecha);

            }
            return lista;
        }

        public IEnumerable<NotasDeCreditoYDebito> GetNota()
        {
            IEnumerable<NotasDeCreditoYDebito> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.NotasDeCreditoYDebito.Where(x => x.estado == false).Include(x => x.Facturas).OrderByDescending(x => x.fecha).ToList<NotasDeCreditoYDebito>();
            }
            return lista;
        }

        public NotasDeCreditoYDebito GetNotaByID(int id)
        {
            NotasDeCreditoYDebito nota = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    nota = ctx.NotasDeCreditoYDebito.Include(f => f.Facturas).FirstOrDefault(f => f.id == id);
                }

                return nota;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = "";
                Infraestructure.Util.Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                throw;
            }
        }

        public NotasDeCreditoYDebito Save(NotasDeCreditoYDebito nota)
        {
            int retorno = 0;
            NotasDeCreditoYDebito oNotas = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oNotas = GetNotaByID((int)nota.id);

                if (oNotas == null)
                {
                    ctx.NotasDeCreditoYDebito.Add(nota);
                    string tipoNota = (bool)nota.tipoNota ? "Débito" : "Crédito";
                    string msj = $"Se ha creado una nota {tipoNota} por un monto de ₡{nota.monto:N2}, a nombre de: {nota.nombreCliente}.";
                    Infraestructure.Util.Log.Info(msj);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.NotasDeCreditoYDebito.Add(nota);
                    ctx.Entry(nota).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oNotas = GetNotaByID((int)nota.id);

            return oNotas;
        }
    }
}
