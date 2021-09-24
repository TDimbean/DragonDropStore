using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DragonDrop.DAL.Implementation.Resources
{
    public class CategoryRepository : ICategoryRepository, IResourceRepo
    {
        private DragonDrop_DbContext _context;

        public List<Category> GetAll() => _context.Categories.ToList();

        public CategoryRepository(DragonDrop_DbContext context) => _context = context;

        public Category Get(int id) => _context.Categories.SingleOrDefault(s => s.CategoryId == id);

        public string GetCategoryName(int id)
        {
            var cat = Get(id);
            if (cat == null) return null;
            return cat.Name;
        }

        public void Update(Category cat, bool isSwap = false)
        {
            if (!isSwap && GetCategoryId(cat.Name) != null)
            {
                // log error
                return;
            }
            try
            {
                Get(cat.CategoryId).Name = cat.Name;
                _context.SaveChanges();
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

        public void Update(DefaultValueDto item, bool isSwap = false)
        {
            if (!isSwap && GetCategoryId(item.Name) != null)
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
                StaticLogger.LogError(GetType(), "Tried to update Category, but found none with ID: " + item.ID + ". Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                StaticLogger.LogError(GetType(), "Tried to update Category, but encountered unexpected error. Details: " + ex.Message + "\n StackTrace:" + ex.StackTrace);
            }
        }

        public int? GetCategoryId(string name)
        {
            var cat = _context.Categories.SingleOrDefault(s => s.Name == name);
            return cat == null ? (int?)null : cat.CategoryId;
        }

        public void Add(string catName)
        {
            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var newId = 0;
                    try
                    {
                        newId = _context.Categories.OrderBy(c => c.CategoryId).ToList().LastOrDefault().CategoryId +1;
                    }
                    catch { }

                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Categories] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name]) Values ("
                        + newId + ",'" + catName + "')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Categories] OFF");

                    transaction.Commit();
                }
            }
        }

        //public void Remove(int catId)
        //{
        //    var delItem = Get(catId);
        //    _context.Categories.Remove(delItem);
        //    _context.SaveChanges();
        //}

        public void Remove(int catId)
        {
            var lateItems = _context.Categories.Where(s => s.CategoryId >= catId).ToList();
            for (int i = 0; i < lateItems.Count() - 1; i++)
            {
                Update(new DefaultValueDto { ID = lateItems[i].CategoryId, Name = lateItems[i + 1].Name }, true);
            }
            _context.Categories.Remove(lateItems.LastOrDefault());
            _context.SaveChanges();
        }

        public void Reset()
        {
            Nuke();

            using (var context = new TempResourceStore())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Categories] ON");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (0, 'Miscelaneous')");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (1, 'Traditional Games')");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (2, 'Table Top RPG')");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (3, 'Cube Puzzles')");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (4, 'Dice')");
                    context.Database.ExecuteSqlCommand("INSERT INTO Categories ([CategoryId], [Name])" +
                        " Values (5, 'Tarrot')");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand(@"SET IDENTITY_INSERT [dbo].[Categories] OFF");

                    transaction.Commit();
                }
            }
        }

        public void Nuke()
        {
            _context.Categories.RemoveRange(_context.Categories);
            _context.SaveChanges();
        }
    }
}
