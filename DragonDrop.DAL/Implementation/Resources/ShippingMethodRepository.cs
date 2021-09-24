using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.DAL.Implementation.Resources
{
    public class ShippingMethodRepository : IShippingMethodRepository, IResourceRepo
    {
        private DragonDrop_DbContext _context;

        public ShippingMethodRepository(DragonDrop_DbContext context) => _context = context;

        public List<ShippingMethod> GetAll() => _context.ShippingMethods.ToList();

        public ShippingMethod Get(int id) => _context.ShippingMethods.SingleOrDefault(m => m.ShippingMethodId == id);

        public string GetMethodName(int id)
        {
            var meth = Get(id);
            if (meth == null) return null;
            return meth.Name;
        }

        public void Update(ShippingMethod meth, bool isSwap = false)
        {
            if (!isSwap && GetMethodId(meth.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(meth.ShippingMethodId).Name = meth.Name;
                _context.SaveChanges();
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Shipping Method, but found none with ID: " + meth.ShippingMethodId + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Shipping Method, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public void Update(DefaultValueDto item, bool isSwap = false)
        {
            if (!isSwap && GetMethodId(item.Name) != null)
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
                StaticLogger.LogError(GetType(), "Tried to update Shipping Method, but found none with ID: " + item.ID + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Shipping Method, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public int? GetMethodId(string name)
        {
            var meth = _context.ShippingMethods.SingleOrDefault(m => m.Name == name);
            return meth == null ? (int?)null : meth.ShippingMethodId;
        }

        public void Add(string shipName)
        {
            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var newId = 1;
                    try
                    {
                        newId = _context.ShippingMethods.OrderBy(s => s.ShippingMethodId).ToList().LastOrDefault().ShippingMethodId + 1;
                    }
                    catch { }

                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[ShippingMethods] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO ShippingMethods ([ShippingMethodId], [Name]) Values ("
                        + newId + ",'" + shipName + "')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[ShippingMethods] OFF");

                    transaction.Commit();
                }
            }
        }

        //public void Remove(int shipId)
        //{
        //    var delItem = Get(shipId);
        //    _context.ShippingMethods.Remove(delItem);
        //    _context.SaveChanges();
        //}

        public void Remove(int shipId)
        {
            var lateItems = _context.ShippingMethods.Where(s => s.ShippingMethodId >= shipId).ToList();
            for (int i = 0; i < lateItems.Count() - 1; i++)
            {
                Update(new DefaultValueDto { ID = lateItems[i].ShippingMethodId, Name = lateItems[i + 1].Name }, true);
            }
            _context.ShippingMethods.Remove(lateItems.LastOrDefault());
            _context.SaveChanges();
        }

        public void Reset()
        {
            Nuke();

            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[ShippingMethods] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO ShippingMethods ([ShippingMethodId], [Name])" +
                        " Values (1, 'FedEx')");
                    context.Database.ExecuteSqlCommand("INSERT INTO ShippingMethods ([ShippingMethodId], [Name])" +
                        " Values (2, 'DHL')");
                    context.Database.ExecuteSqlCommand("INSERT INTO ShippingMethods ([ShippingMethodId], [Name])" +
                        " Values (3, 'UPS')");
                    context.Database.ExecuteSqlCommand("INSERT INTO ShippingMethods ([ShippingMethodId], [Name])" +
                        " Values (4, 'Snail Mail')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[ShippingMethods] OFF");

                    transaction.Commit();
                }
            }
        }

        public void Nuke()
        {
            _context.ShippingMethods.RemoveRange(_context.ShippingMethods);
            _context.SaveChanges();
        }
    }
}
