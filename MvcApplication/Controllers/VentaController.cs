﻿using AplicationCore.Services;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
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
        public JsonResult ActualizarCheckBox(bool isChecked)
        {
            //Actualiza la variable de sesión con el estado actual del CheckBox
            Session["Facturar"] = isChecked;

            //Devuelve el nuevo estado del CheckBox al cliente
            return Json(isChecked, JsonRequestBehavior.AllowGet);
        }
        public static string idForm { get; set; }
        public static string nombreForm { get; set; }
        public static string apellidosForm { get; set; }
        public static string emailForm { get; set; }
        public static string telefonoForm { get; set; }
        public ActionResult obtenerDatosForm()
        {
            //Obtiene los datos del formulario y los guarda en variables
            idForm = Request.Form["id"];
            nombreForm = Request.Form["name"];
            apellidosForm = Request.Form["lastname"];
            emailForm = Request.Form["email"];
            telefonoForm = Request.Form["phone"];

            //Actualiza la variable de sesión con el resultado del formulario
             Session["Facturar"] = true;

            //Redirige al usuario a la página de venta
            return RedirectToAction("IndexVenta");
        }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]
        public ActionResult Save(Venta venta)
        {
            //Mostrar Mensaje de abrir la caja si esta cerrada
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja cajita = new Arqueos_Caja();
            cajita = _ServiceCaja.GetArqueoLast();
            IServiceUsuario serviceUsuario = new ServiceUsuario();
            try
            {
                if (Carrito.Instancia.Items.Count() <= 0)//Valida si hay articulos
                {
                    TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                           ("Carrito vacío", "No se puede crear una venta sin artículos, por favor verifíque!", SweetAlertMessageType.warning);
                    return RedirectToAction("IndexVenta");
                }
                else
                {
                    if (cajita.estado == false)//Valida si la caja chica esta cerrada
                    {
                        TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                            ("Problema en Venta", "No se pueden registrar ventas con la caja chica cerrada, por favor verifíque!", SweetAlertMessageType.warning);
                        return RedirectToAction("IndexVenta");
                    }
                    else
                    {
                        Caja_Chica cajaChica = new Caja_Chica();
                        Facturas factura = new Facturas();
                        Usuario user = serviceUsuario.GetUsuarioByID(Convert.ToInt32(TempData["idUser"]));

                        if (Convert.ToString(Request.Form["entrada"]).Trim().Length !=0)//Valida que el ingreso de plata no este vacío
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
                            XmlNode codigoActividad = xml.CreateElement("CodigoActividad");
                            codigoActividad.InnerText = "523601";
                            root.AppendChild(codigoActividad);

                            XmlNode clave = xml.CreateElement("Clave");
                            clave.InnerText = "";
                            root.AppendChild(clave);

                            XmlNode numeroConsecutivo = xml.CreateElement("NumeroConsecutivo");
                            numeroConsecutivo.InnerText = "";
                            root.AppendChild(numeroConsecutivo);

                            XmlNode fechaEmision = xml.CreateElement("FechaEmision");
                            string formatted = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssK");
                            fechaEmision.InnerText = formatted;
                            root.AppendChild(fechaEmision);


                            //CREACION DEL NODO DE INFORMACION DE LA EMPRESA
                            XmlNode nodoEmpresa = xml.CreateElement("Emisor");

                            //ASIGNACION DE ELEMENTOS AL NODO DE LA EMPRESA
                            XmlNode nombreEmpresa = xml.CreateElement("Nombre");
                            nombreEmpresa.InnerText = "VYCUZ";

                            XmlNode identificacionEmisor = xml.CreateElement("Identificacion");
                            XmlNode tipo = xml.CreateElement("Tipo");
                            tipo.InnerText = "01";
                            XmlNode numeroIdentificacioEmisor = xml.CreateElement("Numero");
                            numeroIdentificacioEmisor.InnerText = Convert.ToString(200130632);

                            identificacionEmisor.AppendChild(tipo);
                            identificacionEmisor.AppendChild(numeroIdentificacioEmisor);

                            XmlNode nombreComercial = xml.CreateElement("NombreComercial");
                            nombreComercial.InnerText = "VYCUZ";

                            //CREACION DEL NODO DE UBICACION DENTRO DEL NODO DE LA EMRPESA
                            XmlNode nodoUbicacion = xml.CreateElement("Ubicacion");

                            //ASIGNACION DE LOS ELEMENTOS AL NODO DE UBICACION
                            XmlNode provincia = xml.CreateElement("Provincia");
                            provincia.InnerText = "2";
                            XmlNode canton = xml.CreateElement("Canton");
                            canton.InnerText = "01";
                            XmlNode distrito = xml.CreateElement("Distrito");
                            distrito.InnerText = "01";
                            XmlNode otrasSenas = xml.CreateElement("OtrasSenas");
                            otrasSenas.InnerText = "Centro Comercial City Mall segundo piso";

                            //ASGNACION DE LOS ELEMENTOS DENTRO DEL NODO DE UBICACION
                            nodoUbicacion.AppendChild(provincia);
                            nodoUbicacion.AppendChild(canton);
                            nodoUbicacion.AppendChild(distrito);
                            nodoUbicacion.AppendChild(otrasSenas);

                            //CREACION DEL NODO DE TELEFONO
                            XmlNode telefonoEmpresa = xml.CreateElement("Telefono");

                            //ASIGNACION DE ELEMENTOS AL NODO DE TELEFONO
                            XmlNode codigoPais = xml.CreateElement("CodigoPais");
                            codigoPais.InnerText = "506";
                            XmlNode numTelefono = xml.CreateElement("NumTelefono");
                            numTelefono.InnerText = "72791408";

                            //ADJUNTAR ELEMENTOS AL N0DO DE TELEFONO
                            telefonoEmpresa.AppendChild(codigoPais);
                            telefonoEmpresa.AppendChild(numTelefono);

                            XmlNode correoEmpresa = xml.CreateElement("CorreoElectronico");
                            correoEmpresa.InnerText = "vycuz@gmail.com";

                            //ADJUNTAR ELEMENTOS AL NODO DE LA EMPRESA
                            nodoEmpresa.AppendChild(nombreEmpresa);
                            nodoEmpresa.AppendChild(identificacionEmisor);
                            nodoEmpresa.AppendChild(nombreComercial);
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

                            XmlNode identificacionReceptor = xml.CreateElement("Identificacion");
                            XmlNode tipoReceptor = xml.CreateElement("Tipo");
                            tipoReceptor.InnerText = "01";
                            XmlNode numeroIdentificacioReceptor = xml.CreateElement("Numero");
                            numeroIdentificacioReceptor.InnerText = Convert.ToString(idForm);

                            identificacionReceptor.AppendChild(tipoReceptor);
                            identificacionReceptor.AppendChild(numeroIdentificacioReceptor);


                            XmlNode identificacionExtanjero = xml.CreateElement("IdentificacionExtranjero");
                            identificacionExtanjero.InnerText = "";

                            XmlNode correoCliente = xml.CreateElement("CorreoElectronico");
                            correoCliente.InnerText = emailForm;

                            XmlNode telefonoCliente = xml.CreateElement("Telefono");

                            XmlNode codigoTelefonoReceptor = xml.CreateElement("CodigoPais");
                            codigoTelefonoReceptor.InnerText = "506";
                            XmlNode numTelefonoReceptor = xml.CreateElement("NumTelefono");
                            numTelefonoReceptor.InnerText = telefonoForm;

                            telefonoCliente.AppendChild(codigoTelefonoReceptor);
                            telefonoCliente.AppendChild(numTelefonoReceptor);

                            //ADJUNTAS LOS ELEMENTOS AL NODO CLIENTE
                            nodoCliente.AppendChild(nombreCliente);
                            nodoCliente.AppendChild(ApellidoCliente);
                            nodoCliente.AppendChild(identificacionReceptor);
                            nodoCliente.AppendChild(identificacionExtanjero);
                            nodoCliente.AppendChild(telefonoCliente);
                            nodoCliente.AppendChild(correoCliente);
                            root.AppendChild(nodoCliente);

                            //MAS INFORMACION QUE SE AGREGA AL NODO PRINCIPAL
                            XmlNode condicionVenta = xml.CreateElement("CondicionVenta");
                            condicionVenta.InnerText = "01";
                            XmlNode medioPago = xml.CreateElement("MedioPago");
                            medioPago.InnerText = venta.tipopago == true ? "01" : "02";

                            root.AppendChild(condicionVenta);
                            root.AppendChild(medioPago);

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
                                //LINEA DE CODGO PARA ACTUALIZAR EL PRODUCTO CUANDO SE HACE UNA COMPRA, LA COMENTO YA QUE HAY QUE HACER PRUEBAS Y LUEGO NOS QUEDAMOS SIN UNIDADES
                                serviceArticulo.actualizarCantidad(linea.articulo_id, (int)linea.cantidad, false);

                                //CREACION DEL NODO DETALLE VENTA
                                XmlNode detalleServicio = xml.CreateElement("DetalleServicio");

                                //ASIGNACION DE ELEMENTOS DEL NODO DE DETALLE VENTA
                                XmlNode lineaDetalle = xml.CreateElement("LineaDetalle");

                                XmlNode numeroLinea = xml.CreateElement("NumeroLinea");
                                numeroLinea.InnerText = Convert.ToString(contador);

                                XmlNode codigoProductoLinea = xml.CreateElement("Codigo");
                                codigoProductoLinea.InnerText = Convert.ToString(4529000000000);

                                XmlNode cantidad = xml.CreateElement("Cantidad");
                                double cantidadDouble = (double)linea.cantidad;
                                cantidad.InnerText = cantidadDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode unidadMedida = xml.CreateElement("UnidadMedida");
                                unidadMedida.InnerText = "Unid";

                                XmlNode detalle = xml.CreateElement("Detalle");
                                detalle.InnerText = items.articulo.nombre;

                                XmlNode precioUnitario = xml.CreateElement("PrecioUnitario");
                                double precioUnitarioDouble = (double)linea.precio;
                                precioUnitario.InnerText = precioUnitarioDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode montoTotalDetalle = xml.CreateElement("MontoTotal");
                                double montoTotalDetalleDouble = (double)(venta.monto_total - venta.impuesto);
                                montoTotalDetalle.InnerText = montoTotalDetalleDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode subTotal = xml.CreateElement("SubTotal");
                                double subTotalDouble = (double)(venta.monto_total - venta.impuesto);
                                subTotal.InnerText = subTotalDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode impuestoDetalle = xml.CreateElement("Impuesto");

                                XmlNode codigoImpuesto = xml.CreateElement("Codigo");
                                codigoImpuesto.InnerText = "01";

                                XmlNode codigoTarifa = xml.CreateElement("CodigoTarifa");
                                codigoTarifa.InnerText = "08";

                                XmlNode tarifa = xml.CreateElement("Tarifa");
                                double tarifaDouble = 13.00;
                                tarifa.InnerText = tarifaDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode montoImpuesto = xml.CreateElement("Monto");
                                double montoImpuestoDouble = (double)venta.impuesto;
                                montoImpuesto.InnerText = montoImpuestoDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                impuestoDetalle.AppendChild(codigoImpuesto);
                                impuestoDetalle.AppendChild(codigoTarifa);
                                impuestoDetalle.AppendChild(tarifa);
                                impuestoDetalle.AppendChild(montoImpuesto);


                                XmlNode impuestoNeto = xml.CreateElement("ImpuestoNeto");
                                double impuestoNetoDouble = (double)venta.impuesto;
                                impuestoNeto.InnerText = impuestoNetoDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode montoTotalLinea = xml.CreateElement("MontoTotalLinea");
                                double montoTotalLineaDouble = (double)venta.monto_total;
                                montoTotalLinea.InnerText = montoTotalLineaDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));


                                //ADJUNTAR LOS ELEMENTOS AL NODO DETALLE VENTA
                                lineaDetalle.AppendChild(numeroLinea);
                                lineaDetalle.AppendChild(codigoProductoLinea);
                                lineaDetalle.AppendChild(cantidad);
                                lineaDetalle.AppendChild(unidadMedida);
                                lineaDetalle.AppendChild(detalle);
                                lineaDetalle.AppendChild(precioUnitario);
                                lineaDetalle.AppendChild(montoTotalDetalle);
                                lineaDetalle.AppendChild(subTotal);
                                lineaDetalle.AppendChild(impuestoDetalle);
                                lineaDetalle.AppendChild(impuestoNeto);
                                lineaDetalle.AppendChild(montoTotalLinea);

                                detalleServicio.AppendChild(lineaDetalle);

                                root.AppendChild(detalleServicio);
                            }
                               if (Convert.ToDouble(Request.Form["entrada"]) < venta.monto_total)
                               {
                               TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                               ("Problema en Venta", "El monto de entrada es menor al precio total de la venta por favor verifíque!", SweetAlertMessageType.warning);
                               return RedirectToAction("IndexVenta");
                               }
                               else
                               {
                        
                                //SI TODO ESTA BIEN SE GUARDA LA VENTA

                                IServiceVenta _ServiceVenta = new ServiceVenta();
                                Venta ven = _Serviceventa.Save(venta);
                            

                                //FACTURA
                                factura.venta_id = venta.id;
                                factura.empresa_id = 1;
                                factura.tipoFactura = venta.tipopago;


                                //LLENAR DATOS DE CAJA CHICA
                                IServiceCajaChica servicio = new ServiceCajaChica();
                                Caja_Chica ultimacaja = new Caja_Chica();
                                ultimacaja = servicio.GetCajaChicaLast();

                                cajaChica.fecha = DateTime.Now;
                                cajaChica.entrada = Convert.ToDouble(Request.Form["entrada"]);
                                cajaChica.salida = cajaChica.entrada - (double?)Carrito.Instancia.GetTotal();

                                saldoActual += (double)cajaChica.entrada - (double)cajaChica.salida;
                                saldoActual += (double)ultimacaja.saldo;
                                cajaChica.saldo = saldoActual;

                                IServiceCajaChica caja = new ServiceCajaChica();
                                caja.Save(cajaChica);

                                saldoActual = 0;

                                IServiceFactura serviceFactura = new ServiceFactura();
                                Facturas fac = serviceFactura.Save(factura);

                                //CREACION DEL NODO RESUMEN FACTURA
                                XmlNode resumenFactura = xml.CreateElement("ResumenFactura");

                                XmlNode codigoTipoMoneda = xml.CreateElement("CodigoTipoMoneda");

                                XmlNode codigoMoneda = xml.CreateElement("CodigoMoneda");
                                codigoMoneda.InnerText = "CRC";

                                XmlNode tipoCambio = xml.CreateElement("TipoCambio");
                                double tipoCambioDouble = 1.00;
                                tipoCambio.InnerText = tipoCambioDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                codigoTipoMoneda.AppendChild(codigoMoneda);
                                codigoTipoMoneda.AppendChild(tipoCambio);


                                XmlNode totalServGravados = xml.CreateElement("TotalSerGravados");
                                double totalServGravadosDouble = 0.00;
                                totalServGravados.InnerText = totalServGravadosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalServExentos = xml.CreateElement("TotalSerExentos");
                                double totalServExentosDouble = 0.00;
                                totalServExentos.InnerText = totalServExentosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalServExonerados = xml.CreateElement("TotalServExonerados");
                                double totalServExoneradosDouble = 0.00;
                                totalServExonerados.InnerText = totalServExoneradosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalMercanciasGravadas = xml.CreateElement("TotalMercanciasGravadas");
                                double totalMercanciasGravadasDouble = (double)venta.monto_total;
                                totalMercanciasGravadas.InnerText = totalMercanciasGravadasDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalMercanciasExentas = xml.CreateElement("TotalMercanciasExentas");
                                double totalMercanciasExentasDouble = 0.00;
                                totalMercanciasExentas.InnerText = totalMercanciasExentasDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalMercExonerada = xml.CreateElement("TotalMercExonerada");
                                double totalMercExoneradaDouble = 0.00;
                                totalMercExonerada.InnerText = totalMercExoneradaDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalGravados = xml.CreateElement("TotalGravados");
                                double totalGravadosDouble = (double)(venta.monto_total - venta.impuesto);
                                totalGravados.InnerText = totalGravadosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalExento = xml.CreateElement("TotalExento");
                                double totalExentoDouble = 0.00;
                                totalExento.InnerText = totalExentoDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalExonerado = xml.CreateElement("TotalExonerado");
                                double totalExoneradoDouble = 0.00;
                                totalExonerado.InnerText = totalExoneradoDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalVenta = xml.CreateElement("TotalVenta");
                                double totalVentaDouble = (double)(venta.monto_total - venta.impuesto);
                                totalVenta.InnerText = totalVentaDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalDescuentos = xml.CreateElement("TotalDescuentos");
                                double totalDescuentosDouble = Convert.ToDouble(descuento2);
                                totalDescuentos.InnerText = totalDescuentosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalVentaNeta = xml.CreateElement("TotalVentaNeta");
                                double totalVentaNetaDouble = (double)(venta.monto_total - venta.impuesto);
                                totalVentaNeta.InnerText = totalVentaNetaDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalImpuesto = xml.CreateElement("TotalImpuesto");
                                double totalImpuestoDouble = (double)venta.impuesto;
                                totalImpuesto.InnerText = totalImpuestoDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalOtrosCargos = xml.CreateElement("TotalOtrosCargos");
                                double totalOtrosCargosDouble = 0.00;
                                totalOtrosCargos.InnerText = totalOtrosCargosDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                XmlNode totalComprobante = xml.CreateElement("TotalComprobante");
                                double totalComprobanteDouble = (double)venta.monto_total;
                                totalComprobante.InnerText = totalComprobanteDouble.ToString("0.00", CultureInfo.GetCultureInfo("en-US"));

                                //ADJUNTAT LOS ELEMENTOS AL NODO RESUMEN FACTURA
                                resumenFactura.AppendChild(codigoTipoMoneda);
                                resumenFactura.AppendChild(totalServGravados);
                                resumenFactura.AppendChild(totalServExentos);
                                resumenFactura.AppendChild(totalServExonerados);
                                resumenFactura.AppendChild(totalMercanciasGravadas);
                                resumenFactura.AppendChild(totalMercanciasExentas);
                                resumenFactura.AppendChild(totalMercExonerada);
                                resumenFactura.AppendChild(totalGravados);
                                resumenFactura.AppendChild(totalExento);
                                resumenFactura.AppendChild(totalExonerado);
                                resumenFactura.AppendChild(totalVenta);
                                resumenFactura.AppendChild(totalDescuentos);
                                resumenFactura.AppendChild(totalVentaNeta);
                                resumenFactura.AppendChild(totalImpuesto);
                                resumenFactura.AppendChild(totalOtrosCargos);
                                resumenFactura.AppendChild(totalComprobante);
                                root.AppendChild(resumenFactura);

                                string XML = xml.DocumentElement.OuterXml;


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
                                    Paragraph cadenanombre = new Paragraph("Atendido por: " + user.nombre)
                                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                        .SetFontSize(12)
                                        .SetFontColor(ColorConstants.BLACK)
                                        .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro

                                    doc.Add(logo);
                                    doc.Add(cadenanombre);

                                    // Información de la empresa
                                    Paragraph info1 = new Paragraph("Lugar: City Mall")
                                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                        .SetFontSize(10)
                                        .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro
                                    Paragraph info2 = new Paragraph("Ubicación: Montserrat, Alajuela, Costa Rica")
                                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                        .SetFontSize(10)
                                        .SetTextAlignment(TextAlignment.CENTER); // Alinea el contenido al centro

                                    Paragraph info9 = new Paragraph("Número de Factura: #" + factura.id)
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
                                    Paragraph info3 = new Paragraph("Cliente: " + venta.nombre_cliente)
                                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                                        .SetFontSize(10)
                                        .SetFontColor(ColorConstants.BLACK);

                                    doc.Add(info3);

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
                                        string precio = (String.Format("{0:N2}", (item.cantidad * item.precio)));
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

                                    Cell cell2 = new Cell().Add(new Paragraph("¢" + Carrito.Instancia.tirarSubtotal()));
                                    cell2.SetBorder(Border.NO_BORDER);
                                    cell2.SetTextAlignment(TextAlignment.RIGHT);
                                    table2.AddCell(cell2);

                                    Cell cell3 = new Cell().Add(new Paragraph("Impuesto IVA:"));
                                    cell3.SetBorder(Border.NO_BORDER);
                                    cell3.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                                    table2.AddCell(cell3);

                                    Cell cell4 = new Cell().Add(new Paragraph("¢" + Carrito.Instancia.tirarImpuesto()));
                                    cell4.SetBorder(Border.NO_BORDER);
                                    cell4.SetTextAlignment(TextAlignment.RIGHT);
                                    table2.AddCell(cell4);

                                    Cell cell5 = new Cell().Add(new Paragraph("Total:"));
                                    cell5.SetBorder(Border.NO_BORDER);
                                    cell5.SetTextAlignment(TextAlignment.RIGHT); // Alineación de la celda a la derecha
                                    table2.AddCell(cell5);

                                    Cell cell6 = new Cell().Add(new Paragraph("¢" + Carrito.Instancia.tirarTotal()));
                                    cell6.SetBorder(Border.NO_BORDER);
                                    cell6.SetTextAlignment(TextAlignment.RIGHT);
                                    table2.AddCell(cell6);

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

                                    Paragraph footer = new Paragraph("¡Gracias por su compra!")
                                        .SetTextAlignment(TextAlignment.CENTER)
                                        .SetFontSize(17);

                                    doc.Add(footer);

                                    doc.Close();



                                    FileFact = File(ms.ToArray(), "application/pdf", "Ticket Electrónico.pdf");

                                }
                                catch (Exception ex)
                                {
                                    TempData["Mensaje"] = "Error al procesar los datos! " + ex.Message;
                                }

                                //-------------------------------------------------------------------------
                                //ENVIAR EL CORREO---------------------------------------------------------
                                //-------------------------------------------------------------------------

                                if (emailForm != null)
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
                                //Actualiza la variable de sesión con el resultado del formulario
                                Session["Facturar"] = false;
                                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Venta generada!", "La venta se ha registrado en la base de datos!", SweetAlertMessageType.success);
                                TempData["archivo"] = FileFact;
                                return RedirectToAction("IndexCatalogo", "Articulo");
                            }
                        }
                        else
                        {
                            TempData["mensaje"] = Util.SweetAlertHelper.Mensaje
                           ("Monto de Entrada Vacío", "Ingrese el monto de pago del cliente!", SweetAlertMessageType.warning);
                            return RedirectToAction("IndexVenta");
                        }
                    }
                }
            }
            catch (Exception ex)
            {            
                TempData["mensaje"] = "Error al procesar los datos! " + ex.Message;           
                return RedirectToAction("Default", "Error");
            }
        }

        public static string hilera { get; set; }

        [CustomAuthorize((int)Roles.Administrador, (int)Roles.Procesos)]

        public async Task<ActionResult> IndexVenta()
        {
            //Mostrar Mensaje de abrir la caja si esta cerrada
            IServiceCajaChica _ServiceCaja = new ServiceCajaChica();
            Arqueos_Caja caja = new Arqueos_Caja();
            caja = _ServiceCaja.GetArqueoLast();
            if (caja.estado == false)
            {
                TempData["mensaje"] = Util.SweetAlertHelper.Mensaje("Mensaje importante!", "La caja chica se encuentra cerrada, antes de registrar la venta diríjase a la página de arqueos y abra la caja.", SweetAlertMessageType.info);
            }

            if (TempData["mensaje"] != null)
            ViewBag.NotificationMessage = TempData["mensaje"].ToString();
            ViewBag.DetalleOrden = Carrito.Instancia.Items;

            var rate = await GetExchangeRate();
            ViewBag.ExchangeRate = rate;
            
            //Obtiene el estado actual del CheckBox desde la variable de sesión
            bool facturar = (bool)(Session["Facturar"] ?? false);

            //Asigna el valor de la variable al ViewBag
            ViewBag.Facturar = facturar;

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
                Infraestructure.Util.Log.Error(e, MethodBase.GetCurrentMethod());
            }
            return PartialView("Detalle", Carrito.Instancia.Items);
        }

        public async Task<decimal> GetExchangeRate()
        {
            // URL del API que proporciona el tipo de cambio de colón costarricense a dólar estadounidense
            var apiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

            // Crear un cliente HTTP
            using (var httpClient = new HttpClient())
            {
                // Realizar una solicitud GET al API
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    // Si la solicitud fue exitosa, obtener el tipo de cambio
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var json = JsonConvert.DeserializeObject<JObject>(result);
                        var rate = (decimal)json["rates"]["CRC"];
                        return rate;
                    }
                }
            }

            // Si hubo algún problema, retornar un valor predeterminado
            return 0m;
        }
    }
}