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
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;

using Web.ViewModel;

namespace MvcApplication.Controllers
{
    public class CotizacionController : Controller
    {
        // GET: Cotizacion
        public ActionResult Index()
        {
            //IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                ViewBag.listaArticulos = _ServiceArticulo.GetArticulo();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();

            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            ViewBag.DetalleIngreso = Cotizacion.Instancia.Items;
            return View();
        }

        public ActionResult Cotizar(int? id)
        {

            int cantidadCompra = Cotizacion.Instancia.Items.Count();
            ViewBag.NotificationMessage = Cotizacion.Instancia.AgregarItem((int)id);
            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Cotización", "Artículo agregado a la orden", SweetAlertMessageType.success);
            return RedirectToAction("Index");

        }

        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleOrden = Cotizacion.Instancia.Items;
            TempData["NotiCarrito"] = Cotizacion.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("_PartialViewDetalle", Cotizacion.Instancia.Items);

        }

        //POR AQUI PUEDE ANDAR EL ERROR DE QUE SE ESCONDE EL CARRITO 
        public ActionResult actualizarOrdenCantidad()
        {
            if (TempData.ContainsKey("NotiCarrito"))
            {
                ViewBag.NotiCarrito = TempData["NotiCarrito"];
            }
            int cantidadLibros = Carrito.Instancia.Items.Count();
            return PartialView("_PartialViewDetalle");

        }

        public ActionResult eliminarProducto(long? id)
        {
            try
            {
                TempData["mensaje"] = Cotizacion.Instancia.EliminarItem((long)id);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return RedirectToAction("Index");
        }

        public ActionResult buscarArticuloxNombre(string filtro)
        {
            IEnumerable<Articulo> lista = null;
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceArticulo.GetArticulo();
            }
            else
            {
                lista = _ServiceArticulo.GetArticuloByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxNombre", lista);
        }

        public JsonResult ActualizarCheckBoxCotizacion(bool isChecked)
        {
            //Actualiza la variable de sesión con el estado actual del CheckBox
            Session["Cotizar"] = isChecked;

            //Devuelve el nuevo estado del CheckBox al cliente
            return Json(isChecked, JsonRequestBehavior.AllowGet);
        }

        public static string emailCotizacion { get; set; }

        public ActionResult EnviarCorreo(Venta venta)
        {
            //Obtiene los datos del formulario y los guarda en variables
            emailCotizacion = Request.Form["emailCotizar"];

            //Actualiza la variable de sesión con el resultado del formulario
            Session["Cotizar"] = true;

            IServiceUsuario serviceUsuario = new ServiceUsuario();
            try
            {
                if (Cotizacion.Instancia.Items.Count() <= 0)
                {
                    return RedirectToAction("");
                }
                else
                {
                    Facturas factura = new Facturas();
                    Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));

                    //Aqui hay que validar que no vengan cosas vacias
                    if (emailCotizacion != null)
                    {
                        IServiceArticulo serviceArticulo = new ServiceArticulo();

                        Articulo articulo = new Articulo();
                        var listaLinea = Cotizacion.Instancia.Items;

                        string descuento2 = "";
                        int contador = 0;

                        foreach (var items in listaLinea)
                        {
                            Detalle_Venta linea = new Detalle_Venta();


                            contador++;

                            linea.articulo_id = (int)items.idArticulo;
                            linea.cantidad = items.cantidad;
                            venta.impuesto = (double?)Carrito.Instancia.GetImpuesto();
                            venta.usuario_id = Convert.ToInt32(TempData["idUser"]);
                            venta.estado = true;
                            linea.venta_id = venta.id;
                            linea.precio = items.precio;
                            venta.fecha = System.DateTime.Now;
                            linea.descuento = 0;
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal();
                            venta.Detalle_Venta.Add(linea);

                            descuento2 = Convert.ToString(linea.descuento);
                        }
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Cotización exitosa", "Una nueva cotización ha sido registrada", SweetAlertMessageType.success);

                        //FACTURA
                        factura.venta_id = venta.id;
                        factura.empresa_id = 1;

                        IServiceFactura serviceFactura = new ServiceFactura();


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


                            // Fuente personalizada para el número de artículo
                            PdfFont numeroArticuloFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                            // Fuente personalizada para la descripción del artículo
                            PdfFont descripcionFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                            // Fuente personalizada para la cantidad y el precio unitario
                            PdfFont cantidadPrecioFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

                            // Fuente personalizada para el subtotal, impuesto y total
                            PdfFont totalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLDOBLIQUE);

                            // Encabezado
                            Paragraph header = new Paragraph("Cotización de productos")
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                                .SetFontSize(18)
                                .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                            doc.Add(header);

                            // Logo y atendido por
                            Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false))
                                .SetHeight(50)
                                .SetWidth(120)
                                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(12)
                                .SetFontColor(ColorConstants.BLACK)
                                .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro

                            doc.Add(logo);

                            // Información de la empresa
                            Paragraph info1 = new Paragraph("Lugar: City Mall")
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                            Paragraph info2 = new Paragraph("Ubicación: Montserrat, Alajuela, Costa Rica")
                                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                            doc.Add(info1);
                            doc.Add(info2);

                            // Separador
                            LineSeparator separator = new LineSeparator(new SolidLine(1));
                            doc.Add(separator);


                            Paragraph info5 = new Paragraph("Lista de Artículos")
                              .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                              .SetFontSize(10)
                              .SetFontColor(ColorConstants.BLACK);

                            doc.Add(info5);

                            // Separador
                            doc.Add(separator);

                            // Crear tabla con 4 columnas
                            Table table = new Table(new float[] { 5, 45, 15, 35 }).UseAllAvailableWidth();



                            table.AddHeaderCell(new Cell().Add(new Paragraph("#").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("DESCRIPCIÓN").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("CANT.").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("PRECIO EN COLONES").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));


                            int x = 0;
                            double subtotal = 0;
                            foreach (var item in listaLinea)
                            {
                                x++;
                                IServiceArticulo servicioArti = new ServiceArticulo();
                                Articulo arti = servicioArti.GetArticuloByID((int)item.idArticulo);
                                table.AddCell(new Cell().Add(new Paragraph(x.ToString()).SetFont(numeroArticuloFont)).SetTextAlignment(TextAlignment.CENTER));
                                table.AddCell(new Cell().Add(new Paragraph(arti.nombre).SetFont(descripcionFont)).SetTextAlignment(TextAlignment.CENTER));
                                table.AddCell(new Cell().Add(new Paragraph(item.cantidad.ToString()).SetFont(cantidadPrecioFont)).SetTextAlignment(TextAlignment.CENTER));
                                string precio = (String.Format("{0:N2}", item.precio));
                                table.AddCell(new Cell().Add(new Paragraph("¢" + precio).SetFont(cantidadPrecioFont)))
                                    .SetTextAlignment(TextAlignment.RIGHT);
                                subtotal += (double)item.precio;
                            }
                            doc.Add(table);
                            doc.Add(new Paragraph("").SetFontSize(14));

                            doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));

                            Table table2 = new Table(new float[] { 1f, 1f }).UseAllAvailableWidth();

                            Cell cell1 = new Cell().Add(new Paragraph("Subtotal:"));
                            cell1.SetBorder(Border.NO_BORDER);
                            cell1.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                            table2.AddCell(cell1);

                            Cell cell2 = new Cell().Add(new Paragraph("¢" + Cotizacion.Instancia.tirarSubtotal()));
                            cell2.SetBorder(Border.NO_BORDER);
                            cell2.SetTextAlignment(TextAlignment.RIGHT);
                            table2.AddCell(cell2);

                            Cell cell3 = new Cell().Add(new Paragraph("Impuesto IVA:"));
                            cell3.SetBorder(Border.NO_BORDER);
                            cell3.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                            table2.AddCell(cell3);

                            Cell cell4 = new Cell().Add(new Paragraph("¢" + Cotizacion.Instancia.tirarImpuesto()));
                            cell4.SetBorder(Border.NO_BORDER);
                            cell4.SetTextAlignment(TextAlignment.RIGHT);
                            table2.AddCell(cell4);

                            Cell cell5 = new Cell().Add(new Paragraph("Total:"));
                            cell5.SetBorder(Border.NO_BORDER);
                            cell5.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                            table2.AddCell(cell5);

                            Cell cell6 = new Cell().Add(new Paragraph("¢" + Cotizacion.Instancia.tirarTotal()));
                            cell6.SetBorder(Border.NO_BORDER);
                            cell6.SetTextAlignment(TextAlignment.RIGHT);
                            table2.AddCell(cell6);

                            doc.Add(table2);

                            doc.Add(new Paragraph("").SetFontSize(14));
                            doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));


                            doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));

                            Paragraph info4 = new Paragraph("Fecha y Hora de la cotización: " + DateTime.Now.ToString()).SetTextAlignment(TextAlignment.CENTER)
                            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14).SetFontColor(ColorConstants.BLACK);

                            doc.Add(info4);

                            Paragraph footer = new Paragraph("¡Gracias por cotizar con VYCUZ!")
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetFontSize(17);

                            doc.Add(footer);

                            doc.Close();



                            FileFact = File(ms.ToArray(), "application/pdf", "CotizaciónProductos.pdf");

                        }
                        catch (Exception ex)
                        {
                            TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                        }

                        //-------------------------------------------------------------------------
                        //ENVIAR EL CORREO---------------------------------------------------------
                        //-------------------------------------------------------------------------

                        if (emailCotizacion != null)
                        {
                            string urlDomain = "https://localhost:3000/";
                            string EmailOrigen = "soportevycuz@gmail.com";
                            string Contraseña = "ecfykdmojjjlpfcn";
                            MailMessage oMailMessage = new MailMessage(EmailOrigen, emailCotizacion, "Cotización de productos",
                                "<p>Estimado usuario,</br></br><hr/>Ha realizado una cotización en VYCUZ.</p>");
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

                        Cotizacion.Instancia.eliminarCarrito();
                        //Actualiza la variable de sesión con el resultado del formulario
                        Session["Facturar"] = false;
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Cotización generada!", "La cotización fue exitosa!", SweetAlertMessageType.success);
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {

                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                return RedirectToAction("Default", "Error");
            }
        }
    }
}