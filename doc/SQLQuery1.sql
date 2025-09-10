

--------------------------------
-- clientes
--------------------------------
CREATE TABLE Customers (
	IdCliente INT IDENTITY(1,1) PRIMARY KEY,
	TipoIdentificacion VARCHAR(3) NOT NULL,
	NumeroIdentificacion VARCHAR(50) NOT NULL,
	Nombres VARCHAR(100) NOT NULL,
	Email VARCHAR(200),
	Telefono VARCHAR(20),
	Direccion VARCHAR(200),
	Ciudad VARCHAR(100),
	Pais VARCHAR(2)
);
--------------------------------
GO

--------------------------------
-- Productos
--------------------------------
CREATE TABLE Products (
	IdProducto INT IDENTITY(1,1) PRIMARY KEY,
	Nombre VARCHAR(100) NOT NULL,
	PrecioUnitario MONEY,
	Impuesto DECIMAL(5, 2)
);
--------------------------------
GO

--------------------------------
-- Facturas
--------------------------------
CREATE TABLE Invoices (
	IdFactura INT IDENTITY(1,1) PRIMARY KEY,
	NumeroFactura VARCHAR(50) NOT NULL,
	FechaFactura DATETIME,
	IdCliente INT NOT NULL,
	Estado VARCHAR(50) NOT NULL,
	Moneda VARCHAR(3) NOT NULL,
	SubtotalSinImpuestos MONEY,
	TotalImpuestos MONEY,
	CostoEnvio MONEY,
	Descuento MONEY,
	TotalFactura MONEY,
	FOREIGN KEY (IdCliente) REFERENCES Customers(IdCliente)
);
CREATE UNIQUE INDEX i_NumeroFactura ON Invoices (NumeroFactura);
--------------------------------
GO

--------------------------------
-- Productos por factura
--------------------------------
CREATE TABLE InvoicesProducts (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	IdFactura INT NOT NULL,
	IdProducto INT NOT NULL,
	Cantidad INT NOT NULL,
	PrecioUnitario MONEY,
	Impuesto DECIMAL(5, 2),
	FOREIGN KEY (IdFactura) REFERENCES Invoices(IdFactura)
);
--------------------------------
GO

--------------------------------
-- Pedidos
--------------------------------
CREATE TABLE InvoiceLines (
	IdPedido INT IDENTITY(1,1) PRIMARY KEY,
	IdCliente INT NOT NULL,
	FechaPedido DATETIME,
	Estado VARCHAR(50) NOT NULL,
	Moneda VARCHAR(3) NOT NULL,
	SubtotalSinImpuestos MONEY,
	TotalImpuestos MONEY,
	CostoEnvio MONEY,
	Descuento MONEY,
	TotalPedido MONEY,
	FOREIGN KEY (IdCliente) REFERENCES Customers(IdCliente)
);
--------------------------------
GO

--------------------------------
-- Productos por Pedido
--------------------------------
CREATE TABLE InvoiceLinesProducts (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	IdPedido INT NOT NULL,
	IdProducto INT NOT NULL,
	Cantidad INT,
	PrecioUnitario MONEY,
	Impuesto DECIMAL(5, 2),
	FOREIGN KEY (IdPedido) REFERENCES InvoiceLines(IdPedido)
);
--------------------------------
GO

--------------------------------
-- AuditLog
--------------------------------
CREATE TABLE AuditLog (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Accion VARCHAR(50) NOT NULL,
    NombreTabla VARCHAR(100) NOT NULL,
    RegistroId VARCHAR(100) NOT NULL, -- Se usa VARCHAR para soportar diferentes tipos de ID
    UsuarioId VARCHAR(100) NULL,
    FechaHora DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ValorAnterior NVARCHAR(MAX) NULL,
    ValorNuevo NVARCHAR(MAX) NULL
);
--------------------------------
GO

--------------------------------
-- Datos semilla: demo. 
--------------------------------
-- 5 productos
INSERT INTO Products(Nombre, PrecioUnitario, Impuesto)
VALUES ('Camiseta tipo polo M316', 60000, 19),
('Pantalon T460', 150000, 16.5),
('Medias G36', 20000, 0),
('Camisa M789', 90000, 16),
('Sudadera L892', 250000, 16)

-- 3 clientes
INSERT INTO Customers (TipoIdentificacion, NumeroIdentificacion, Nombres, 
Email,Telefono, Direccion, Ciudad, Pais)
VALUES ('CC', '80005515', 'Federico Gonzales', 'zales@gmm.com', '3003734545', 'Calle 45', 'Bogota', 'CO'),
('CC', '52478589', 'Maria Guadalupe', 'mlupe@hotm.com', '34589657465', 'Avenida libertador 63', 'Medellin', 'CO'),
('DNI', '12345678A', 'Juan Perez', 'juanpe@yhm.com', '3145734545', 'Calle de las Palmas 72', 'Madrid', 'ES')

-- 1 factura
declare @success bit = 1
begin transaction
begin try
	DECLARE @idFac INT;
	INSERT INTO Invoices (NumeroFactura, FechaFactura, IdCliente, Estado,
	Moneda, SubtotalSinImpuestos, TotalImpuestos, CostoEnvio, Descuento, TotalFactura)
	VALUES ('001', GETDATE(), 1, 'Emitida', 'COP', 210000, 37200, 0, 0, 172800)

	SELECT @idFac = @@IDENTITY;

	INSERT InvoicesProducts (IdFactura, IdProducto, Cantidad, PrecioUnitario, Impuesto)
	VALUES (@idFac, 1, 2, 60000, 19),
	(@idFac, 3, 1, 90000, 16)
end try
begin catch
    rollback transaction
    set @success = 0
end catch

if(@success = 1)
begin
    commit transaction
end
--------------------------------
GO

--------------------------------
-- Trigger para auditoria de Tabla Facturas
--------------------------------
CREATE TRIGGER TR_Invoices_Audit
ON Invoices
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Usuario VARCHAR(100) = SYSTEM_USER;
    
    -- Inserciones (Nuevos registros)
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorNuevo)
    SELECT 
        'INSERT',
        'Invoices',
        CAST(inserted.IdFactura AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM inserted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted
    WHERE NOT EXISTS (SELECT 1 FROM deleted);
    
    -- Eliminaciones (Registros antiguos)
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorAnterior)
    SELECT 
        'DELETE',
        'Invoices',
        CAST(deleted.IdFactura AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM deleted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM deleted
    WHERE NOT EXISTS (SELECT 1 FROM inserted);
    
    -- Actualizaciones (Antiguo vs Nuevo)
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorAnterior, ValorNuevo)
    SELECT 
        'UPDATE',
        'Invoices',
        CAST(inserted.IdFactura AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM deleted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT * FROM inserted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted
    INNER JOIN deleted ON inserted.IdFactura = deleted.IdFactura;
END;
--------------------------------
GO

--------------------------------
-- Trigger para auditoria de Tabla Detalle Facturas
--------------------------------
CREATE TRIGGER TR_InvoicesProducts_Audit
ON InvoicesProducts
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Usuario VARCHAR(100) = SYSTEM_USER;
    
    -- Inserciones
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorNuevo)
    SELECT 
        'INSERT',
        'InvoicesProducts',
        CAST(inserted.Id AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM inserted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted
    WHERE NOT EXISTS (SELECT 1 FROM deleted);
    
    -- Eliminaciones
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorAnterior)
    SELECT 
        'DELETE',
        'InvoicesProducts',
        CAST(deleted.Id AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM deleted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM deleted
    WHERE NOT EXISTS (SELECT 1 FROM inserted);
    
    -- Actualizaciones
    INSERT INTO AuditLog (Accion, NombreTabla, RegistroId, UsuarioId, ValorAnterior, ValorNuevo)
    SELECT 
        'UPDATE',
        'InvoicesProducts',
        CAST(inserted.Id AS VARCHAR(100)),
        @Usuario,
        (SELECT * FROM deleted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
        (SELECT * FROM inserted FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
    FROM inserted
    INNER JOIN deleted ON inserted.Id = deleted.Id;
END;
--------------------------------
GO
