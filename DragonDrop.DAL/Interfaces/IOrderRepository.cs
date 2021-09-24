using DragonDrop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Interfaces
{
    public interface IOrderRepository
    {
        void Create(Order ord);
        void Update(Order ord);
        bool OrderIdExists(int ordId);
        #region Gets
        Order Get(int id);
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetAllUnprocessed();
        IEnumerable<Order> GetAllProcessed();
        IEnumerable<Order> GetAllByCustomerId(int custId);
        IEnumerable<Order> GetAllFiltered(string searchBy, IEnumerable<Order> ordList = null);
        IEnumerable<Order> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Order> ordList = null);
        IEnumerable<Order> GetAllFilteredAndSorted(string searchyBy, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Order> ordList = null);
        IEnumerable<Order> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdAndFiltered(int custId, string searchBy);
        IEnumerable<Order> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region IdVerifiers
        bool CustomerIdExists(int custId);
        bool ShippingMethodIdExists(int shipId);
        bool PaymentMethodIdExists(int payId);
        bool OrderStatusIdExists(int statId);
        #endregion
        #region StatusAdvancements
        void PromoteReceived(int ordId);
        void PromoteProcessed(int ordId, DateTime shippingDate);
        void BatchPromoteReceived(List<int> ordIds);
        #endregion
        #region Async
        Task CreateAsync(Order ord);
        Task UpdateAsync(Order ord);
        Task<bool> OrderIdExistsAsync(int ordId);
        #region Gets
        Task<Order> GetAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllUnprocessedAsync();
        Task<IEnumerable<Order>> GetAllProcessedAsync();
        Task<IEnumerable<Order>> GetAllByCustomerIdAsync(int custId);
        Task<IEnumerable<Order>> GetAllFilteredAsync(string searchBy, IEnumerable<Order> ordList = null);
        Task<IEnumerable<Order>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<Order> ordList = null);
        Task<IEnumerable<Order>> GetAllFilteredAndSortedAsync(string searchyBy, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndSortedAsync(int custId, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<Order> ordList = null);
        Task<IEnumerable<Order>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllByCustomerIdSortedAndPagedAsync(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllFilteredSortedAndPagedAsync(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndFilteredAsync(int custId, string searchBy);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndPagedAsync(int custId, string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndSortedAsync(int custId, string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredSortedAndPagedAsync(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region IdVerifiers
        Task<bool> CustomerIdExistsAsync(int custId);
        Task<bool> ShippingMethodIdExistsAsync(int shipId);
        Task<bool> PaymentMethodIdExistsAsync(int payId);
        Task<bool> OrderStatusIdExistsAsync(int statId);
        #endregion
        #region StatusAdvancements
        Task PromoteReceivedAsync(int ordId);
        Task PromoteProcessedAsync(int ordId, DateTime shippingDate);
        Task BatchPromoteReceivedAsync(List<int> ordIds);
        #endregion
        #endregion
    }
}