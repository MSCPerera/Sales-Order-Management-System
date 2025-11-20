IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SalesOrderDB')
BEGIN
    CREATE DATABASE SalesOrderDB;
END
GO

USE SalesOrderDB;
GO

/* Clients Table */
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clients')
BEGIN
    CREATE TABLE Clients (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerName NVARCHAR(200) NOT NULL,
        Address NVARCHAR(500),
        City NVARCHAR(100),
        PostalCode NVARCHAR(20),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

/* Items Table */
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Items')
BEGIN
    CREATE TABLE Items (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ItemCode NVARCHAR(50) NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

/* Sales Table */
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesOrders')
BEGIN
    CREATE TABLE SalesOrders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderNumber NVARCHAR(50) NOT NULL,
        ClientId INT NOT NULL,
        OrderDate DATETIME2 NOT NULL,
        DeliveryAddress NVARCHAR(500),
        City NVARCHAR(100),
        PostalCode NVARCHAR(20),
        TotalExclAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalTaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalInclAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2 NULL,
        CONSTRAINT FK_SalesOrders_Clients FOREIGN KEY (ClientId) REFERENCES Clients(Id)
    );
END
GO

/* SalesOrderLine Table */
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SalesOrderLines')
BEGIN
    CREATE TABLE SalesOrderLines (
        Id INT PRIMARY KEY IDENTITY(1,1),
        SalesOrderId INT NOT NULL,
        ItemId INT NOT NULL,
        Note NVARCHAR(500),
        Quantity INT NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        TaxRate DECIMAL(5,2) NOT NULL,
        ExclAmount DECIMAL(18,2) NOT NULL,
        TaxAmount DECIMAL(18,2) NOT NULL,
        InclAmount DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_SalesOrderLines_SalesOrders FOREIGN KEY (SalesOrderId) 
            REFERENCES SalesOrders(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SalesOrderLines_Items FOREIGN KEY (ItemId) 
            REFERENCES Items(Id)
    );
END
GO

/* Insert Clients */
IF NOT EXISTS (SELECT * FROM Clients)
BEGIN
    INSERT INTO Clients (CustomerName, Address, City, PostalCode, CreatedDate) VALUES
    ('ABC Corporation', '123 Main St', 'Colombo', '00100', GETUTCDATE()),
    ('XYZ Ltd', '456 Galle Road', 'Colombo', '00300', GETUTCDATE()),
    ('Tech Solutions', '789 Kandy Road', 'Kandy', '20000', GETUTCDATE()),
    ('Global Traders', '321 Negombo Road', 'Negombo', '11500', GETUTCDATE()),
    ('Digital Services', '654 Matara Road', 'Galle', '80000', GETUTCDATE());
    
    PRINT 'Sample clients inserted successfully.';
END
GO

/* Insert Items*/
IF NOT EXISTS (SELECT * FROM Items)
BEGIN
    INSERT INTO Items (ItemCode, Description, Price, CreatedDate) VALUES
    ('ITEM001', 'Laptop Computer', 85000.00, GETUTCDATE()),
    ('ITEM002', 'Wireless Mouse', 1500.00, GETUTCDATE()),
    ('ITEM003', 'Keyboard', 2500.00, GETUTCDATE()),
    ('ITEM004', 'Monitor 24 inch', 35000.00, GETUTCDATE()),
    ('ITEM005', 'USB Cable', 500.00, GETUTCDATE()),
    ('ITEM006', 'External Hard Drive 1TB', 12000.00, GETUTCDATE()),
    ('ITEM007', 'Webcam HD', 8500.00, GETUTCDATE()),
    ('ITEM008', 'Headset with Mic', 3500.00, GETUTCDATE()),
    ('ITEM009', 'Laptop Bag', 2000.00, GETUTCDATE()),
    ('ITEM010', 'USB Hub 4 Port', 1800.00, GETUTCDATE());
    
    PRINT 'Sample items inserted successfully.';
END
GO

PRINT '============================================';
PRINT 'Database Setup Complete!';
PRINT '============================================';
PRINT 'Clients count: ' + CAST((SELECT COUNT(*) FROM Clients) AS VARCHAR);
PRINT 'Items count: ' + CAST((SELECT COUNT(*) FROM Items) AS VARCHAR);
PRINT 'SalesOrders count: ' + CAST((SELECT COUNT(*) FROM SalesOrders) AS VARCHAR);
PRINT '============================================';

SELECT 'Clients' AS TableName;
SELECT * FROM Clients;

SELECT 'Items' AS TableName;
SELECT * FROM Items;

GO