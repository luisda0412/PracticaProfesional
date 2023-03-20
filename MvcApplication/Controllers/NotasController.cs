using AplicationCore.Services;
using Infraestructure.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MvcApplication.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Web.Security;

using Web.ViewModel;


namespace MvcApplication.Controllers
{
    public class NotasController : Controller
    {

        public ActionResult FrameNotas()
        {
            IEnumerable<Facturas> lista = null;
            return View(lista);
        }
        public ActionResult buscarFacturaxID(string filtro)
        {
            IEnumerable<Facturas> lista = null;
            IServiceFactura _ServiceFactura = new ServiceFactura();
        

            
            if (string.IsNullOrEmpty(filtro))
            {
                lista = null;
            }
            else
            {
                lista = _ServiceFactura.GetListaFacturaID(Convert.ToInt32(filtro));
               
            }


            // Retorna un Partial View
            return PartialView("_PartialViewFactura", lista);
        }

        public static string idFactura { get; set; }
        public static string email { get; set; }
        public static string nuevoMonto { get; set; }
        public static string motivo { get; set; }
        public ActionResult obtenerDatosFormNotaCredito()
        {
            idFactura = Request.Form["id"];
            email = Request.Form["email"];
            nuevoMonto = Request.Form["monto"];
            motivo = Request.Form["motivo"];
            
            Venta venta = new Venta();

            IServiceFactura serviceFactura = new ServiceFactura();
            IServiceVenta serviceVenta = new ServiceVenta();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(idFactura));
            venta = serviceVenta.GetVentaByID((long)factura.venta_id);





            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la factura--------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------
            MemoryStream ms = new MemoryStream();
            FileContentResult FileFact = null;
            if (email != null)
            {
                try
                {

                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document doc = new Document(pdfDoc, PageSize.A4, false);

                    Paragraph header = new Paragraph("Nota de Crédito").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(20);

                    //Imagen de la empresa
                    Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                    logo = logo.SetHeight(50).SetWidth(120);

                    Paragraph header2 = new Paragraph("2ndo Piso, City Mall").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                    Paragraph header3 = new Paragraph("Alajuela, Costa Rica").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14).SetFontColor(ColorConstants.BLACK);
                    Paragraph header4 = new Paragraph("Cliente " + venta.nombre_cliente).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK);
                    Paragraph header5 = new Paragraph("-------------------------").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10).SetFontColor(ColorConstants.BLACK);

                    doc.Add(header);
                    doc.Add(fecha);
                    doc.Add(logo);
                    doc.Add(header2);
                    doc.Add(header3);
                    doc.Add(header4);

                    // Crear tabla con 3 columnas 
                    Table table = new Table(3, true);

                    table.AddHeaderCell("ID de la factura");
                    table.AddHeaderCell("Motivo de la nota de crédito");
                    table.AddHeaderCell("Monto");

                    
                    table.AddCell(new Paragraph(factura.id.ToString()));
                    IServiceArticulo servicio = new ServiceArticulo();
                    table.AddCell(new Paragraph(motivo.ToString()));

                    string montoCantidad = (String.Format("{0:N2}", nuevoMonto));
                    table.AddCell(new Paragraph("¢" + montoCantidad));

                    doc.Add(table);


                    doc.Add(header5);


                    doc.Add(header5);

                    doc.Close();


                    FileFact = File(ms.ToArray(), "application/pdf", "NotaDeCredito.pdf");

                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                }
            }


            //-------------------------------------------------------------------------
            //ENVIAR EL CORREO---------------------------------------------------------
            //-------------------------------------------------------------------------

            if (email != null)
            {
                string urlDomain = "https://localhost:3000/";
                string EmailOrigen = "soportevycuz@gmail.com";
                string Contraseña = "ecfykdmojjjlpfcn";
                string url = urlDomain + "/Usuario/Recuperacion/?token=";
                MailMessage oMailMessage = new MailMessage(EmailOrigen, email, "Nota de Crédito",
                    "<p>Estimado usuario,</br></br><hr/>Se ha realizado una nota de crédito por parte de VYCUZ.</p>");

                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                var attachmentFile = FileFact;
                var attachmentStream = new MemoryStream((attachmentFile as FileContentResult).FileContents);
                var attachmentTitle = (attachmentFile as FileContentResult).FileDownloadName;

                oMailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentTitle, contentType.ToString()));

                oMailMessage.IsBodyHtml = true;

                SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;
                oSmtpClient.Port = 587;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

                oSmtpClient.Send(oMailMessage);


                oSmtpClient.Dispose();
            }

            idFactura = "";
            email = "";
            nuevoMonto = "";
            motivo = "";

            return View("CrearNotaDeCredito");
        }

        public ActionResult CrearNotaCredito()
        {
            return View();
        }

        public ActionResult obtenerDatosFormNotaDebito()
        {
            idFactura = Request.Form["id"];
            email = Request.Form["email"];
            nuevoMonto = Request.Form["monto"];
            motivo = Request.Form["motivo"];

            Venta venta = new Venta();

            IServiceFactura serviceFactura = new ServiceFactura();
            IServiceVenta serviceVenta = new ServiceVenta();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(idFactura));
            venta = serviceVenta.GetVentaByID((long)factura.venta_id);





            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la factura--------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------
            MemoryStream ms = new MemoryStream();
            FileContentResult FileFact = null;
            if (email != null)
            {
                try
                {

                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document doc = new Document(pdfDoc, PageSize.A4, false);

                    Paragraph header = new Paragraph("Nota de Débito").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(20);

                    //Imagen de la empresa
                    Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                    logo = logo.SetHeight(50).SetWidth(120);

                    Paragraph header2 = new Paragraph("2ndo Piso, City Mall").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                    Paragraph header3 = new Paragraph("Alajuela, Costa Rica").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14).SetFontColor(ColorConstants.BLACK);
                    Paragraph header4 = new Paragraph("Cliente " + venta.nombre_cliente).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK);
                    Paragraph header5 = new Paragraph("-------------------------").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10).SetFontColor(ColorConstants.BLACK);

                    doc.Add(header);
                    doc.Add(fecha);
                    doc.Add(logo);
                    doc.Add(header2);
                    doc.Add(header3);
                    doc.Add(header4);

                    // Crear tabla con 3 columnas 
                    Table table = new Table(3, true);

                    table.AddHeaderCell("ID de la factura");
                    table.AddHeaderCell("Motivo de la nota de débito");
                    table.AddHeaderCell("Monto");


                    table.AddCell(new Paragraph(factura.id.ToString()));
                    IServiceArticulo servicio = new ServiceArticulo();
                    table.AddCell(new Paragraph(motivo.ToString()));

                    string montoCantidad = (String.Format("{0:N2}", nuevoMonto));
                    table.AddCell(new Paragraph("¢" + montoCantidad));

                    doc.Add(table);


                    doc.Add(header5);


                    doc.Add(header5);

                    doc.Close();


                    FileFact = File(ms.ToArray(), "application/pdf", "NotaDeDébito.pdf");

                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                }
            }


            //-------------------------------------------------------------------------
            //ENVIAR EL CORREO---------------------------------------------------------
            //-------------------------------------------------------------------------

            if (email != null)
            {
                string urlDomain = "https://localhost:3000/";
                string EmailOrigen = "soportevycuz@gmail.com";
                string Contraseña = "ecfykdmojjjlpfcn";
                string url = urlDomain + "/Usuario/Recuperacion/?token=";
                MailMessage oMailMessage = new MailMessage(EmailOrigen, email, "Nota de Débito",
                    "<p>Estimado usuario,</br></br><hr/>Se ha realizado una nota de débito por parte de VYCUZ.</p>");

                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                var attachmentFile = FileFact;
                var attachmentStream = new MemoryStream((attachmentFile as FileContentResult).FileContents);
                var attachmentTitle = (attachmentFile as FileContentResult).FileDownloadName;

                oMailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentTitle, contentType.ToString()));

                oMailMessage.IsBodyHtml = true;

                SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;
                oSmtpClient.Port = 587;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

                oSmtpClient.Send(oMailMessage);


                oSmtpClient.Dispose();
            }

            idFactura = "";
            email = "";
            nuevoMonto = "";
            motivo = "";
            return View("CrearNotaDeCredito");
        }

        public ActionResult CrearNotaDebito()
        {
            return View();
        }
    }
}