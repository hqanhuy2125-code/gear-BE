-- Fix product categories based on ImageUrl filename patterns
-- Mirrors the exact same logic in src/data/products.js

-- Step 1: Reset bad/placeholder rows to Accessories as default
UPDATE Products
SET Category = 'Accessories'
WHERE Category IN ('string', 'Uncategorized', '', 'accessories')
   OR Category IS NULL;

-- Step 2: Mice — mouse files (excluding mousepad), b3, a3, bh patterns
UPDATE Products
SET Category = 'Mice'
WHERE (
    (ImageUrl LIKE '%mouse%' AND ImageUrl NOT LIKE '%mousepad%')
    OR ImageUrl LIKE '%-b3%' OR ImageUrl LIKE '%_b3%'
    OR ImageUrl LIKE '%-a3%' OR ImageUrl LIKE '%_a3%'
    OR ImageUrl LIKE '%-bh%' OR ImageUrl LIKE '%_bh%'
    OR Name LIKE '%mouse%'
    OR Name LIKE '%viper%'
    OR Name LIKE '%deathadder%'
    OR Name LIKE '%superlight%'
    OR Name LIKE '%g pro x%'
    OR Name LIKE '%razer viper%'
);

-- Step 3: Keyboards — keyboard, mercury patterns
UPDATE Products
SET Category = 'Keyboards'
WHERE (
    ImageUrl LIKE '%keyboard%'
    OR ImageUrl LIKE '%mercury%'
    OR Name LIKE '%keyboard%'
    OR Name LIKE '%keychron%'
    OR Name LIKE '%apex pro%'
    OR Name LIKE '%steelseries apex%'
    OR Name LIKE '%wooting%'
    OR Name LIKE '%hhkb%'
);

-- Step 4: Headphones — audio/headset patterns
UPDATE Products
SET Category = 'Headphones'
WHERE (
    ImageUrl LIKE '%headset%'
    OR ImageUrl LIKE '%headphone%'
    OR ImageUrl LIKE '%audio%'
    OR ImageUrl LIKE '%gsp%'
    OR ImageUrl LIKE '%gpro%'
    OR ImageUrl LIKE '%a50%'
    OR ImageUrl LIKE '%a20%'
    OR ImageUrl LIKE '%corsair%'
    OR ImageUrl LIKE '%razer%' AND (ImageUrl LIKE '%headset%' OR ImageUrl LIKE '%kraken%')
    OR ImageUrl LIKE '%nova7%'
    OR ImageUrl LIKE '%marshal%'
    OR ImageUrl LIKE '%hyperx%'
    OR ImageUrl LIKE '%rkv3%'
    OR Name LIKE '%headset%'
    OR Name LIKE '%headphone%'
    OR Name LIKE '%kraken%'
    OR Name LIKE '%cloud%'
    OR Name LIKE '%arctis%'
);

-- Step 5: Verify the result
SELECT Category, COUNT(*) AS Total
FROM Products
GROUP BY Category
ORDER BY Category;

GO
