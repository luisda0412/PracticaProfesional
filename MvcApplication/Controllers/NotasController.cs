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
using System.Data.Entity;
using System.Globalization;
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
        public ActionResult FrameLiquidar()
        {
            IEnumerable<NotasDeCreditoYDebito> lista = null;
            try
            {
                IServiceNotas _ServiceNot = new ServiceNotas();
                lista = _ServiceNot.GetNota();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }

            return View(lista);
        }

        public ActionResult FrameNotas()
        {
            if (TempData["mensaje"] != null)
                ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            IEnumerable<Facturas> lista = null;

            return View(lista);

        }
        public ActionResult buscarFacturaxID(string filtro)
        {
            IEnumerable<Facturas> lista;
            IServiceFactura _ServiceFactura = new ServiceFactura();

            if (string.IsNullOrEmpty(filtro) || filtro == "0")
            {
                filtro = "0";
                lista = _ServiceFactura.GetListaFacturaID(Convert.ToInt32(filtro));
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Error en la búsqueda", "Digíte un número válido", SweetAlertMessageType.error);
                // Redirigir a la vista principal en caso de error
                ViewBag.ReloadPage = true;
            }
            else
            {
                lista = _ServiceFactura.GetListaFacturaID(Convert.ToInt32(filtro));
            }

            // Retorna un Partial View
            return PartialView("_PartialViewFactura", lista);
        }

        public static double saldoActual { get; set; }

        public static string email { get; set; }
        public static string nuevoMonto { get; set; }
        public static string motivo { get; set; }
        public ActionResult obtenerDatosFormNotaCredito()
        {
            IServiceNotas _ServiceNota = new ServiceNotas();

            email = Request.Form["email"];
            nuevoMonto = Request.Form["monto"];
            motivo = Request.Form["motivo"];

       

            IServiceFactura serviceFactura = new ServiceFactura();
            NotasDeCreditoYDebito nota = new NotasDeCreditoYDebito();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(TempData["idFacturaCredito"]));


            nota.idFactura = factura.id;
            nota.tipoNota = false;
            nota.estado = false;
            nota.nombreCliente = factura.Venta.nombre_cliente;
            nota.motivo = motivo;
            nota.monto = Convert.ToDouble(nuevoMonto);
            nota.fecha = DateTime.Now;

            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la Nota--------------------------------------------------------------------------------------
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

                    // Definir los estilos a utilizar
                    Style titleStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(20);

                    Style subtitleStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(14);

                    Style smallTextStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.BLACK);

                    Style tableHeaderStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12);

                    Style tableCellStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.BLACK);

                    // Agregar el encabezado
                    Paragraph header = new Paragraph("Nota de Crédito").AddStyle(titleStyle);
                    doc.Add(header);

                    // Agregar la información de la empresa
                    Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false))
                        .SetHeight(50)
                        .SetWidth(120);
                    doc.Add(logo);

                    Paragraph header2 = new Paragraph("2ndo Piso, City Mall").AddStyle(subtitleStyle);
                    Paragraph header3 = new Paragraph("Alajuela, Costa Rica").AddStyle(subtitleStyle);
                    doc.Add(header2);
                    doc.Add(header3);

                    // Agregar la información del cliente y la fecha
                    Paragraph cliente = new Paragraph("Cliente: " + factura.Venta.nombre_cliente).AddStyle(smallTextStyle);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).AddStyle(smallTextStyle);
                    doc.Add(cliente);
                    doc.Add(fecha);

                    // Agregar la tabla con la información de la nota de crédito
                    Table table = new Table(new float[] { 1, 3, 1 })
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Número de Factura")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Motivo de la nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Monto en colones")).AddStyle(tableHeaderStyle));

                    table.AddCell(new Paragraph(factura.id.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));
                    table.AddCell(new Paragraph(motivo.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));


                    double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                    string montoCantidad = montoDouble.ToString("C2", CultureInfo.GetCultureInfo("es-CR")); // formatear como moneda en colones (CRC)
                    table.AddCell(new Paragraph(montoCantidad).SetVerticalAlignment(VerticalAlignment.TOP));

                    doc.Add(table);

                    doc.Close();

                    _ServiceNota.Save(nota);

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

            email = "";
            nuevoMonto = "";
            motivo = "";
            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Nota Creada", "La nota de crédito se ha enviado al cliente", SweetAlertMessageType.success);
            return RedirectToAction("FrameNotas");
        }



        public ActionResult CrearNotaCredito(int? id)
        {
            ServiceFactura _ServiceFact = new ServiceFactura();
            Facturas fact = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                fact = _ServiceFact.GetFacturaByID(id.Value);
                ViewBag.Factura = fact;
                return View();

            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult obtenerDatosFormNotaDebito()
        {

            IServiceNotas _ServiceNota = new ServiceNotas();
            NotasDeCreditoYDebito nota = new NotasDeCreditoYDebito();

            email = Request.Form["email"];
            nuevoMonto = Request.Form["monto"];
            motivo = Request.Form["motivo"];
            string tipoNotaDebitoValue = Request.Form["tipoNotaDebito"];

          

            IServiceFactura serviceFactura = new ServiceFactura();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(TempData["idFacturaDebito"]));
          

            nota.idFactura = factura.id;
            nota.tipoNota = true;
            nota.estado = false;
            nota.nombreCliente = factura.Venta.nombre_cliente;
            nota.motivo = motivo;
            nota.monto = Convert.ToDouble(nuevoMonto);
            nota.fecha = DateTime.Now;



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

                    // Definir los estilos a utilizar
                    Style titleStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(20);

                    Style subtitleStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(14);

                    Style smallTextStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.BLACK);

                    Style tableHeaderStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12);

                    Style tableCellStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.BLACK);

                    // Agregar el encabezado
                    Paragraph header = new Paragraph("Nota de Débito").AddStyle(titleStyle);
                    doc.Add(header);

                    // Agregar la información de la empresa
                    Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false))
                        .SetHeight(50)
                        .SetWidth(120);
                    doc.Add(logo);

                    Paragraph header2 = new Paragraph("2ndo Piso, City Mall").AddStyle(subtitleStyle);
                    Paragraph header3 = new Paragraph("Alajuela, Costa Rica").AddStyle(subtitleStyle);
                    doc.Add(header2);
                    doc.Add(header3);

                    // Agregar la información del cliente y la fecha
                    Paragraph cliente = new Paragraph("Cliente: " + factura.Venta.nombre_cliente).AddStyle(smallTextStyle);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).AddStyle(smallTextStyle);
                    doc.Add(cliente);
                    doc.Add(fecha);

                    // Agregar la tabla con la información de la nota de crédito
                    Table table = new Table(new float[] { 1, 3, 1 })
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Número de Factura")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Motivo de la nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Monto en colones")).AddStyle(tableHeaderStyle));

                    table.AddCell(new Paragraph(factura.id.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));
                    table.AddCell(new Paragraph(motivo.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));


                    double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                    string montoCantidad = montoDouble.ToString("C2", CultureInfo.GetCultureInfo("es-CR")); // formatear como moneda en colones (CRC)
                    table.AddCell(new Paragraph(montoCantidad).SetVerticalAlignment(VerticalAlignment.TOP));

                    doc.Add(table);

                    doc.Close();

                    _ServiceNota.Save(nota);


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

            email = "";
            nuevoMonto = "";
            motivo = "";
            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Nota Creada", "La nota de débito se ha enviado al cliente", SweetAlertMessageType.success);
            return RedirectToAction("FrameNotas");
        }

        public ActionResult CrearNotaDebito(int? id)
        {
            ServiceFactura _ServiceFact = new ServiceFactura();
            Facturas fact = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }
                fact = _ServiceFact.GetFacturaByID(id.Value);
                ViewBag.Factura = fact;
                return View();

            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
                return RedirectToAction("Index");
            }
        }

        //Buscar la nota por la fecha
        public ActionResult buscarNotaxFecha(DateTime filtro)
        {
            IEnumerable<NotasDeCreditoYDebito> lista = null;
            IServiceNotas _ServiceNota = new ServiceNotas();

            // Error porque viene en blanco 
            if (filtro == null)
            {
                lista = _ServiceNota.GetNota();
            }
            else
            {
                lista = _ServiceNota.GetListaNotasByFecha(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxFecha", lista);
        }

        //Metodo que cobra la de Credito, hace que salga plata de la caja chica 
        public ActionResult LiquidarNotaCredito(int? id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    IServiceNotas ServiceNota = new ServiceNotas();
                    NotasDeCreditoYDebito nota = ServiceNota.GetNotaByID((int)id);

                    //REGISTRAR LOS MONTOS EN LA CAJA CHICA----------------------------------------------------------------
                    IServiceCajaChica servicio = new ServiceCajaChica();
                    Caja_Chica ultimacaja = new Caja_Chica();
                    ultimacaja = servicio.GetCajaChicaLast();

                    Caja_Chica cajaChica = new Caja_Chica();
                    cajaChica.fecha = DateTime.Now;
                    cajaChica.entrada = Convert.ToDouble(nota.monto);
                    cajaChica.salida = cajaChica.entrada - nota.monto;

                    saldoActual = ((double)cajaChica.entrada - (double)cajaChica.salida) + (double)ultimacaja.saldo;
                    cajaChica.saldo = saldoActual;

                    IServiceCajaChica caja = new ServiceCajaChica();
                    caja.Save(cajaChica);

                    NotasDeCreditoYDebito notaCredito = cdt.NotasDeCreditoYDebito.Where(x => x.id == id).Where(x => x.tipoNota == false).FirstOrDefault();
                    notaCredito.estado = !notaCredito.estado;
                    cdt.NotasDeCreditoYDebito.Add(notaCredito);

                    cdt.Entry(notaCredito).State = EntityState.Modified;
                    cdt.SaveChanges();


                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
            return RedirectToAction("FrameLiquidar");
        }

        //Metodo que cobra la de Debito, busca los datos del Modal y mete la plata y saca vuelto si fuese el caso
        public ActionResult LiquidarNotaDebito()
        {
            string id = Request.Form["factura"];//Con este se tiene que ir a buscar a la BD la fatura correspondiente si fuese el caso
            string montopago = Request.Form["monto"];
            string tipopago = Request.Form["pago"];

            return RedirectToAction("FrameLiquidar");
        }
    }
}