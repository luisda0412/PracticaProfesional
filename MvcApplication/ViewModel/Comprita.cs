using MvcApplication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication.ViewModel
{
    public class Comprita
    {

        public List<ViewModelDetalleIngreso> Items { get; private set; }

        //Implementación Singleton

        // Las propiedades de solo lectura solo se pueden establecer en la inicialización o en un constructor
        public static readonly Comprita Instancia;

        // Se llama al constructor estático tan pronto como la clase se carga en la memoria
        static Comprita()
        {
            // Si el carrito no está en la sesión, cree uno y guarde los items.
            if (HttpContext.Current.Session["carrito"] == null)
            {
                Instancia = new Comprita();
                Instancia.Items = new List<ViewModelDetalleIngreso>();
                HttpContext.Current.Session["carrito"] = Instancia;
            }
            else
            {
                // De lo contrario, obténgalo de la sesión.
                Instancia = (Comprita)HttpContext.Current.Session["carrito"];
            }
        }

        // Un constructor protegido asegura que un objeto no se puede crear desde el exterior
        protected Comprita() { }

        /**
         * AgregarItem (): agrega un artículo a la compra
         */
        public String AgregarItem(int productoID)
        {
            String mensaje = "";
            // Crear un nuevo artículo para agregar al carrito
            ViewModelDetalleIngreso nuevoItem = new ViewModelDetalleIngreso(productoID);
            // Si este artículo ya existe en lista de libros, aumente la Cantidad
            // De lo contrario, agregue el nuevo elemento a la lista
            if (nuevoItem != null)
            {
                if (Items.Exists(x => x.idArticulo == productoID))

                {
                    ViewModelDetalleIngreso item = Items.Find(x => x.idArticulo == productoID);
                    item.cantidad++;
                }
                else
                {
                    nuevoItem.cantidad = 1;
                    Items.Add(nuevoItem);
                }
                mensaje = SweetAlertHelper.Mensaje("Nuevo Ingreso", "Artículo agregado a la compra", SweetAlertMessageType.success);

            }
            else
            {
                mensaje = SweetAlertHelper.Mensaje("Nueva Venta", "El artículo solicitado no se encuentra en stock", SweetAlertMessageType.warning);
            }
            return mensaje;
        }


        /**
         * SetItemCantidad(): cambia la Cantidad de un artículo en el carrito
         */
        public String SetItemCantidad(int productoID, int Cantidad)
        {
            String mensaje = "";
            // Si estamos configurando la Cantidad a 0, elimine el artículo por completo
            if (Cantidad == 0)
            {
                EliminarItem(productoID);
                mensaje = SweetAlertHelper.Mensaje("Orden Compra", "Artículo eliminado", SweetAlertMessageType.success);

            }
            else
            {
                // Encuentra el artículo y actualiza la Cantidad
                ViewModelDetalleIngreso actualizarItem = new ViewModelDetalleIngreso(productoID);
                if (Items.Exists(x => x.idArticulo == productoID))
                {
                    ViewModelDetalleIngreso item = Items.Find(x => x.idArticulo == productoID);
                    item.cantidad = Cantidad;
                    mensaje = SweetAlertHelper.Mensaje("Orden Compra", "Cantidad actualizada", SweetAlertMessageType.success);

                }
            }
            return mensaje;

        }

        /**
         * EliminarItem (): elimina un artículo del carrito de compras
         */
        public String EliminarItem(long productoID)
        {
            String mensaje = "El libro no existe";
            if (Items.Exists(x => x.idArticulo == productoID))
            {
                var itemEliminar = Items.Single(x => x.idArticulo == productoID);
                Items.Remove(itemEliminar);
                mensaje = SweetAlertHelper.Mensaje("Orden Compra", "Artículo eliminado", SweetAlertMessageType.success);
            }
            return mensaje;

        }

        /**
         * GetTotal() - Devuelve el precio total de todos los libros.
         */
        public decimal GetTotal()
        {
            decimal total = 0;
            total = GetSubTotal() + GetImpuesto();

            return total;
        }
        public int GetCountItems()
        {
            int total = 0;
            total = (int)Items.Sum(x => x.cantidad);

            return total;
        }

        public long GetTotalLinea()
        {
            long total = 0;

            foreach (var i in Items)
            {
                total = total + (long)(i.precio * i.cantidad);
            }

            return total;
        }


        /*Saca el subtotal de la venta*/
        public decimal GetSubTotal()
        {
            decimal total = 0;
            total = GetTotalLinea();
            return total;
        }

        /*Sacar el impuesto total de la venta*/
        public decimal GetImpuesto()
        {
            long impuesto = 0;

            foreach (var i in Items)
            {
                impuesto = impuesto + (long)((0.13 * i.articulo.precio) * i.cantidad);
            }

            return impuesto;
        }

        public void eliminarCarrito()
        {
            Items.Clear();

        }
    }
}