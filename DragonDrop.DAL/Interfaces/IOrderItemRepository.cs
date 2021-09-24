using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Interfaces
{
    public interface IOrderItemRepository
    {
        void Create(OrderItem item);
        void Update(OrderItem item);
        #region Gets
        OrderItem Get(int ordId, int prodId);
        IEnumerable<OrderItem> GetAll();
        IEnumerable<OrderItem> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<OrderItem> itemList = null);
        IEnumerable<OrderItem> GetAllSorted(string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null);
        IEnumerable<OrderItem> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null);
        IEnumerable<OrderItem> GetAllByOrderId(int ordId);
        IEnumerable<OrderItem> GetAllByOrderIdAndSorted(int ordId, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByOrderIdAndPaged(int ordId, int pgSize, int pgIndex);
        IEnumerable<OrderItem> GetAllByOrderIdSortedAndPaged(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByProductId(int prodId);
        IEnumerable<OrderItem> GetAllByProductIdAndPaged(int prodId, int pgSize, int pgIndex);
        IEnumerable<OrderItem> GetAllByProductIdAndSorted(int prodId, string sortBy, bool desc = false);
        IEnumerable<OrderItem> GetAllByProductIdSortedAndPaged(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        bool ProductIdExists(int prodId);
        bool OrderIdExists(int ordId);
        #region Async
        Task CreateAsync(OrderItem item);
        Task UpdateAsync(OrderItem item);
        #region Gets
        Task<OrderItem> GetAsync(int ordId, int prodId);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem>> GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<OrderItem> itemList = null);
        Task<IEnumerable<OrderItem>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null);
        Task<IEnumerable<OrderItem>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(int ordId);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAndSortedAsync(int ordId, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdAndPagedAsync(int ordId, int pgSize, int pgIndex);
        Task<IEnumerable<OrderItem>> GetAllByOrderIdSortedAndPagedAsync(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAsync(int prodId);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAndPagedAsync(int prodId, int pgSize, int pgIndex);
        Task<IEnumerable<OrderItem>> GetAllByProductIdAndSortedAsync(int prodId, string sortBy, bool desc = false);
        Task<IEnumerable<OrderItem>> GetAllByProductIdSortedAndPagedAsync(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        Task<bool> ProductIdExistsAsync(int prodId);
        Task<bool> OrderIdExistsAsync(int ordId);
        #endregion
    }
}