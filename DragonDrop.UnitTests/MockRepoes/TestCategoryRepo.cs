using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Implementation.Resources;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestCategoryRepo : ICategoryRepository
    {
        private TestDb _context;

        public TestCategoryRepo(TestDb context) => _context = context;

        public List<Category> GetAll() => _context.Categories.ToList();

        public Category Get(int id) => _context.Categories.SingleOrDefault(s => s.CategoryId == id);

        public string GetCategoryName(int id)
        {
            var cat = Get(id);
            if (cat == null) return null;
            return cat.Name;
        }

        public void Update(Category cat, bool isSwap = false)
        {
            if (GetCategoryId(cat.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(cat.CategoryId).Name = cat.Name;
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Product Category, but found none with ID: " + cat.CategoryId + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Product Category but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public int? GetCategoryId(string name)
        {
            var cat = _context.Categories.SingleOrDefault(s => s.Name == name);
            return cat == null ? (int?)null : cat.CategoryId;
        }

        public void Add(string catName) => _context.Categories.Add(new Category { Name = catName });

        public void Remove(int id) => _context.Categories.Remove(Get(id));
    }
}
