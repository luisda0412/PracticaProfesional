--CREACION DE LA BASE DE DATOS

CREATE DATABASE [Registro_Inventario_VYCUZ] 

GO

Use Registro_Inventario_VYCUZ

--CREACION DE LAS TABLAS 

CREATE TABLE Detalle_Ingreso(
id int Identity(1,1) NOT NULL,
ingreso_id int,
articulo_id int

CONSTRAINT Detalle_Ingreso_PK PRIMARY KEY (id)
);

CREATE TABLE Ingreso(
id int Identity(1,1) NOT NULL,
usuario_id int,
proveedor_id int,
tipo_comprobante nvarchar(50),
num_comprobante nvarchar(50),
fecha datetime,
monto_total float,
impuesto float,

CONSTRAINT Ingreso_PK PRIMARY KEY (id)
);

CREATE TABLE Proveedor(
id int NOT NULL,
descripcion nvarchar(50),
costo float,
estado bit

CONSTRAINT Proveedor_PK PRIMARY KEY (id)
);

CREATE TABLE Resena(
id int Identity(1,1) NOT NULL,
comentario nvarchar(max),
articulo_id int

CONSTRAINT Resena_PK PRIMARY KEY (id)
);

CREATE TABLE Articulo(
id int Identity(1,1) NOT NULL,
nombre nvarchar(50),
precio float,
imagen varbinary(max),
categoria_id int,
stock int,
estado bit

CONSTRAINT Articulo_PK PRIMARY KEY (id)
);

CREATE TABLE Categoria(
id int Identity(1,1) NOT NULL,
nombre nvarchar(50),
descripcion nvarchar(50),
estado bit

CONSTRAINT Categoria_PK PRIMARY KEY (id)
);

CREATE TABLE Reparaciones(
id int Identity(1,1) NOT NULL,
cliente_id int,
direccion nvarchar(50),
telefono nvarchar(50),
categoria_producto_id int,
servicio_reparacion_id int,
descripcion_articulo nvarchar(50),
descripcion_problema nvarchar(50),
fecha datetime,
monto_total float

CONSTRAINT Reparaciones_PK PRIMARY KEY (id)
);

CREATE TABLE Servicio_Reparacion(
id int Identity(1,1) NOT NULL,
descripcion nvarchar(50),
costo float,
estado bit

CONSTRAINT Servicio_Reparacion_PK PRIMARY KEY (id)
);

CREATE TABLE Reportes_Tecnicos(
id int Identity(1,1) NOT NULL,
reparacion_id int,
reporte nvarchar(50),
fecha datetime

CONSTRAINT Reportes_Tecnicos_PK PRIMARY KEY (id)
);

CREATE TABLE Detalle_Venta(
id int Identity(1,1) NOT NULL,
producto_id int,
venta_id int,
precio float,
descuento float,
cantidad int,
tipo_venta bit

CONSTRAINT Detalle_Venta_PK PRIMARY KEY (id)
);

CREATE TABLE Facturas(
id int Identity(1,1) NOT NULL,
detalle_venta_id int,
empresa_id int,
tipoFactura bit

CONSTRAINT Facturas_PK PRIMARY KEY (id)
);

CREATE TABLE Venta(
id int Identity(1,1) NOT NULL,
usuario_id int,
nombre_cliente nvarchar(50),
tipo_comprobante nvarchar(50),
num_comprobante nvarchar(50),
fecha datetime,
monto_total float,
impuesto float,
estado bit

CONSTRAINT Venta_PK PRIMARY KEY (id)
);

CREATE TABLE Usuario(
id int NOT NULL,
clave nvarchar(max),
nombre nvarchar(50),
apellidos nvarchar(50),
correo_electronico nvarchar(max),
telefono nvarchar(50),
rol_id int

CONSTRAINT Usuario_PK PRIMARY KEY (id)
);

CREATE TABLE Empresa(
id int NOT NULL,
nombre nvarchar(50),
direccion nvarchar(50),
telefono nvarchar(50)

CONSTRAINT Empresa_PK PRIMARY KEY (id)
);

CREATE TABLE Rol(
id int Identity(1,1) NOT NULL,
tipo nvarchar(50),
estado bit

CONSTRAINT Rol_PK PRIMARY KEY (id)
);

CREATE TABLE Caja_Chica(
id int Identity(1,1) NOT NULL,
fecha datetime,
efectivo_total float,
billetes float,
monedas float,
usuario_id int

CONSTRAINT Caja_Chica_PK PRIMARY KEY (id)
);

--CREACION DE LLAVES FORANEAS

--DETALLE INGRESO
ALTER TABLE Detalle_Ingreso ADD CONSTRAINT Detalle_Ingreso_Ingreso_FK FOREIGN KEY (ingreso_id) REFERENCES Ingreso (id)
ALTER TABLE Detalle_Ingreso ADD CONSTRAINT Detalle_Ingreso_Articulo_FK FOREIGN KEY (articulo_id) REFERENCES Articulo (id)

--INGRESO
ALTER TABLE Ingreso ADD CONSTRAINT Ingreso_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)
ALTER TABLE Ingreso ADD CONSTRAINT Ingreso_Proveedor_FK FOREIGN KEY (proveedor_id) REFERENCES Proveedor (id)

--RESENA
ALTER TABLE Resena ADD CONSTRAINT Resena_Articulo_FK FOREIGN KEY (articulo_id) REFERENCES Articulo (id)

--REPARACIONES
ALTER TABLE Reparaciones ADD CONSTRAINT Reparaciones_Cliente_FK FOREIGN KEY (cliente_id) REFERENCES Usuario (id)
ALTER TABLE Reparaciones ADD CONSTRAINT Reparaciones_Categoria_FK FOREIGN KEY (categoria_producto_id) REFERENCES Categoria (id)
ALTER TABLE Reparaciones ADD CONSTRAINT Reparaciones_Articulo_FK FOREIGN KEY (servicio_reparacion_id) REFERENCES Servicio_Reparacion (id)

--REPORTES TECNICOS
ALTER TABLE Reportes_Tecnicos ADD CONSTRAINT Reportes_Tecnicos_Reparacion_FK FOREIGN KEY (reparacion_id) REFERENCES Reparaciones (id)

--DETALLE VENTA
ALTER TABLE Detalle_Venta ADD CONSTRAINT Detalle_Venta_Producto_FK FOREIGN KEY (producto_id) REFERENCES Articulo (id)
ALTER TABLE Detalle_Venta ADD CONSTRAINT Detalle_Venta_Venta_FK FOREIGN KEY (venta_id) REFERENCES Venta (id)

--FACTURAS
ALTER TABLE Facturas ADD CONSTRAINT Facturas_Detalle_Venta_FK FOREIGN KEY (detalle_venta_id) REFERENCES Detalle_Venta (id)
ALTER TABLE Facturas ADD CONSTRAINT Facturas_Empresa_FK FOREIGN KEY (empresa_id) REFERENCES Empresa (id)

--VENTA 
ALTER TABLE Venta ADD CONSTRAINT Facturas_Fisica_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)

--USUARIO
ALTER TABLE Usuario ADD CONSTRAINT Usuario_Rol_FK FOREIGN KEY (rol_id) REFERENCES Rol (id)


--CAJA CHICA
ALTER TABLE Caja_Chica ADD CONSTRAINT Caja_Chica_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)

