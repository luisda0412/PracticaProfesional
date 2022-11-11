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
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Web.Security;
using Web.Utils;
using Web.ViewModel;

namespace MvcApplication.Controllers
{
    public class VentaController: Controller
    {
        IServiceVenta _Serviceventa = new ServiceVenta();
        public static double saldoActual { get; set; }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Venta> lista = null;
            try
            {
                IServiceVenta _ServiceVenta = new ServiceVenta();
                lista = _ServiceVenta.GetVentas();
                if (TempData["mensaje"] != null)
                    ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

        public ActionResult facturaElectronica(string nombre, string apellidos, string email, string telefono)
        {
            if (nombre.Trim().Length != 0 || apellidos.Trim().Length != 0 || email.Trim().Length != 0 || telefono.Trim().Length != 0)
            {
                TempData["Nombre"]= nombre;
                TempData["Apellidos"] = apellidos;
                TempData["Email"] = email;
                TempData["Telefono"]= telefono;
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Factura generada!", "La factura electrónica se ha creado!", SweetAlertMessageType.success);
              
            }
            else
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Fallo en factura!", "No puede dejar espacios vacíos!", SweetAlertMessageType.error);     
            }
            return RedirectToAction("IndexVenta");

        }

        public static string nombreForm { get; set; }
        public static string apellidosForm { get; set; }
        public static string emailForm { get; set; }
        public static string telefonoForm { get; set; }
        public ActionResult obtenerDatosForm()
        {
            nombreForm = Request.Form["name"];
            apellidosForm = Request.Form["lastname"];
            emailForm = Request.Form["email"];
            telefonoForm = Request.Form["phone"];

            return RedirectToAction("IndexVenta");
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Save(Venta venta)
        {
             IServiceUsuario serviceUsuario = new ServiceUsuario();
            try
            {
                if (Carrito.Instancia.Items.Count() <= 0)
                {
                    return RedirectToAction("");
                }
                else
                {
                    Caja_Chica cajaChica = new Caja_Chica();
                    Facturas factura= new Facturas();
                    Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));
                    //venta.nombre_cliente = user.nombre;
                    if (venta.nombre_cliente != null)
                    {
                        IServiceArticulo serviceArticulo = new ServiceArticulo();
                       
                        Articulo articulo = new Articulo();
                        var listaLinea = Carrito.Instancia.Items;

                        XmlDocument xml = new XmlDocument();
                        XmlNode root = xml.CreateElement("Factura_Electronica");
                        xml.AppendChild(root);

                        //ATRIBUTOS DE FACTURA
                        XmlAttribute xmlns = xml.CreateAttribute("xmlns");
                        xmlns.Value = "https://cdn.comprobanteselectronicos.go.cr/xml-schemas/v4.3/facturaElectronica";
                        XmlAttribute xsd = xml.CreateAttribute("xmlns:xsd");
                        xsd.Value = "http://www.w3.org/2001/XMLSchema";
                        XmlAttribute xsi = xml.CreateAttribute("xmlns:xsi");
                        xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
                        root.Attributes.Append(xmlns);
                        root.Attributes.Append(xsd);
                        root.Attributes.Append(xsi);

                        //CREACION DEL NODO DE INFO DE LA FACTURA ELECTRONICA
                        XmlNode clave = xml.CreateElement("Clave");
                        clave.InnerText = "";
                        root.AppendChild(clave);
                        XmlNode codigoActividad = xml.CreateElement("Codigo_de_Actividad");
                        codigoActividad.InnerText = "";
                        root.AppendChild(codigoActividad);
                        XmlNode fechaEmision = xml.CreateElement("Fecha_de_Emision");
                        string formatted = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK");
                        fechaEmision.InnerText = formatted;
                        root.AppendChild(fechaEmision);


                        //CREACION DEL NODO DE INFORMACION DE LA EMPRESA
                        XmlNode nodoEmpresa = xml.CreateElement("Emisor");

                        //ASIGNACION DE ELEMENTOS AL NODO DE LA EMPRESA
                        XmlNode nombreEmpresa = xml.CreateElement("Nombre");
                        nombreEmpresa.InnerText = "VYCUZ";

                        //CREACION DEL NODO DE UBICACION DENTRO DEL NODO DE LA EMRPESA
                        XmlNode nodoUbicacion = xml.CreateElement("Ubicacion");

                        //ASIGNACION DE LOS ELEMENTOS AL NODO DE UBICACION
                        XmlNode provincia = xml.CreateElement("Provincia");
                        provincia.InnerText = "Alajuela";
                        XmlNode centroComercial = xml.CreateElement("Centro_Comercial");
                        centroComercial.InnerText = "City Mall";
                        XmlNode otrasSenas = xml.CreateElement("Otras_Senas");
                        otrasSenas.InnerText = "Segundo piso al frente de CellCom";

                        //ASGNACION DE LOS ELEMENTOS DENTRO DEL NODO DE UBICACION
                        nodoUbicacion.AppendChild(provincia);
                        nodoUbicacion.AppendChild(centroComercial);
                        nodoUbicacion.AppendChild(otrasSenas);

                        //CREACION DEL NODO DE TELEFONO
                        XmlNode telefonoEmpresa = xml.CreateElement("Telefono");

                        //ASIGNACION DE ELEMENTOS AL NODO DE TELEFONO
                        XmlNode codigo = xml.CreateElement("Codigo_Pais");
                        codigo.InnerText = "506";
                        XmlNode telefono = xml.CreateElement("Telefono_Empresa");
                        telefono.InnerText = "72791408";

                        //ADJUNTAR ELEMENTOS AL N0DO DE TELEFONO
                        telefonoEmpresa.AppendChild(codigo);
                        telefonoEmpresa.AppendChild(telefono);

                        XmlNode correoEmpresa = xml.CreateElement("Correo_Electronico");
                        correoEmpresa.InnerText = "vycuz@gmail.com";

                        //ADJUNTAR ELEMENTOS AL NODO DE LA EMPRESA
                        nodoEmpresa.AppendChild(nombreEmpresa);
                        nodoEmpresa.AppendChild(nodoUbicacion);
                        nodoEmpresa.AppendChild(telefonoEmpresa);
                        nodoEmpresa.AppendChild(correoEmpresa);
                        root.AppendChild(nodoEmpresa);

                        //CREACION DEL NODO CLIENTE
                        XmlNode nodoCliente = xml.CreateElement("Receptor");

                        //ASIGNACION DE ELEMENTOS AL NODO CLIENTE
                        XmlNode nombreCliente = xml.CreateElement("Nombre");
                        nombreCliente.InnerText = nombreForm;
                        XmlNode ApellidoCliente = xml.CreateElement("Apellidos");
                        ApellidoCliente.InnerText = apellidosForm;
                        XmlNode correoCliente = xml.CreateElement("Email");
                        correoCliente.InnerText = emailForm;
                        XmlNode telefonoCliente = xml.CreateElement("Telefono");
                        telefonoCliente.InnerText = telefonoForm;

                        //ADJUNTAS LOS ELEMENTOS AL NODO CLIENTE
                        nodoCliente.AppendChild(nombreCliente);
                        nodoCliente.AppendChild(ApellidoCliente);
                        nodoCliente.AppendChild(correoCliente);
                        nodoCliente.AppendChild(telefonoCliente);
                        root.AppendChild(nodoCliente);

                        string descuento2="";
                        Detalle_Venta linea = new Detalle_Venta();
                        
                        foreach (var items in listaLinea)
                        {
                            
                            linea.articulo_id = (int)items.idArticulo;
                            linea.cantidad = items.cantidad;
                            venta.impuesto = (double?)Carrito.Instancia.GetImpuesto();
                            venta.usuario_id = Convert.ToInt32(TempData["idUser"]);
                            venta.estado = true;
                            linea.venta_id = venta.id;
                            linea.precio = items.precio;
                            //DESCUENTO POR MAS DE 3 PRODUCTOS Y QUE EL TOTAL A PAGAR SEA MAYOR A 30000
                            linea.descuento= (listaLinea.Count() >= 3 || items.cantidad >=3) && (double?)Carrito.Instancia.GetTotal() > 30000? linea.descuento = (double?)Carrito.Instancia.GetTotal() * 0.10: linea.descuento=0;
                            descuento2 = Convert.ToString(linea.descuento);
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal() - linea.descuento;
                            venta.Detalle_Venta.Add(linea);

                            descuento2 = Convert.ToString(linea.descuento);
                            //LINEA DE CODGO PARA ACTUALIZAR EL PRODUCTO CUANDO SE HACE UNA COMPRA, LA COMENTO YA QUE HAY QUE HACER PRUEBAS Y LUEGO NOS QUEDAMOS SIN UNIDADES
                            serviceArticulo.actualizarCantidad(linea.articulo_id, (int)linea.cantidad, false);

                            //CREACION DEL NODO DETALLE VENTA
                            XmlNode nodoDetalle = xml.CreateElement("Detalle_Venta");

                            //ASIGNACION DE ELEMENTOS DEL NODO DE DETALLE VENTA
                            XmlNode articuloID = xml.CreateElement("Articulo_ID");
                            articuloID.InnerText = Convert.ToString(linea.articulo_id);
                            XmlNode cantidad = xml.CreateElement("Cantidad");
                            cantidad.InnerText = Convert.ToString(linea.cantidad);
                            XmlNode precio = xml.CreateElement("Precio_Articulo");
                            precio.InnerText = Convert.ToString(linea.precio);
                            XmlNode descuento = xml.CreateElement("Descuento");
                            descuento.InnerText = Convert.ToString(linea.descuento);

                            //ADJUNTAR LOS ELEMENTOS AL NODO DETALLE VENTA
                            nodoDetalle.AppendChild(articuloID);
                            nodoDetalle.AppendChild(cantidad);
                            nodoDetalle.AppendChild(precio);
                            nodoDetalle.AppendChild(descuento);
                            root.AppendChild(nodoDetalle);
                        }
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Venta exitosa", "Una nueva venta ha sido registrada", SweetAlertMessageType.success);
                        //venta.impuesto = (double)Carrito.Instancia.GetSubTotal();
                        //venta.monto_total = (double?)Carrito.Instancia.GetTotal() + ((double?)Carrito.Instancia.GetTotal() * venta.impuesto);
                        //CREAR EL XML

                        //SI TODO ESTA BIEN SE GUARDA LA VENTA

                        IServiceVenta _ServiceVenta = new ServiceVenta();
                        Venta ven = _Serviceventa.Save(venta);

                        //FACTURA
                        factura.venta_id = venta.id;
                        factura.empresa_id = 1;
                        factura.tipoFactura = user.rol_id == 1 || user.rol_id == 2 ? factura.tipoFactura = true : factura.tipoFactura = false;

                        //LLENAR DATOS DE CAJA CHICA
                        if (venta.tipopago==true)
                        {
                            IServiceCajaChica servicio  = new ServiceCajaChica();
                            Caja_Chica ultimacaja = new Caja_Chica();
                            ultimacaja = servicio.GetCajaChicaLast();

                            cajaChica.fecha = DateTime.Now;
                            cajaChica.entrada = Convert.ToDouble(Request.Form["entrada"]);
                            cajaChica.salida = cajaChica.entrada - (double?)Carrito.Instancia.GetTotal();

                            saldoActual += (double)cajaChica.entrada-(double)cajaChica.salida;
                            saldoActual += (double)ultimacaja.saldo;
                            cajaChica.saldo = saldoActual;

                            IServiceCajaChica caja = new ServiceCajaChica();
                            caja.Save(cajaChica);
                        }

                        IServiceFactura serviceFactura = new ServiceFactura();
                        Facturas fac = serviceFactura.Save(factura);
                        //CREACION DEL NODO DETALLE VENTA
                        XmlNode nodoVenta = xml.CreateElement("Venta");

                        //ASIGNACION DE ELEMENTOS AL NODO DE VENTA
                        XmlNode ventaID = xml.CreateElement("ID_Venta");
                        ventaID.InnerText = Convert.ToString(venta.id);
                        XmlNode impuesto = xml.CreateElement("Impuesto");
                        impuesto.InnerText = Convert.ToString(venta.impuesto);
                        XmlNode montoTotal = xml.CreateElement("Monto_Total");
                        montoTotal.InnerText = Convert.ToString(venta.monto_total);

                        //ADJUNTAR ELEMENMTOS AL NODO DE VENTA
                        nodoVenta.AppendChild(ventaID);
                        nodoVenta.AppendChild(impuesto);
                        nodoVenta.AppendChild(montoTotal);
                        root.AppendChild(nodoVenta);

                        //CREACION DEL NODO RESUMEN FACTURA
                        XmlNode resumenFactura = xml.CreateElement("Resumen_Factura");

                        //ASIGNACION DE LOS ELEMENTOS DEL NODO FACTURA
                        XmlNode servGravados = xml.CreateElement("TotalServGravados");
                        servGravados.InnerText = Convert.ToString("");
                        XmlNode servExento = xml.CreateElement("TotalServExento");
                        servExento.InnerText = Convert.ToString("");
                        XmlNode mercanciaGravada = xml.CreateElement("TotalMercanciasGravadas");
                        mercanciaGravada.InnerText = Convert.ToString("");
                        XmlNode mercanciaExenta = xml.CreateElement("TotalMercanciasExentas");
                        mercanciaExenta.InnerText = Convert.ToString("");
                        XmlNode impuestoResumen = xml.CreateElement("Impuesto");
                        impuestoResumen.InnerText = Convert.ToString(venta.impuesto);
                        XmlNode descuentoResumen = xml.CreateElement("Descuento");
                        descuentoResumen.InnerText = descuento2;
                        XmlNode montoTotalResumen = xml.CreateElement("Monto_Total");
                        montoTotalResumen.InnerText = Convert.ToString(venta.monto_total);

                        //ADJUNTAT LOS ELEMENTOS AL NODO FACTURA
                        resumenFactura.AppendChild(servGravados);
                        resumenFactura.AppendChild(servExento);
                        resumenFactura.AppendChild(mercanciaGravada);
                        resumenFactura.AppendChild(mercanciaExenta);
                        resumenFactura.AppendChild(impuestoResumen);
                        resumenFactura.AppendChild(descuentoResumen);
                        resumenFactura.AppendChild(montoTotalResumen);
                        root.AppendChild(resumenFactura);

                        string XML = xml.DocumentElement.OuterXml;


                        //------------------------------------------------------------------------------------------------------------------------
                        //Crear el pdf de la factura--------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------------------------------
                        MemoryStream ms = new MemoryStream();
                        FileContentResult FileFact = null;
                        if (emailForm!= null)
                        {
                            try
                            {
                               
                                PdfWriter writer = new PdfWriter(ms);
                                PdfDocument pdfDoc = new PdfDocument(writer);
                                Document doc = new Document(pdfDoc, PageSize.A4, false);

                                Paragraph header = new Paragraph("Factura Electrónica").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(20);

                                //Imagen de la empresa
                                Image logo = new Image(ImageDataFactory.Create("C:/logo1.png", false));
                                logo = logo.SetHeight(50).SetWidth(120);

                                Paragraph cadenanombre = new Paragraph("Atendido por: " + user.nombre).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK);
                                Paragraph header2 = new Paragraph("2ndo Piso, City Mall").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                                Paragraph header3 = new Paragraph("Alajuela, Costa Rica").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12);
                                Paragraph fecha = new Paragraph("Fecha de Compra: " + DateTime.Now.ToString()).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14).SetFontColor(ColorConstants.BLACK);
                                Paragraph header4 = new Paragraph("Cliente " + nombreForm).SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(12).SetFontColor(ColorConstants.BLACK);
                                Paragraph header5 = new Paragraph("-------------------------").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10).SetFontColor(ColorConstants.BLACK);

                                doc.Add(header);
                                doc.Add(fecha);
                                doc.Add(logo);
                                doc.Add(cadenanombre);
                                doc.Add(header2);
                                doc.Add(header3);
                                doc.Add(header4);

                                // Crear tabla con 4 columnas 
                                Table table = new Table(4, true);

                                table.AddHeaderCell("#");
                                table.AddHeaderCell("Descripción");
                                table.AddHeaderCell("Unidades");
                                table.AddHeaderCell("Precio Unidad");

                                int x = 0;
                                double subtotal = 0;
                                foreach (var item in listaLinea)
                                {
                                    x++;
                                    table.AddCell(new Paragraph(x.ToString()));
                                    IServiceArticulo servicio = new ServiceArticulo();
                                    Articulo arti = servicio.GetArticuloByID((int)item.idArticulo);
                                    table.AddCell(new Paragraph(arti.nombre));
                                    table.AddCell(new Paragraph(item.cantidad.ToString()));

                                    string precio = (String.Format("{0:N2}", item.precio));
                                    table.AddCell(new Paragraph("¢" + precio));
                                    subtotal += (double)item.precio;
                                }
                                doc.Add(table);


                                doc.Add(header5);



                                Table table2 = new Table(3, true);

                                table2.AddHeaderCell("Subtotal");
                                table2.AddHeaderCell("Impuesto IVA");
                                table2.AddHeaderCell("Total");

                                table2.AddCell(new Paragraph("¢" + Carrito.Instancia.tirarSubtotal()));
                                table2.AddCell(new Paragraph("¢" + Carrito.Instancia.tirarImpuesto()));
                                table2.AddCell(new Paragraph("¢" + Carrito.Instancia.tirarTotal()));

                                doc.Add(table2);

                                Paragraph header6 = new Paragraph("Gracias por comprar con VYCUZ!").SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(14);

                                doc.Add(header5);
                                doc.Add(header6);

                                doc.Close();

                               
                                FileFact = File(ms.ToArray(), "application/pdf", "FacturaElectrónica.pdf");

                            }
                            catch (Exception ex)
                            {
                                TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                            }
                        }


                        //-------------------------------------------------------------------------
                        //ENVIAR EL CORREO---------------------------------------------------------
                        //-------------------------------------------------------------------------

                        if (emailForm!= null)
                        {
                            string urlDomain = "https://localhost:3000/";
                            string EmailOrigen = "soportevycuz@gmail.com";
                            string Contraseña = "ecfykdmojjjlpfcn";
                            string url = urlDomain + "/Usuario/Recuperacion/?token=";
                            MailMessage oMailMessage = new MailMessage(EmailOrigen, emailForm, "Compra exitosa",
                                "<p>Estimado usuario,</br></br><hr/>Ha realizado una compra en VYCUZ.</p>");
                            oMailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(XML, "facturaElectronica.xml"));

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

                        Carrito.Instancia.eliminarCarrito();
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Venta generada!", "La venta se ha registrado en la base de datos!", SweetAlertMessageType.success);
                        return RedirectToAction("IndexCatalogo", "Articulo");
                    }
                    return RedirectToAction("IndexCatalogo","Articulo");
                }
            }
            catch (Exception ex)
            {
              
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;           
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult IndexVenta()
        {
            if (TempData["mensaje"] != null)
                ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            ViewBag.DetalleOrden = Carrito.Instancia.Items;
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult ordenarProducto(int? idArticulo)
        {
            if (idArticulo == 0)
            {
                idArticulo = Convert.ToInt32(TempData["idArticulo"]);
            }

            int cantidadLibros = Carrito.Instancia.Items.Count();
            TempData["mensaje"] = Carrito.Instancia.AgregarItem((int)idArticulo);            
         
            return RedirectToAction("IndexCatalogo", "Articulo");

        }

        public ActionResult actualizarCantidad(int idArticulo, int cantidad)
        {
            ViewBag.DetalleOrden = Carrito.Instancia.Items;
            TempData["NotiCarrito"] = Carrito.Instancia.SetItemCantidad(idArticulo, cantidad);
            TempData.Keep();
            return PartialView("Detalle", Carrito.Instancia.Items);

        }

        //Actualizar solo la cantidad de libros que se muestra en el menú
        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]


        //POR AQUI PUEDE ANDAR EL ERROR DE QUE SE ESCONDE EL CARRITO 
        public ActionResult actualizarOrdenCantidad()
        {
            if (TempData.ContainsKey("NotiCarrito"))
            {
                ViewBag.NotiCarrito = TempData["NotiCarrito"];
            }
            int cantidadLibros = Carrito.Instancia.Items.Count();
            return PartialView("Detalle");

        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult eliminarProducto(long? idArticulo)
        {
            try
            {
                ViewBag.NotificationMessage = Carrito.Instancia.EliminarItem((long)idArticulo);
                return PartialView("Detalle", Carrito.Instancia.Items);
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return PartialView("Detalle", Carrito.Instancia.Items);
        }
    }
}