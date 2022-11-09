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
using Web.Utils;
using System.Globalization;
using iText.IO.Font;
using iText.Layout.Borders;
using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;


namespace MvcApplication.Controllers
{
    public class ReportesController: Controller
    {

        public ActionResult ProductosLista()
        {
            IEnumerable<Articulo> lista = null;
            try
            {
                IServiceArticulo _ServiceProducto = new ServiceArticulo();
                lista = _ServiceProducto.GetArticulo();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;      
                return RedirectToAction("Default", "Error");
            }
        }


        
        public ActionResult CreatePdfArticulos()
        {
            //Ejemplos IText7 https://kb.itextpdf.com/home/it7kb/examples
            IEnumerable<Articulo> lista = null;
            Usuario user = null;
            try
            {
                // Extraer informacion
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                lista = _ServiceArticulo.GetArticulo();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                int iduser= Convert.ToInt32(TempData["idUser"]);
                user = _ServiceUsuario.GetUsuarioByID(iduser);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);

                //Initialize document
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc, PageSize.A4, false);
                

                //Titulo
                Paragraph header = new Paragraph("Reporte de Artículos")
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(14)
                                   .SetFontColor(ColorConstants.GREEN);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);


                //Nombre y apellidos del usuario
                Paragraph cadenanombre = new Paragraph("Usuario:" + user.nombre)
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(12)
                                   .SetFontColor(ColorConstants.BLACK);

                //Para la fecha del sistema
                Paragraph fecha = new Paragraph(DateTime.Now.ToString())
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(10)
                                   .SetFontColor(ColorConstants.BLACK);


                doc.Add(cadenanombre);
                doc.Add(fecha);
                doc.Add(logo);
                doc.Add(header);
               


                // Crear tabla con 5 columnas 
                Table table = new Table(5, true);

                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Precio");
                table.AddHeaderCell("Disponibles");
                table.AddHeaderCell("Descripción");
                table.AddHeaderCell("Imagen");

                foreach (var item in lista)
                {

                    // Agregar datos a las celdas
                    table.AddCell(new Paragraph(item.nombre));
                    string preciobien = "₡" + item.precio.ToString();
                    table.AddCell(new Paragraph(preciobien));
                    table.AddCell(new Paragraph(item.stock.ToString()));
                    table.AddCell(new Paragraph(item.Categoria.descripcion));
                    // Convierte la imagen que viene en Bytes en imagen para PDF
                    Image image = new Image(ImageDataFactory.Create(item.imagen));
                    // Tamaño de la imagen
                    image = image.SetHeight(50).SetWidth(50);
                    table.AddCell(image);
                }
                doc.Add(table);


                //Close document
                doc.Close();
                // Retorna un File
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Creación del reporte", "Reporte del catálogo realizado con éxito!", SweetAlertMessageType.success);
                return File(ms.ToArray(), "application/pdf", "Reporte de artículos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

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
                int iduser = Convert.ToInt32(TempData["idUser"]);
                user = _ServiceUsuario.GetUsuarioByID(iduser);

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
                _cell = new Cell().Add(new Paragraph("Precio Unitario"));
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

                int numberOfPages = pdfDocument.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {
                    doc.ShowTextAligned(new Paragraph(String.Format("pág {0} de {1}", i, numberOfPages)), 540, 100, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
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

                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Reporte generado!", "el documento se ha creado con éxito!", SweetAlertMessageType.success);

                return File(ms.ToArray(), "application/pdf", "Reporte de artículos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }
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
                Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 75, page.GetPageSize().GetWidth()-72, 60);
                new Canvas(canvas1, pdfDoc, rootArea)
                  .Add(getTable(docEvent, User));


            }

            //Eventos para el pdf
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
            //Eventos para el pdf
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
                tableEvent. AddCell(cell
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

        //Reporte de compras/ingresos

        public ActionResult IngresosLista()
        {
            IEnumerable<Ingreso> lista = null;
            try
            {

                IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                lista = _ServiceIngreso.GetIngresos();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                return RedirectToAction("Default", "Error");
            }
        }
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
                int iduser = Convert.ToInt32(TempData["idUser"]);
                user = _ServiceUsuario.GetUsuarioByID(iduser);

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
                _cell = new Cell().Add(new Paragraph("Monto Total"));
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
                doc.Close();
                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;

                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Reporte generado!", "el documento se ha creado con éxito!", SweetAlertMessageType.success);
                return File(ms.ToArray(), "application/pdf", "Reporte de ingresos.pdf");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        public ActionResult CreatePdfArqueosCaja()
        {
            //Ejemplos IText7 https://kb.itextpdf.com/home/it7kb/examples
            IEnumerable<Articulo> lista = null;
            Usuario user = null;
            try
            {
                // Extraer informacion
                IServiceArticulo _ServiceArticulo = new ServiceArticulo();
                lista = _ServiceArticulo.GetArticulo();

                //Extraer Usuario
                IServiceUsuario _ServiceUsuario = new ServiceUsuario();
                int iduser = Convert.ToInt32(TempData["idUser"]);
                user = _ServiceUsuario.GetUsuarioByID(iduser);

                // Crear stream para almacenar en memoria el reporte 
                MemoryStream ms = new MemoryStream();
                //Initialize writer
                PdfWriter writer = new PdfWriter(ms);

                //Initialize document
                PdfDocument pdfDoc = new PdfDocument(writer);
                Document doc = new Document(pdfDoc, PageSize.A4, false);


                //Titulo
                Paragraph header = new Paragraph("Reporte de Artículos")
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(14)
                                   .SetFontColor(ColorConstants.GREEN);

                //Imagen de la empresa
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                logo = logo.SetHeight(50).SetWidth(120);


                //Nombre y apellidos del usuario
                Paragraph cadenanombre = new Paragraph("Usuario:" + user.nombre)
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(12)
                                   .SetFontColor(ColorConstants.BLACK);

                //Para la fecha del sistema
                Paragraph fecha = new Paragraph(DateTime.Now.ToString())
                                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                   .SetFontSize(10)
                                   .SetFontColor(ColorConstants.BLACK);


                doc.Add(cadenanombre);
                doc.Add(fecha);
                doc.Add(logo);
                doc.Add(header);



                // Crear tabla con 5 columnas 
                Table table = new Table(5, true);

                table.AddHeaderCell("Nombre");
                table.AddHeaderCell("Precio");
                table.AddHeaderCell("Disponibles");
                table.AddHeaderCell("Descripción");
                table.AddHeaderCell("Imagen");

                foreach (var item in lista)
                {

                    // Agregar datos a las celdas
                    // table.AddCell(new Paragraph(item.id));
                    table.AddCell(new Paragraph(item.nombre));
                    // double preciobien =  Convert.ToDouble(String.Format("%1$,.2f", (double)item.precio);)String.Format("%1$,.2f",(double)item.precio);

                    string preciobien = "₡" + item.precio.ToString();
                    table.AddCell(new Paragraph(preciobien));
                    table.AddCell(new Paragraph(item.stock.ToString()));
                    // table.AddCell(new Paragraph(item.cantidadMinima.ToString()));
                    //  table.AddCell(new Paragraph(item.cantidadMaxima.ToString()));
                    table.AddCell(new Paragraph(item.Categoria.descripcion));
                    // Convierte la imagen que viene en Bytes en imagen para PDF
                    Image image = new Image(ImageDataFactory.Create(item.imagen));
                    // Tamaño de la imagen
                    image = image.SetHeight(50).SetWidth(50);
                    table.AddCell(image);
                }
                doc.Add(table);



                // Colocar número de páginas
                int numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i <= numberOfPages; i++)
                {

                    // Write aligned text to the specified by parameters point



                    Paragraph p = new Paragraph("Reporte del catálogo de productos");
                    doc.ShowTextAligned(new Paragraph(String.Format("pág {0} de {1}", i, numberOfPages)), 559, 826, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
                }


                //Close document
                doc.Close();
                // Retorna un File
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Creación del reporte", "Reporte del catálogo realizado con éxito!", SweetAlertMessageType.success);
                return File(ms.ToArray(), "application/pdf", "Reporte de artículos.pdf");


            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Log.Error(ex, MethodBase.GetCurrentMethod());
                ViewBag.NotificationMessage = Util.SweetAlertHelper.Mensaje("Error en reporte", ex.Message, SweetAlertMessageType.warning);
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }
    }
}