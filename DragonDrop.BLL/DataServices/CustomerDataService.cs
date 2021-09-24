using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.BLL.DataServices
{
    public class CustomerDataService : ICustomerDataService
    {
        private ICustomerRepository _repo;

        public CustomerDataService(ICustomerRepository repo) => _repo = repo;

        public Customer Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service fetching Customer with ID:\t" + id);
            var cust = _repo.Get(id);
            if (cust == null) StaticLogger.LogWarn(GetType(), "Customer Data Service found no Customer with ID:\t" + id);
            return cust;
        }

        public async Task<Customer> GetAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service fetching Customer with ID:\t" + id);
            var cust = await _repo.GetAsync(id);
            if (cust == null) StaticLogger.LogWarn(GetType(), "Customer Data Service found no Customer with ID:\t" + id);
            return cust;
        }

        public string Create(Customer cust, bool requiresErrList = false)
        {
            cust.Email = cust.Email != null && cust.Email.Trim() == "" ? null : cust.Email;

            var validation = ValidateCustomer(cust);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Customer failed Data Service validation and will not be submitted to the Repo. Details:\n"+validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Customer Data Service sending Customer creation request to Repo. Name:\t" + cust.Name);
            _repo.Create(cust);
            StaticLogger.LogInfo(GetType(), "Repo returned to Customer Data Service.");
            return null;
        }

        public string Update(Customer cust, bool requiresErrList = false)
        {
            var idError = "";
            var idExists = true;

            if (!_repo.CustomerIdExists(cust.CustomerId))
            {
                idExists = false;
                idError = "Customer ID must exist within the Repo for an update to be performed.";
            }

            var validation = ValidateCustomer(cust);

            if (!idExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Customer failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), 
                "Customer Data Service sending Customer update request for Customer with ID:\t" +
                cust.CustomerId + " to Repo.");
            _repo.Update(cust);
            StaticLogger.LogInfo(GetType(), "Repo returned to Customer Data Service.");
            return null;
        }

        public async Task<string> CreateAsync(Customer cust, bool requiresErrList = false)
        {
            cust.Email = cust.Email != null && cust.Email.Trim() == "" ? null : cust.Email;

            var validation = ValidateCustomer(cust);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Customer failed Data Service validation and will not be submitted to the Repo."+
                    " Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Customer Data Service sending Customer creation request to Repo. Name:\t" + cust.Name);
            await _repo.CreateAsync(cust);
            StaticLogger.LogInfo(GetType(), "Repo returned to Customer Data Service.");
            return null;
        }

        public async Task<string> UpdateAsync(Customer cust, bool requiresErrList = false)
        {
            var idError = "";
            var idExists = true;

            if (!await _repo.CustomerIdExistsAsync(cust.CustomerId))
            {
                idExists = false;
                idError = "Customer ID must exist within the Repo for an update to be performed.";
            }

            var validation = ValidateCustomer(cust);

            if (!idExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Customer failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Customer Data Service sending Customer update request for Customer with ID:\t" + cust.CustomerId+" to Repo.");
            await _repo.UpdateAsync(cust);
            StaticLogger.LogInfo(GetType(), "Repo returned to Customer Data Service.");
            return null;
        }

        public bool EmailExists(string email) => _repo.EmailExists(email);

        public async Task<bool> EmailExistsAsync(string email) => await _repo.EmailExistsAsync(email);

        public bool PhoneExists(string phone) => _repo.PhoneExists(phone);

        public async Task<bool> PhoneExistsAsync(string phone) => await _repo.PhoneExistsAsync(phone);

        public IEnumerable<Customer> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service sending fetching all request to Repo");
            var custs = _repo.GetAll();
            StaticLogger.LogInfo(GetType(), "Repo returned: "+ custs.ToList().Count().ToString() + " records.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service sending fetching all request to Repo");
            var custs = await _repo.GetAllAsync();
            StaticLogger.LogInfo(GetType(), "Repo returned: " + custs.ToList().Count().ToString() + " records.");
            return custs;
        }

        #region Queried Gets

        public IEnumerable<Customer> GetAllPaginated(int pgSize, int pgIndex)
        {
            if (pgSize == 0 || pgIndex == 0) return GetAll();
            pgSize = pgSize < 0 ? pgSize * -1 : pgSize;
            pgIndex = pgIndex < 0 ? pgIndex * -1 : pgIndex;

            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting page " + pgIndex + " of size " + pgSize + " from all Customer in Repo.");
            var custs = _repo.GetAllPaginated(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records from repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllPaginatedAsync(int pgSize, int pgIndex)
        {
            if (pgSize == 0 || pgIndex == 0) return GetAll();
            pgSize = pgSize < 0 ? pgSize * -1 : pgSize;
            pgIndex = pgIndex < 0 ? pgIndex * -1 : pgIndex;

            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting page " + pgIndex + " of size " + pgSize + " from all Customer in Repo.");
            var custs = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records from repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFiltered(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customers with Search:\t" + searchBy+" from Repo.");
            var custs = _repo.GetAllFiltered(searchBy);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.ToList().Count() + " from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAsync(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customers with Search:\t" + searchBy + " from Repo.");
            var custs = await _repo.GetAllFilteredAsync(searchBy);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.ToList().Count() + " from Repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customer records, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize +" from Repo");
            var custs = _repo.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customer records, according to \n SEARCH: " +
                searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo");
            var custs = await _repo.GetAllFilteredAndPagedAsync(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllSorted(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting records sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var custs = _repo.GetAllSorted(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service received: "+ custs.Count() + " records from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllSortedAsync(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting records sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var custs = await _repo.GetAllSortedAsync(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service received: " + custs.Count() + " records from Repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customer records by Filter: "+ searchBy + " and Filtered according to: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var custs = _repo.GetAllFilteredAndSorted(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Customer records by Filter: " + searchBy + " and Filtered according to: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var custs = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Page: "+pgSize+" of Size: "+pgIndex+" from records Filtered by: " + sortBy + " in " + (desc ? "descending" : "ascending") + "  order from Repo.");
            var custs = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting Page: " + pgSize + " of Size: " + pgIndex + " from records Filtered by: " + sortBy + " in " + (desc ? "descending" : "ascending") + "  order from Repo.");
            var custs = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredSortedAndPaged
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting records filtered by: "+searchBy+", Filtered by: " + sortBy + " in " + (desc ? "descending" : "ascending") + "  order, Page: " + pgSize + " of Size: " + pgIndex + " from Repo.");
            var custs = _repo.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Customer Data Service requesting records filtered by: " +
                searchBy + ", Filtered by: " + sortBy + " in " + (desc ? "descending" : "ascending") + 
                "  order, Page: " + pgSize + " of Size: " + pgIndex + " from Repo.");
            var custs = await _repo.GetAllFilteredSortedAndPagedAsync(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Customer Data Service got " + custs.Count().ToString() + " records back from Repo.");
            return custs;
        }

        #endregion

        #region MainWindow Methods

        public bool OneCustWithEmailExists(string email) => ValidateEmail(email).isValid ? _repo.FindIdByEmail(email) != null : false;

        public async Task<bool> OneCustWithEmailExistsAsync(string email)
            => ValidateEmail(email).isValid ? await _repo.FindIdByEmailAsync(email) != null : false;

        public bool PassMatchesEmail(string email, string pass)
            => OneCustWithEmailExists(email) ?
            _repo.GetPassByCustomerId(_repo.FindIdByEmail(email).GetValueOrDefault()) == pass : false;

        public async Task<bool> PassMatchesEmailAsync(string email, string pass)
            => await OneCustWithEmailExistsAsync(email) ?
            await _repo.GetPassByCustomerIdAsync(
                (await _repo.FindIdByEmailAsync(email)).GetValueOrDefault()
                ) == pass : false;

        public int? FindIdByEmail(string email) => _repo.FindIdByEmail(email);

        public async Task<int?> FindIdByEmailAsync(string email) => await _repo.FindIdByEmailAsync(email);

        #endregion

        #region Validators

        public (bool isValid, string errorList) ValidateCustomer(Customer cust)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            var nameVal = ValidateName(cust.Name);
            var emailVal = ValidateEmail(cust.Email);
            var phoneVal = ValidatePhone(cust.Phone);
            var adrVal = ValidateAddress(cust.Address);
            var cityVal = ValidateCity(cust.City);
            var stateVal = ValidateState(cust.State);

            isValid = nameVal.isValid && emailVal.isValid && phoneVal.isValid && adrVal.isValid && cityVal.isValid && stateVal.isValid;
            errorList.Append(nameVal.errorList);
            errorList.Append(emailVal.errorList);
            errorList.Append(phoneVal.errorList);
            errorList.Append(adrVal.errorList);
            errorList.Append(cityVal.errorList);
            errorList.Append(stateVal.errorList);

            errorList.Replace("\r\n","");

            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateName(string name)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (name == null || name.Trim() == "")
            {
                errorList.AppendLine("Name cannot be blank.");
                isValid = false;
            }

            if (!string.IsNullOrEmpty(name) && name.Length > 100)
            {
                errorList.AppendLine("Name must not exceed 100 characters.");
                isValid = false;
            }


            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateEmail(string email)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (!string.IsNullOrEmpty(email))
            {
                if (email.Length > 100)
                {
                    errorList.AppendLine("Email must not exceed 100 characters.");
                    isValid = false;
                }

                try
                {
                    var standardized = new MailAddress(email);
                }
                catch
                {
                    errorList.AppendLine("Email must be in a valid format (account@provider).");
                    isValid = false;
                }
            }


            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidatePhone(string phone)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (phone == null || phone.Trim().Length == 0)
            {
                errorList.AppendLine("Customer requires a phone number.");
                isValid = false;
            }
            else
            {
                if (phone.Length != 12)
                {
                    errorList.AppendLine("Phone Numbers must have precisely 10 characters.");
                    isValid = false;
                }

                if (phone.Length > 7 && (phone.Substring(3, 1) != "-" || phone.Substring(7, 1) != "-"))
                {
                    errorList.AppendLine("Phone number must be in valid format: XXX-XXX-XXXX.");
                    isValid = false;
                }

                if (!phone.Replace("-", "").IsDigitsOnly())
                {
                    errorList.AppendLine("Phone number may only contain digits.");
                    isValid = false;
                }
            }

            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateAddress(string adr)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (adr == null || adr.Trim().Length == 0)
            {
                errorList.AppendLine("Address cannot be empty.");
                isValid = false;
            }
            else if (adr.Length > 200)
            {
                errorList.AppendLine("Address may not exceed 200 characters.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateCity(string city)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (city == null || city.Trim().Length == 0)
            {
                errorList.AppendLine("City field cannot be blank.");
                isValid = false;
            }
            else if (city.Length > 100)
            {
                errorList.AppendLine("City should not exceed 100 characters; If necessary, please use an abbreviation.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateState(string state)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (state == null || state.Trim().Length == 0)
            {
                errorList.AppendLine("State/County cannot be blank.");
                isValid = false;
            }
            else if (state.Length > 50)
            {
                errorList.Append("State/County is limited to 50 characters; Where necessary, please use an abbreviation.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");
            return (isValid, errorList.ToString());
        }

        #endregion

    }
}
