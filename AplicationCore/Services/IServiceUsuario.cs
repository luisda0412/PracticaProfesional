﻿using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicationCore.Services
{
    public interface IServiceUsuario
    {
        Usuario LogIn(String correo, String clave);
        IEnumerable<Usuario> GetUsuario();
        Usuario GetUsuarioByID(int id);
        Usuario Save(Usuario user);
        IEnumerable<Rol> GetRol();
        Usuario VerificarUsuario(string email);
        Usuario GetUsuarioByToken(string token);
    }
}