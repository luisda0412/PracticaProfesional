//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infraestructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(NotasMetadata))]
    public partial class NotasDeCreditoYDebito
    {
        public int id { get; set; }
        public Nullable<int> idFactura { get; set; }
        public string nombreCliente { get; set; }
        public string motivo { get; set; }
        public Nullable<double> monto { get; set; }
        public Nullable<bool> tipoNota { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<bool> estado { get; set; }
    
        public virtual Facturas Facturas { get; set; }
    }
}
