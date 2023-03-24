--CREACION DE LA BASE DE DATOS

CREATE DATABASE [Registro_Inventario_VYCUZ] 

GO

Use Registro_Inventario_VYCUZ


--CREACION DE LAS TABLAS 

CREATE TABLE Detalle_Ingreso(
id int Identity(1,1) NOT NULL,
ingreso_id int,
articulo_id int, 
cantidad int

CONSTRAINT Detalle_Ingreso_PK PRIMARY KEY (id)
);

CREATE TABLE Ingreso(
id int Identity(1,1) NOT NULL,
usuario_id int,
fecha datetime,
monto_total float,
comentario nvarchar(60)

CONSTRAINT Ingreso_PK PRIMARY KEY (id)
);

CREATE TABLE Proveedor(
id int Identity(1,1),
descripcion nvarchar(50),
direccion nvarchar(50),
telefono nvarchar(50),
estado bit

CONSTRAINT Proveedor_PK PRIMARY KEY (id)
);

CREATE TABLE Resena(
id int Identity(1,1) NOT NULL,
encabezado nvarchar(50),
comentario nvarchar(max),
articulo_id int,
usuario_id int

CONSTRAINT Resena_PK PRIMARY KEY (id)
);

CREATE TABLE Articulo(
id int Identity(1,1) NOT NULL,
nombre nvarchar(50),
precio float,
imagen varbinary(max),
categoria_id int,
proveedor_id int,
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
usuario_id int,
cliente_id int,
telefono nvarchar(50),
servicio_reparacion_id int,
descripcion_articulo nvarchar(50),
descripcion_problema nvarchar(50),
fecha datetime,
monto_total float,
entregaestimada datetime,
estado bit

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
fecha datetime,

CONSTRAINT Reportes_Tecnicos_PK PRIMARY KEY (id)
);

CREATE TABLE Detalle_Venta(
venta_id int NOT NULL,
articulo_id int NOT NULL,
precio float,
descuento float,
cantidad int,

CONSTRAINT Detalle_Venta_PK PRIMARY KEY (venta_id, articulo_id)
);

CREATE TABLE Facturas(
id int Identity(1,1) NOT NULL,
venta_id int,
empresa_id int,
tipoFactura bit

CONSTRAINT Facturas_PK PRIMARY KEY (id)
);

CREATE TABLE Venta(
id int Identity(1,1) NOT NULL,
usuario_id int,
nombre_cliente nvarchar(50),
fecha datetime,
monto_total float,
impuesto float,
tipopago bit,
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
rol_id int,
estado bit,
tokenRecuperacion nvarchar(max)

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
saldo float,
entrada float,
salida float,

CONSTRAINT Caja_Chica_PK PRIMARY KEY (id)
);


CREATE TABLE Arqueos_Caja(
id int Identity(1,1) NOT NULL,
usuario_id int,
fecha datetime,
saldo float,
estado bit

CONSTRAINT Caja_Arqueos_PK PRIMARY KEY (id)
);

--CREACION DE LLAVES FORANEAS

--DETALLE INGRESO
ALTER TABLE Detalle_Ingreso ADD CONSTRAINT Detalle_Ingreso_Ingreso_FK FOREIGN KEY (ingreso_id) REFERENCES Ingreso (id)
ALTER TABLE Detalle_Ingreso ADD CONSTRAINT Detalle_Ingreso_Articulo_FK FOREIGN KEY (articulo_id) REFERENCES Articulo (id)

--INGRESO
ALTER TABLE Ingreso ADD CONSTRAINT Ingreso_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)
--ALTER TABLE Ingreso ADD CONSTRAINT Ingreso_Proveedor_FK FOREIGN KEY (proveedor_id) REFERENCES Proveedor (id)

--RESENA
ALTER TABLE Resena ADD CONSTRAINT Resena_Articulo_FK FOREIGN KEY (articulo_id) REFERENCES Articulo (id)
ALTER TABLE Resena ADD CONSTRAINT Resena_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)

--REPARACIONES
ALTER TABLE Reparaciones ADD CONSTRAINT Reparaciones_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)
ALTER TABLE Reparaciones ADD CONSTRAINT Reparaciones_Articulo_FK FOREIGN KEY (servicio_reparacion_id) REFERENCES Servicio_Reparacion (id)

--REPORTES TECNICOS
ALTER TABLE Reportes_Tecnicos ADD CONSTRAINT Reportes_Tecnicos_Reparacion_FK FOREIGN KEY (reparacion_id) REFERENCES Reparaciones (id)

--DETALLE VENTA
ALTER TABLE Detalle_Venta ADD CONSTRAINT Detalle_Venta_Producto_FK FOREIGN KEY (articulo_id) REFERENCES Articulo (id)
ALTER TABLE Detalle_Venta ADD CONSTRAINT Detalle_Venta_Venta_FK FOREIGN KEY (venta_id) REFERENCES Venta (id)

--FACTURAS
ALTER TABLE Facturas ADD CONSTRAINT Facturas__Venta_FK FOREIGN KEY (venta_id) REFERENCES Venta (id)
ALTER TABLE Facturas ADD CONSTRAINT Facturas_Empresa_FK FOREIGN KEY (empresa_id) REFERENCES Empresa (id)

--VENTA 
ALTER TABLE Venta ADD CONSTRAINT Facturas_Fisica_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)

--USUARIO
ALTER TABLE Usuario ADD CONSTRAINT Usuario_Rol_FK FOREIGN KEY (rol_id) REFERENCES Rol (id)

--ARTICULO

ALTER TABLE Articulo ADD CONSTRAINT Articulo_Categoria_FK FOREIGN KEY (categoria_id) REFERENCES Categoria (id)
ALTER TABLE Articulo ADD CONSTRAINT Articulo_Proveedor_FK FOREIGN KEY (proveedor_id) REFERENCES Proveedor (id)

--Arqueos
ALTER TABLE Arqueos_Caja ADD CONSTRAINT Caja_Usuario_FK FOREIGN KEY (usuario_id) REFERENCES Usuario (id)

--INSERTS

--INSERT A PROVEEDOR
INSERT INTO Proveedor VALUES('Neocases','Tibás','60003333',1)
INSERT INTO Proveedor VALUES('Rotolight','Moravia','60005555',1)
INSERT INTO Proveedor VALUES('Multicelulares','Montes de Oca','60007777',1)
INSERT INTO Proveedor VALUES('Cool Accesorios','Curridabat','60008888',1)
INSERT INTO Proveedor VALUES('Multicel','Curridabat','60008888',1)
INSERT INTO Proveedor VALUES('Planet Group','Escazú','60008888',1)

--INSERT A CATEGORIA

INSERT INTO Categoria VALUES('Estuche', 'Protector para celular', 1)
INSERT INTO Categoria VALUES('Temperado', 'Protector de pantalla', 1)
INSERT INTO Categoria VALUES('Teclado', 'Teclado para computadora', 1)
INSERT INTO Categoria VALUES('Ratón', 'Ratón para computadora', 1)
INSERT INTO Categoria VALUES('Audífonos', 'Periféricos para audio', 1)
INSERT INTO Categoria VALUES('Luces', 'Dispositivos de luz Led', 1)
INSERT INTO Categoria VALUES('Cargadores', 'Para dispositivos moviles', 1)
INSERT INTO Categoria VALUES('Controles', 'Controles de consola', 1)
INSERT INTO Categoria VALUES('Relojes', 'Relojes de mano', 1)
INSERT INTO Categoria VALUES('Parlantes', 'Periféricos para audio', 1)
INSERT INTO Categoria VALUES('Adaptadores', 'Dispositivos de diverso uso', 1)
INSERT INTO Categoria VALUES('Cables', 'Cables USB tipo cargador', 1)

--INSERT A ARTICULO

--Estuches 1
INSERT INTO Articulo VALUES('Estuche Iphone 14 Plus', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche2.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)
INSERT INTO Articulo VALUES('Estuche Iphone 13 Pro Max', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche3.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)
INSERT INTO Articulo VALUES('Estuche Huawei p50 Pro', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche5.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)
INSERT INTO Articulo VALUES('Estuche Samsung A22', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche6.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)
INSERT INTO Articulo VALUES('Estuche Xiaomi Note 10C', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche7.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)
INSERT INTO Articulo VALUES('Estuche Samsung A23', 10000, (SELECT * FROM OPENROWSET(BULK N'C:\estuche8.jpg', SINGLE_BLOB) as T1), 1,1, 5, 1)

--Temperados 2
INSERT INTO Articulo VALUES('Temperado Iphone 14', 3000, (SELECT * FROM OPENROWSET(BULK N'C:\temperado1.jpg', SINGLE_BLOB) as T1), 2,5, 8, 1)
INSERT INTO Articulo VALUES('Temperado Iphone 7/8', 3000, (SELECT * FROM OPENROWSET(BULK N'C:\temperado2.jpg', SINGLE_BLOB) as T1), 2,5, 8, 1)
INSERT INTO Articulo VALUES('Temperado Iphone 7/8 Plus', 3000, (SELECT * FROM OPENROWSET(BULK N'C:\temperado3.jpg', SINGLE_BLOB) as T1), 2,5, 8, 1)
INSERT INTO Articulo VALUES('Temperado Iphone 14', 3000, (SELECT * FROM OPENROWSET(BULK N'C:\temperado4.jpg', SINGLE_BLOB) as T1), 2,5, 8, 1)

--Teclados 3
INSERT INTO Articulo VALUES('Teclado Gaming AULA', 30000, (SELECT * FROM OPENROWSET(BULK N'C:\teclado1.jpg', SINGLE_BLOB) as T1), 3,6, 5, 1)
INSERT INTO Articulo VALUES('Teclado XTRIKE ME', 30000, (SELECT * FROM OPENROWSET(BULK N'C:\teclado2.jpg', SINGLE_BLOB) as T1), 3,6, 5, 1)
INSERT INTO Articulo VALUES('Teclado GK986', 30000, (SELECT * FROM OPENROWSET(BULK N'C:\teclado3.jpg', SINGLE_BLOB) as T1), 3,6, 5, 1)

--Audifonos 5
INSERT INTO Articulo VALUES('Air Pods 3 Generación', 25000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos1.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Audífonos Pro 8', 18000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos2.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Audífonos SOMOSTEL SMS-J13', 20000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos3.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Audífonos m10', 35000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos4.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Air Pods 2 Generación', 32000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos5.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Audífonos SOMOSTEL', 20000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos6.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)
INSERT INTO Articulo VALUES('Heatset SOMOSTEL', 29000, (SELECT * FROM OPENROWSET(BULK N'C:\audifonos7.jpg', SINGLE_BLOB) as T1), 5,3, 4, 1)

--Luces 6
INSERT INTO Articulo VALUES('Aro de Luz 1.5m', 8000, (SELECT * FROM OPENROWSET(BULK N'C:\aro de luz1.png', SINGLE_BLOB) as T1), 6,2, 6, 1)

--Cargadores 7
INSERT INTO Articulo VALUES('Cargador Huawei v8', 8000, (SELECT * FROM OPENROWSET(BULK N'C:\cargador1.jpg', SINGLE_BLOB) as T1), 7,4, 4, 1)
INSERT INTO Articulo VALUES('Cargador Huawei Tipo C', 8000, (SELECT * FROM OPENROWSET(BULK N'C:\cargador2.jpg', SINGLE_BLOB) as T1), 7,4, 4, 1)
INSERT INTO Articulo VALUES('Cargador SOMOSTEL', 7000, (SELECT * FROM OPENROWSET(BULK N'C:\cargador3.jpg', SINGLE_BLOB) as T1), 7,4, 4, 1)
INSERT INTO Articulo VALUES('Cargador Iphone Belking', 12000, (SELECT * FROM OPENROWSET(BULK N'C:\cargador4.jpg', SINGLE_BLOB) as T1), 7,4, 4, 1)

--Controles 8
INSERT INTO Articulo VALUES('Control ps4', 25000, (SELECT * FROM OPENROWSET(BULK N'C:\controles1.jpg', SINGLE_BLOB) as T1), 8,6, 5, 1)

--Parlantes 10
INSERT INTO Articulo VALUES('Parlante Nex Rixing', 31000, (SELECT * FROM OPENROWSET(BULK N'C:\parlante1.jpg', SINGLE_BLOB) as T1), 10,6, 5,1)
INSERT INTO Articulo VALUES('Parlante Gaming Aula', 45000, (SELECT * FROM OPENROWSET(BULK N'C:\parlante2.jpg', SINGLE_BLOB) as T1), 10,6, 5,1)
INSERT INTO Articulo VALUES('Parlante Rixing NR555', 42000, (SELECT * FROM OPENROWSET(BULK N'C:\parlante3.jpg', SINGLE_BLOB) as T1), 10,6, 5,1)



--INSERT A ROL

INSERT INTO Rol VALUES('Administrador', 1)
INSERT INTO Rol VALUES('Cliente', 1)
INSERT INTO Rol VALUES('Empleado', 1)

--INSERT A USUARIO

INSERT INTO Usuario VALUES(1,'Admin2022@', 'Victor Admin', 'Admin', 'admin@gmail.com', 88888888, 1, 1, '')
INSERT INTO Usuario VALUES(208130675, 'Kenethmcr16@', 'Keneth', 'Miranda Chaves', 'kennethmiranda56@gmail.com', 85878912, 2, 1, '')
INSERT INTO Usuario VALUES(208120070,'LuisDa12$', 'Luis', 'Cordero Valverde', 'corderoluisdavid@gmail.com', 87529425, 2, 1, '')

INSERT INTO Usuario VALUES(208880999,'Empleado@01', 'Felix', 'Cenat', 'felix@gmail.com', 88776655, 3, 1, '')


--INSERT A SERVICIO REPARACION

INSERT INTO Servicio_Reparacion VALUES('Reparación de micrófono',15000,1)
INSERT INTO Servicio_Reparacion VALUES('Reparación de altavoz',16000,1)
INSERT INTO Servicio_Reparacion VALUES('Cambio de pantalla',40000,1)
INSERT INTO Servicio_Reparacion VALUES('Reparación de puerto de carga',20000,1)
INSERT INTO Servicio_Reparacion VALUES('Cambio de batería',30000,1)
INSERT INTO Servicio_Reparacion VALUES('Reparación de placa base',35000,1)

--INSERT A EMPRESA

INSERT INTO Empresa VALUES(1, 'VYCUZ', 'City Mall, Alajuela', 24335000)


--INSERT A VENTA
--1 Tarjeta 2 Efectivo
INSERT INTO Venta VALUES(208130675,'Keneth', GETDATE() ,22600,2600,1,1)
INSERT INTO Venta VALUES(208130675,'Keneth', GETDATE() ,18080,2080,0,1)
INSERT INTO Venta VALUES(208130675,'Keneth', GETDATE() ,19340,2340,0,1)


--INSERT A DETALLE VENTA
INSERT INTO Detalle_Venta VALUES(1,1,20000,0,2)
INSERT INTO Detalle_Venta VALUES(2,2,16000,0,2)
INSERT INTO Detalle_Venta VALUES(3,1,10000,0,1)
INSERT INTO Detalle_Venta VALUES(3,2,8000,0,1)

--INSERT A RESENA
INSERT INTO Resena VALUES('Estuche Perfecto', 'Muy buena relación calidad precio',2,208120070)

--INSERT A REPARACION
INSERT INTO Reparaciones VALUES(1,208130675,24337955,3,'Telefono Huawei P20','Pantalla quebrada',GETDATE(),47000,GETDATE(),1)


--Insert Reporte Tecnico
INSERT INTO Reportes_Tecnicos VALUES(1,'Teléfono Enviado a Taller',GETDATE())


--Insert Caja Chica
INSERT INTO Caja_Chica VALUES(GETDATE(),20000,0,0)

--Insert Arqueo
INSERT INTO Arqueos_Caja VALUES(208880999,GETDATE(),20000,0)


--SELECTS

--CATEGORIA
SELECT * FROM Categoria

--ARTICULO
SELECT * FROM Articulo

--ROL

SELECT * FROM Rol

--USUARIO

SELECT * FROM Usuario


--Caja Chica
Select * FROM Caja_Chica
Select* from Arqueos_Caja

--RESENA 
Select * from Detalle_Ingreso

SELECT * FROM Ingreso
SELECT * FROM Reparaciones
Select * From Facturas
Select * From Venta