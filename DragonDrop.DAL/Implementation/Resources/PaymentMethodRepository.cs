using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.DAL.Implementation.Resources
{
    public class PaymentMethodRepository : IPaymentMethodRepository, IResourceRepo
    {
        private DragonDrop_DbContext _context;

        public PaymentMethodRepository(DragonDrop_DbContext context) => _context = context;

        public List<PaymentMethod> GetAll() => _context.PaymentMethods.ToList();

        public PaymentMethod Get(int id) => _context.PaymentMethods.SingleOrDefault(m => m.PaymentMethodId == id);

        public string GetMethodName(int id)
        {
            var meth = Get(id);
            if (meth == null) return null;
            return meth.Name;
        }

        public void Update(PaymentMethod meth, bool isSwap = false)
        {
            if (!isSwap && GetMethodId(meth.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(meth.PaymentMethodId).Name = meth.Name;
                _context.SaveChanges();
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
                StaticLogger.LogError(GetType(), "Tried to update Payment Method, but found none with ID: " + item.ID + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
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

        public void Add(string payName)
        {
            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var newId = 1;
                    try
                    {
                        newId = _context.PaymentMethods.OrderBy(s => s.PaymentMethodId).ToList().LastOrDefault().PaymentMethodId + 1;
                    }
                    catch { }

                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[PaymentMethods] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO PaymentMethods ([PaymentMethodId], [Name]) Values ("
                        + newId + ",'" + payName + "')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF");

                    transaction.Commit();
                }
            }
        }

        //public void Remove(int payId)
        //{
        //    var delItem = Get(payId);
        //    _context.PaymentMethods.Remove(delItem);
        //    _context.SaveChanges();
        //}

        public void Remove(int payId)
        {
            var lateItems = _context.PaymentMethods.Where(p=>p.PaymentMethodId >= payId).ToList();
            for (int i = 0; i < lateItems.Count() - 1; i++)
            {
                Update(new DefaultValueDto { ID = lateItems[i].PaymentMethodId, Name = lateItems[i + 1].Name }, true);
            }
            _context.PaymentMethods.Remove(lateItems.LastOrDefault());
            _context.SaveChanges();
        }

        public void Reset()
        {
            Nuke();

            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[PaymentMethods] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO PaymentMethods ([PaymentMethodId], [Name])" +
                        " Values (1, 'Credit Card')");
                    context.Database.ExecuteSqlCommand("INSERT INTO PaymentMethods ([PaymentMethodId], [Name])" +
                        " Values (2, 'Cash')");
                    context.Database.ExecuteSqlCommand("INSERT INTO PaymentMethods ([PaymentMethodId], [Name])" +
                        " Values (3, 'PayPal')");
                    context.Database.ExecuteSqlCommand("INSERT INTO PaymentMethods ([PaymentMethodId], [Name])" +
                        " Values (4, 'Wire Transfer')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[PaymentMethods] OFF");

                    transaction.Commit();
                }
            }
        }

        public void Nuke()
        {
            _context.PaymentMethods.RemoveRange(_context.PaymentMethods);
            _context.SaveChanges();
        }
    }
}
