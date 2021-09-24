using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.DAL.Implementation.Resources
{
    public class OrderStatusRepository : IOrderStatusRepository, IResourceRepo
    {
        private DragonDrop_DbContext _context;

        public OrderStatusRepository(DragonDrop_DbContext context) => _context = context;

        public List<OrderStatus> GetAll() => _context.OrderStatuses.ToList();

        public OrderStatus Get(int id) => _context.OrderStatuses.SingleOrDefault(s => s.OrderStatusId == id);

        public string GetStatusName(int id)
        {
            var stat = Get(id);
            if (stat == null) return null;
            return stat.Name;
        }

        public void Update(OrderStatus stat, bool isSwap =false)
        {
            if (!isSwap && GetStatusId(stat.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(stat.OrderStatusId).Name = stat.Name;
                _context.SaveChanges();
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

        public void Update(DefaultValueDto item, bool isSwap = false)
        {
            if (!isSwap && GetStatusId(item.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(item.ID).Name = item.Name;
                _context.SaveChanges();
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Order Status, but found none with ID: " + item.ID + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Order Status, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public int? GetStatusId(string name)
        {
            var stat = _context.OrderStatuses.SingleOrDefault(s => s.Name == name);
            return stat == null ? (int?)null : stat.OrderStatusId;
        }

        public void Add(string statName)
        {
            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var newId = 0;
                    try
                    {
                        newId = _context.OrderStatuses.OrderBy(s => s.OrderStatusId).ToList().LastOrDefault().OrderStatusId + 1;
                    }
                    catch { }

                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[OrderStatuses] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO OrderStatuses ([OrderStatusId], [Name]) Values ("
                        + newId + ",'" + statName + "')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[OrderStatuses] OFF");

                    transaction.Commit();
                }
            }
        }

        //public void Remove(int statId)
        //{
        //    var delItem = Get(statId);
        //    _context.OrderStatuses.Remove(delItem);
        //    _context.SaveChanges();
        //}


        public void Remove(int statId)
        {
            var lateItems=_context.OrderStatuses.Where(s => s.OrderStatusId >= statId).ToList();
            for (int i = 0; i < lateItems.Count()-1; i++)
            {
                Update(new DefaultValueDto { ID = lateItems[i].OrderStatusId, Name=lateItems[i + 1].Name }, true);
            }
            _context.OrderStatuses.Remove(lateItems.LastOrDefault());
            _context.SaveChanges();
        }

        public void Reset()
        {
            Nuke();

            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[OrderStatuses] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO OrderStatuses ([OrderStatusId], [Name])" +
                        " Values (0, 'Received')");
                    context.Database.ExecuteSqlCommand("INSERT INTO OrderStatuses ([OrderStatusId], [Name])" +
                        " Values (1, 'Processed')");
                    context.Database.ExecuteSqlCommand("INSERT INTO OrderStatuses ([OrderStatusId], [Name])" +
                        " Values (2, 'Shipped')");
                    context.Database.ExecuteSqlCommand("INSERT INTO OrderStatuses ([OrderStatusId], [Name])" +
                        " Values (3, 'Delivered')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[OrderStatuses] OFF");

                    transaction.Commit();
                }
            }
        }

        public void Nuke()
        {
            _context.OrderStatuses.RemoveRange(_context.OrderStatuses);
            _context.SaveChanges();
        }
    }
}

