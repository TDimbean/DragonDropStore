using DragonDrop.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.BLL.Interfaces.DataServices
{
    public interface IPaymentDataService
    {
        Payment Get(int id);
        string Create(Payment pay, bool requiresErrList = false);
        string Update(Payment pay, bool requiresErrList = false);
        #region Gets
        IEnumerable<Payment> GetAll();
        IEnumerable<Payment> GetAllByCustomerId(int custId);
        IEnumerable<Payment> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllFiltered(DateTime when, bool before);
        IEnumerable<Payment> GetAllFiltered(decimal amount, bool smaller);
        IEnumerable<Payment> GetAllFiltered(string searchBy);
        IEnumerable<Payment> GetAllFilteredAndPaged(DateTime when, bool before, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllFilteredAndPaged(decimal amount, bool smaller, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllPaginated(int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllSorted(string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredAndSorted(DateTime when, bool before, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredAndSorted(decimal amount, bool over, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, string searchBy);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, DateTime when, bool before);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, decimal amount, bool over);
        IEnumerable<Payment> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, DateTime when, bool before, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, decimal amount, bool over, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, decimal amount, bool smaller, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, decimal amount, bool under, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        (bool isValid, string errorList) ValidatePayment(Payment pay);
        (bool isValid, string errorList) ValidateDate(DateTime? date);
        (bool isValid, string errorList) ValidateAmount(decimal? amt);
        (bool isValid, string errorList) ValidateCustomerId(int custId);
        #region Async

        Task<Payment> GetAsync(int id);
        Task<string> CreateAsync(Payment pay, bool requiresErrList = false);
        Task<string> UpdateAsync(Payment pay, bool requiresErrList = false);
        #region Gets
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetAllByCustomerIdAsync(int custId);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndPagedAsync
            (int custId, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAsync(DateTime when, bool before);
        Task<IEnumerable<Payment>> GetAllFilteredAsync(decimal amount, bool smaller);
        Task<IEnumerable<Payment>> GetAllFilteredAsync(string searchBy);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (decimal amount, bool smaller, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync
            (string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllPaginatedAsync(int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllSortedAsync(string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (DateTime when, bool before, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredAndSortedAsync
            (decimal amount, bool over, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, string searchBy);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, DateTime when, bool before);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndFilteredAsync
            (int custId, decimal amount, bool over);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllFilteredSortedAndPagedAsync
            (decimal amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, DateTime when, bool before, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, decimal amount, bool over, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, decimal amount, bool smaller, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Payment>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, decimal amount, bool under, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion

        #endregion
    }
}