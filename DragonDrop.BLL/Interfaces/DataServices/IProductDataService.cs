using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.BLL.Interfaces.DataServices
{
    public interface IProductDataService
    {
        Product Get(int id);
        Product GetByBarcode(string code);
        string Create(Product prod, bool requiresErrList = false);
        string Update(Product prod, bool requiresErrList = false);
        void AddStock(int prodId, int qty);
        #region Validates
        (bool isValid, string errorList) ValidateProduct(Product prod);
        (bool isValid, string errorList) ValidateName(string name);
        (bool isValid, string errorList) ValidateDescription(string desc);
        (bool isValid, string errorList) ValidateUnitPrice(decimal? price);
        (bool isValid, string errorList) ValidateBarCode(string barCode, int prodId);
        (bool? isValid, string errorList) ValidateBarCodeFormat(string barCode);
        #endregion
        #region Gets
        IEnumerable<Product> GetAll();
        IEnumerable<Product> GetAllFiltered(decimal price, bool greater);
        IEnumerable<Product> GetAllFiltered(int stock, bool greater);
        IEnumerable<Product> GetAllFiltered(string searchBy);
        IEnumerable<Product> GetAllFilteredAndPaged(decimal price, bool greater, int pgSize, int pgIndex);
        IEnumerable<Product> GetAllFilteredAndPaged(int stock, bool greater, int pgSize, int pgIndex);
        IEnumerable<Product> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Product> GetAllPaginated(int pgSize, int pgIndex);
        IEnumerable<Product> GetAllSorted(string sortBy, bool desc = false);
        IEnumerable<Product> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(int stock, bool greater, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredAndSorted(decimal price, bool greater, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Product> GetAllFilteredSortedAndPaged(decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        bool BarcodeExists(string code);
        #region Async
        Task<Product> GetAsync(int id);
        Task<Product> GetByBarcodeAsync(string code);
        Task<string> CreateAsync(Product prod, bool requiresErrList = false);
        Task<string> UpdateAsync(Product prod, bool requiresErrList = false);
        Task AddStockAsync(int prodId, int qty);
        #region Validates
        Task<(bool isValid, string errorList)> ValidateProductAsync(Product prod);
        Task<(bool isValid, string errorList)> ValidateNameAsync(string name);
        Task<(bool isValid, string errorList)> ValidateDescriptionAsync(string desc);
        Task<(bool isValid, string errorList)> ValidateUnitPriceAsync(decimal? price);
        Task<(bool isValid, string errorList)> ValidateBarCodeAsync(string barCode, int prodId);
        Task<(bool? isValid, string errorList)> ValidateBarCodeFormatAsync(string barCode);
        #endregion
        #region Gets
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetAllFilteredAsync(decimal price, bool greater);
        Task<IEnumerable<Product>> GetAllFilteredAsync(int stock, bool greater);
        Task<IEnumerable<Product>> GetAllFilteredAsync(string searchBy);
        Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex);
        Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex);
        Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync
            (string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Product>> GetAllPaginatedAsync(int pgSize, int pgIndex);
        Task<IEnumerable<Product>> GetAllSortedAsync(string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (int stock, bool greater, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (decimal price, bool greater, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        Task<bool> BarcodeExistsAsync(string code);
        #endregion
    }
}