using DragonDrop.DAL.Entities;
using DragonDrop.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestDb : DbContext
    {
        #region Brokin Ctor
        //    public TestDb()
        //    {
        //        Categories.AddRange(
        //            new List<Category>
        //            {
        //                new Category { CategoryId = 0, Name = "Miscelaneous" },
        //                new Category { CategoryId = 1, Name = "Traditional Games" },
        //                new Category { CategoryId = 2, Name = "Table Top RPG" },
        //                new Category { CategoryId = 3, Name = "Cube Puzzles" },
        //                new Category { CategoryId = 4, Name = "Dice" },
        //                new Category { CategoryId = 5, Name = "Tarrot" }
        //            });

        //        PaymentMethods.AddRange( 
        //            new List<PaymentMethod>
        //            {
        //                new PaymentMethod{PaymentMethodId = 1, Name="Credit Card" },
        //                new PaymentMethod{PaymentMethodId = 2, Name="Cash" },
        //                new PaymentMethod{PaymentMethodId = 3, Name="PayPal" },
        //                new PaymentMethod{PaymentMethodId = 4, Name="Wire Transfer" }
        //            });

        //        ShippingMethods.AddRange(
        //            new List<ShippingMethod>
        //            {
        //                new ShippingMethod{ ShippingMethodId=1, Name="FedEx" },
        //                new ShippingMethod{ ShippingMethodId=2, Name="DHL" },
        //                new ShippingMethod{ ShippingMethodId=3, Name="UPS" },
        //                new ShippingMethod{ ShippingMethodId=4, Name="Snail Mail" }
        //            });

        //        OrderStatuses.AddRange(
        //            new List<OrderStatus>
        //            {
        //                new OrderStatus{OrderStatusId=0, Name="Received" },
        //                new OrderStatus{OrderStatusId=1, Name="Processed" },
        //                new OrderStatus{OrderStatusId=2, Name="Shipped" },
        //                new OrderStatus{OrderStatusId=3, Name="Delivered" }
        //            });

        //        Customers.AddRange(
        //            new List<Customer>
        //            {
        //                new Customer
        //                {
        //                    CustomerId = 1,
        //                    Name = "Shayna Chaman",
        //                    Phone = "202-865-8027",
        //                    Email = "schaman0@hexun.com",
        //                    Address = "4871 Holy Cross Way",
        //                    City = "Washington",
        //                    State = "District of Columbia"
        //                },
        //                new Customer
        //                {
        //                    CustomerId = 2,
        //                    Name = "Regine Reimers",
        //                    Phone = "913-989-6845",
        //                    Email = "rreimers1@microsoft.com",
        //                    Address = "538 Thierer Court",
        //                    City = "Shawnee Mission",
        //                    State = "Kansas"
        //                },
        //                new Customer
        //                {
        //                    CustomerId = 3,
        //                    Name = "Ced Brasher",
        //                    Phone = "619-118-8476",
        //                    Email = null,
        //                    Address = "146 La Follette Crossing",
        //                    City = "San Diego",
        //                    State = "California"
        //                }
        //            });

        //        Products.AddRange(
        //            new List<Product>
        //            {
        //                new Product
        //                {
        //                    ProductId = 1,
        //                    BarCode = "876259219018",
        //                    Stock = 16,
        //                    Name = "DnD 5e Player's Handbook",
        //                    Description = "All the rules for newcomers to the Dungeons and Dragons RPG as well as details for series veterans.      Paperback",
        //                    UnitPrice = 25.59m,
        //                    CategoryId = 2
        //                },
        //                new Product
        //                {
        //                    ProductId = 2,
        //                    BarCode = "876259495023",
        //                    Stock = 25,
        //                    Name = "DnD 5e Dungeon Master's Guide",
        //                    Description = "Includes all the rules and guidelines for a Game Master to run their campaign smothly, with detailed     indexes     for conditions, maps and lots of encounter prompts. Paperback",
        //                    UnitPrice = 25.59m,
        //                    CategoryId = 2
        //                },
        //                new Product
        //                {
        //                    ProductId = 3,
        //                    BarCode = "885628572067",
        //                    Stock = 143,
        //                    Name = "Dnd 5e Monster Manual",
        //                    Description = "A to Z Bestiary with all the staple mobs found in the Forgotten Realms. Contains detailed descriptions,  stat     sheets and lore. Hardcover",
        //                    UnitPrice = 60.00m,
        //                    CategoryId = 2
        //                }
        //            });

        //        Orders.AddRange(
        //            new List<Order>
        //            {
        //                new Order
        //                {
        //                    OrderId = 1,
        //                    CustomerId = 1,
        //                    ShippingMethodId = 1,
        //                    PaymentMethodId = 1,
        //                    OrderDate = new DateTime(2019, 9 ,12),
        //                    ShippingDate = null,
        //                    OrderStatusId = 0
        //                },
        //                new Order
        //                {
        //                    OrderId = 2,
        //                    CustomerId = 2,
        //                    ShippingMethodId = 2,
        //                    PaymentMethodId = 2,
        //                    OrderDate = new DateTime(2019, 9, 10),
        //                    ShippingDate = new DateTime(2019, 9, 12),
        //                    OrderStatusId = 3
        //                },
        //                new Order
        //                {
        //                    OrderId = 3,
        //                    CustomerId = 3,
        //                    ShippingMethodId = 3,
        //                    PaymentMethodId = 3,
        //                    OrderDate = new DateTime(2019, 9, 13),
        //                    ShippingDate = null,
        //                    OrderStatusId = 2
        //                }
        //            });

        //        Payments.AddRange(
        //            new List<Payment>
        //            {
        //                new Payment
        //                {
        //                    PaymentId = 1,
        //                    CustomerId = 3,
        //                    Amount = 25.59m,
        //                    Date = new DateTime(2019, 9, 10),
        //                    PaymentMethodId = 1
        //                },
        //                new Payment
        //                {
        //                    PaymentId = 2,
        //                    CustomerId = 2,
        //                    Amount = 25.59m,
        //                    Date = new DateTime(2019, 9, 12),
        //                    PaymentMethodId = 2,
        //                },
        //                new Payment
        //                {
        //                    PaymentId = 3,
        //                    CustomerId = 1,
        //                    Amount = 60.00m,
        //                    Date = new DateTime(2019, 9, 13),
        //                    PaymentMethodId = 3
        //                }
        //            });

        //        OrderItems.AddRange(
        //            new List<OrderItem>
        //            {
        //                new OrderItem {OrderId = 1, ProductId = 3, Quantity = 2 },
        //                new OrderItem {OrderId = 2, ProductId = 2, Quantity = 1 },
        //                new OrderItem {OrderId = 3, ProductId = 1, Quantity = 3 }
        //            });
        //}
        #endregion

            public void BumpReceivedOrders()
        {
            var target1 = Generators.GenOrder();
            var target2 = Generators.GenOrder();
            var target3 = Generators.GenOrder();
            target1.OrderId = 4;
            target2.OrderId = 5;
            target3.OrderId = 6;
            target1.OrderStatusId = 0;
            target2.OrderStatusId = 0;
            target3.OrderStatusId = 0;
            target1.ShippingDate = null;
            target2.ShippingDate = null;
            target3.ShippingDate = null;

            Orders.AddRange(new List<Order> { target1, target2, target3 });
        }

        public List<Category> Categories = new List<Category>
        {
            new Category{ CategoryId = 0, Name = "Miscelaneous" },
            new Category{ CategoryId = 1, Name = "Traditional Games" },
            new Category{ CategoryId = 2, Name = "Table Top RPG" },
            new Category{ CategoryId = 3, Name = "Cube Puzzles" },
            new Category{ CategoryId = 4, Name = "Dice" },
            new Category{ CategoryId = 5, Name = "Tarrot" }
        };

        public List<PaymentMethod> PaymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod{PaymentMethodId = 1, Name="Credit Card" },
            new PaymentMethod{PaymentMethodId = 2, Name="Cash" },
            new PaymentMethod{PaymentMethodId = 3, Name="PayPal" },
            new PaymentMethod{PaymentMethodId = 4, Name="Wire Transfer" }
        };

        public List<ShippingMethod> ShippingMethods = new List<ShippingMethod>
        {
            new ShippingMethod{ ShippingMethodId=1, Name="FedEx" },
            new ShippingMethod{ ShippingMethodId=2, Name="DHL" },
            new ShippingMethod{ ShippingMethodId=3, Name="UPS" },
            new ShippingMethod{ ShippingMethodId=4, Name="Snail Mail" }
        };

        public List<OrderStatus> OrderStatuses = new List<OrderStatus>
        {
            new OrderStatus{OrderStatusId=0, Name="Received" },
            new OrderStatus{OrderStatusId=1, Name="Processed" },
            new OrderStatus{OrderStatusId=2, Name="Shipped" },
            new OrderStatus{OrderStatusId=3, Name="Delivered" }
        };

        public List<Customer> Customers = new List<Customer>
        {
            new Customer
            {
                CustomerId = 1,
                Name = "Shayna Chaman",
                Phone = "202-865-8027",
                Email = "schaman0@hexun.com",
                Address = "4871 Holy Cross Way",
                City = "Washington",
                State = "District of Columbia"
            },
            new Customer
            {
                CustomerId = 2,
                Name = "Regine Reimers",
                Phone = "913-989-6845",
                Email = "rreimers1@microsoft.com",
                Address = "538 Thierer Court",
                City = "Shawnee Mission",
                State = "Kansas"
            },
            new Customer
            {
                CustomerId = 3,
                Name = "Ced Brasher",
                Phone = "619-118-8476",
                Email = null,
                Address = "146 La Follette Crossing",
                City = "San Diego",
                State = "California"
            }
        };

        public List<Product> Products = new List<Product>
        {
            new Product
            {
                ProductId = 1,
                BarCode = "876259219018",
                Stock = 16,
                Name = "DnD 5e Player's Handbook",
                Description = "All the rules for newcomers to the Dungeons and Dragons RPG as well as details for series veterans. Paperback",
                UnitPrice = 25.59m,
                CategoryId = 2
            },
            new Product
            {
                ProductId = 2,
                BarCode = "876259495023",
                Stock = 25,
                Name = "DnD 5e Dungeon Master's Guide",
                Description = "Includes all the rules and guidelines for a Game Master to run their campaign smothly, with detailed indexes for conditions, maps and lots of encounter prompts. Paperback",
                UnitPrice = 25.59m,
                CategoryId = 2
            },
            new Product
            {
                ProductId = 3,
                BarCode = "885628572067",
                Stock = 143,
                Name = "Dnd 5e Monster Manual",
                Description = "A to Z Bestiary with all the staple mobs found in the Forgotten Realms. Contains detailed descriptions, stat sheets and lore. Hardcover",
                UnitPrice = 60.00m,
                CategoryId = 2
            }
        };

        public List<Order> Orders = new List<Order>
        {
            new Order
            {
                OrderId = 1,
                CustomerId = 1,
                ShippingMethodId = 1,
                PaymentMethodId = 1,
                OrderDate = new DateTime(2019, 9 ,12),
                ShippingDate = null,
                OrderStatusId = 0
            },
            new Order
            {
                OrderId = 2,
                CustomerId = 2,
                ShippingMethodId = 2,
                PaymentMethodId = 2,
                OrderDate = new DateTime(2019, 9, 10),
                ShippingDate = new DateTime(2019, 9, 12),
                OrderStatusId = 3
            },
            new Order
            {
                OrderId = 3,
                CustomerId = 3,
                ShippingMethodId = 3,
                PaymentMethodId = 3,
                OrderDate = new DateTime(2019, 9, 13),
                ShippingDate = null,
                OrderStatusId = 2
            }
        };

        public List<Payment> Payments = new List<Payment>
        {
            new Payment
            {
                PaymentId = 1,
                CustomerId = 3,
                Amount = 25.59m,
                Date = new DateTime(2019, 9, 10),
                PaymentMethodId = 1
            },
            new Payment
            {
                PaymentId = 2,
                CustomerId = 2,
                Amount = 25.59m,
                Date = new DateTime(2019, 9, 12),
                PaymentMethodId = 2,
            },
            new Payment
            {
                PaymentId = 3,
                CustomerId = 1,
                Amount = 60.00m,
                Date = new DateTime(2019, 9, 13),
                PaymentMethodId = 3
            }
        };

        public List<OrderItem> OrderItems = new List<OrderItem>
        {
            new OrderItem {OrderId = 1, ProductId = 3, Quantity = 2 },
            new OrderItem {OrderId = 2, ProductId = 2, Quantity = 1 },
            new OrderItem {OrderId = 3, ProductId = 1, Quantity = 3 }
        };


        //public List<Category> Categories { get; set; }
        //public List<PaymentMethod> PaymentMethods { get; set; }
        //public List<ShippingMethod> ShippingMethods { get; set; }
        //public List<OrderStatus> OrderStatuses { get; set; }
        //public List<Customer> Customers { get; set; }
        //public List<OrderItem> OrderItems { get; set; }
        //public List<Order> Orders { get; set; }
        //public List<Product> Products { get; set; }
        //public List<Payment> Payments { get; set; }

        // Barcodes from BumpProducts()  are most likely invalid, I only made them for the Get tests by messing around with the Manufacturer(first 6 digits of barcode) since the original Table takes the first 3 entries from the real db, and, those are too simillar
        public void BumpProducts()
            => Products.AddRange(
                new List<Product>
                {
                  new Product
                    {
                ProductId = 4,
                BarCode = "223122572067",
                Stock = 53,
                Name = "Fidget Spinner",
                Description = "Color: Orange",
                UnitPrice = 3.99m,
                CategoryId = 0
                    },
                    new Product
                    {
                ProductId = 5,
                BarCode = "113122572067",
                Stock = 23,
                Name = "Tarrot Reading for Dummies",
                Description = "A complete guide for all newcomers to the practice of Tarrot reading, with detailed instructions for over 50 types of spreads and an in-depth look at interpreting all the major Arcana.",
                UnitPrice = 12.35m,
                CategoryId = 5
                    },
                    new Product
                    {
                ProductId = 6,
                BarCode = "105122572067",
                Stock = 3,
                Name = "Hand-crafted Backgammon Set",
                Description = "12\"x15\"",
                UnitPrice = 49.95m,
                CategoryId = 1
                    }
                });

        public void BumpPayments() =>
            Payments.AddRange(new List<Payment>
            {
                new Payment
            {
                PaymentId = 4,
                CustomerId = 3,
                Amount = 120.39m,
                Date = new DateTime(2019, 3, 21),
                PaymentMethodId = 4
            },
            new Payment
            {
                PaymentId = 5,
                CustomerId = 3,
                Amount = 74.59m,
                Date = new DateTime(2019, 3, 17),
                PaymentMethodId = 2,
            },
            new Payment
            {
                PaymentId = 6,
                CustomerId = 2,
                Amount = 32.49m,
                Date = new DateTime(2019, 3, 19),
                PaymentMethodId = 2
            }
            });

        public void BumpPaymentsDouble()
        {
            BumpPayments();
            Payments.AddRange(new List<Payment>
            {
                new Payment
            {
                PaymentId = 7,
                CustomerId = 3,
                Amount = 63.49m,
                Date = new DateTime(2019, 6, 10),
                PaymentMethodId = 1
            },
            new Payment
            {
                PaymentId = 8,
                CustomerId = 3,
                Amount = 74.59m,
                Date = new DateTime(2019, 6, 12),
                PaymentMethodId = 2,
            },
            new Payment
            {
                PaymentId = 9,
                CustomerId = 3,
                Amount = 80.59m,
                Date = new DateTime(2019, 6, 15),
                PaymentMethodId = 3
            }
            });
        }

        public void BumpCustomers()
        {
            var newItems = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 4,
                    Name = "Cecille Herbert",
                    Phone = Generators.GenPhoneNumber(),
                    Address = "2B Yorheim Avenue",
                    Email = null,
                    City = "San Diego",
                    State = "California"
                },
                new Customer
                {
                    CustomerId = 5,
                    Name = "Will Smith",
                    Phone = Generators.GenPhoneNumber(),
                    Address = "3B Yorheim Avenue",
                    Email = null,
                    City = "San Diego",
                    State = "California"
                },
                new Customer
                {
                    CustomerId = 6,
                    Name = "Xi Yang",
                    Phone = Generators.GenPhoneNumber(),
                    Address = "4B Yorheim Avenue",
                    Email = null,
                    City = "San Diego",
                    State = "California"
                }
            };

            Customers.AddRange(newItems);
        }

        public void BumpOrders()
        {
            var newItems = new List<Order>
            {
                new Order
                    {
                        OrderId = 4,
                        CustomerId = 2,
                        ShippingMethodId = 3,
                        PaymentMethodId = 2,
                        OrderDate = new DateTime(2019, 9 ,29),
                        ShippingDate = null,
                        OrderStatusId = 0
                    },
                new Order
                    {
                        OrderId = 5,
                        CustomerId = 3,
                        ShippingMethodId = 2,
                        PaymentMethodId = 2,
                        OrderDate = new DateTime(2019, 9, 24),
                        ShippingDate = null,
                        OrderStatusId = 1
                    },
                new Order
                    {
                        OrderId = 6,
                        CustomerId = 2,
                        ShippingMethodId = 1,
                        PaymentMethodId = 2,
                        OrderDate = new DateTime(2019, 9, 22),
                        ShippingDate = new DateTime(2019, 9 ,24),
                        OrderStatusId = 2
                    }
                };

            Orders.AddRange(newItems);
        }

        public void BumpOrdersDouble()
        {
            BumpOrders();
            Orders.AddRange(new List<Order>
            {
                new Order
                {
                    OrderId = 7,
                    CustomerId = 2,
                    PaymentMethodId = 1,
                    ShippingMethodId = 1,
                    OrderDate = new DateTime(2019, 3, 12),
                    ShippingDate = new DateTime(2019, 3, 15),
                    OrderStatusId = 3
                },
                    new Order
                {
                    OrderId = 8,
                    CustomerId = 2,
                    PaymentMethodId = 1,
                    ShippingMethodId = 2,
                    OrderDate = new DateTime(2019, 3, 17),
                    ShippingDate = new DateTime(2019, 3, 19),
                    OrderStatusId = 3
                },
                    new Order
                {
                    OrderId = 9,
                    CustomerId = 2,
                    PaymentMethodId = 1,
                    ShippingMethodId = 3,
                    OrderDate = new DateTime(2019, 3, 21),
                    ShippingDate = new DateTime(2019, 3, 24),
                    OrderStatusId = 2
                },
            });
        }

        public void BumpOrderItems()
            => OrderItems.AddRange(new List<OrderItem>
            {
                new OrderItem {OrderId=1, ProductId = 2, Quantity = 5},
                new OrderItem {OrderId=2, ProductId = 1, Quantity = 7},
                new OrderItem {OrderId=3, ProductId = 2, Quantity = 12},
            });
    }
}
