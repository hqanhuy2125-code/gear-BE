
USE FGG_MADLIONS_DB_V2;
GO

-- Xóa các test data
DELETE FROM Products WHERE Name = 'string' OR Name = 'Test' OR Category = 'string';
GO

-- Seed & Update dữ liệu
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-A20.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'A20', N'Mô tả sản phẩm A20', 950000, '/src/assets/images/accessory-A20.jpg', 'Headphones', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 950000, Name = N'A20',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-A20.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-A3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'A3', N'Mô tả sản phẩm A3', 800000, '/src/assets/images/accessory-A3.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'A3',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-A3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-A50.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'A50', N'Mô tả sản phẩm A50', 950000, '/src/assets/images/accessory-A50.jpg', 'Headphones', 50, 1, '2026-04-20', 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 950000, Name = N'A50',
        Stock = 50, IsPreOrder = 1, PreOrderDate = '2026-04-20', IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-A50.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-B3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'B3', N'Mô tả sản phẩm B3', 800000, '/src/assets/images/accessory-B3.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'B3',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-B3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-BH.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Bh', N'Mô tả sản phẩm Bh', 800000, '/src/assets/images/accessory-BH.jpg', 'Mice', 50, 0, NULL, 1, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'Bh',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 1
    WHERE ImageUrl = '/src/assets/images/accessory-BH.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-Gpro X.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Gpro X', N'Mô tả sản phẩm Gpro X', 1400000, '/src/assets/images/accessory-Gpro X.jpg', 'Headphones', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1400000, Name = N'Gpro X',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-Gpro X.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-GSP670.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Gsp670', N'Mô tả sản phẩm Gsp670', 1400000, '/src/assets/images/accessory-GSP670.jpg', 'Headphones', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1400000, Name = N'Gsp670',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-GSP670.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-hyperX cloud II.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Hyperx Cloud Ii', N'Mô tả sản phẩm Hyperx Cloud Ii', 2750000, '/src/assets/images/accessory-hyperX cloud II.jpg', 'Headphones', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 2750000, Name = N'Hyperx Cloud Ii',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-hyperX cloud II.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-marshal.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Marshal', N'Mô tả sản phẩm Marshal', 1550000, '/src/assets/images/accessory-marshal.jpg', 'Headphones', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1550000, Name = N'Marshal',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-marshal.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-mercury K1 pro.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Mercury K1 Pro', N'Mô tả sản phẩm Mercury K1 Pro', 2600000, '/src/assets/images/accessory-mercury K1 pro.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 2600000, Name = N'Mercury K1 Pro',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-mercury K1 pro.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/accessory-Nova7.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Nova7', N'Mô tả sản phẩm Nova7', 1250000, '/src/assets/images/accessory-Nova7.jpg', 'Headphones', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1250000, Name = N'Nova7',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/accessory-Nova7.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-gh60.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Gh60', N'Mô tả sản phẩm Gh60', 1100000, '/src/assets/images/keyboard-gh60.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1100000, Name = N'Gh60',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-gh60.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-IQUNIX.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Iqunix', N'Mô tả sản phẩm Iqunix', 1400000, '/src/assets/images/keyboard-IQUNIX.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1400000, Name = N'Iqunix',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-IQUNIX.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-mad68R.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Mad68r', N'Mô tả sản phẩm Mad68r', 1400000, '/src/assets/images/keyboard-mad68R.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1400000, Name = N'Mad68r',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-mad68R.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-NEO65.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Neo65', N'Mô tả sản phẩm Neo65', 1250000, '/src/assets/images/keyboard-NEO65.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1250000, Name = N'Neo65',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-NEO65.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-RKV3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Rkv3', N'Mô tả sản phẩm Rkv3', 1100000, '/src/assets/images/keyboard-RKV3.jpg', 'Keyboards', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1100000, Name = N'Rkv3',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-RKV3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-RS7 pro.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Rs7 Pro', N'Mô tả sản phẩm Rs7 Pro', 1550000, '/src/assets/images/keyboard-RS7 pro.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1550000, Name = N'Rs7 Pro',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-RS7 pro.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-S75 pro.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'S75 Pro', N'Mô tả sản phẩm S75 Pro', 1550000, '/src/assets/images/keyboard-S75 pro.jpg', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 1550000, Name = N'S75 Pro',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-S75 pro.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/keyboard-Wooting-60HE.png')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Wooting 60he', N'Mô tả sản phẩm Wooting 60he', 2300000, '/src/assets/images/keyboard-Wooting-60HE.png', 'Keyboards', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Keyboards', Price = 2300000, Name = N'Wooting 60he',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/keyboard-Wooting-60HE.png';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-DA3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Da3', N'Mô tả sản phẩm Da3', 950000, '/src/assets/images/mouse-DA3.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 950000, Name = N'Da3',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-DA3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-F1.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'F1', N'Mô tả sản phẩm F1', 800000, '/src/assets/images/mouse-F1.jpg', 'Mice', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'F1',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-F1.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-G502.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'G502', N'Mô tả sản phẩm G502', 1100000, '/src/assets/images/mouse-G502.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 1100000, Name = N'G502',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-G502.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-O2.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'O2', N'Mô tả sản phẩm O2', 800000, '/src/assets/images/mouse-O2.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'O2',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-O2.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-scyrox-V6.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Scyrox V6', N'Mô tả sản phẩm Scyrox V6', 1850000, '/src/assets/images/mouse-scyrox-V6.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 1850000, Name = N'Scyrox V6',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-scyrox-V6.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-V3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'V3', N'Mô tả sản phẩm V3', 800000, '/src/assets/images/mouse-V3.jpg', 'Mice', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Mice', Price = 800000, Name = N'V3',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-V3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mouse-X3.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'X3', N'Mô tả sản phẩm X3', 800000, '/src/assets/images/mouse-X3.jpg', 'Accessories', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Accessories', Price = 800000, Name = N'X3',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mouse-X3.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mousepad-850x.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'850x', N'Mô tả sản phẩm 850x', 1100000, '/src/assets/images/mousepad-850x.jpg', 'Accessories', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Accessories', Price = 1100000, Name = N'850x',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mousepad-850x.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mousepad-980pro.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'980pro', N'Mô tả sản phẩm 980pro', 1400000, '/src/assets/images/mousepad-980pro.jpg', 'Accessories', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Accessories', Price = 1400000, Name = N'980pro',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mousepad-980pro.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mousepad-Corsair.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Corsair', N'Mô tả sản phẩm Corsair', 1550000, '/src/assets/images/mousepad-Corsair.jpg', 'Headphones', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1550000, Name = N'Corsair',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mousepad-Corsair.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mousepad-nintendo.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Nintendo', N'Mô tả sản phẩm Nintendo', 1700000, '/src/assets/images/mousepad-nintendo.jpg', 'Accessories', 50, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Accessories', Price = 1700000, Name = N'Nintendo',
        Stock = 50, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mousepad-nintendo.jpg';
END
GO
IF NOT EXISTS (SELECT 1 FROM Products WHERE ImageUrl = '/src/assets/images/mousepad-razer.jpg')
BEGIN
    INSERT INTO Products (Name, Description, Price, ImageUrl, Category, Stock, IsPreOrder, PreOrderDate, IsOrderOnly, CreatedAt)
    VALUES (N'Razer', N'Mô tả sản phẩm Razer', 1250000, '/src/assets/images/mousepad-razer.jpg', 'Headphones', 0, 0, NULL, 0, GETDATE());
END
ELSE
BEGIN
    UPDATE Products
    SET Category = 'Headphones', Price = 1250000, Name = N'Razer',
        Stock = 0, IsPreOrder = 0, PreOrderDate = NULL, IsOrderOnly = 0
    WHERE ImageUrl = '/src/assets/images/mousepad-razer.jpg';
END
GO
