using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestPaymentMethodRepo : IPaymentMethodRepository
    {
        private TestDb _context;

        public TestPaymentMethodRepo(TestDb context) => _context = context;

        public List<PaymentMethod> GetAll() => _context.PaymentMethods.ToList();

        public PaymentMethod Get(int id) => _context.PaymentMethods.SingleOrDefault(m => m.PaymentMethodId == id);

        public void Remove(int id) => _context.PaymentMethods.Remove(Get(id));

        public string GetMethodName(int id)
        {
            var meth = Get(id);
            if (meth == null) return null;
            return meth.Name;
        }

        public void Update(PaymentMethod meth, bool isSwap=false)
        {
            if (GetMethodId(meth.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(meth.PaymentMethodId).Name = meth.Name;
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Payment Method, but found none with ID: " + meth.PaymentMethodId + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Payment Method, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public int? GetMethodId(string name)
        {
            var meth = _context.PaymentMethods.SingleOrDefault(m => m.Name == name);
            return meth == null ? (int?)null : meth.PaymentMethodId;
        }

        public void Add(string methName) => _context.PaymentMethods.Add(new PaymentMethod { Name = methName });
    }
}
