﻿using AplicationCore.Services;
using Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.ViewModel
{
    public class ViewModelDetalleCotizacion
    {

        public long idDetalle { get; set; }
        public long idArticulo { get; set; }

        public Nullable<int> cantidad { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        public Nullable<double> precio
        {
            get { return articulo.precio; }
        }

        public virtual Detalle_Ingreso detalleIngreso { get; set; }
        public virtual Articulo articulo { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
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


        public ViewModelDetalleCotizacion(int IdArticulo)
        {
            IServiceArticulo _ServiceArticulo = new ServiceArticulo();
            this.idArticulo = IdArticulo;
            this.articulo = _ServiceArticulo.GetArticuloByID(IdArticulo);
        }
    }
}