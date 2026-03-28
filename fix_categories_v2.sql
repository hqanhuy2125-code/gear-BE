-- Targeted category fix based on product Names
-- First, check current state
SELECT Category, COUNT(*) AS Total FROM Products GROUP BY Category;
GO

-- Fix Mice: known mouse product names
UPDATE Products SET Category = 'Mice' WHERE Category != 'Mice' AND (
    Name LIKE '%mouse%' OR Name LIKE '%viper%' OR Name LIKE '%deathadder%'
    OR Name LIKE '%superlight%' OR Name LIKE '%G Pro X Super%'
    OR Name LIKE '%razer%' AND Name NOT LIKE '%headset%' AND Name NOT LIKE '%kraken%'
    OR Name LIKE '%G502%' OR Name LIKE '%G305%' OR Name LIKE '%G403%'
    OR Name LIKE '%EC2%' OR Name LIKE '%EC3%'
    OR Name LIKE '%Model O%' OR Name LIKE '%Model D%'
    OR Name LIKE '%Viper Mini%' OR Name LIKE '%Viper V2%'
    OR Name LIKE '%Basilisk%' OR Name LIKE '%Naga%'
    OR Name LIKE '%Rival%' AND Name NOT LIKE '%keyboard%'
    OR Name LIKE '%Sensei%'
    OR Name LIKE '%Zowie%'
    OR Name LIKE '%Logitech G%' AND Name NOT LIKE '%keyboard%' AND Name NOT LIKE '%headset%'
);
GO

-- Fix Keyboards: known keyboard product names
UPDATE Products SET Category = 'Keyboards' WHERE Category != 'Keyboards' AND (
    Name LIKE '%keyboard%' OR Name LIKE '%keychron%'
    OR Name LIKE '%wooting%' OR Name LIKE '%HHKB%'
    OR Name LIKE '%Neo65%' OR Name LIKE '%Iqunix%'
    OR Name LIKE '%Apex Pro%' -- SteelSeries Apex Pro is a keyboard
    OR Name LIKE '%Apex 7%'
    OR Name LIKE '%Apex 5%'
    OR Name LIKE '%K70%' OR Name LIKE '%K65%' OR Name LIKE '%K95%'
    OR Name LIKE '%BlackWidow%'
    OR Name LIKE '%Huntsman%'
    OR Name LIKE '%TKL%'
    OR Name LIKE '%60%%' AND Name NOT LIKE '%mouse%'
    OR Name LIKE '%75%%' AND Name NOT LIKE '%mouse%'
    OR Name LIKE '%Anne Pro%'
    OR Name LIKE '%Drop ALT%' OR Name LIKE '%Drop CTRL%'
    OR Name LIKE '%Ducky%'
    OR Name LIKE '%Leopold%'
    OR Name LIKE '%Topre%'
    OR Name LIKE '%Mercury%'
);
GO

-- Fix Headphones: known headphone product names
UPDATE Products SET Category = 'Headphones' WHERE Category != 'Headphones' AND (
    Name LIKE '%headset%' OR Name LIKE '%headphone%'
    OR Name LIKE '%kraken%' OR Name LIKE '%cloud%'
    OR Name LIKE '%arctis%' OR Name LIKE '%nova%'
    OR Name LIKE '%hyperx%' OR Name LIKE '%HyperX%'
    OR Name LIKE '%wireless%' AND (Name LIKE '%audio%' OR Name LIKE '%ear%')
    OR Name LIKE '%Corsair HS%' OR Name LIKE '%Corsair Void%'
    OR Name LIKE '%Steelseries Arctis%'
    OR Name LIKE '%Razer Kraken%' OR Name LIKE '%Razer BlackShark%'
    OR Name LIKE '%blackshark%'
    OR Name LIKE '%A50%' OR Name LIKE '%A20%'
    OR Name LIKE '%GSP%'
    OR Name LIKE '%Logitech G%' AND Name LIKE '%headset%'
    OR Name LIKE '%Audio-Technica%'
);
GO

-- Verify final result
SELECT Category, COUNT(*) AS Total FROM Products GROUP BY Category ORDER BY Category;
SELECT Id, Name, Category FROM Products ORDER BY Category, Id;
GO
