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
                        

                        foreach (var items in listaLinea)
                        {
                            Detalle_Venta linea = new Detalle_Venta();
                            linea.articulo_id = (int)items.idArticulo;
                            linea.cantidad = items.cantidad;
                            venta.impuesto = 0.13;
                            venta.tipoventa= user.rol_id == 2 ? venta.tipoventa = true : venta.tipoventa = false;
                            venta.usuario_id = Convert.ToInt32(TempData["idUser"]);
                            venta.estado = true;
                            linea.venta_id = venta.id;
                            linea.descuento = 0;
                            linea.venta_id = venta.id;
                            venta.monto_total = (double?)Carrito.Instancia.GetTotal() + ((double?)Carrito.Instancia.GetTotal() * venta.impuesto)- linea.descuento;
                            linea.precio = venta.monto_total;
                            venta.Detalle_Venta.Add(linea);
                        }

                        //CREAR EL XML

                        XmlDocument xml = new XmlDocument();
                        XmlNode root = xml.CreateElement("Venta");
                        xml.AppendChild(root);

                        XmlNode nodoVenta = xml.CreateElement("Venta");
                        //nodoVenta.Attributes.Append(atributo);

                        XmlNode nodoCliente = xml.CreateElement("Cliente");
                        nodoCliente.InnerText = user.nombre;
                        nodoCliente.InnerText = user.apellidos;
                        nodoCliente.InnerText = user.correo_electronico;
                        nodoCliente.InnerText = user.telefono;

                        XmlNode nodoMontoTotal = xml.CreateElement("Monto_Total");
                        nodoMontoTotal.InnerText = Convert.ToString(venta.monto_total);

                        XmlNode nodoImpuesto = xml.CreateElement("Impuesto");
                        nodoImpuesto.InnerText = Convert.ToString(venta.impuesto);

                        nodoVenta.AppendChild(nodoCliente);
                        nodoVenta.AppendChild(nodoMontoTotal);
                        nodoVenta.AppendChild(nodoImpuesto);

                        root.AppendChild(nodoVenta);

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

                        //SI TODO ESTA BIEN SE GUARA LA VENTA

                        IServiceVenta _ServiceVenta = new ServiceVenta();
                        Venta ven = _Serviceventa.Save(venta);
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
            return PartialView("MovimientoCantidad");

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