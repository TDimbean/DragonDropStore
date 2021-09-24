using DragonDrop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.BLL.Interfaces.DataServices
{
    public interface IOrderDataService
    {
        string Create(Order ord, bool requiresErrList = false);
        string Update(Order ord, bool requiresErrList = false);
        #region Gets
        Order Get(int id);
        IEnumerable<Order> GetAll();
        IEnumerable<Order> GetAllUnprocessed();
        IEnumerable<Order> GetAllProcessed();
        IEnumerable<Order> GetAllPaginated(int pgSize, int pgIndex);
        IEnumerable<Order> GetAllFiltered(string searchBy);
        IEnumerable<Order> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllByCustomerId(int id);
        IEnumerable<Order> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllByCustomerIdAndFiltered(int custId, string searchBy);
        IEnumerable<Order> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex);
        IEnumerable<Order> GetAllSorted(string sortBy, bool desc = false);
        IEnumerable<Order> GetAllSortedAndPaged(int pgSize, int pgIndex, string searchBy, bool desc = false);
        IEnumerable<Order> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Order> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region StatusAdvancements
        void PromoteReceived(int ordId);
        void BatchPromoteReceived(List<int> ordIds);
        void PromoteProcessed(int ordId, DateTime shipDate);
        #endregion
        #region Async
        Task<string> CreateAsync(Order ord, bool requiresErrList = false);
        Task<string> UpdateAsync(Order ord, bool requiresErrList = false);
        #region Gets
        Task<Order> GetAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllUnprocessedAsync();
        Task<IEnumerable<Order>> GetAllProcessedAsync();
        Task<IEnumerable<Order>> GetAllPaginatedAsync(int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllFilteredAsync(string searchBy);
        Task<IEnumerable<Order>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllByCustomerIdAsync(int id);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndFilteredAsync(int custId, string searchBy);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Order>> GetAllSortedAsync(string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string searchBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Order>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region StatusAdvancements
        Task PromoteReceivedAsync(int ordId);
        Task BatchPromoteReceivedAsync(List<int> ordIds);
        Task PromoteProcessedAsync(int ordId, DateTime shipDate);
        #endregion
        #endregion
        #region Validates
        (bool isValid, string errorList) ValidateOrder(Order ord);
        (bool isValid, string errorList) ValidateCustomerId(int custId);
        (bool isValid, string errorList) ValidateOrderDate(DateTime date);
        (bool isValid, string errorList) ValidateShippingDate(DateTime? shipDate, DateTime ordDate);
        (bool isValid, string errorList) ValidateOrderStatusId(int statId);
        (bool isValid, string errorList) ValidatePaymentMethodId(int methId);
        (bool isValid, string errorList) ValidateShippingMethodId(int shipId);
        (bool isValid, string errorList) ValidateShipWithStatus(DateTime? shipDate, int statId);
        #endregion
    }
}