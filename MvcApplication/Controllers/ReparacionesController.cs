using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AplicationCore.Services;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Infraestructure.Models;

using System.IO;
using Web.Security;
using MvcApplication.Util;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Layout.Properties;
using System.Globalization;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;

namespace MvcApplication.Controllers
{
    public class ReparacionesController : Controller
    {
        Reportes_Tecnicos re = new Reportes_Tecnicos();
        //Para guardadr la reparacion que se clickee
        int? codigo = 0;
        public static double saldoActual { get; set; }
        //private MyContext db = new MyContext();
        private SelectList listaServicios(long idSer = 0)
        {
            IServiceServicio _ServiceServicio = new ServiceServicio();
            IEnumerable<Servicio_Reparacion> listaServicios = _ServiceServicio.GetServicio();
            return new SelectList(listaServicios, "id", "descripcion", idSer);
        }


        // GET: Reparaciones
        public ActionResult Index()
        {
            IEnumerable<Reparaciones> lista = null;
            try
            {
                IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
                lista = _ServiceReparaciones.GetReparacion();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult IndexUsuario()
        {
            IEnumerable<Reparaciones> lista = null;
            try
            {
                IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
                int idUsuario = Convert.ToInt32(TempData["idUser"]);
                lista = _ServiceReparaciones.GetReparacionPorUsuario(idUsuario);
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult ReportesTecnicosUsuario(int? id)
        {

            IEnumerable<Reportes_Tecnicos> lista = null;
            try
            {
                IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
                lista = _ServiceRTecnico.GetReportesByID((long)id);
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        [HttpPost]
        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Save(Reparaciones repa)
        {
        
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            ModelState.Remove("imagen");
            if (ModelState.IsValid)
            {
                try
                {
                    repa.usuario_id = Convert.ToInt32(TempData["idUser"]);
                    repa.fecha = DateTime.Now;
                    _ServiceReparaciones.Save(repa);
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Datos registrados", "reparación guardada con éxito", SweetAlertMessageType.success);
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                    return RedirectToAction("Default", "Error");
                }
            }
           
            return RedirectToAction("Index");
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Create()
        {
            ViewBag.ServiciosLista = listaServicios();
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador)]
        public ActionResult Edit(int? id)
        {
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            Reparaciones repa = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                repa = _ServiceReparaciones.GetReparacionByID(id.Value);

                if (repa == null)
                {
                    TempData["Message"] = "No existe el proveedor solicitado";
                    TempData["Redirect"] = "Proveedor";
                    TempData["Redirect-Action"] = "Index";
                    // Redireccion a la captura del Error
                    return RedirectToAction("Default", "Error");
                }

                ViewBag.ServiciosLista = listaServicios();
                return View(repa);
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Rol";
                TempData["Redirect-Action"] = "Index";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }

        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Details(int? id)
        {
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();
            Reparaciones repa = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                repa = _ServiceReparaciones.GetReparacionByID(id.Value);

                if (repa == null)
                {
                    return RedirectToAction("Index");
                }
                return View(repa);

            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
                return RedirectToAction("IndexAdmin");
            }
        }

        public ActionResult desabilitar(long id)
        {
            using (MyContext cdt = new MyContext())
            {
                cdt.Configuration.LazyLoadingEnabled = false;

                try
                {
                    Reparaciones art = cdt.Reparaciones.Where(x => x.id == id).FirstOrDefault();
                    art.estado = !art.estado;
                    cdt.Reparaciones.Add(art);
                    cdt.Entry(art).State = EntityState.Modified;
                    cdt.SaveChanges();
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Reparación eliminada", "Datos eliminados de la base", SweetAlertMessageType.success);
                    return RedirectToAction("Index");

                }
                catch (Exception e)
                {
                    string mensaje = "";
                    Infraestructure.Util.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod(), ref mensaje);
                    throw;
                }
            }
        }

        public ActionResult buscarReparacionxCedula(string filtro)
        {
            IEnumerable<Reparaciones> lista = null;
            IServiceReparaciones _ServiceReparaciones = new ServiceReparaciones();

            // Error porque viene en blanco 
            if (string.IsNullOrEmpty(filtro))
            {
                lista = _ServiceReparaciones.GetReparacion();
            }
            else
            {
                lista = _ServiceReparaciones.GetReparacionByNombre(filtro);
            }


            // Retorna un Partial View
            return PartialView("_PartialViewVistaxCedula", lista);
        }



        //Reportes Tecnicos--------------------------------------------------
        //-------------------------------------------------------------------
        //-------------------------------------------------------------------

        //VARIABLE QUE LLENA EL CODIGO DEL SERVICIO PARA EL REPORTE
        public static int codigoSer { get; set; }
        public ActionResult ReportesTecnicos(int? id)
        {
            this.codigo = id;
            codigoSer = (int)id;

             IEnumerable<Reportes_Tecnicos> lista = null;
            try
            {
                IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
                lista = _ServiceRTecnico.GetReportesByID((long)id);
            }
            catch (Exception e)
            {
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult CreateReporte()
        {
            
            return View();
        }

        public ActionResult SaveReporte(Reportes_Tecnicos repo)
        {
            MemoryStream target = new MemoryStream();
            IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();

            ModelState.Remove("id");
            ModelState.Remove("reparacion_id");
            if (ModelState.IsValid)
            {
                try
                {
                    repo.reparacion_id = codigoSer;
                    repo.fecha = DateTime.Now;
                    _ServiceRTecnico.Save(repo);

                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Reporte creado", "El trabajo realizado ha quedado registrado en la reparación", SweetAlertMessageType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                    TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                    return RedirectToAction("Default", "Error");
                }
            }
            return RedirectToAction("Index");

        }

        public ActionResult EliminarReporte(int? id)
        {
            MemoryStream target = new MemoryStream();
            IServiceRTecnico _ServiceRTecnico = new ServiceRTecnico();
            try
            {

                _ServiceRTecnico.Eliminar((long)id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Salvar el error en un archivo 
                Infraestructure.Util.Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Libro";
                TempData["Redirect-Action"] = "IndexAdmin";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }


        //Para registrar los pagos de las reparaciones
        public ActionResult IndexCobros()
        {
            //Mostrar Mensaje de abrir la caja si esta cerrada
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja caja = new Arqueos_Caja();
            caja = _ServiceCaja.GetArqueoLast();
            if (caja.estado == false)
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Mensaje importante!", "La caja chica está cerrada, antes de registrar pagos vaya a la página de arqueos y abra la caja.", SweetAlertMessageType.info);
            }

            if (TempData["mensaje"] != null)
                ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            IEnumerable<Reparaciones> lista = null;
          
            FileContentResult documento = TempData["archivoPDF"] as FileContentResult;
            if (documento != null)
            {
                ViewBag.File = documento;
            }
            // Si no hay un archivo PDF en TempData, simplemente se muestra la página "IndexCobros"
            return View(lista);
        }

        public FileContentResult DownloadFile()
        {
            FileContentResult file = TempData["file"] as FileContentResult;
            TempData.Clear();
            return File(file.FileContents, file.ContentType, file.FileDownloadName);
        }

        //Buscar la reparacion que se va a cobrar
        public ActionResult buscarReparacionxID(string filtro)
        {
            IEnumerable<Reparaciones> lista;
            IServiceReparaciones _ServiceRep = new ServiceReparaciones();

            if (string.IsNullOrEmpty(filtro) || filtro == "0")
            {
                filtro = "0";
                lista = _ServiceRep.GetReparacionByNombre(filtro);
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Error en la búsqueda", "No existen reparaciones registradas bajo la cédula digitada por favor verifíque los datos!", SweetAlertMessageType.error);
                // Redirigir a la vista principal en caso de error
                ViewBag.ReloadPage = true;
            }
            else
            {
                lista = _ServiceRep.GetReparacionByNombre(filtro);
            }

            // Retorna un Partial View
            return PartialView("_PartialViewCobrarRepa", lista);
        }

        //Pagina de registrar los cobros 
        public ActionResult IndexCrearCobros(int? id)
        {
            ServiceReparaciones _ServiceRep = new ServiceReparaciones();
            Reparaciones rep = null;

            try
            {
                if (id == null)
                {
                    return RedirectToAction("IndexCobros");
                }
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
                rep = _ServiceRep.GetReparacionByID(id.Value);
                ViewBag.Reparacion = rep;
                return View();

            }
            catch (Exception e)
            {
                TempData["Message"] = "Error al procesar los datos! " + e.Message;
                return RedirectToAction("Index");
            }
        }

        //Registrar los pagos de la reparacion
        public static string nuevoMonto { get; set; }
        public static string comentario { get; set; }
        public static string tipopago { get; set; }
        public ActionResult obtenerDatosFormCobros()
        {
            nuevoMonto = Request.Form["monto"];
            comentario = Request.Form["comentario"];
            tipopago = Request.Form["pago"];
          
            IServiceReparaciones serviceRepa = new ServiceReparaciones();
            Reparaciones reparaciones = serviceRepa.GetReparacionByID(Convert.ToInt32(TempData["idReparacion"]));

            if (string.IsNullOrEmpty(nuevoMonto) || string.IsNullOrEmpty(comentario) || string.IsNullOrEmpty(tipopago))
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Formulario inválido", "Por favor verifíque el monto, comentario y la selección del tipo de pago", SweetAlertMessageType.warning);
                return RedirectToAction("IndexCrearCobros", new { id = reparaciones.id});
            }

         
            if (Convert.ToDouble(nuevoMonto) < reparaciones.monto_total)
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Monto inválido", "El monto ingresado es menor al cobro total de la reparación por favor verifíque!", SweetAlertMessageType.warning);
                return RedirectToAction("IndexCrearCobros", new { id = reparaciones.id });
            }

            //Mostrar Mensaje de abrir la caja si esta cerrada
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja cajita = new Arqueos_Caja();
            cajita = _ServiceCaja.GetArqueoLast();

            if (cajita.estado==false)
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Caja Cerrada", "la caja chica se encuentra cerrada, antes de efectuar el cobro debe abrir la caja!", SweetAlertMessageType.warning);
                return RedirectToAction("IndexCrearCobros", new { id = reparaciones.id });
            }
            //Cambia el estado de la reparacion
            serviceRepa.Desabilitar(reparaciones.id);

            //REGISTRAR LOS MONTOS EN LA CAJA CHICA----------------------------------------------------------------
            IServiceCajaChica servicio = new ServiceCajaChica();
            Caja_Chica ultimacaja = new Caja_Chica();
            ultimacaja = servicio.GetCajaChicaLast();

            Caja_Chica cajaChica = new Caja_Chica();
            cajaChica.fecha = DateTime.Now;
            cajaChica.entrada = Convert.ToDouble(nuevoMonto);
            cajaChica.salida = cajaChica.entrada - reparaciones.monto_total;

            saldoActual = ((double)cajaChica.entrada - (double)cajaChica.salida) + (double)ultimacaja.saldo;
            cajaChica.saldo = saldoActual;

            IServiceCajaChica caja = new ServiceCajaChica();
            caja.Save(cajaChica);

            //------------------------------------------------------------------------------------------------------------------------
            //Crear el pdf de la reparacion--------------------------------------------------------------------------------------
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
                Paragraph header = new Paragraph("Ticket Electrónico")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                doc.Add(header);

                // Logo y atendido por
                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false))
                    .SetHeight(50)
                    .SetWidth(120)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                /*Paragraph cadenanombre = new Paragraph("Atendido por: " + user.nombre)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro*/

                doc.Add(logo);
                //doc.Add(cadenanombre);

                // Información de la empresa
                Paragraph info1 = new Paragraph("Lugar: City Mall")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                Paragraph info2 = new Paragraph("Ubicación: Montserrat, Alajuela, Costa Rica")
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro

                Paragraph info9 = new Paragraph("Número de Reparación: #" + reparaciones.id)
                   .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                   .SetFontSize(12)
                   .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                doc.Add(info1);
                doc.Add(info2);
                doc.Add(info9);

                // Separador
                LineSeparator separator = new LineSeparator(new SolidLine(1));
                doc.Add(separator);

                // Información del cliente
                Paragraph info3 = new Paragraph("Cédula del Cliente: " + reparaciones.cliente_id)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(10)
                    .SetFontColor(ColorConstants.BLACK);

                doc.Add(info3);

                Paragraph info5 = new Paragraph("Detalle de la reparación")
                  .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                  .SetFontSize(10)
                  .SetFontColor(ColorConstants.BLACK);

                doc.Add(info5);

                // Separador
                doc.Add(separator);

                // Crear tabla con 4 columnas
                Table table = new Table(new float[] { 1, 2, 1, 1 }).UseAllAvailableWidth();

                table.AddHeaderCell(new Cell().Add(new Paragraph("Número de Reparación").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Comentario").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Tipo de Pago").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Monto en Colones").SetFont(numeroArticuloFont).SetFontSize(12)).SetTextAlignment(TextAlignment.CENTER));
                
                table.AddCell(new Cell().Add(new Paragraph(reparaciones.id.ToString()).SetFont(numeroArticuloFont)).SetTextAlignment(TextAlignment.CENTER));
                table.AddCell(new Cell().Add(new Paragraph(comentario.ToString()).SetFont(descripcionFont)).SetTextAlignment(TextAlignment.CENTER));
                
                table.AddCell(new Cell().Add(new Paragraph(tipopago.ToString()).SetFont(descripcionFont))).SetTextAlignment(TextAlignment.CENTER);
                double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                string montoCantidad = montoDouble.ToString("C2", CultureInfo.GetCultureInfo("es-CR")); // formatear como moneda en colones (CRC)
                table.AddCell(new Cell().Add(new Paragraph(montoCantidad).SetFont(cantidadPrecioFont)).SetTextAlignment(TextAlignment.RIGHT));


                doc.Add(table);
                doc.Add(new Paragraph("").SetFontSize(14));

                doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));

                Table table2 = new Table(new float[] { 1f, 1f }).UseAllAvailableWidth();

                Cell cell1 = new Cell().Add(new Paragraph("Monto Total:"));
                cell1.SetBorder(Border.NO_BORDER);
                cell1.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                table2.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("¢"+ montoCantidad));
                cell2.SetBorder(Border.NO_BORDER);
                cell2.SetTextAlignment(TextAlignment.RIGHT);
                table2.AddCell(cell2);

                doc.Add(table2);

                doc.Add(new Paragraph("").SetFontSize(14));
                doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));

                Table table3 = new Table(new float[] { 1f, 1f }).UseAllAvailableWidth();

                Cell cell7 = new Cell().Add(new Paragraph("Su Pago:"));
                cell7.SetBorder(Border.NO_BORDER);
                cell7.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                table3.AddCell(cell7);

                string pago = (String.Format("{0:N2}", cajaChica.entrada));
                Cell cell8 = new Cell().Add(new Paragraph("¢" + pago));
                cell8.SetBorder(Border.NO_BORDER);
                cell8.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                table3.AddCell(cell8);

                Cell cell9 = new Cell().Add(new Paragraph("Su Cambio:"));
                cell9.SetBorder(Border.NO_BORDER);
                cell9.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                table3.AddCell(cell9);

                string vuelto = (String.Format("{0:N2}", cajaChica.salida));
                Cell cell10 = new Cell().Add(new Paragraph("¢" + vuelto));
                cell10.SetBorder(Border.NO_BORDER);
                cell10.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                table3.AddCell(cell10);

                doc.Add(table3);


                doc.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(10f).SetMarginBottom(10f));

                Paragraph info4 = new Paragraph("Fecha y Hora de venta: " + DateTime.Now.ToString()).SetTextAlignment(TextAlignment.CENTER)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14).SetFontColor(ColorConstants.BLACK);

                doc.Add(info4);

                Paragraph footer = new Paragraph("¡Gracias por su confianza!")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(17);

                doc.Add(footer);

                doc.Close();

                byte[] bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);

                FileFact = File(ms.ToArray(), "application/pdf", "Ticket Reparación.pdf");

                // Limpiar el MemoryStream y reposicionar el cursor al inicio
                ms.Flush();
                ms.Position = 0;

                // Guardar el archivo PDF en TempData
                TempData["archivoPDF"] = FileFact;
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Pago realizado", "Se ha registrado el pago de la reparación efectivamente", SweetAlertMessageType.success);

                nuevoMonto = "";
                comentario = "";
                tipopago = "";

                return RedirectToAction("IndexCobros");


               

            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
            }

            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Error en el pago", "No se ha realizado el pago correspondiente, intente de nuevo", SweetAlertMessageType.error);
            return RedirectToAction("IndexCobros");
        }
    }

}
