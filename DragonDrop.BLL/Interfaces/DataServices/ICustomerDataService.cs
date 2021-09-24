using DragonDrop.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DragonDrop.BLL.Interfaces.DataServices
{
    public interface ICustomerDataService
    {
        Customer Get(int id);
        string Create(Customer cust, bool requiresErrList = false);
        string Update(Customer cust, bool requiresErrList = false);
        IEnumerable<Customer> GetAll();
        bool EmailExists(string email);
        bool PhoneExists(string phone);
        #region Queried Gets
        IEnumerable<Customer> GetAllPaginated(int pgSize, int pgIndex);
        IEnumerable<Customer> GetAllSorted(string sortBy, bool desc = false);
        IEnumerable<Customer> GetAllFiltered(string searchBy);
        IEnumerable<Customer> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex);
        IEnumerable<Customer> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false);
        IEnumerable<Customer> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false);
        IEnumerable<Customer> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region MainWindow Methods
        bool OneCustWithEmailExists(string email);
        bool PassMatchesEmail(string email, string pass);
        int? FindIdByEmail(string email);
        #endregion
        #region Validators
        (bool isValid, string errorList) ValidateCustomer(Customer cust);
        (bool isValid, string errorList) ValidateName(string name);
        (bool isValid, string errorList) ValidateEmail(string email);
        (bool isValid, string errorList) ValidatePhone(string phone);
        (bool isValid, string errorList) ValidateAddress(string adr);
        (bool isValid, string errorList) ValidateCity(string city);
        (bool isValid, string errorList) ValidateState(string state);
        #endregion
        #region Async
        Task<Customer> GetAsync(int id);
        Task<string> CreateAsync(Customer cust, bool requiresErrList = false);
        Task<string> UpdateAsync(Customer cust, bool requiresErrList = false);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phone);
        Task<IEnumerable<Customer>> GetAllAsync();
        #region Queried Gets
        Task<IEnumerable<Customer>> GetAllPaginatedAsync(int pgSize, int pgIndex);
        Task<IEnumerable<Customer>> GetAllSortedAsync(string sortBy, bool desc = false);
        Task<IEnumerable<Customer>> GetAllFilteredAsync(string searchBy);
        Task<IEnumerable<Customer>> GetAllFilteredAndPagedAsync
            (string searchBy, int pgSize, int pgIndex);
        Task<IEnumerable<Customer>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false);
        Task<IEnumerable<Customer>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false);
        Task<IEnumerable<Customer>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false);
        #endregion
        #region MainWindow Methods
        Task<bool> OneCustWithEmailExistsAsync(string email);
        Task<bool> PassMatchesEmailAsync(string email, string pass);
        Task<int?> FindIdByEmailAsync(string email);
        #endregion
        #endregion
    }
}