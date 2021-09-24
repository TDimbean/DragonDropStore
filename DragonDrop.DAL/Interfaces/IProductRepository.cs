using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Interfaces
{
    public interface IProductRepository
    {
        void Create(Product prod);
        void Update(Product prod);
        void AddStock(int prodId, int qty);
        #region Gets
        Product Get(int id);
        Product GetByBarcode(string code);
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Product> prodList = null);
        #region Filters
        IEnumerable<Product> GetAllFiltered(string searchBy);
        IEnumerable<Product> GetAllFiltered(int stock, bool greater);
        IEnumerable<Product> GetAllFiltered(decimal price, bool greater);
        IEnumerable<Product> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Product> GetAllFilteredAndPaged(int stock, bool greater, int pgSize, int pgIndex);
        IEnumerable<Product> GetAllFilteredAndPaged(decimal price, bool greater, int pgSize, int pgIndex);
        #endregion
        #region Sorts
        IEnumerable<Product> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Product> prodList = null);
        IEnumerable<Product> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(int stock, bool greater, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(decimal price, bool greater, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #endregion
        #region IdVerifiers
        bool IsUniqueBarCode(string barCode, int? prodId = null);
        bool ProductIdExists(int prodId);
        bool CategoryIdExists(int catId);
        #endregion
        #region Async
        Task CreateAsync(Product prod);
        Task UpdateAsync(Product prod);
        Task AddStockAsync(int prodId, int qty);
        #region Gets
        Task<Product> GetAsync(int id);
        Task<Product> GetByBarcodeAsync(string code);
        Task <IEnumerable<Product>>GetAllAsync();
        Task <IEnumerable<Product>>GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<Product> prodList = null);
        #region Filters
        Task <IEnumerable<Product>> GetAllFilteredAsync(string searchBy);
        Task <IEnumerable<Product>> GetAllFilteredAsync(int stock, bool greater);
        Task <IEnumerable<Product>> GetAllFilteredAsync(decimal price, bool greater);
        Task <IEnumerable<Product>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex);
        Task <IEnumerable<Product>> GetAllFilteredAndPagedAsync(int stock, bool greater, int pgSize, int pgIndex);
        Task <IEnumerable<Product>> GetAllFilteredAndPagedAsync(decimal price, bool greater, int pgSize, int pgIndex);
        #endregion
        #region Sorts
        Task <IEnumerable<Product>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<Product> prodList = null);
        Task <IEnumerable<Product>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task <IEnumerable<Product>> GetAllFilteredAndSortedAsync(string searchBy, string sortBy, bool desc = false);
        Task <IEnumerable<Product>> GetAllFilteredAndSortedAsync(int stock, bool greater, string sortBy, bool desc = false);
        Task <IEnumerable<Product>> GetAllFilteredAndSortedAsync(decimal price, bool greater, string sortBy, bool desc = false);
        Task <IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task <IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #endregion
        #region IdVerifiers
        Task<bool> IsUniqueBarCodeAsync(string barCode, int? prodId = null);
        Task<bool> ProductIdExistsAsync(int prodId);
        Task<bool> CategoryIdExistsAsync(int catId);
        #endregion
        #endregion
    }
}