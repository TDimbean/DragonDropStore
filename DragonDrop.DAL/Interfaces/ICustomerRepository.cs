using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Interfaces
{
    public interface ICustomerRepository
    {
        void Create(Customer cust);
        void Update(Customer cust);
        bool CustomerIdExists(int id);
        bool EmailExists(string email);
        bool PhoneExists(string phone);
        #region Gets
        Customer Get(int id);
        IEnumerable<Customer> GetAll();
        IEnumerable<Customer> GetAllFiltered(string searchBy);
        IEnumerable<Customer> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Customer> custList = null);
        IEnumerable<Customer> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Customer> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Customer> custList = null);
        IEnumerable<Customer> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Customer> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Customer> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region MainWindow Methods
        int? FindIdByEmail(string email);
        string GetPassByCustomerId(int custId);
        #endregion
        #region Async
        Task CreateAsync(Customer cust);
        Task UpdateAsync(Customer cust);
        Task<bool> CustomerIdExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        #region Gets
        Task<Customer> GetAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<Customer> custList = null);
        Task<IEnumerable<Customer>> GetAllFilteredAsync(string searchBy);
        Task<IEnumerable<Customer>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Customer>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<Customer> custList = null);
        Task<IEnumerable<Customer>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Customer>> GetAllFilteredAndSortedAsync(string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Customer>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region MainWindow Methods
        Task <int?> FindIdByEmailAsync(string email);
        Task <string> GetPassByCustomerIdAsync(int custId);
        #endregion
        #endregion
    }
}