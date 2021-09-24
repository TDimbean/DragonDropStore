using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestOrderStatusRepo : IOrderStatusRepository
    {
        private TestDb _context;

        public TestOrderStatusRepo(TestDb context) => _context = context;

        public OrderStatus Get(int id) => _context.OrderStatuses.SingleOrDefault(s => s.OrderStatusId == id);

        public string GetStatusName(int id)
        {
            var stat = Get(id);
            if (stat == null) return null;
            return stat.Name;
        }

        public void Update(OrderStatus stat, bool isSwap=false)
        {
            if (GetStatusId(stat.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(stat.OrderStatusId).Name = stat.Name;
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Order Status, but found none with ID: " + stat.OrderStatusId + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Order Status, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public void Remove(int id) => _context.OrderStatuses.Remove(Get(id));

        public int? GetStatusId(string name)
        {
            var stat = _context.OrderStatuses.SingleOrDefault(s => s.Name == name);
            return stat == null ? (int?)null : stat.OrderStatusId;
        }

        public void Add(string statName) => _context.OrderStatuses.Add(new OrderStatus { Name = statName });

        public List<OrderStatus> GetAll() => _context.OrderStatuses.ToList();
    }
}
