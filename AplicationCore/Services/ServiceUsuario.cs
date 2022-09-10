using ApplicationCore.Utils;
using Infraestructure.Models;
using Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public class ServiceUsuario : IServiceUsuario
    {
        public IEnumerable<Rol> GetRol()
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetRol();
        }

        public IEnumerable<Usuario> GetUsuario()
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetUsuario();
        }

        public Usuario GetUsuarioByID(int id)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetUsuarioByID(id);
        }

        public Usuario Save(Usuario user)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            string crytpPasswd = Cryptography.EncrypthAES(user.clave);
            user.clave = crytpPasswd;
            return repository.Save(user);
        }
    }
}
