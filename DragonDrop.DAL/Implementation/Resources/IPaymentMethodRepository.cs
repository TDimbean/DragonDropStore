using DragonDrop.DAL.Entities;
using System.Collections.Generic;

namespace DragonDrop.DAL.Implementation.Resources
{
    public interface IPaymentMethodRepository
    {
        void Add(string methName);
        PaymentMethod Get(int id);
        List<PaymentMethod> GetAll();
        int? GetMethodId(string name);
        string GetMethodName(int id);
        void Update(PaymentMethod meth, bool isSwap = false);
        void Remove(int payId);
    }
}