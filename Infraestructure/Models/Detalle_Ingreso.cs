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

    [MetadataType(typeof(Detalle_IngresoMetadata))]
    public partial class Detalle_Ingreso
    {
        public int id { get; set; }
        public Nullable<int> ingreso_id { get; set; }
        public Nullable<int> articulo_id { get; set; }
        public Nullable<int> cantidad { get; set; }
    
        public virtual Articulo Articulo { get; set; }
        public virtual Ingreso Ingreso { get; set; }
    }
}
