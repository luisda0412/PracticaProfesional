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

    [MetadataType(typeof(Reportes_TecnicosMetadata))]
    public partial class Reportes_Tecnicos
    {
        public int id { get; set; }
        public Nullable<int> reparacion_id { get; set; }
        public string reporte { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
    
        public virtual Reparaciones Reparaciones { get; set; }
    }
}
