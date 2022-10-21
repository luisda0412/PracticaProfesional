using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class ContactoController : Controller
    {

        //Index del contato
        public ActionResult Index()
        {
            return View();
        }


        //Metodo para enviar el correo
        public ActionResult EnviarCorreo()
        {
            try
            {
                //ENVIAR EL CORREO
                string EmailOrigen = "soportevycuz@gmail.com";
                string Contraseña = "ecfykdmojjjlpfcn";
 
                MailMessage oMailMessage = new MailMessage(EmailOrigen, "kennethmiranda56@@gmail.com", "Mensaje del formulario VYCUZ",
                    "<p>Im trippin,</br></br><hr />hopefully this thing works, bcause im done honestly</p>");
                //oMailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(XML, "ejemplo2.xml"));
                oMailMessage.IsBodyHtml = true;

                SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;
                oSmtpClient.Port = 587;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

                oSmtpClient.Send(oMailMessage);


                oSmtpClient.Dispose();
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Mensaje enviado con éxito", "Trataremos responderte los más pronto posible :)", SweetAlertMessageType.success);
            }
            catch(Exception ex)
            {
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;      
                return RedirectToAction("Default", "Error");
            }          
            return RedirectToAction("Index");
        }

    }
}