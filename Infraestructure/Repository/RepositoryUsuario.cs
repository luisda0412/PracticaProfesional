using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
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

        public Usuario VerificarUsuario(string email)
        {
            try
            {
                Usuario oUsuario = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    oUsuario = ctx.Usuario.Where(p => p.correo_electronico == email).Include("Rol").FirstOrDefault();
                }
                if (oUsuario != null)
                {
                    oUsuario.tokenRecuperacion = GetSha256(Guid.NewGuid().ToString());
                    SendEmail(email, oUsuario.tokenRecuperacion);

                    return oUsuario;
                }
                else
                {
                    return null;
                }
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

        public Usuario GetUsuarioByToken(string token)
        {
            try
            {
                Usuario oUsuario = null;
                using (MyContext ctx = new MyContext())
                {
                    ctx.Configuration.LazyLoadingEnabled = false;
                    oUsuario = ctx.Usuario.Where(p => p.tokenRecuperacion == token)
                        .Include("Rol").FirstOrDefault<Usuario>();
                }
                return oUsuario;
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
        private string GetSha256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        private void SendEmail(string EmailDestino, string token)
        {
            string urlDomain = "https://localhost:3000/";
            string EmailOrigen = "soportevycuz@gmail.com";
            string Contraseña = "ecfykdmojjjlpfcn";
            string url = urlDomain + "Usuario/Recuperacion/?token=" + token;
            MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperación de Contraseña",
                "<p>Estimado usuario,</br></br><hr />Ha iniciado el proceso para la recuperación de contraseña del sistema VYCUZ, a continuación se le presentará el link para que proceda a realizar el cambio de su respectiva contraseña.</p><br>" +
                "<a href='" + url + "'>Click para recuperar la contraseña</a></br>" +
                "<p>De no haber sido usted quién inició el proceso de recuperación, por favor comuníquelo a su administrador lo más pronto posible.</p>");

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
            oSmtpClient.EnableSsl = true;
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Port = 587;
            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

            oSmtpClient.Send(oMailMessage);

            oSmtpClient.Dispose();
        }
    }
}

