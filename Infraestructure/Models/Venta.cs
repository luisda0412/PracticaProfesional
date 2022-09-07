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
    
    public partial class Venta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Venta()
        {
            this.Detalle_Venta = new HashSet<Detalle_Venta>();
        }
    
        public int id { get; set; }
        public Nullable<int> usuario_id { get; set; }
        public string nombre_cliente { get; set; }
        public string tipo_comprobante { get; set; }
        public string num_comprobante { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<double> monto_total { get; set; }
        public Nullable<double> impuesto { get; set; }
        public Nullable<bool> estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Detalle_Venta> Detalle_Venta { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
