CREATE TABLE [dbo].[Products]
(
	ProductId INT NOT NULL IDENTITY PRIMARY KEY,
	CategoryId INT NOT NULL,
	Name nvarchar(50) NOT NULL,
	Description nvarchar(300),
	Stock INT NOT NULL,
	UnitPrice DECIMAL (6,2) NOT NULL,
	BarCode nchar(12) NOT NULL
);

CREATE TABLE [dbo].[Categories]
(
	CategoryId INT NOT NULL IDENTITY PRIMARY KEY,
	Name nvarchar(20)
);


CREATE TABLE [dbo].[PaymentMethods]
(
	PaymentMethodId int NOT NULL IDENTITY PRIMARY KEY,
	Name nvarchar(50)
);

CREATE TABLE [dbo].[ShippingMethods]
(
	ShippingMethodId INT NOT NULL IDENTITY PRIMARY KEY,
	Name nvarchar(50)
);

CREATE TABLE [dbo].[Customers]
(
	CustomerId INT NOT NULL IDENTITY PRIMARY KEY,
	Name nvarchar(100) NOT NULL,
	Phone nchar(12) NOT NULL,
	Email nvarchar(100),
	Address nvarchar(200) NOT NULL,
	City nvarchar(100) NOT NULL,
	State nvarchar(50) NOT NULL
);

CREATE TABLE [dbo].[Payments]
(
	PaymentId INT NOT NULL IDENTITY PRIMARY KEY,
	CustomerId INT NOT NULL,
	Date date NOT NULL,
	Ammount decimal(7,2) NOT NULL,
	PaymentMethodId INT NOT NULL 
);


CREATE TABLE [dbo].[OrderStatuses]
(
	OrderStatusId INT NOT NULL IDENTITY PRIMARY KEY,
	Name nvarchar(15)
);

CREATE TABLE [dbo].[Orders]
(
	OrderId INT NOT NULL IDENTITY PRIMARY KEY,
	CustomerId INT NOT NULL,
	OrderDate date NOT NULL,
	ShippingDate date,
	OrderStatusId INT NOT NULL,
	ShippingMethodId INT NOT NULL,
	PaymentMethodId INT NOT NULL
);

CREATE TABLE [dbo].[OrderItems]
(
	OrderId INT NOT NULL,
	ProductId INT NOT NULL,
	Quantity INT NOT NULL,
	CONSTRAINT OrderItem_CompKey PRIMARY KEY 
	(OrderId, ProductId)
);

SET IDENTITY_INSERT Categories ON
INSERT INTO Categories(CategoryId, Name) VALUES (0, 'Miscelaneous')
INSERT INTO Categories(CategoryId, Name) VALUES (1, 'Traditional Games')
INSERT INTO Categories(CategoryId, Name) VALUES (2, 'Table Top RPG')
INSERT INTO Categories(CategoryId, Name) VALUES (3, 'Cube Puzzles')
INSERT INTO Categories(CategoryId, Name) VALUES (4, 'Dice')
INSERT INTO Categories(CategoryId, Name) VALUES (5, 'Tarrot')
SET IDENTITY_INSERT Categories OFF

SET IDENTITY_INSERT Products ON
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (1, '876259219018', 16, 'DnD 5e Player''s Handbook', 'All the rules for newcomers to the Dungeons and Dragons RPG as well as details for series veterans. Paperback', 25.59, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (2, '876259495023', 25, 'DnD 5e Dungeon Master''s Guide', 'Includes all the rules and guidelines for a Game Master to run their campaign smothly, with detailed indexes for conditions, maps and lots of encounter prompts. Paperback', 25.59, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (3, '885628572067', 143, 'Dnd 5e Monster Manual', 'A to Z Bestiary with all the staple mobs found in the Forgotten Realms. Contains detailed descriptions, stat sheets and lore. Hardcover', 60.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (4, '876259326037', 26, 'DnD 5e Campaign Book: Tales of the Yawning Portal', 'A new adventure to take your party on. Contains 89 wondefully illustrated pages of Lore, Maps and encounters. 6 new spells, 2 playble classes and 14 monsters from Unearthed Arcana. Reccommended level: 4-5. Paperback.', 43.29, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (5, '876259970049', 140, 'DnD 5e Campaign Book: Meltazar''s Trials', 'A short but sweet campaign that will prove both a learning experience and unforgetable step in your party''s journey. Storyline co-written by Joe Mimble. Reccomended level 5-7 party. Paperback.', 35.69, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (6, '876259901050', 65, 'Table Top Figurine Set: 5 Heroes', ' Contains: Archer, Fighter, Cleric, Sorcerer and Thief figurines.', 52.39, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (7, '885628199066', 7, 'Table Top Figurine: Archer', 'Enhance your roleplaying experience with the new line of 3d  printed and hand painted figurines by DungeonDelvers(TM). 5 inches, 10 oz. Model: Archer/Ranger', 15.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (8, '885628572043', 102, 'Table Top Figurine: Fighter', 'Enhance your roleplaying experience with the new line of 3d  printed and hand painted figurines by DungeonDelvers(TM). 5 inches, 10 oz. Model: Fighter/Barbarian', 15.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (9, '885628572050', 126, 'Table Top Figurine: Cleric', 'Enhance your roleplaying experience with the new line of 3d  printed and hand painted figurines by DungeonDelvers(TM). 5 inches, 10 oz. Model: Cleric/Priest', 15.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (10, '885628572074', 119, 'Table Top Figurine: Sorcerer', 'Enhance your roleplaying experience with the new line of 3d  printed and hand painted figurines by DungeonDelvers(TM). 5 inches, 10 oz. Model: Sorcerer/Mage', 15.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (11, '885628572081', 18, 'Table Top Figurine: Thief', 'Enhance your roleplaying experience with the new line of 3d  printed and hand painted figurines by DungeonDelvers(TM). 5 inches, 10 oz. Model: Thief, Rogue', 15.00, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (12, '969475009010', 24, 'Acrylic Dice Set: Flaming Red', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (13, '969475009027', 51, 'Acrylic Dice Set: Lapis Enchanted', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (14, '969475009034', 83, 'Acrylic Dice Set: Crimson', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (15, '969475009041', 16, 'Acrylic Dice Set: Druid Emerald', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (16, '969475009058', 9, 'Acrylic Dice Set: OceanBorn Turqouise', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (17, '969475009065', 53, 'Acrylic Dice Set: Smokey Gray', 'Expertly painted and well-balanced role-playing dice. Contains 1 of each: D4, D6, D8, D10, D12, D20', 13.25, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (18, '510778909103', 140, 'Metal Dice Set', 'Perfeclty balanced, forged with real steel alloy and painted in gold leaf, these die are essential for any serious table-top gamer. Contains 1 of each: two-sided coin, D4, D6, D8, D10, D12, D20', 24.39, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (19, '510778796178', 29, 'Giant D20', 'Feel the weight of your decissions with the giant twenty sided die.', 10.00, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (20, '510778794129', 142, 'Loaded D20(Lucky)', 'Tired of your players runing the flow of your campaign by performing amazing feats out of nowhere or failing to achieve even the most basic of tasks? Take back control of your story-telling with the official CheatingFox(TM) dice. Balanced to hit high numbers8', 7.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (21, '510777941364', 72, 'Loaded D20(Unlucky)', 'Tired of your players runing the flow of your campaign by performing amazing feats out of nowhere or failing to achieve even the most basic of tasks? Take back control of your story-telling with the official CheatingFox(TM) dice. Balanced to hit low numbers', 7.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (22, '510778045184', 126, 'Double Critical D20(Success)', 'Tired of your players runing the flow of your campaign by performing amazing feats out of nowhere or failing to achieve even the most basic of tasks? Take back control of your story-telling with the official CheatingFox(TM) dice. Both 1 and 20 are now 20.', 7.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (23, '510778045191', 7, 'Double Critical D20(Failure)', 'Tired of your players runing the flow of your campaign by performing amazing feats out of nowhere or failing to achieve even the most basic of tasks? Take back control of your story-telling with the official CheatingFox(TM) dice. Both 1 and 20 are now 1', 7.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (24, '115558974021', 55, 'Dice Poker Set - Black and White', 'Contains 5 standard dotted D6', 4.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (25, '115558987045', 22, 'Dice Poker Set - White and Red', 'Contains 5 standard dotted D6', 4.50, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (26, '410641794044', 119, 'Plastic D100', 'Sometimes 2 D10''s aren''t enough. May the odds be ever in your favour.', 10.99, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (27, '303148423015', 140, 'Metal D100', 'The king of all dice. Perfect for rolling Wild Magic Surges or attacking with behemoth monsters. True black-steel finish and gold-leaf numbers. 12 oz', 14.99, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (28, '303148660052', 72, 'Dice Mill', 'Tired of rolling hand after hand for multiple mobs or gint boss attacks. Improve your gameflow with the Dice Mill. Make each drop of the die have the impact of a judge''s gavel!', 17.89, 2);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (29, '879724934034', 5, 'Poker Set', 'Contains two packs of nylon cards, 5 transparent dotted D6s and 200 chips(10x500, 20x100, 20x50, 40x20, 30x10, 10x5, 50x1).', 35.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (30, '879724832095', 22, 'Poker Chip Set', 'Contains:  200 chips(10x500, 20x100, 20x50, 40x20, 30x10, 10x5, 50x1).', 14.49, 0);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (31, '541828461043', 39, 'Card Deck', 'Standard issue Poker Card Deck. Nylon. 2xJoker.', 5.49, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (32, '244969465993', 13, 'Deluxe Card Deck', 'Made from 310 GSM Card Stock . Water ressistant. Competition sied. Great handling', 12.35, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (33, '541828396277', 144, 'Classic Tarrot Deck', 'All 78 cards from the Major and Minor Arcana printed on high quality, fold-ressistant cardborad.', 7.89, 5);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (34, '148183740204', 128, 'GoT themed Tarrot Deck', 'Your destiny reconteqtualized with the world-reknown characters of the GoT TV show. Contains all 78 Major and Minor Arcana beautifully illustrated by Daren Forelay and Hannah Scotts.', 54.69, 5);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (35, '148183371293', 110, 'Harry Potter themed Tarrot Deck', 'All 22 Major Arcana as represented by the beloved characters in the HP book and film series. Enchanting illustrations done by Scott Tailor. Comes with a papreback handbook packed with instructions on interpretation, shuffling and spliting.', 34.59, 5);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (36, '665038106232', 97, 'Magnetic Chess Set', 'Take your strategic mastery wherever you go. On a windy mountain-top, on a bumpy road or on a rocking boat, the pieces in this set will stick to the gameboard like glue so you never have a worry.', 19.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (37, '665038649234', 77, 'Plastic Chess Set', 'Ivory and Ebony textured chess set. 3D printed with the latest technologies and best plastics. 2 small turn timers included.', 9.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (38, '804919753206', 118, 'Laquered Chess Set', 'Hand-carved oak-wood and ash-wood pieces covered in a fine laquer with accents painted in gold leaf. Comes with matching hourglass', 24.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (39, '665038961206', 102, 'Checkers Set', 'Standard checkers set. Comes with 5 spare pieces for each colour and a set of two die.', 5.49, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (40, '804919598265', 132, 'Shougi Set', 'Explore the Japanese variation of chess with this hand-made wood set. Contains 4 blank spare pieces.', 6.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (41, '804919194276', 21, 'Go Set', 'Basic Go set with wooden board and plastic stones and cups.', 34.99, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (42, '614534160224', 34, 'MoYu WeiLong 3x3x3 V2 Black', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (43, '614534160231', 85, 'MoYu WeiLong 3x3x3 V2 White', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (44, '614534160248', 42, 'MoYu WeiLong 3x3x3 V2 Pink', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (45, '614534160255', 132, 'MoYu WeiLong 3x3x3 V2 Green', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (46, '614534160262', 41, 'MoYu WeiLong 3x3x3 V2 Blue', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (47, '614534160279', 41, 'MoYu WeiLong 3x3x3 V2 Orange', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (48, '614534160286', 121, 'MoYu WeiLong 3x3x3 V2 Red', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (49, '614534160293', 106, 'MoYu WeiLong 3x3x3 V2 mini White', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (50, '614534160309', 136, 'MoYu WeiLong 3x3x3 V2 mini Black', 'Revolutionary corner-cutting technology that assures your pieces won''t pop out even when cutting 45 degree angles. Endores by the World Record holder: Felix Zemdegs(4.73s)', 12.35, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (51, '040239814184', 58, 'ShenShui 3x3x3 Black', 'Perfectly smooth turning with more ressistant springs adjusted to the perfect tension. Comes with fold-out instructions covering the basic alghorytms.', 11.50, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (52, '040239814191', 29, 'ShenShui 3x3x3 White', 'Perfectly smooth turning with more ressistant springs adjusted to the perfect tension. Comes with fold-out instructions covering the basic alghorytms.', 11.50, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (53, '199713359182', 30, 'Mirror Cube 3x3x3', 'Tired of standard cubes. Prove your mastery by matching these colouress fine-grained silver texture of this magic cube by their differing sizes and shapes.', 7.59, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (54, '296810630197', 13, 'Spare 3x3x3 Stickers', 'Contains five sheets of adhesive replacement stickers. Colours: White, Yellow, Red, Orange, Green and Blue', 3.25, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (55, '296810034131', 90, 'Speedcubing Timer', 'Competition grade timer. Ressistant hand rest and accurate to the microsecond.', 24.99, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (56, '296810414124', 149, 'Rubik''s Cube Silicone-based Lubricant (50ml)', 'Enhance the performance of your cube and ensure the springs and cogs don''t wear out.', 7.59, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (57, '614534705128', 24, 'MoYu WeiPo 2x2x2 White', 'The WeiPo has a more substantial feel than other 2x2x2, making it esasier to control.', 8.50, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (58, '614534705135', 102, 'Moyu WeiPo 2x2x2 Black', 'The WeiPo has a more substantial feel than other 2x2x2, making it esasier to control.', 8.50, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (59, '614534052192', 71, 'MoYu AoSu 4x4x4 White', 'Take your puzzle mastery to the next level with the AoSu 4x4x4. A well tensioned cored gives it a smooth yet substantial feel. All of your favorite 3x3x3 speed-solving alghorhytms can carry over, with the addition of new wing-oriented swaps.', 17.45, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (60, '614534052185', 8, 'MoYu AoSu 4x4x4 Black', 'Take your puzzle mastery to the next level with the AoSu 4x4x4. A well tensioned cored gives it a smooth yet substantial feel. All of your favorite 3x3x3 speed-solving alghorhytms can carry over, with the addition of new wing-oriented swaps.', 17.45, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (61, '654623338004', 114, 'Yuxin 4x4x4 Fluorescent', 'The Yuxin 4x4x4 is made to last. Lube-serviced and tension-adjusted, you can bet this puzzle won''t let you down. The Flurescent stickers give it an enchanted shine that carries even in low light conditions.', 15.69, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (62, '753805150233', 96, 'MeiLong 5x5x5 6-Colours', 'Stickerless 5x5x5 ready for speedcubing. The highest challenge, combinig all alghorhytms from smaller cube puzzle configurations.', 24.49, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (63, '654623392259', 50, 'Yuxin 5x5x5 Fluorescent', 'The Yuxin 5x5x5 is made to offer a difficult yet rewarding test. Lube-serviced and tension-adjusted, it''s reliable out of the box. The Flurescent stickers give it an enchanted shine that carries even in low light conditions.', 27.49, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (64, '753805983213', 28, 'MoFang Pyramix', 'Tackle an unorthodox, yet familiar puzzle with the MoFang Pyraminx.', 15.89, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (65, '654623983211', 96, 'Yuxin Carbon-Fiber Pyraminx', 'The perfect puzzle to sit on top of your cube game collection, this Pyramix is made with the lightest yet most ressilient materials comercially available.', 19.99, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (66, '699914420266', 125, 'ShengShou Megaminx', 'Push past your speed-solving limits with the Dodecahedron 3x3 puzzle.', 25.99, 3);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (67, '804919273216', 136, 'Wooden 9-Man Morris Set', 'A game as old as time. Fine wood-carved board with plastic stones(3 spares for each colour).', 13.49, 1);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (68, '510778273211', 56, 'Assorted Dice (1 pound)', 'You can never have enough dice in your campaign. Random assortment of dice of all configurations and colours. Guaranteed to have 10xD20s, one of which is metal. Comes in a convenient hemp fiber drawstring bag.', 49.99, 4);
INSERT INTO Products(ProductId, BarCode, Stock, Name, Description, UnitPrice, CategoryId) VALUES (69, '296810273219', 13, 'Cube and Dice Drawstring Bag', 'Hiqh quality carrying drawstring bag that fits in a purse or large pocket. Envelop your gear in the finest of protective fabrics.', 7.49, 0);
SET IDENTITY_INSERT Products OFF

SET IDENTITY_INSERT PaymentMethods ON
INSERT INTO PaymentMethods(PaymentMethodId, Name) VALUES(1, 'Credit Card')
INSERT INTO PaymentMethods(PaymentMethodId, Name) VALUES(2, 'Cash')
INSERT INTO PaymentMethods(PaymentMethodId, Name) VALUES(3, 'PayPal')
INSERT INTO PaymentMethods(PaymentMethodId, Name) VALUES(4, 'Wire Transfer')
SET IDENTITY_INSERT PaymentMethods OFF

SET IDENTITY_INSERT ShippingMethods ON
INSERT INTO ShippingMethods(ShippingMethodId, Name) VALUES (1, 'FedEx')
INSERT INTO ShippingMethods(ShippingMethodId, Name) VALUES (2, 'DHL')
INSERT INTO ShippingMethods(ShippingMethodId, Name) VALUES (3, 'UPS')
INSERT INTO ShippingMethods(ShippingMethodId, Name) VALUES (4, 'Snail Mail')
SET IDENTITY_INSERT ShippingMethods OFF

SET IDENTITY_INSERT Payments ON
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (1, 1, '2/16/2019', 352.44, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (2, 2,'5/4/2017', 289.13, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (3, 3,'9/28/2017', 669.84, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (4, 4, '10/25/2018', 238.33, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (5, 5, '7/16/2017', 1236.01, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (6, 6, '7/22/2017', 1119.44, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (7, 7, '8/6/2017', 881.78, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (8, 8, '12/12/2018', 1532.56, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (9, 9, '3/16/2018', 686.7, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (10, 10, '9/18/2016', 430.67, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (11, 11, '12/30/2017', 853.64, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (12, 12, '4/3/2017', 1176.64, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (13, 13, '9/24/2016', 1473.11, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (14, 14, '11/15/2018', 798.01, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (15, 15, '2/23/2019', 1289.1, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (16, 16, '12/15/2016', 167.57, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (17, 17, '1/27/2017', 1583.19, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (18, 18, '3/2/2019', 419.61, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (19, 19, '3/14/2018', 817.13, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (20, 20, '1/17/2019', 671.71, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (21, 21, '4/9/2018', 1517.26, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (22, 22, '4/6/2019', 979.54, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (23, 23, '2/6/2019', 1218.03, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (24, 24, '5/19/2019', 869.54, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (25, 25, '5/11/2018', 1269.23, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (26, 26, '6/20/2018', 431.08, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (27, 27, '9/27/2018', 1288.69, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (28, 28, '4/26/2019', 21.17, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (29, 29, '6/12/2019', 908.81, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (30, 30, '4/5/2019', 929.25, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (31, 31, '3/14/2017', 1525.83, 1);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (32, 32, '10/19/2017', 1507.63, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (33, 33, '3/13/2019', 304.8, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (34, 34, '1/21/2018', 817.84, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (35, 35, '2/8/2018', 370.64, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (36, 1, '10/26/2016', 1567.6, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (37, 2, '1/19/2017', 374.73, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (38, 3, '2/24/2017', 1188.36, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (39, 4, '2/1/2017', 712.93, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (40, 5, '1/1/2017', 1034.2, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (41, 6, '2/9/2019', 502.97, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (42, 7, '5/2/2019', 473.19, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (43, 8, '4/2/2019', 388.39, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (44, 9, '3/3/2017', 544.96, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (45, 10, '12/2/2018', 530.97, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (46, 11, '7/12/2017', 968.61, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (47, 12, '9/28/2016', 318.19, 4);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (48, 13, '10/16/2018', 167.51, 3);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (49, 4, '6/4/2019', 1523.15, 2);
INSERT INTO Payments (PaymentId, CustomerId, Date, Ammount, PaymentMethodId) VALUES (50, 5, '3/4/2018', 174.31, 4);
SET IDENTITY_INSERT Payments OFF

SET IDENTITY_INSERT Customers ON
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (1, 'Shayna Chaman', '202-865-8027', 'schaman0@hexun.com', '4871 Holy Cross Way', 'Washington', 'District of Columbia');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (2, 'Regine Reimers', '913-989-6845', 'rreimers1@microsoft.com', '538 Thierer Court', 'Shawnee Mission', 'Kansas');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (3, 'Ced Brasher', '619-118-8476', null, '146 La Follette Crossing', 'San Diego', 'California');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (4, 'Pearle Todarini', '573-558-4246', 'ptodarini3@huffingtonpost.com', '7 Troy Road', 'Columbia', 'Missouri');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (5, 'Brion Ridewood', '502-259-3691', null, '7327 Hanover Parkway', 'Louisville', 'Kentucky');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (6, 'Culver Haythorne', '704-470-6377', 'chaythorne5@tamu.edu', '2 Susan Court', 'Winston Salem', 'North Carolina');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (7, 'Caroljean Goulden', '812-161-5317', 'cgoulden6@google.it', '79442 Melody Crossing', 'Evansville', 'Indiana');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (8, 'Kelvin Harbottle', '316-324-5700', 'kharbottle7@blogtalkradio.com', '05913 Graedel Court', 'Wichita', 'Kansas');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (9, 'Andras Basso', '908-871-4028', 'abasso8@sciencedirect.com', '006 Ruskin Hill', 'Elizabeth', 'New Jersey');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (10, 'Oren Harris', '830-283-0326', null, '0 Sachtjen Lane', 'San Antonio', 'Texas');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (11, 'Valaree Purchon', '203-397-4036', 'vpurchona@bing.com', '59946 Kensington Alley', 'Waterbury', 'Connecticut');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (12, 'Crosby Barday', '915-510-8204', 'cbardayb@sphinn.com', '7345 Westerfield Parkway', 'El Paso', 'Texas');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (13, 'Collin People', '570-981-0158', 'cpeoplec@reference.com', '59273 Pepper Wood Circle', 'Scranton', 'Pennsylvania');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (14, 'Brenna Coners', '209-157-5847', 'bconersd@zimbio.com', '4 Commercial Drive', 'Stockton', 'California');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (15, 'Shoshanna Stonhewer', '309-219-8907', 'sstonhewere@gnu.org', '41 Mayer Trail', 'Carol Stream', 'Illinois');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (16, 'Bearnard Fairholme', '704-645-4565', 'bfairholmef@live.com', '47711 Linden Road', 'Charlotte', 'North Carolina');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (17, 'Robin Andreev', '206-379-5077', 'randreevg@ezinearticles.com', '06 Brentwood Center', 'Seattle', 'Washington');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (18, 'Bartlett Lewerenz', '954-702-2999', null, '0004 Shoshone Center', 'Fort Lauderdale', 'Florida');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (19, 'Glori Warlowe', '612-876-6940', null, '9335 Commercial Lane', 'Minneapolis', 'Minnesota');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (20, 'Aldon Cristou', '626-457-9435', 'acristouj@addtoany.com', '9 Maywood Plaza', 'Van Nuys', 'California');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (21, 'Abbe Sidnell', '315-990-5621', 'asidnellk@virginia.edu', '9068 Esker Parkway', 'Utica', 'New York');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (22, 'Mary Korbmaker', '570-407-9549', 'mkorbmakerl@latimes.com', '57069 Northwestern Street', 'Scranton', 'Pennsylvania');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (23, 'Tully Blindmann', '615-938-5689', 'tblindmannm@1688.com', '25 Dayton Park', 'Nashville', 'Tennessee');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (24, 'Karlan Walby', '970-122-4060', 'kwalbyn@netvibes.com', '01 Village Court', 'Grand Junction', 'Colorado');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (25, 'Charline Triner', '570-808-6002', 'ctrinero@businessinsider.com', '3922 Old Gate Point', 'Wilkes Barre', 'Pennsylvania');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (26, 'Berky Caldairou', '510-115-2207', 'bcaldairoup@360.cn', '2 Killdeer Point', 'Oakland', 'California');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (27, 'Madlin Facey', '717-119-5074', 'mfaceyq@va.gov', '23936 Hagan Parkway', 'Harrisburg', 'Pennsylvania');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (28, 'Sarina Devaney', '253-772-1412', 'sdevaneyr@go.com', '6 Knutson Crossing', 'Tacoma', 'Washington');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (29, 'Gregg Storrie', '601-834-5654', 'gstorries@a8.net', '4 Sloan Drive', 'Hattiesburg', 'Mississippi');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (30, 'Sapphira Smalles', '702-646-4593', null, '5161 Buena Vista Place', 'Las Vegas', 'Nevada');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (31, 'Smith-Ebert Table Top Supplies', '202-315-2899', 'vhowler0@1und1.de', '8 Kings Point', 'Washington', 'District of Columbia');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (32, 'UberPlay Group', '860-634-5773', 'smarco.supply@uber-pg.com', '00323 Toban Circle', 'Hartford', 'Connecticut');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (33, 'The Pen, Paper and Die', '816-397-4481', 'mleech2.ppd@mozilla.org', '60740 Hudson Junction', 'Kansas City', 'Missouri');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (34, 'Hansen Player''s', '469-499-8104', 'hansenplay_logistics@hansenmedia.com', '2834 Corben Center', 'Dallas', 'Texas');
INSERT INTO Customers (CustomerId, Name, Phone, Email, Address, City, State) VALUES (35, 'Hettinger Joux', '212-597-4730', 'hettinger_supply@gmail.edu', '9763 Ronald Regan Hill', 'New York City', 'New York');
SET IDENTITY_INSERT Customers OFF

SET IDENTITY_INSERT OrderStatuses ON
INSERT INTO OrderStatuses (OrderStatusId, Name) VALUES (0, 'Received')
INSERT INTO OrderStatuses (OrderStatusId, Name) VALUES (1, 'Processed')
INSERT INTO OrderStatuses (OrderStatusId, Name) VALUES (2, 'Shipped')
INSERT INTO OrderStatuses (OrderStatusId, Name) VALUES (3, 'Delivered')
SET IDENTITY_INSERT OrderStatuses OFF

SET IDENTITY_INSERT Orders ON
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (1, 16, '8/28/2019', null, 1, 2, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (2, 31, '5/26/2019', null, 0, 1, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (3, 15, '8/29/2019', null, 1, 4, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (4, 24, '9/10/2019', null, 0, 1, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (5, 2, '8/29/2019', null, 1, 1, 3);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (6, 27, '8/23/2019', null, 0, 1, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (7, 23, '1/19/2018', '1/14/2018', 3, 2, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (8, 12, '3/28/2017', '4/1/2017', 3, 1, 3);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (9, 6, '9/9/2019', null, 0, 3, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (10, 17, '6/9/2019', null, 1, 3, 3);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (11, 13, '3/5/2019', '3/8/2019', 2, 2, 3);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (12, 29, '6/22/2017', '6/25/2017', 3, 1, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (13, 17, '7/7/2019', null, 1, 1, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (14, 21, '8/3/2017', '10/9/2017', 3, 2, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (15, 21, '10/22/2017', null, 1, 3, 1);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (16, 22, '5/31/2017', '6/17/2017', 3, 4, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (17, 6, '7/10/2018', '6/20/2018', 3, 4, 4);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (18, 8, '9/11/2019', null, 0, 3, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (19, 28, '9/12/2019', null, 0, 2, 2);
INSERT INTO Orders (OrderId, CustomerId, OrderDate, ShippingDate, OrderStatusId, ShippingMethodId, PaymentMethodId) VALUES (20, 32, '8/3/2017', '8/5/2017', 3, 3, 4);
SET IDENTITY_INSERT Orders OFF

INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 68, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (9, 21, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (17, 9, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (13, 67, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (4, 26, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (6, 19, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 27, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (20, 21, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 47, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 16, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (1, 64, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (16, 15, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 53, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 40, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (18, 2, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (18, 49, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (20, 18, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 45, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (15, 23, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (6, 12, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (18, 7, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (14, 15, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (7, 8, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 61, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 11, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 28, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (14, 6, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 67, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (9, 2, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (20, 58, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 26, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 61, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 34, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 35, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (9, 42, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (20, 47, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (15, 24, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 66, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (4, 4, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (10, 6, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (4, 6, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (1, 34, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (10, 55, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (20, 49, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 37, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 53, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (12, 36, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (1, 65, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 36, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (14, 63, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 13, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (7, 32, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (7, 65, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (12, 50, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 26, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (1, 25, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (7, 61, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (4, 68, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (14, 20, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (16, 45, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (18, 23, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (11, 25, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 60, 1);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (6, 34, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (11, 32, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (10, 54, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 48, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (18, 58, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (15, 58, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 58, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (3, 69, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (9, 12, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (1, 12, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (8, 23, 3);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 41, 2);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (19, 25, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (2, 22, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (5, 68, 4);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (16, 66, 5);
INSERT INTO OrderItems(OrderId, ProductId, Quantity) VALUES (17, 51, 5);
