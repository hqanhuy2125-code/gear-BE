IF NOT EXISTS (SELECT * FROM Promotions WHERE Name = 'Mua từ 5 triệu giảm 20%')
BEGIN
    INSERT INTO Promotions (Name, MinOrderValue, DiscountPercent, MaxDiscount, IsActive)
    VALUES (N'Mua từ 5 triệu giảm 20%', 5000000, 20, 2000000, 1);
END
GO

SELECT * FROM Promotions;
