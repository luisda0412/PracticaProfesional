using AplicationCore.Services;
using Infraestructure.Models;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Usa una expresión regular para verificar el formato y el dominio de la dirección de correo electrónico
                var regex = new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@gmail\.com$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool EsValorSeleccionadoValido(string valorSeleccionado)
        {
            // Lista de valores válidos del select
            string[] valoresValidos = { "1", "2", "3", "4" };

            // Verificar si el valor seleccionado está en la lista de valores válidos
            return valoresValidos.Contains(valorSeleccionado);
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
            string Value = Request.Form["tipoNota"];
            int temporal = Convert.ToInt32(TempData["idFacturaCredito"]);

         
            // Validar que el email no esté vacío y sea un correo válido
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "El correo está vacío o no es válido, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaCredito", new { id = temporal });
            }

            // Validar que el nuevo monto no esté vacío
            if (string.IsNullOrEmpty(nuevoMonto))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "El monto de la nota está vacío, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaCredito", new { id = temporal });
            }

            // Validar el tipo de nota
            if (string.IsNullOrEmpty(Value) || !EsValorSeleccionadoValido(Value))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "No ha ingresado el tipo de nota de crédito, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaCredito", new { id = temporal });
            }

            // Validar que el motivo no esté vacío
            if (string.IsNullOrEmpty(motivo))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "No ha ingresado un motivo, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaCredito", new { id = temporal });
            }

            string tipoNota = string.Empty;
            switch (Value)
            {
                case "1":
                    tipoNota = "Por devolución del producto";
                    break;
                case "2":
                    tipoNota = "Por error en el monto";
                    break;
                case "3":
                    tipoNota = "Por descuento en artículo";
                    break;
                case "4":
                    tipoNota = "Por otros motivos";
                    break;
                default:
                    tipoNota = "Sin tipo";
                    break;
            }
        
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

            IServiceEmpresa servicoEmpre = new ServiceEmpresa();
            Empresa vycuz = servicoEmpre.GetEmpresaByID(1);
         
            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la Nota--------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------
            MemoryStream ms = new MemoryStream();
            FileContentResult FileFact = null;

            try
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc, PageSize.A4);

                // Definir los estilos a utilizar
                Style titleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER);

                Style subtitleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER);

                Style smallTextStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.BLACK);

                Style tableHeaderStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

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
                    .SetWidth(120)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                doc.Add(logo);

                // Separador
                LineSeparator separator = new LineSeparator(new SolidLine(1));
        

                Paragraph header2 = new Paragraph("Lugar: " + vycuz.direccion).AddStyle(subtitleStyle);
                Paragraph header3 = new Paragraph("Provincia: Alajuela, Costa Rica").AddStyle(subtitleStyle);
                Paragraph header4 = new Paragraph("Número Telefónico: " + vycuz.telefono).AddStyle(subtitleStyle);
                doc.Add(header2);
                doc.Add(header3);
                doc.Add(header4);

               
                doc.Add(separator);

                // Agregar la información del cliente y la fecha
                Paragraph cliente = new Paragraph("Cliente: " + factura.Venta.nombre_cliente).AddStyle(smallTextStyle);
                Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).AddStyle(smallTextStyle);
                doc.Add(cliente);
                doc.Add(fecha);

               
                doc.Add(separator);

                // Agregar la tabla con la información de la nota de crédito
                Table table = new Table(new float[] { 1, 2, 2, 1 })
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(20)
                    .AddHeaderCell(new Cell().Add(new Paragraph("Número de Factura")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Tipo de Nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Motivo de la nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Monto en Colones")).AddStyle(tableHeaderStyle));

                table.AddCell(new Paragraph(factura.id.ToString()).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Paragraph(tipoNota).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Paragraph(motivo.ToString()).AddStyle(tableCellStyle));
                double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                string montoCantidad = String.Format("{0:N2}", montoDouble); ; // formatear como moneda en colones (CRC)
                table.AddCell(new Paragraph("¢" + montoCantidad).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.RIGHT));
            

                 doc.Add(table);

                Paragraph mensaje = new Paragraph("Le informamos que se ha creado su nota de crédito correspondiente a la factura número " +
                       factura.id + ", por un monto total de ¢" + montoCantidad + "." +
                      " Agradecemos su preferencia y quedamos a su disposición para cualquier consulta adicional. " +
                      "Atentamente, VYCUZ").AddStyle(smallTextStyle);
                doc.Add(mensaje);

                doc.Close();

                 _ServiceNota.Save(nota);

                 FileFact = File(ms.ToArray(), "application/pdf", "Nota de Crédito.pdf");

                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                }
            


            //-------------------------------------------------------------------------
            //ENVIAR EL CORREO---------------------------------------------------------
            //-------------------------------------------------------------------------
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
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
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
            string Value = Request.Form["tipoNotaDebito"];

            int temporal = Convert.ToInt32(TempData["idFacturaDebito"]);

            // Validar que el email no esté vacío y sea un correo válido
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "El correo está vacío o no es válido, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaDebito", new { id = temporal });
            }

            // Validar que el nuevo monto no esté vacío
            if (string.IsNullOrEmpty(nuevoMonto))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "El monto de la nota está vacío, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaDebito", new { id = temporal });
            }

            //Validar que el tipo de nota no este vaci
            if (string.IsNullOrEmpty(Value) || !EsValorSeleccionadoValido(Value))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "No ha ingresado el tipo de nota de débito, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaDebito", new { id = temporal });
            }

            // Validar que el motivo no esté vacío
            if (string.IsNullOrEmpty(motivo))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Consulta no realizada", "No ha ingresado un motivo, por favor verifíque", SweetAlertMessageType.error);
                return RedirectToAction("CrearNotaDebito", new { id = temporal });
            }



            string tipoNota = string.Empty;
            switch (Value)
            {
                case "1":
                    tipoNota = "Por aumento de precio del artículo/servicio";
                    break;
                case "2":
                    tipoNota = "Por cargos no incluidos en la factura original";
                    break;
                case "3":
                    tipoNota = "Por impuestos adicionales";
                    break;
                case "4":
                    tipoNota = "Por otros motivos";
                    break;
                default:
                    tipoNota = "Sin tipo";
                    break;
            }


            IServiceFactura serviceFactura = new ServiceFactura();

            Facturas factura = serviceFactura.GetFacturaByID(Convert.ToInt32(TempData["idFacturaDebito"]));
          

            nota.idFactura = factura.id;
            nota.tipoNota = true;
            nota.estado = false;
            nota.nombreCliente = factura.Venta.nombre_cliente;
            nota.motivo = motivo;
            nota.monto = Convert.ToDouble(nuevoMonto);
            nota.fecha = DateTime.Now;

            IServiceEmpresa servicoEmpre = new ServiceEmpresa();
            Empresa vycuz = servicoEmpre.GetEmpresaByID(1);

            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la factura--------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------
            MemoryStream ms = new MemoryStream();
            FileContentResult FileFact = null;
         
                try
                {

                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdfDoc = new PdfDocument(writer);
                    Document doc = new Document(pdfDoc, PageSize.A4, false);

                // Definir los estilos a utilizar
                Style titleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER);

                Style subtitleStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER);

                Style smallTextStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.BLACK);

                Style tableHeaderStyle = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetBackgroundColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

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
                        .SetWidth(120)
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    doc.Add(logo);

                    // Separador
                    LineSeparator separator = new LineSeparator(new SolidLine(1));
    
                
                    Paragraph header2 = new Paragraph("Lugar: " + vycuz.direccion).AddStyle(subtitleStyle);
                    Paragraph header3 = new Paragraph("Provincia: Alajuela, Costa Rica").AddStyle(subtitleStyle);
                    Paragraph header4 = new Paragraph("Número Telefónico: " + vycuz.telefono).AddStyle(subtitleStyle);
                    doc.Add(header2);
                    doc.Add(header3);
                    doc.Add(header4);

             
                    doc.Add(separator);

                    // Agregar la información del cliente y la fecha
                    Paragraph cliente = new Paragraph("Cliente: " + factura.Venta.nombre_cliente).AddStyle(smallTextStyle);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).AddStyle(smallTextStyle);
                    doc.Add(cliente);
                    doc.Add(fecha);

               
                    doc.Add(separator);


                    // Agregar la tabla con la información de la nota de crédito
                    Table table = new Table(new float[] { 1, 2, 2, 1 })
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(20)
                    .AddHeaderCell(new Cell().Add(new Paragraph("Número de Factura")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Tipo de Nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Motivo de la nota")).AddStyle(tableHeaderStyle))
                    .AddHeaderCell(new Cell().Add(new Paragraph("Monto en Colones")).AddStyle(tableHeaderStyle));

                    table.AddCell(new Paragraph(factura.id.ToString()).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Paragraph(tipoNota).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Paragraph(motivo.ToString()).AddStyle(tableCellStyle));
                    double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                    string montoCantidad = String.Format("{0:N2}", montoDouble); ; // formatear como moneda en colones (CRC)
                    table.AddCell(new Paragraph("¢" + montoCantidad).AddStyle(tableCellStyle).SetTextAlignment(TextAlignment.RIGHT));

                    doc.Add(table);

                    Paragraph mensaje = new Paragraph("Le informamos que se ha creado su nota de débito correspondiente a la factura número " +
                         factura.id + ", por un monto total de ¢" + montoCantidad + "."+
                        " Agradecemos su preferencia y quedamos a su disposición para cualquier consulta adicional. " +
                        "Atentamente, VYCUZ").AddStyle(smallTextStyle);
                    doc.Add(mensaje);

                doc.Close();

                    _ServiceNota.Save(nota);


                    FileFact = File(ms.ToArray(), "application/pdf", "Nota de Débito.pdf");

                }
                catch (Exception ex)
                {
                    TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                }
            


            //-------------------------------------------------------------------------
            //ENVIAR EL CORREO---------------------------------------------------------
            //-------------------------------------------------------------------------
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
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
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
        public ActionResult buscarNotaxNombre(string filtro)
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
                lista = _ServiceNota.GetListaNotasByNombre(filtro);
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


                    //Mostrar Mensaje de abrir la caja si esta cerrada
                    IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
                    Arqueos_Caja cajita = new Arqueos_Caja();
                    cajita = _ServiceCaja.GetArqueoLast();

                    if (cajita.estado == false)//Valida si la caja chica esta cerrada
                    {
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                            ("Caja Cerrada", "No se pueden registrar movimientos de dinero con la caja chica cerrada, por favor verifíque!", SweetAlertMessageType.warning);
                        return RedirectToAction("FrameLiquidar");
                    }

                    string msj = "Se ha liquidado la nota Crédito número: " + nota.id + ", por un monto de: ₡";
                    Infraestructure.Util.Log.Info(msj + String.Format("{0:N2}", nota.monto));

                    //REGISTRAR LOS MONTOS EN LA CAJA CHICA----------------------------------------------------------------
                    IServiceCajaChica servicio = new ServiceCajaChica();
                    Caja_Chica ultimacaja = new Caja_Chica();
                    ultimacaja = servicio.GetCajaChicaLast();

                    Caja_Chica cajaChica = new Caja_Chica();
                    cajaChica.fecha = DateTime.Now;
                    cajaChica.entrada = 0;
                    cajaChica.salida = nota.monto;

                    saldoActual = ((double)cajaChica.salida - (double)cajaChica.entrada) + (double)ultimacaja.saldo;
                    cajaChica.saldo = saldoActual;

                    IServiceCajaChica caja = new ServiceCajaChica();
                    caja.Save(cajaChica);

                    NotasDeCreditoYDebito notaCredito = cdt.NotasDeCreditoYDebito.Where(x => x.id == id).Where(x => x.tipoNota == false).FirstOrDefault();
                    notaCredito.estado = !notaCredito.estado;
                    cdt.NotasDeCreditoYDebito.Add(notaCredito);

                    cdt.Entry(notaCredito).State = EntityState.Modified;
                    cdt.SaveChanges();
                   
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Nota Liquidada", "Por favor otorgar los ₡" + cajaChica.salida +" "+ "del monto de la nota de Crédito al cliente.", SweetAlertMessageType.success);


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
        public ActionResult LiquidarNotaDebito(int notaId, decimal monto, string tipoPago)
        {
            IServiceNotas ServiceNota = new ServiceNotas();
            NotasDeCreditoYDebito nota = ServiceNota.GetNotaByID(notaId);

            //Mostrar Mensaje de abrir la caja si esta cerrada
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja cajita = new Arqueos_Caja();
            cajita = _ServiceCaja.GetArqueoLast();

            if (Convert.ToDouble(monto) < nota.monto)
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                          ("Monto Inválido", "El monto ingresado es menor al registrado en la nota de débito", SweetAlertMessageType.warning);
                return new EmptyResult();
            }

            if (cajita.estado == false)//Valida si la caja chica esta cerrada
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                    ("Caja Cerrada", "No se pueden registrar ingresos de dinero con la caja chica cerrada, por favor verifíque!", SweetAlertMessageType.warning);

                return new EmptyResult();
            }
            ServiceNota.Desabilitar(notaId);

            string msj = "Se ha liquidado la nota Débito número: " + nota.id + ", por un monto de: ₡";
            Infraestructure.Util.Log.Info(msj + String.Format("{0:N2}", nota.monto));

            //SI TODO ESTA BIEN, REGISTRAR LOS MONTOS EN LA CAJA CHICA----------------------------------------------------------------
            Caja_Chica ultimacaja = new Caja_Chica();
            ultimacaja = _ServiceCaja.GetCajaChicaLast();

            Caja_Chica cajaChica = new Caja_Chica();
            cajaChica.fecha = DateTime.Now;
            cajaChica.entrada = Convert.ToDouble(monto);
            cajaChica.salida = cajaChica.entrada - nota.monto;
           
            saldoActual = ((double)cajaChica.entrada - (double)cajaChica.salida) + (double)ultimacaja.saldo;
            cajaChica.saldo = saldoActual;
            string mensaje2 = " ";
            if (cajaChica.salida != 0.0)
            {
                 mensaje2 = " El cambio para el cliente es de" + "   ₡" + cajaChica.salida;
            }

            IServiceCajaChica caja = new ServiceCajaChica();
            caja.Save(cajaChica);

            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Pago registrado", "Se ha registrado el pago de la nota de débito efectivamente." + mensaje2, SweetAlertMessageType.success);
            return new EmptyResult();
        }
    }
}