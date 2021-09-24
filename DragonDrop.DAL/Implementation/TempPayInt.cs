using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonDrop.DAL.Entities;

namespace DragonDrop.DAL.Implementation
{
    public interface TempPayInt
    {
        IEnumerable<Payment> GetAll();
        IEnumerable<Payment> GetAllByCustomerId(int custId);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, DateTime when, bool before);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, decimal amount, bool over);
        IEnumerable<Payment> GetAllByCustomerIdAndFiltered(int custId, string searchBy);
        IEnumerable<Payment> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, decimal amount, bool smaller, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, DateTime when, bool before, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, decimal amount, bool over, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, decimal amount, bool under, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFiltered(DateTime when, bool before, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllFiltered(decimal amount, bool smaller, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllFiltered(string searchBy, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllFilteredAndPaged(DateTime when, bool before, int pgSize, int pgIndex, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllFilteredAndPaged(decimal amount, bool smaller, int pgSize, int pgIndex, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync(DateTime when, bool before, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync(decimal amount, bool smaller, int pgSize, int pgIndex);
        Task<IEnumerable<Payment>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Payment> GetAllFilteredAndSorted(DateTime when, bool before, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredAndSorted(decimal amount, bool over, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(DateTime when, bool before, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(decimal amount, bool under, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Payment> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Payment> payList = null);
        IEnumerable<Payment> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
    }
}