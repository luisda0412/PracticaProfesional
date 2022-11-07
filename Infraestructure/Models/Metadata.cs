using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Models
{
    internal partial class ArticuloMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "*El nombre es un campo obligatorio")]
        public string nombre { get; set; }
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "*El precio es un campo obligatorio")]
        public Nullable<double> precio { get; set; }
        [Display(Name = "Imagen")]
        [Required(ErrorMessage = "*La imagen es un campo obligatorio")]
        public byte[] imagen { get; set; }
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "*Necesita seleccionar la categoría")]
        public Nullable<int> categoria_id { get; set; }
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "*Necesita seleccionar el proveedor")]
        public Nullable<int> proveedor_id { get; set; }
        [Display(Name = "Unidades")]
        [Required(ErrorMessage = "*El número de unidades es un campo obligatorio")]
        public Nullable<int> stock { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }

        [Display(Name = "Categoría")]
        public virtual Categoria Categoria { get; set; }

    }

    internal partial class Caja_ChicaMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "*La fecha es un campo obligatorio")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> fecha { get; set; }


        [Display(Name = "Saldo")]
        [Required(ErrorMessage = "*El saldo es un campo obligatorio")]
        public Nullable<double> saldo { get; set; }

        [Display(Name = "Efectivo Entrante")]
        public Nullable<double> entrada { get; set; }

        [Display(Name = "Efectivo Saliente")]
        public Nullable<double> salida { get; set; }
    }

    internal partial class CategoriaMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "*El nombre es un campo obligatorio")]
        public string nombre { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "*La descripción es un campo obligatorio")]
        public string descripcion { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
    }

    internal partial class Detalle_IngresoMetadata
    {
        public int id { get; set; }
        public Nullable<int> ingreso_id { get; set; }
        public Nullable<int> articulo_id { get; set; }
        public virtual Articulo Articulo { get; set; }
        public virtual Ingreso Ingreso { get; set; }
    }

    internal partial class Detalle_VentaMetadata
    {
        public int venta_id { get; set; }
        public int articulo_id { get; set; }
        public Nullable<double> precio { get; set; }
        public Nullable<double> descuento { get; set; }
        public Nullable<int> cantidad { get; set; }

        public virtual Articulo Articulo { get; set; }
        public virtual Venta Venta { get; set; }
    }

    internal partial class EmpresaMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "*El nombre es un campo obligatorio")]
        public string nombre { get; set; }
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "*La dirección es un campo obligatorio")]
        public string direccion { get; set; }
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "*El teléfono es un campo obligatorio")]
        public string telefono { get; set; }
    }

    internal partial class FacturaMetadata
    {
        public int id { get; set; }
        public Nullable<int> venta_id { get; set; }
        public Nullable<int> empresa_id { get; set; }
        public Nullable<bool> tipoFactura { get; set; }

        public virtual Empresa Empresa { get; set; }
        public virtual Venta Venta { get; set; }
    }

    internal partial class IngresoMetadata
    {
        [Display(Name = "Num. Ingreso")]
        public int id { get; set; }
        public Nullable<int> usuario_id { get; set; }
        [Display(Name = "Fecha del ingreso")]
        public Nullable<System.DateTime> fecha { get; set; }
        [Display(Name = "Monto Total")]
        public Nullable<double> monto_total { get; set; }
        [Display(Name = "Detalle")]
        public virtual ICollection<Detalle_Ingreso> Detalle_Ingreso { get; set; }
        [Display(Name = "Comentario")]
        public string comentario { get; set; }

        [Display(Name = "Usuario")]
        public virtual Usuario Usuario { get; set; }
    }

    internal partial class ProveedorMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "*La descripción es un campo obligatorio")]
        public string descripcion { get; set; }
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "*La dirección es un campo obligatorio")]
        public string direccion { get; set; }
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "*El teléfono es un campo obligatorio")]
        [MinLength(8, ErrorMessage = "*El teléfono debe ser exactamente 8 dígitos")]
        [MaxLength(8, ErrorMessage = "*El teléfono debe ser exactamente 8 dígitos")]
        public string telefono { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
    }

    internal partial class ReparacionesMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Usuario")]
        public Nullable<int> usuario_id { get; set; }
        [Display(Name = "ID Cliente")]
        [Required(ErrorMessage = "*La cédula es requerida")]
        public Nullable<int> cliente_id { get; set; }
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "*El número de teléfono es requerido")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "*Solo se permiten números.")]
        public string telefono { get; set; }
        [Display(Name = "Servicio de reparación")]
        [Required(ErrorMessage = "*El servicio a realizar es requerido")]
        public Nullable<int> servicio_reparacion_id { get; set; }
        [Display(Name = "Artículo recibido")]
        [Required(ErrorMessage = "*La descripción es requerida")]
        public string descripcion_articulo { get; set; }
        [Display(Name = "Problema")]
        [Required(ErrorMessage = "*El problema o fallo es requerido")]
        public string descripcion_problema { get; set; }
        [Display(Name = "Fecha de Ingreso")]
        [DisplayFormat(DataFormatString ="{0:dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        public Nullable<System.DateTime> fecha { get; set; }
        [Display(Name = "Costo Aproximado")]
        [Required(ErrorMessage = "*El monto aproximado es requerido")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "* Solo se permiten números.")]
        public Nullable<double> monto_total { get; set; }
        [Display(Name = "Entrega estimada")]
        [Required(ErrorMessage = "*La entrega estimada es requerida")]
        public string entregaestimada { get; set; }

        [Display(Name = "Servicio de reparación")]
        public virtual Servicio_Reparacion Servicio_Reparacion { get; set; }
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Reportes Técnicos")]
        public virtual ICollection<Reportes_Tecnicos> Reportes_Tecnicos { get; set; }

        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
    }

    internal partial class Reportes_TecnicosMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Reparación")]
        public Nullable<int> reparacion_id { get; set; }
        [Display(Name = "Reporte")]
        public string reporte { get; set; }
        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> fecha { get; set; }

        public virtual Reparaciones Reparaciones { get; set; }
    }

    internal partial class ResenaMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Encabezado")]
        [Required(ErrorMessage = "*El encabezado es un campo obligatorio")]
        public string encabezado { get; set; }
        [Display(Name = "Comentario")]
        [Required(ErrorMessage = "*El comentario es un campo obligatorio")]
        public string comentario { get; set; }
        [Display(Name = "Artículo")]
        public Nullable<int> articulo_id { get; set; }
        [Display(Name = "Usuario")]
        public Nullable<int> usuario_id { get; set; }

        public virtual Articulo Articulo { get; set; }
        public virtual Usuario Usuario { get; set; }
    }

    internal partial class RolMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Tipo de rol")]
        public string tipo { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
        public virtual ICollection<Usuario> Usuario { get; set; }
    }

    internal partial class Servicio_ReparacionMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "*La descripción es un campo obligatorio")]
        public string descripcion { get; set; }
        [Display(Name = "Costo")]
        [Required(ErrorMessage = "*El costo es un campo obligatorio")]
        public Nullable<double> costo { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
        public virtual ICollection<Reparaciones> Reparaciones { get; set; }
    }

    internal partial class UsuarioMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "*La contraseña es un campo obligatorio")]
        public string clave { get; set; }
       
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "*El nombre es un campo obligatorio")]
        public string nombre { get; set; }
        
        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "*Los apellidos son un campo obligatorio")]
        public string apellidos { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "*{0} es un campo requerido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0} no tiene formato válido")]
        public string correo_electronico { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "*El teléfono es un campo obligatorio")]
        [MinLength(8, ErrorMessage = "*El teléfono debe ser exactamente 8 dígitos")]
        [MaxLength(8, ErrorMessage = "*El teléfono debe ser exactamente 8 dígitos")]
        public string telefono { get; set; }
       
        [Display(Name = "Tipo de rol")]
        [Required(ErrorMessage = "El tipo de rol es un campo requerido")]
        public Nullable<int> rol_id { get; set; }
        
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
        public virtual ICollection<Ingreso> Ingreso { get; set; }
        public virtual ICollection<Reparaciones> Reparaciones { get; set; }
        public virtual ICollection<Resena> Resena { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual ICollection<Venta> Venta { get; set; }
    }

    internal partial class VentaMetadata
    {
        [Display(Name = "Identificación")]
        public int id { get; set; }
        [Display(Name = "Usuario")]
        public Nullable<int> usuario_id { get; set; }
        [Display(Name = "Cliente")]
        public string nombre_cliente { get; set; }
        [Display(Name = "Fecha")]
        public Nullable<System.DateTime> fecha { get; set; }
        [Display(Name = "Monto total")]
        public Nullable<double> monto_total { get; set; }
        [Display(Name = "Impuesto")]
        public Nullable<double> impuesto { get; set; }
        [Display(Name = "Tipo de venta")]
        public Nullable<bool> tipoventa { get; set; }
        [Display(Name = "Estado")]
        public Nullable<bool> estado { get; set; }
        public virtual ICollection<Detalle_Venta> Detalle_Venta { get; set; }
        public virtual ICollection<Facturas> Facturas { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
