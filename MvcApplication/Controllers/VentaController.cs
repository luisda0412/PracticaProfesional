using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Index()
        {
            IEnumerable<Venta> lista = null;
            try
            {
                IServiceVenta _ServiceVenta = new ServiceVenta();
                lista = _ServiceVenta.GetVentas();
            }
            catch (Exception e)
            {
                Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return View(lista);
        }

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
                    Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));
                    venta.nombre_cliente = user.nombre;
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
                        clave.InnerText = "1";
                        root.AppendChild(clave);
                        XmlNode codigoActividad = xml.CreateElement("Codigo_Actividad");
                        codigoActividad.InnerText = "1";
                        root.AppendChild(codigoActividad);
                        XmlNode fechaEmision = xml.CreateElement("Fecha_Emision");
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
                        nombreCliente.InnerText = user.nombre;
                        XmlNode ApellidoCliente = xml.CreateElement("Apellidos");
                        ApellidoCliente.InnerText = user.apellidos;
                        XmlNode correoCliente = xml.CreateElement("Email");
                        correoCliente.InnerText = user.correo_electronico;
                        XmlNode telefonoCliente = xml.CreateElement("Telefono");
                        telefonoCliente.InnerText = user.telefono;

                        //ADJUNTAS LOS ELEMENTOS AL NODO CLIENTE
                        nodoCliente.AppendChild(nombreCliente);
                        nodoCliente.AppendChild(ApellidoCliente);
                        nodoCliente.AppendChild(correoCliente);
                        nodoCliente.AppendChild(telefonoCliente);
                        root.AppendChild(nodoCliente);

                        string descuento2="";

                        foreach (var items in listaLinea)
                        {
                            Detalle_Venta linea = new Detalle_Venta();
                            linea.articulo_id = (int)items.idArticulo;
                            linea.cantidad = items.cantidad;
                            venta.impuesto = (double?)Carrito.Instancia.GetImpuesto();
                            venta.tipoventa= user.rol_id == 2 ? venta.tipoventa = true : venta.tipoventa = false;
                            venta.usuario_id = Convert.ToInt32(TempData["idUser"]);
                            venta.estado = true;
                            linea.venta_id = venta.id;
                            linea.descuento = 0;
                            linea.venta_id = venta.id;
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal() - linea.descuento;
                            linea.precio = items.precio;
                            venta.Detalle_Venta.Add(linea);

                            descuento2 = Convert.ToString(linea.descuento);
                            //LINEA DE CODGO PARA ACTUALIZAR EL PRODUCTO CUANDO SE HACE UNA COMPRA, LA COMENTO YA QUE HAY QUE HACER PRUEBAS Y LUEGO NOS QUEDAMOS SIN UNIDADES
                            //serviceArticulo.actualizarCantidad(linea.articulo_id, (int)linea.cantidad);

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

                        //venta.impuesto = (double)Carrito.Instancia.GetSubTotal();
                        //venta.monto_total = (double?)Carrito.Instancia.GetTotal() + ((double?)Carrito.Instancia.GetTotal() * venta.impuesto);
                        //CREAR EL XML

                        //SI TODO ESTA BIEN SE GUARA LA VENTA

                        IServiceVenta _ServiceVenta = new ServiceVenta();
                        Venta ven = _Serviceventa.Save(venta);

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
                        XmlNode impuestoResumen = xml.CreateElement("Impuesto");
                        impuestoResumen.InnerText = Convert.ToString(venta.impuesto);
                        XmlNode descuentoResumen = xml.CreateElement("Descuento");
                        descuentoResumen.InnerText = descuento2;
                        XmlNode montoTotalResumen = xml.CreateElement("Monto_Total");
                        montoTotalResumen.InnerText = Convert.ToString(venta.monto_total);

                        //ADJUNTAT LOS ELEMENTOS AL NODO FACTURA
                        resumenFactura.AppendChild(impuestoResumen);
                        resumenFactura.AppendChild(descuentoResumen);
                        resumenFactura.AppendChild(montoTotalResumen);
                        root.AppendChild(resumenFactura);

                        //ADJUNTAR ELEMENTOS AL NODO DE FACTURA ELECTRONICA
                        //root.AppendChild(nodoEmpresa);
                        //root.AppendChild(nodoCliente);
                        //root.AppendChild(nodoVenta);
                        //root.AppendChild(nodoDetalle);

                        //root.AppendChild(nodoVenta);


                        string XML = xml.DocumentElement.OuterXml;

                        //ENVIAR EL CORREO

                        string urlDomain = "https://localhost:3000/";
                        string EmailOrigen = "dumbmail130@gmail.com";
                        string Contraseña = "vhowlqsgdymyxyho";
                        string url = urlDomain + "/Usuario/Recuperacion/?token=";
                        MailMessage oMailMessage = new MailMessage(EmailOrigen, user.correo_electronico, "Compra exitosa",
                            "<p>Estimado usuario,</br></br><hr />Ha realizado una compra y la misma ha sido exitosa.</p>");
                        oMailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(XML, "ejemplo2.xml"));
                        oMailMessage.IsBodyHtml = true;

                        SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
                        oSmtpClient.EnableSsl = true;
                        oSmtpClient.UseDefaultCredentials = false;
                        oSmtpClient.Port = 587;
                        oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

                        oSmtpClient.Send(oMailMessage);

                        oSmtpClient.Dispose();

                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MethodBase.GetCurrentMethod());
                TempData["Message"] = "Error al procesar los datos! " + ex.Message;
                TempData["Redirect"] = "Factura";
                TempData["Redirect-Action"] = "IndexFactura";
                // Redireccion a la captura del Error
                return RedirectToAction("Default", "Error");
            }
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult IndexVenta()
        {
            if (TempData.ContainsKey("NotificationMessage"))
            {
                ViewBag.NotificationMessage = TempData["NotificationMessage"];
            }

            ViewBag.DetalleOrden = Carrito.Instancia.Items;
            return View();
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public ActionResult ordenarProducto(int? idArticulo)
        {
            idArticulo = Convert.ToInt32(TempData["idArticulo"]);
            int cantidadLibros = Carrito.Instancia.Items.Count();
            ViewBag.NotiCarrito = Carrito.Instancia.AgregarItem((int)idArticulo);

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

        //private void SendXmlByEmail(string EmailDestino)
        //{
        //    //CREAR EL XML

        //    XmlDocument xml = new XmlDocument();
        //    XmlNode root = xml.CreateElement("Libros");
        //    xml.AppendChild(root);

        //    XmlNode nodoLibro = xml.CreateElement("Libro");
        //    XmlAttribute atributo = xml.CreateAttribute("Autor");
        //    atributo.Value = "Michael Ende";
        //    nodoLibro.Attributes.Append(atributo);
        //    XmlNode nodoTitulo = xml.CreateElement("Titulo");
        //    nodoTitulo.InnerText = "La historia interminable";

        //    XmlNode nodoPaginas = xml.CreateElement("Paginas");
        //    nodoTitulo.InnerText = "234";

        //    nodoLibro.AppendChild(nodoTitulo);
        //    nodoLibro.AppendChild(nodoPaginas);

        //    root.AppendChild(nodoLibro);

        //    string XML = xml.DocumentElement.OuterXml;

        //    //xml.Save(@"D:\Universidad\Z-Otros\ejemplo.xml");

        //    //ENVIAR EL CORREO

        //    string urlDomain = "https://localhost:3000/";
        //    string EmailOrigen = "dumbmail130@gmail.com";
        //    string Contraseña = "vhowlqsgdymyxyho";
        //    string url = urlDomain + "/Usuario/Recuperacion/?token=";
        //    MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Compra exitosa",
        //        "<p>Estimado usuario,</br></br><hr />Ha realizado una compra y la misma ha sido exitosa.</p>");
        //    oMailMessage.Attachments.Add(Attachment.CreateAttachmentFromString(XML, "ejemplo2.xml"));
        //    oMailMessage.IsBodyHtml = true;

        //    SmtpClient oSmtpClient = new SmtpClient("smtp.gmail.com");
        //    oSmtpClient.EnableSsl = true;
        //    oSmtpClient.UseDefaultCredentials = false;
        //    oSmtpClient.Port = 587;
        //    oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

        //    oSmtpClient.Send(oMailMessage);

        //    oSmtpClient.Dispose();
        //}
    }
}