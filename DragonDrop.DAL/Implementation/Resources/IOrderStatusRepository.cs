using DragonDrop.DAL.Entities;
using System.Collections.Generic;

namespace DragonDrop.DAL.Implementation.Resources
{
    public interface IOrderStatusRepository
    {
        void Add(string statName);
        OrderStatus Get(int id);
        int? GetStatusId(string name);
        string GetStatusName(int id);
        void Update(OrderStatus stat, bool isSwap = false);
        List<OrderStatus> GetAll();
        void Remove(int statId);
    }
}