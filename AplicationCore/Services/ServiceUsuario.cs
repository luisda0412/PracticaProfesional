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

        public Usuario GetUsuarioByToken(string token)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.GetUsuarioByToken(token);
        }

        public Usuario LogIn(string correo, string clave)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            //string crytpPasswd = Cryptography.EncrypthAES(clave);
            return repository.logIn(correo, clave);
        }

        public Usuario Save(Usuario user)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            string crytpPasswd = Cryptography.EncrypthAES(user.clave);
            user.clave = crytpPasswd;
            return repository.Save(user);
        }

        public Usuario VerificarUsuario(string email)
        {
            IRepositoryUsuario repository = new RepositoryUsuario();
            return repository.VerificarUsuario(email);
        }
    }
}
