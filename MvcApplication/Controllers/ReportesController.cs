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

namespace MvcApplication.Controllers
{
    public class ReportesController: Controller
    {
        CultureInfo culture;
        string specifier;

        public ActionResult ProductosLista()
        {
            IEnumerable<Articulo> lista = null;
            try
            {

                IServiceArticulo _ServiceProducto = new ServiceArticulo();
                lista = _ServiceProducto.GetArticulo();
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
                    doc.ShowTextAligned(new Paragraph(String.Format("pág {0} de {1}", i, numberOfPages)),559, 826, i, TextAlignment.CENTER, VerticalAlignment.TOP, 0);
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


        //Reporte de compras/ingresos

        public ActionResult IngresosLista()
        {
            IEnumerable<Ingreso> lista = null;
            try
            {

                IServiceIngreso _ServiceIngreso = new ServiceIngreso();
                lista = _ServiceIngreso.GetIngresos();
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
                   // table.AddCell(new Paragraph(item.nombre));
                    // double preciobien =  Convert.ToDouble(String.Format("%1$,.2f", (double)item.precio);)String.Format("%1$,.2f",(double)item.precio);

                  //  string preciobien = "₡" + item.precio.ToString();
                   // table.AddCell(new Paragraph(preciobien));
                   // table.AddCell(new Paragraph(item.stock.ToString()));
                    // table.AddCell(new Paragraph(item.cantidadMinima.ToString()));
                    //  table.AddCell(new Paragraph(item.cantidadMaxima.ToString()));
                    //table.AddCell(new Paragraph(item.Categoria.descripcion));
                    // Convierte la imagen que viene en Bytes en imagen para PDF
                   // Image image = new Image(ImageDataFactory.Create(item.imagen));
                    // Tamaño de la imagen
                   // image = image.SetHeight(50).SetWidth(50);
                   // table.AddCell(image);
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