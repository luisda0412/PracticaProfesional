using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repository
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        public Usuario logIn(string correo, string clave)
        {
            Usuario user = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                user = ctx.Usuario.Where(u => u.correo_electronico == correo && u.clave == clave && u.estado == true).FirstOrDefault<Usuario>();
            }
            return user;
        }
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

        public IEnumerable<Usuario> GetUsuario()
        {
            IEnumerable<Usuario> lista = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                lista = ctx.Usuario.Include(x => x.Rol).ToList<Usuario>();
            }
            return lista;
        }

        public Usuario GetUsuarioByID(int id)
        {
            Usuario oUsuario = null;
            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oUsuario = ctx.Usuario.Where(p => p.id == id).
                    Include(x => x.Rol).FirstOrDefault();
            }
            return oUsuario;
        }

        public Usuario Save(Usuario user)
        {
            int retorno = 0;
            Usuario oUser = null;

            using (MyContext ctx = new MyContext())
            {
                ctx.Configuration.LazyLoadingEnabled = false;
                oUser = GetUsuarioByID((int)user.id);

                if (oUser == null)
                {
                    user.estado = true;
                    ctx.Usuario.Add(user);
                    retorno = ctx.SaveChanges();
                }
                else
                {
                    ctx.Usuario.Add(user);
                    ctx.Entry(user).State = EntityState.Modified;
                    retorno = ctx.SaveChanges();
                }
            }

            if (retorno >= 0)
                oUser = GetUsuarioByID((int)user.id);

            return oUser;
        }
    }
}

