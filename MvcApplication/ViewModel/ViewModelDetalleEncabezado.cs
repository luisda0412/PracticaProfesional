using AplicationCore.Services;
using Infraestructure.Models;
using System;

namespace Web.ViewModel
{
    public class ViewModelDetalleEncabezado
    {
        public long idDetalle { get; set; }
        public long idArticulo{ get; set; }

        public Nullable<int> cantidad { get; set; }
        public Nullable<double> precio
        {
            get { return articulo.precio; }
        }

        public virtual Detalle_Venta detalleVenta { get; set; }
        public virtual Articulo articulo { get; set; }
        public long SubTotal
        {
            get
            {
                return calculoSubtotal();
            }
        }
        private long calculoSubtotal()
        {
            return (long)(this.precio * this.cantidad);
        }


        public ViewModelDetalleEncabezado(int IdArticulo)
        {
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();
            this.idArticulo = IdArticulo;
            this.articulo = _ServiceArticulo.GetArticuloByID(IdArticulo);
        }
    }
}