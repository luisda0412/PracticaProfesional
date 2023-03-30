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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Security;

using System.Globalization;
using iText.IO.Font;
using iText.Layout.Borders;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;


namespace MvcApplication.Controllers
{
    public class ReportesController: Controller
    {
        public ActionResult MenuReportes()
        {
            return View();
        }
       

        public ActionResult CreatePdfPrueba()
        {
            Registro_Inventario_VYCUZEntities dbArticulos = new Registro_Inventario_VYCUZEntities();

            //Ejemplos IText7 https://kb.itextpdf.com/home/it7kb/examples
            IEnumerable<Articulo> listaarticulos = null;
            Usuario user = null;
            try
            {
                // Extraer  informacion
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                listaarticulos = _ServiceArticulo.GetArticulo();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                Usuario usuario = (Infraestructure.Models.Usuario)Session["User"];
                user = _ServiceUsuario.GetUsuarioByID(usuario.id);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35,70,35);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);

               


                //Eventos de pie y encabezado de pagina
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(logo, user));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());

                Table table = new Table(1).UseAllAvailableWidth();
                Cell cell = new Cell().Add(new Paragraph("Reporte de Artículos").SetFontSize(14))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER);
                table.AddCell(cell);
               
                cell = new Cell().Add(new Paragraph("Artículos en existencia"))
                      .SetTextAlignment(TextAlignment.CENTER)
                      .SetBorder(Border.NO_BORDER); ;
                table.AddCell(cell);

                doc.Add(table);


                Style styleCell = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

                Table _table = new Table(5).UseAllAvailableWidth();
                //2 filas y 1 celda
                Cell _cell = new Cell(2,1).Add(new Paragraph("#"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell(1, 3).Add(new Paragraph("Artículo"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell(2, 1).Add(new Paragraph("Unidades en existencia"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell)); 
                _cell = new Cell().Add(new Paragraph("Nombre"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Precio en Colones"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Imagen"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));

                List<Articulo> model = dbArticulos.Articulo.Where(t => t.stock > 0).ToList();

                int x = 0;
                foreach (var item in model)
                { 
                    x++;
                    _cell = new Cell().Add(new Paragraph(x.ToString()));
                    _table.AddCell(_cell.SetBackgroundColor(ColorConstants.GREEN));
                    _cell = new Cell().Add(new Paragraph(item.nombre)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);

                    string precio =(String.Format("{0:N2}", item.precio));
                    _cell = new Cell().Add(new Paragraph(precio)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);                   
                    Image image = new Image(ImageDataFactory.Create(item.imagen)).SetTextAlignment(TextAlignment.CENTER);
                    // Tamaño de la imagen
                    image = image.SetHeight(50).SetWidth(50).SetTextAlignment(TextAlignment.CENTER);                  
                    _cell = new Cell().Add(image);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.stock.ToString())).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                }

                doc.Add(_table);

                Paragraph fin = new Paragraph("Fin del Reporte")
                                  .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                  .SetFontSize(14)
                                  .SetFontColor(ColorConstants.BLACK);
                doc.Add(fin);

                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

              

                return File(ms.ToArray(), "application/pdf", "Reporte de artículos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }


        //Reporte de compras/ingresos
        public ActionResult CreatePdfIngresos()
        {
            string FONT = "c:/windows/fonts/arial.ttf";
            PdfFont fuente = PdfFontFactory.CreateFont(FONT);


            Registro_Inventario_VYCUZEntities dbIngresos = new Registro_Inventario_VYCUZEntities();

            IEnumerable<Ingreso> lista = null;
            Usuario user = null;
            try
            {
                // Extraer informacion
                IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                lista = _ServiceIngreso.GetIngresos();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                Usuario usuario = (Infraestructure.Models.Usuario)Session["User"];
                user = _ServiceUsuario.GetUsuarioByID(usuario.id);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35, 70, 35);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);

                

                //Eventos de pie y encabezado de pagina
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(logo, user));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());

                Table table = new Table(1).UseAllAvailableWidth();
                Cell cell = new Cell().Add(new Paragraph("Reporte de Ingresos").SetFontSize(14))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER);
                table.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Compras registradas en el sistema"))
                      .SetTextAlignment(TextAlignment.CENTER)
                      .SetBorder(Border.NO_BORDER); ;
                table.AddCell(cell);

                doc.Add(table);


                Style styleCell = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

                Table _table = new Table(4).UseAllAvailableWidth();
                //2 filas y 1 celda
                Cell _cell = new Cell().Add(new Paragraph("#"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Usuario"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Fecha"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Monto Total Colones"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));

                List<Ingreso> model = dbIngresos.Ingreso.ToList();

                int x = 0;
                foreach (var item in model)
                {
                    x++;
                    _cell = new Cell().Add(new Paragraph(x.ToString()));
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.Usuario.nombre)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.fecha.ToString())).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    string precio = (String.Format("{0:N2}", item.monto_total));
                    _cell = new Cell().Add(new Paragraph(precio)).SetTextAlignment(TextAlignment.CENTER);
                  
                    _table.AddCell(_cell);
                }
                doc.Add(_table);
                Paragraph fin = new Paragraph("Fin del Reporte")
                 .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                 .SetFontSize(14)
                 .SetFontColor(ColorConstants.BLACK);
                doc.Add(fin);
                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

               
                return File(ms.ToArray(), "application/pdf", "Reporte de ingresos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult CreatePdfArqueos()
        {
            string FONT = "c:/windows/fonts/arial.ttf";
            PdfFont fuente = PdfFontFactory.CreateFont(FONT);


            Registro_Inventario_VYCUZEntities dbCaja = new Registro_Inventario_VYCUZEntities();

            IEnumerable<Arqueos_Caja> lista = null;
            Usuario user = null;
            try
            {
            
                IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
                lista = _ServiceCaja.GetArqueos();

        
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                Usuario usuario = (Infraestructure.Models.Usuario)Session["User"];
                user = _ServiceUsuario.GetUsuarioByID(usuario.id);

                MemoryStream ms = new MemoryStream();
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35, 70, 35);

                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(logo, user));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());

                Table table = new Table(1).UseAllAvailableWidth();
                Cell cell = new Cell().Add(new Paragraph("Reporte de Arqueos de Caja Chica").SetFontSize(14))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER);
                table.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Cierres y aperturas de Caja Chica"))
                      .SetTextAlignment(TextAlignment.CENTER)
                      .SetBorder(Border.NO_BORDER); ;
                table.AddCell(cell);

                doc.Add(table);


                Style styleCell = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

                Table _table = new Table(5).UseAllAvailableWidth();
       

                Cell _cell = new Cell().Add(new Paragraph("#"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Usuario encargado"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Fecha"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Saldo en Colones"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Estado"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));

                List<Arqueos_Caja> model = dbCaja.Arqueos_Caja.ToList();

                int x = 0;
                foreach (var item in model)
                {
                    x++;
                    _cell = new Cell().Add(new Paragraph(x.ToString()));
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.Usuario.nombre)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.fecha.ToString())).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    string saldo= (String.Format("{0:N2}", item.saldo));
                    _cell = new Cell().Add(new Paragraph(saldo)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    string isActive = item.estado == true ? "Abierta" : "Cerrada";
                    _cell = new Cell().Add(new Paragraph(isActive)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                }

                doc.Add(_table);
                Paragraph fin = new Paragraph("Fin del Reporte")
                 .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                 .SetFontSize(14)
                 .SetFontColor(ColorConstants.BLACK);
                doc.Add(fin);
                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;


                return File(ms.ToArray(), "application/pdf", "Reporte de Arqueos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }

        // REPORTE DE VENTAS/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult CreatePdfVentas()
        {
            string FONT = "c:/windows/fonts/arial-unicode-ms.ttf";         
            PdfFont font = PdfFontFactory.CreateFont(FONT);


            Registro_Inventario_VYCUZEntities dbVentas = new Registro_Inventario_VYCUZEntities();

            IEnumerable<Venta> lista = null;
            Usuario user = null;
            try
            {
                // Extraer informacion
                IServiceVenta _ServiceVentas = new ServiceVenta();
                lista = _ServiceVentas.GetVentas();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                Usuario usuario = (Infraestructure.Models.Usuario)Session["User"];
                user = _ServiceUsuario.GetUsuarioByID(usuario.id);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35, 70, 35);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);



                //Eventos de pie y encabezado de pagina
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(logo, user));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());

                Table table = new Table(1).UseAllAvailableWidth();
                Cell cell = new Cell().Add(new Paragraph("Reporte de Ventas").SetFontSize(14))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER);
                table.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Ventas de artículos registradas en el sistema"))
                      .SetTextAlignment(TextAlignment.CENTER)
                      .SetBorder(Border.NO_BORDER); ;
                table.AddCell(cell);

                doc.Add(table);


                Style styleCell = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

                Table _table = new Table(6).UseAllAvailableWidth();
                //2 filas y 1 celda
                Cell _cell = new Cell().Add(new Paragraph("#"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Usuario"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Fecha"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Impuesto"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Monto Total Colones"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Tipo de Venta"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));

                List<Venta> model = dbVentas.Venta.ToList();

                int x = 0;
                foreach (var item in model)
                {
                    x++;
                    _cell = new Cell().Add(new Paragraph(x.ToString()));
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.Usuario.nombre)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.fecha.ToString())).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    string impuesto = "\u20A1" + (String.Format("{0:N2}", item.impuesto));
                    _cell = new Cell().Add(new Paragraph(impuesto)).SetTextAlignment(TextAlignment.CENTER);
                    _cell.SetFont(font);
                    _table.AddCell(_cell);
                    string precio = "\u20A1" + (String.Format("{0:N2}", item.monto_total));
                    _cell = new Cell().Add(new Paragraph(precio)).SetTextAlignment(TextAlignment.CENTER);
                    _cell.SetFont(font);
                    _table.AddCell(_cell);
                    string isActive = item.tipopago == true ? "Efectivo" : "Tarjeta";
                    _cell = new Cell().Add(new Paragraph(isActive)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                }

                doc.Add(_table);
                Paragraph fin = new Paragraph("Fin del Reporte")
                 .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                 .SetFontSize(14)
                 .SetFontColor(ColorConstants.BLACK);
                doc.Add(fin);
                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

               
                return File(ms.ToArray(), "application/pdf", "Reporte de Ventas.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        //Crear reporte de proveedores
        public ActionResult CreatePdfProveedores()
        {
            Registro_Inventario_VYCUZEntities dbProveedores = new Registro_Inventario_VYCUZEntities();

        
            IEnumerable<Proveedor> listaproveedores = null;
            Usuario user = null;
            try
            {
                // Extraer  informacion
                IServiceProveedor _ServiceProvee = new ServiceProveedor();
                listaproveedores = _ServiceProvee.GetProveedor();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                Usuario usuario = (Infraestructure.Models.Usuario)Session["User"];
                user = _ServiceUsuario.GetUsuarioByID(usuario.id);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(75, 35, 70, 35);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);




                //Eventos de pie y encabezado de pagina
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler1(logo, user));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler1());

                Table table = new Table(1).UseAllAvailableWidth();
                Cell cell = new Cell().Add(new Paragraph("Reporte de Proveedores").SetFontSize(14))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER);
                table.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Proveedores de artículos"))
                      .SetTextAlignment(TextAlignment.CENTER)
                      .SetBorder(Border.NO_BORDER); ;
                table.AddCell(cell);

                doc.Add(table);


                Style styleCell = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER);

                Table _table = new Table(5).UseAllAvailableWidth();
                //2 filas y 1 celda
                Cell _cell = new Cell().Add(new Paragraph("#"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Proveedor"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Teléfono"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Dirección"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));
                _cell = new Cell().Add(new Paragraph("Estado"));
                _table.AddHeaderCell(_cell.AddStyle(styleCell));

                List<Proveedor> model = dbProveedores.Proveedor.ToList();

                int x = 0;
                foreach (var item in model)
                {
                    x++;
                    _cell = new Cell().Add(new Paragraph(x.ToString()));
                    _table.AddCell(_cell.SetBackgroundColor(ColorConstants.GREEN));
                    _cell = new Cell().Add(new Paragraph(item.descripcion)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.telefono)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    _cell = new Cell().Add(new Paragraph(item.direccion)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                    string isActive = item.estado == true ? "Activo" : "Inactivo";
                    _cell = new Cell().Add(new Paragraph(isActive)).SetTextAlignment(TextAlignment.CENTER);
                    _table.AddCell(_cell);
                }
                doc.Add(_table);

                Paragraph fin = new Paragraph("Fin del Reporte")
                                  .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                  .SetFontSize(14)
                                  .SetFontColor(ColorConstants.BLACK);
                doc.Add(fin);

                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;



                return File(ms.ToArray(), "application/pdf", "Reporte de Proveedores.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }


        // -----------------------------------------------------------------------------------------------------------------
        // Eventos del Header Y footer de los PDFS--------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------------------------
        public class HeaderEventHandler1 : IEventHandler
        {
            Image Img;
            Usuario User;
            public HeaderEventHandler1(Image img, Usuario user)
            {
                Img = img;
                User = user;
            }
            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                PdfCanvas canvas1 = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 75, page.GetPageSize().GetWidth() - 72, 60);
                new Canvas(canvas1, pdfDoc, rootArea)
                  .Add(getTable(docEvent, User));


            }

         
            public Table getTable(PdfDocumentEvent docEvent, Usuario user)
            {
                float[] cellWidth = { 20f, 80f };
                Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
                //tableEvent.SetWidth (UnitValue. CreatePercentValue(100f));

                Style styleCell = new Style()
                .SetBorder(Border.NO_BORDER);

                Style styleText = new Style()
                .SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10f);

                Cell cell = new Cell().Add(Img.SetAutoScale(true));

                tableEvent.AddCell(cell.AddStyle(styleCell)
                .SetTextAlignment(TextAlignment.LEFT)); PdfFont bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);

                cell = new Cell()
                .Add(new Paragraph("Reporte del día\n").SetFont(bold))
                    .Add(new Paragraph("Fecha de emisión: " + DateTime.Now))
                    .Add(new Paragraph("Usuario: " + user.nombre))
                    .AddStyle(styleText).AddStyle(styleCell);


                tableEvent.AddCell(cell);

                return tableEvent;
            }
        }


        public class FooterEventHandler1 : IEventHandler
        {

            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                PdfCanvas canvas1 = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                Rectangle rootArea = new Rectangle(36, 20, page.GetPageSize().GetWidth() - 70, 50);



                new Canvas(canvas1, pdfDoc, rootArea)
                  .Add(getTable(docEvent));



            }
      
            public Table getTable(PdfDocumentEvent docEvent)
            {
                float[] cellWidth = { 92f, 8f };
                Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
                //tableEvent.SetWidth (UnitValue. CreatePercentValue(100f));



                PdfPage page = docEvent.GetPage();
                int pageNum = docEvent.GetDocument().GetPageNumber(page);

                Style styleCell = new Style()
                    .SetPadding(5).
                    SetBorder(Border.NO_BORDER).
                    SetBorderTop(new SolidBorder(ColorConstants.BLACK, 2));

                Cell cell = new Cell().Add(new Paragraph(DateTime.Now.ToLongDateString()));
                tableEvent.AddCell(cell
                .AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT)
                .SetFontColor(ColorConstants.LIGHT_GRAY));

                cell = new Cell().Add(new Paragraph(pageNum.ToString()));

                tableEvent.AddCell(cell.AddStyle(styleCell)
                .SetBackgroundColor(ColorConstants.BLACK)
                .SetFontColor(ColorConstants.WHITE)
                .SetTextAlignment(TextAlignment.CENTER));

                return tableEvent;
            }

        }
    }
}