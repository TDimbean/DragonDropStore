using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestShippingMethodRepo : IShippingMethodRepository
    {
        private TestDb _context;

        public TestShippingMethodRepo(TestDb context) => _context = context;

        public ShippingMethod Get(int id) => _context.ShippingMethods.SingleOrDefault(m => m.ShippingMethodId == id);

        public string GetMethodName(int id)
        {
            var meth = Get(id);
            if (meth == null) return null;
            return meth.Name;
        }

        public void Update(ShippingMethod meth, bool isSwap=false)
        {
            if (GetMethodId(meth.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(meth.ShippingMethodId).Name = meth.Name;
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

        public void Remove(int id) => _context.ShippingMethods.Remove(Get(id));

        public int? GetMethodId(string name)
        {
            var meth = _context.ShippingMethods.SingleOrDefault(m => m.Name == name);
            return meth == null ? (int?)null : meth.ShippingMethodId;
        }

        public void Add(string methName) => _context.ShippingMethods.Add(new ShippingMethod { Name = methName });

        public List<ShippingMethod> GetAll() => _context.ShippingMethods.ToList();
    }
}
