/* ============================================================
   PET SHOP MANAGEMENT SYSTEM - DATABASE SCRIPT
   Target: Microsoft SQL Server Management Studio 2022
   Run this whole script once (F5). It creates the DB,
   all tables, constraints, seed data, a view and stored
   procedures used by the C# application.
   ============================================================ */

IF DB_ID('PetShopDB') IS NOT NULL
BEGIN
    ALTER DATABASE PetShopDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PetShopDB;
END
GO

CREATE DATABASE PetShopDB;
GO

USE PetShopDB;
GO

/* ============================================================
   TABLES
   ============================================================ */

CREATE TABLE Admin (
    AdminID     INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)   NOT NULL,
    Username    NVARCHAR(50)    NOT NULL UNIQUE,
    Password    NVARCHAR(256)   NOT NULL,   -- store SHA256 hash from C#
    Email       NVARCHAR(100)   NULL,
    Phone       NVARCHAR(20)    NULL,
    Status      TINYINT         NOT NULL DEFAULT 1 CHECK (Status = 1) -- always active
);
GO

CREATE TABLE Manager (
    ManagerID    INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)   NOT NULL,
    Username    NVARCHAR(50)    NOT NULL UNIQUE,
    Password    NVARCHAR(256)   NOT NULL,
    Email       NVARCHAR(100)   NULL,
    Phone       NVARCHAR(20)    NULL,
    Status      TINYINT         NOT NULL DEFAULT 0 CHECK (Status IN (0,1,2))
);
GO

CREATE TABLE Customer (
    CustomerID  INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)   NOT NULL,
    Username    NVARCHAR(50)    NOT NULL UNIQUE,
    Password    NVARCHAR(256)   NOT NULL,
    Email       NVARCHAR(100)   NULL,
    Phone       NVARCHAR(20)    NULL,
    Address     NVARCHAR(200)   NULL,
    Status      TINYINT         NOT NULL DEFAULT 0 CHECK (Status IN (0,1,2))
);
GO

/* ============================================================
   MAIN USER TABLE
   ID/USERNAME/PASSWORD/ROLE/STATUS are synchronized from the
   role-specific tables. ROLE: 1=ADMIN, 2=CUSTOMER, 3=MANAGER.
   STATUS: 0=INACTIVE, 1=ACTIVE, 2=BLOCKED.
   ============================================================ */
CREATE TABLE [Main table] (
    ID          INT             NOT NULL,
    USERNAME    NVARCHAR(50)    NOT NULL,
    PASSWORD    NVARCHAR(256)   NOT NULL,
    ROLE        TINYINT         NOT NULL CHECK (ROLE IN (1,2,3)),
    STATUS      TINYINT         NOT NULL CHECK (STATUS IN (0,1,2)),
    CONSTRAINT PK_MainTable PRIMARY KEY (ID, ROLE)
);
GO

CREATE UNIQUE INDEX UX_MainTable_Username ON [Main table](USERNAME);
GO

CREATE TABLE FoodType (
    FoodTypeID  INT IDENTITY(1,1) PRIMARY KEY,
    TypeName    NVARCHAR(20)    NOT NULL UNIQUE   -- 'Cat', 'Dog'
);
GO

CREATE TABLE Product (
    ProductID     INT IDENTITY(1,1) PRIMARY KEY,
    ManagerID      INT             NOT NULL,
    FoodTypeID    INT             NOT NULL,
    ProductName   NVARCHAR(100)   NOT NULL,
    Description   NVARCHAR(300)   NULL,
    Price         DECIMAL(10,2)   NOT NULL CHECK (Price >= 0),
    StockQuantity INT             NOT NULL CHECK (StockQuantity >= 0),
    CONSTRAINT FK_Product_Manager   FOREIGN KEY (ManagerID)   REFERENCES Manager(ManagerID),
    CONSTRAINT FK_Product_FoodType FOREIGN KEY (FoodTypeID) REFERENCES FoodType(FoodTypeID)
);
GO

CREATE TABLE Orders (   -- named Orders because ORDER is a reserved word
    OrderID      INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID   INT             NOT NULL,
    ManagerID     INT             NOT NULL,
    OrderDate    DATETIME        NOT NULL DEFAULT GETDATE(),
    TotalAmount  DECIMAL(10,2)   NOT NULL DEFAULT 0,
    OrderStatus  NVARCHAR(20)    NOT NULL DEFAULT 'PENDING'
                 CHECK (OrderStatus IN ('PENDING','HOLD','CONFIRMED','DENIED')),
    CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    CONSTRAINT FK_Order_Manager   FOREIGN KEY (ManagerID)   REFERENCES Manager(ManagerID)
);
GO

CREATE TABLE OrderItem (
    OrderItemID  INT IDENTITY(1,1) PRIMARY KEY,
    OrderID      INT             NOT NULL,
    ProductID    INT             NOT NULL,
    Quantity     INT             NOT NULL CHECK (Quantity > 0),
    UnitPrice    DECIMAL(10,2)   NOT NULL,
    SubTotal     AS (Quantity * UnitPrice) PERSISTED,
    CONSTRAINT FK_OrderItem_Order   FOREIGN KEY (OrderID)   REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderItem_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);
GO

CREATE TABLE Payment (
    PaymentID     INT IDENTITY(1,1) PRIMARY KEY,
    OrderID       INT             NOT NULL UNIQUE,
    Amount        DECIMAL(10,2)   NOT NULL,
    PaymentMethod NVARCHAR(20)    NOT NULL DEFAULT 'COD' CHECK (PaymentMethod = 'COD'),
    PaymentStatus NVARCHAR(20)    NOT NULL DEFAULT 'UNPAID' CHECK (PaymentStatus IN ('UNPAID','PAID')),
    PaymentDate   DATETIME        NULL,
    CONSTRAINT FK_Payment_Order FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
);
GO

/* ============================================================
   MAIN TABLE SYNCHRONIZATION
   ============================================================ */
CREATE TRIGGER trg_Admin_MainTable
ON Admin
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE m FROM [Main table] m
    WHERE m.ROLE = 1
      AND NOT EXISTS (SELECT 1 FROM Admin a WHERE a.AdminID = m.ID);

    MERGE [Main table] AS target
    USING (SELECT AdminID AS ID, Username, Password, 1 AS ROLE, Status FROM Admin) AS source
    ON target.ID = source.ID AND target.ROLE = source.ROLE
    WHEN MATCHED THEN UPDATE SET USERNAME=source.Username, PASSWORD=source.Password, STATUS=source.Status
    WHEN NOT MATCHED THEN INSERT (ID, USERNAME, PASSWORD, ROLE, STATUS)
         VALUES (source.ID, source.Username, source.Password, source.ROLE, source.Status);
END
GO

CREATE TRIGGER trg_Customer_MainTable
ON Customer
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE m FROM [Main table] m
    WHERE m.ROLE = 2
      AND NOT EXISTS (SELECT 1 FROM Customer c WHERE c.CustomerID = m.ID);

    MERGE [Main table] AS target
    USING (SELECT CustomerID AS ID, Username, Password, 2 AS ROLE, Status FROM Customer) AS source
    ON target.ID = source.ID AND target.ROLE = source.ROLE
    WHEN MATCHED THEN UPDATE SET USERNAME=source.Username, PASSWORD=source.Password, STATUS=source.Status
    WHEN NOT MATCHED THEN INSERT (ID, USERNAME, PASSWORD, ROLE, STATUS)
         VALUES (source.ID, source.Username, source.Password, source.ROLE, source.Status);
END
GO

CREATE TRIGGER trg_Manager_MainTable
ON Manager
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE m FROM [Main table] m
    WHERE m.ROLE = 3
      AND NOT EXISTS (SELECT 1 FROM Manager s WHERE s.ManagerID = m.ID);

    MERGE [Main table] AS target
    USING (SELECT ManagerID AS ID, Username, Password, 3 AS ROLE, Status FROM Manager) AS source
    ON target.ID = source.ID AND target.ROLE = source.ROLE
    WHEN MATCHED THEN UPDATE SET USERNAME=source.Username, PASSWORD=source.Password, STATUS=source.Status
    WHEN NOT MATCHED THEN INSERT (ID, USERNAME, PASSWORD, ROLE, STATUS)
         VALUES (source.ID, source.Username, source.Password, source.ROLE, source.Status);
END
GO

/* ============================================================
   SEED DATA
   ============================================================ */

-- Default Admin (always Status = 1). Password is stored as SHA256 hash.
INSERT INTO Admin (Name, Username, Password, Email, Phone)
VALUES ('System Admin', 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'admin@petshop.com', '01700000000');

-- The single Manager (owner/manager). Status: 0=INACTIVE, 1=ACTIVE, 2=BLOCKED.
INSERT INTO Manager (Name, Username, Password, Email, Phone, Status)
VALUES ('Shop Manager', 'manager', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'manager@petshop.com', '01711111111', 1);

-- New customer accounts are inserted with Status=0 (INACTIVE) by the C# registration form.

-- Food types
INSERT INTO FoodType (TypeName) VALUES ('Cat'), ('Dog');

-- Sample products
INSERT INTO Product (ManagerID, FoodTypeID, ProductName, Description, Price, StockQuantity)
VALUES
(1, 1, 'Whiskas Adult Cat Food 1.2kg', 'Ocean fish flavor, adult cats', 650.00, 40),
(1, 1, 'Me-O Kitten Food 400g', 'For kittens up to 12 months',    280.00, 25),
(1, 2, 'Pedigree Adult Dog Food 3kg', 'Chicken & vegetables',      1250.00, 30),
(1, 2, 'Royal Canin Puppy Food 1kg',  'For puppies under 1 year',  950.00, 5);
GO

/* ============================================================
   VIEW - used by Admin/Manager Reports form
   ============================================================ */
CREATE VIEW vw_TotalRevenue AS
SELECT
    ISNULL(SUM(p.Amount), 0)              AS TotalRevenue,
    COUNT(DISTINCT p.OrderID)             AS PaidOrderCount
FROM Payment p
WHERE p.PaymentStatus = 'PAID';
GO

CREATE VIEW vw_OrderSummary AS
SELECT
    o.OrderID, o.OrderDate, o.OrderStatus, o.TotalAmount,
    c.Name AS CustomerName, c.Phone AS CustomerPhone,
    s.Name AS ManagerName,
    p.PaymentStatus
FROM Orders o
JOIN Customer c ON o.CustomerID = c.CustomerID
JOIN Manager s   ON o.ManagerID   = s.ManagerID
LEFT JOIN Payment p ON p.OrderID = o.OrderID;
GO

/* ============================================================
   STORED PROCEDURES
   ============================================================ */

-- Login check (parameterized, prevents SQL injection at the DB layer too)
CREATE PROCEDURE sp_Login
    @Username NVARCHAR(50),
    @Password NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Admin WHERE Username=@Username AND Password=@Password)
        SELECT 'Admin' AS Role, AdminID AS UserID, Name FROM Admin WHERE Username=@Username;
    ELSE IF EXISTS (SELECT 1 FROM Manager WHERE Username=@Username AND Password=@Password AND Status=1)
        SELECT 'Manager' AS Role, ManagerID AS UserID, Name FROM Manager WHERE Username=@Username;
    ELSE IF EXISTS (SELECT 1 FROM Customer WHERE Username=@Username AND Password=@Password AND Status=1)
        SELECT 'Customer' AS Role, CustomerID AS UserID, Name FROM Customer WHERE Username=@Username;
    ELSE
        SELECT CAST(NULL AS NVARCHAR(20)) AS Role, CAST(NULL AS INT) AS UserID, CAST(NULL AS NVARCHAR(100)) AS Name;
END
GO

-- Place an order + order items atomically, auto-decide PENDING vs HOLD,
-- and only deduct stock when the manager later CONFIRMS the order.
CREATE PROCEDURE sp_PlaceOrder
    @CustomerID INT,
    @ManagerID   INT,
    @ProductIDs NVARCHAR(MAX),   -- comma separated
    @Quantities NVARCHAR(MAX)    -- comma separated, same order as ProductIDs
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @NewOrderID INT;
    DECLARE @Total DECIMAL(10,2) = 0;
    DECLARE @HasHold BIT = 0;

    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO Orders (CustomerID, ManagerID, OrderDate, TotalAmount, OrderStatus)
        VALUES (@CustomerID, @ManagerID, GETDATE(), 0, 'PENDING');

        SET @NewOrderID = SCOPE_IDENTITY();

        ;WITH Split AS (
            SELECT
                value AS ProductID,
                ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
            FROM STRING_SPLIT(@ProductIDs, ',')
        ),
        SplitQty AS (
            SELECT
                value AS Quantity,
                ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
            FROM STRING_SPLIT(@Quantities, ',')
        )
        INSERT INTO OrderItem (OrderID, ProductID, Quantity, UnitPrice)
        SELECT @NewOrderID, CAST(s.ProductID AS INT), CAST(q.Quantity AS INT), pr.Price
        FROM Split s
        JOIN SplitQty q ON s.rn = q.rn
        JOIN Product pr ON pr.ProductID = CAST(s.ProductID AS INT);

        -- if requested quantity exceeds current stock for ANY item, mark HOLD
        IF EXISTS (
            SELECT 1 FROM OrderItem oi
            JOIN Product pr ON pr.ProductID = oi.ProductID
            WHERE oi.OrderID = @NewOrderID AND oi.Quantity > pr.StockQuantity
        )
            SET @HasHold = 1;

        SELECT @Total = SUM(SubTotal) FROM OrderItem WHERE OrderID = @NewOrderID;

        UPDATE Orders
        SET TotalAmount = @Total,
            OrderStatus = CASE WHEN @HasHold = 1 THEN 'HOLD' ELSE 'PENDING' END
        WHERE OrderID = @NewOrderID;

        INSERT INTO Payment (OrderID, Amount, PaymentMethod, PaymentStatus)
        VALUES (@NewOrderID, @Total, 'COD', 'UNPAID');

        COMMIT TRANSACTION;
        SELECT @NewOrderID AS NewOrderID, @Total AS TotalAmount,
               (CASE WHEN @HasHold = 1 THEN 'HOLD' ELSE 'PENDING' END) AS OrderStatus;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Manager confirms an order: deducts stock atomically, fails safely if stock ran out
CREATE PROCEDURE sp_ConfirmOrder
    @OrderID  INT,
    @ManagerID INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM Orders
            WHERE OrderID = @OrderID
              AND ManagerID = @ManagerID
              AND OrderStatus IN ('PENDING','HOLD')
        )
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Only pending orders can be approved.', 16, 1);
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM OrderItem oi
            JOIN Product pr ON pr.ProductID = oi.ProductID
            WHERE oi.OrderID = @OrderID AND oi.Quantity > pr.StockQuantity
        )
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Insufficient stock for one or more items. Order kept on HOLD.', 16, 1);
            RETURN;
        END

        UPDATE pr
        SET pr.StockQuantity = pr.StockQuantity - oi.Quantity
        FROM Product pr
        JOIN OrderItem oi ON oi.ProductID = pr.ProductID
        WHERE oi.OrderID = @OrderID;

        UPDATE Orders SET OrderStatus = 'CONFIRMED' WHERE OrderID = @OrderID AND OrderStatus IN ('PENDING','HOLD');

        COMMIT TRANSACTION;
        SELECT 'CONFIRMED' AS NewStatus;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE PROCEDURE sp_RejectOrder
    @OrderID INT,
    @ManagerID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Orders
    SET OrderStatus = 'DENIED'
    WHERE OrderID = @OrderID
      AND ManagerID = @ManagerID
      AND OrderStatus IN ('PENDING','HOLD');
END
GO

CREATE PROCEDURE sp_MarkPaymentPaid
    @OrderID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p
    SET PaymentStatus = 'PAID', PaymentDate = GETDATE()
    FROM Payment p
    INNER JOIN Orders o ON o.OrderID = p.OrderID
    WHERE p.OrderID = @OrderID
      AND p.PaymentStatus = 'UNPAID'
      AND o.OrderStatus = 'CONFIRMED';
END
GO

/* ============================================================
   SAMPLE ADVANCED / SUBQUERY EXAMPLES
   (use these directly in your report as "advanced queries" evidence,
    and reuse the patterns in Repositories/*.cs)
   ============================================================ */

-- Products with stock below the average stock level (subquery)
-- SELECT ProductName, StockQuantity FROM Product
-- WHERE StockQuantity < (SELECT AVG(StockQuantity) FROM Product);

-- Customers whose total spend is above the average customer spend (correlated subquery)
-- SELECT c.Name, SUM(o.TotalAmount) AS Spend
-- FROM Customer c JOIN Orders o ON o.CustomerID = c.CustomerID
-- GROUP BY c.Name
-- HAVING SUM(o.TotalAmount) > (
--     SELECT AVG(CustTotal) FROM (
--         SELECT SUM(TotalAmount) AS CustTotal FROM Orders GROUP BY CustomerID
--     ) t
-- );

-- Best selling product (aggregate + TOP)
-- SELECT TOP 1 pr.ProductName, SUM(oi.Quantity) AS UnitsSold
-- FROM OrderItem oi JOIN Product pr ON pr.ProductID = oi.ProductID
-- GROUP BY pr.ProductName ORDER BY UnitsSold DESC;

PRINT 'PetShopDB created successfully.';
GO
