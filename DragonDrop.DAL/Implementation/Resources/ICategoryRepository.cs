using DragonDrop.DAL.Entities;
using System.Collections.Generic;

namespace DragonDrop.DAL.Implementation.Resources
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        void Add(string catName);
        Category Get(int id);
        int? GetCategoryId(string name);
        string GetCategoryName(int id);
        void Update(Category cat, bool isSwap = false);
        void Remove(int catId);
    }
}