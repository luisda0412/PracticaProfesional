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

    [MetadataType(typeof(FacturaMetadata))]
    public partial class Facturas
    {
        public int id { get; set; }
        public Nullable<int> venta_id { get; set; }
        public Nullable<int> empresa_id { get; set; }
        public Nullable<bool> tipoFactura { get; set; }
    
        public virtual Empresa Empresa { get; set; }
        public virtual Venta Venta { get; set; }
    }
}
