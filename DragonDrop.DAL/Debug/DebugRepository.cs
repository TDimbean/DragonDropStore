using DragonDrop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.DAL.Debug
{
    public class DebugRepository
    {
        private DragonDrop_DbContext _context;

        public DebugRepository(DragonDrop_DbContext context) => _context = context;

        public void RemoveNewOrders()
        {
            var dateTreshhold = DateTime.Now.AddDays(-7);
            var ords = _context.Orders.Where(o => o.OrderDate > dateTreshhold).ToList();

            foreach (var ord in ords)
            {
                _context.Orders.Remove(ord);

                var items = _context.OrderItems.Where(i => i.OrderId == ord.OrderId).ToList();

                foreach (var item in items)
                {
                    _context.Products.Find(item.ProductId).Stock += item.Quantity;
                    _context.OrderItems.Remove(item);
                }
            }

            _context.SaveChanges();
        }

        public void ResetOrderStatuses()
        {
            var receivedOrdIds = new List<int> { 2, 4, 6 , 9, 18, 19, 2006, 3002, 3005};
            var processedOrdIds = new List<int> {1,3,5,10,13,15,1002,2005,5001,6003 };

            foreach (var id in receivedOrdIds)
            {
                var ord = _context.Orders.SingleOrDefault(o => o.OrderId == id);
                if (ord.OrderStatusId != 0)
                {
                    ord.OrderStatusId = 0;
                    var items = _context.OrderItems.Where(i => i.OrderId == id);
                    foreach (var item in items)
                    {
                        _context.Products.SingleOrDefault(p => p.ProductId == item.ProductId).Stock += item.Quantity;
                    }
                }
            }

            foreach (var id in processedOrdIds)
            {
                var ord = _context.Orders.SingleOrDefault(o => o.OrderId == id);
                ord.OrderStatusId = 1;
                ord.ShippingDate = null;
            }

            _context.SaveChanges();
        }

        public void RemoveNewCustomers()
        {
            var newCusts = _context.Customers.Where(c => c.CustomerId > 35);
            _context.Customers.RemoveRange(newCusts);
            _context.SaveChanges();
        }
    }
}
