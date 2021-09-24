using DragonDrop.DAL.Entities;
using System.Collections.Generic;

namespace DragonDrop.DAL.Implementation.Resources
{
    public interface IShippingMethodRepository
    {
        void Add(string methName);
        ShippingMethod Get(int id);
        int? GetMethodId(string name);
        string GetMethodName(int id);
        void Update(ShippingMethod meth, bool isSwap=false);
        List<ShippingMethod> GetAll();
        void Remove(int shipId);
    }
}