using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.BLL.Interfaces.DataServices
{
    public interface IOrderItemDataService
    {
        OrderItem Get(int ordId, int prodId);
        string Create(OrderItem item, bool requiresErrList = false);
        string Update(OrderItem item, bool requiresErrList = false);
        #region Gets
        IEnumerable<OrderItem> GetAll();
        IEnumerable<OrderItem> GetAllByOrderId(int ordId);
        IEnumerable<OrderItem> GetAllByOrderIdAndPaged(int ordId, int pgSize, int pgIndex);
        IEnumerable<OrderItem> GetAllByProductId(int prodId);
        IEnumerable<OrderItem> GetAllByProductIdAndPaged(int prodId, int pgSize, int pgIndex);
        IEnumerable<OrderItem> GetAllPaginated(int pgSize, int pgIndex);
        IEnumerable<OrderItem> GetAllSorted(string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByOrderIdAndSorted(int ordId, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByProductIdAndSorted(int prodId, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByOrderIdSortedAndPaged(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByProductIdSortedAndPaged(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        (bool isValid, string errorList) ValidateOrderItem(OrderItem item);
        (bool isValid, string error) ValidateQuantity(int qty);
        (bool isValid, string error) ValidateOrderId(int ordId);
        (bool isValid, string error) ValidateProductId(int prodId);
        #region Async
        Task<OrderItem> GetAsync(int ordId, int prodId);
        Task<string> CreateAsync(OrderItem item, bool requiresErrList = false);
        Task<string> UpdateAsync(OrderItem item, bool requiresErrList = false);
        #region Gets
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(int ordId);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAndPagedAsync(int ordId, int pgSize, int pgIndex);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAsync(int prodId);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAndPagedAsync(int prodId, int pgSize, int pgIndex);
        Task<IEnumerable<OrderItem>> GetAllPaginatedAsync(int pgSize, int pgIndex);
        Task<IEnumerable<OrderItem>> GetAllSortedAsync(string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAndSortedAsync(int ordId, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAndSortedAsync(int prodId, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdSortedAndPagedAsync(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByProductIdSortedAndPagedAsync(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #endregion
    }
}