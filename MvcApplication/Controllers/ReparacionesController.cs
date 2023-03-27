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
            if (TempData["archivoPDF"] != null)
            {
                byte[] pdfBytes = (byte[])TempData["archivoPDF"];

                // Limpiar el TempData
                TempData["archivoPDF"] = null;

                // Descargar el archivo PDF
                return File(pdfBytes, "application/pdf", "Factura de Reparación.pdf");
            }

            // Si no hay un archivo PDF en TempData, simplemente se muestra la página "IndexCobros"
            return View(lista);
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
            //Crear el pdf del Pago--------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------
            MemoryStream ms = new MemoryStream();
            //FileContentResult FileFact = null;
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
                        .SetFontSize(11);

                    Style tableCellStyle = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.BLACK);

                    // Agregar el encabezado
                    Paragraph header = new Paragraph("Factura de Reparación").AddStyle(titleStyle);
                    doc.Add(header);

                    // Agregar la información de la empresa
                    Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false))
                        .SetHeight(50)
                        .SetWidth(120);
                    doc.Add(logo);

                    Paragraph header2 = new Paragraph("Segundo Piso, City Mall").AddStyle(subtitleStyle);
                    Paragraph header3 = new Paragraph("Alajuela, Costa Rica").AddStyle(subtitleStyle);
                    doc.Add(header2);
                    doc.Add(header3);

                    // Agregar la información del cliente y la fecha
                    Paragraph cliente = new Paragraph("Cédula Cliente: " + reparaciones.cliente_id).AddStyle(smallTextStyle);
                    Paragraph fecha = new Paragraph("Fecha de Creación: " + DateTime.Now.ToString()).AddStyle(smallTextStyle);
                    doc.Add(cliente);
                    doc.Add(fecha);

                        // Agregar la tabla con la información de la nota de crédito
                        Table table = new Table(new float[] { 1, 2, 1, 1 })
                        .SetWidth(UnitValue.CreatePercentValue(100)).SetHeight(UnitValue.CreatePercentValue(100));

                        table.AddHeaderCell(new Cell().Add(new Paragraph("Número de Reparación")).AddStyle(tableHeaderStyle));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Comentario del Pago")).AddStyle(tableHeaderStyle));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Monto en colones")).AddStyle(tableHeaderStyle));
                        table.AddHeaderCell(new Cell().Add(new Paragraph("Tipo de Pago")).AddStyle(tableHeaderStyle));
                
                    table.AddCell(new Paragraph(reparaciones.id.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));
                    table.AddCell(new Paragraph(comentario.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));


                    double montoDouble = Convert.ToDouble(nuevoMonto); // convertir a double y dividir entre 100 para obtener decimales
                    string montoCantidad = montoDouble.ToString("C2", CultureInfo.GetCultureInfo("es-CR")); // formatear como moneda en colones (CRC)
                    table.AddCell(new Paragraph(montoCantidad).SetVerticalAlignment(VerticalAlignment.TOP));

                    table.AddCell(new Paragraph(tipopago.ToString()).SetVerticalAlignment(VerticalAlignment.TOP));
                    doc.Add(table);

                    doc.Close();
                    byte[] bytesStream = ms.ToArray();
                    ms = new MemoryStream();
                    ms.Write(bytesStream, 0, bytesStream.Length);

                     byte[] pdfBytes = ms.ToArray();

                        // Limpiar el MemoryStream y reposicionar el cursor al inicio
                        ms.Flush();
                        ms.Position = 0;

                        // Guardar el archivo PDF en TempData
                        TempData["archivoPDF"] = pdfBytes;
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Pago realizado", "Se ha registrado el pago de la reparación efectivamente", SweetAlertMessageType.success);

                nuevoMonto = null;
                comentario = null;
                tipopago = null;

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
