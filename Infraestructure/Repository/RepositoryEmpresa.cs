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
    public class RepositoryEmpresa : IRepositoryEmpresa
    {
        public IEnumerable<Empresa> GetEmpresa()
        {
            IEnumerable<Empresa> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Empresa.ToList<Empresa>();
            }
            return lista;
        }

        public void Save(Empresa empresa)
        {
            Empresa empExiste = GetEmpresaByID(empresa.id);

            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    if (empExiste == null)
                    {
                    
                        cdt.Empresa.Add(empresa);
                        cdt.SaveChanges();
                    }
                    else
                    {
                        cdt.Empresa.Add(empresa);
                        cdt.Entry(empresa).State = EntityState.Modified;
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

        public Empresa GetEmpresaByID(int id)
        {
            Empresa emp = null;
            try
            {

                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    emp = ctx.Empresa.Find(id);
                }

                return emp;
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
    }
}
