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
    public class RepositoryArticulo : IRepositoryArticulo
    {
        IRepositoryProveedor repoP = new RepositoryProveedor();

        public void actualizarCantidad(int id, int cantidad)
        {
            using (MyContext cdt = new MyContext())
            {
                try
                {
                    Articulo oldArti = GetArticuloByID(id);
                    oldArti.Categoria = null;
                    oldArti.Proveedor = null;

                    oldArti.stock -= cantidad;

                    cdt.Articulo.Add(oldArti);
                    cdt.Entry(oldArti).State = EntityState.Modified;
                    cdt.SaveChanges();
                }
                catch (Exception ex)
                {
                    string mensaje = "";
                    Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;

                }
            }
        }

        public void Eliminar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;
                try
                {

                    //Articulo art = cdt.Articulo.Where(a => a.id == id).Include(x => x.Proveedor).Include(x => x.Categoria).FirstOrDefault();
                    Articulo art = cdt.Articulo.Where(a => a.id == id).Include("Proveedor").Include("Categoria").FirstOrDefault();
                    art.Proveedor.Clear();
                    cdt.Articulo.Remove(art);                 
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

        public IEnumerable<Articulo> GetArticulo()
        {
            try
            {
                IEnumerable<Articulo> lista = null;
                using (MyContext ctx= new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    lista = ctx.Articulo.Include(x=> x.Categoria).ToList<Articulo>();
                }
                return lista;
            }
            catch (DbUpdateException dbEx)
            {
                string mensaje = "";
                Log.Error(dbEx, System.Reflection.MethodBase.GetCurrentMethod(), ref(mensaje));
                throw new Exception(mensaje);
            }
            catch (Exception e)
            {
                string mensaje = "";
                Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref (mensaje));
                throw new Exception(mensaje);
            }
        }

        public Articulo GetArticuloByID(int id)
        {
            Articulo oArticulo = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oArticulo = ctx.Articulo.Where(a => a.id == id).Include(x => x.Proveedor).Include(x => x.Categoria).FirstOrDefault();
            }
            return oArticulo;
        }

        public IEnumerable<Articulo> GetArticuloByProveedor(long id)
        {
            IEnumerable<Articulo> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Articulo.Include(p => p.Proveedor).
                Where(p => p.Proveedor.Any(o => o.id == id))
                .ToList();
            }
            return lista;
        }

        public IEnumerable<Articulo> GetProductoByNombre(string nombre)
        {
            IEnumerable<Articulo> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Articulo.Include(x => x.Categoria).ToList().
                    FindAll(l => l.nombre.ToLower().Contains(nombre.ToLower()));
            }
            return lista;
        }

        public void Save(Articulo articulo, string[] proveedor)
        {
            
            Articulo articuloExist = GetArticuloByID(articulo.id);
            Proveedor oPoveedor;

                using (MyContext cdt = new MyContext())
                {
                    cdt.Configuration.LazyLoadingEnabled = false;

                    try
                    {
                        if (articuloExist == null)
                        {

                            oPoveedor = repoP.GetProveedorByID(int.Parse(proveedor[0]));
                            cdt.Proveedor.Attach(oPoveedor);
                            articulo.Proveedor.Add(oPoveedor);
                            articulo.estado = true;
                            cdt.Articulo.Add(articulo);
                            cdt.SaveChanges();
                        }
                        else
                        {
                            cdt.Articulo.Add(articulo);
                            cdt.Entry(articulo).State = EntityState.Modified;

                            var proveedoresLista = new HashSet<string>(proveedor);
                            cdt.Entry(articulo).Collection(p => p.Proveedor).Load();
                            var nuevoProveedorLista = cdt.Proveedor.Where(x => proveedoresLista.Contains(x.id.ToString())).ToList();
                            articulo.Proveedor = nuevoProveedorLista;
                            cdt.Entry(articulo).State = EntityState.Modified;
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
