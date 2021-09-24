using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestCustomerRepo : ICustomerRepository
    {
        private TestDb _context;

        public TestCustomerRepo(TestDb context) => _context = context;

        public void Create(Customer cust)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for Customer named:\t" + cust.Name);
            if (cust.Name == null || cust.Phone == null || cust.Address == null || cust.City == null || cust.State == null) return;
            _context.Customers.Add(cust);
            StaticLogger.LogInfo(GetType(), "Customer:/t" + cust.Name + " succesfully registered.");
        }

        public void Update(Customer cust)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Customer with ID:\t" + cust.CustomerId);
            var custToUpd = Get(cust.CustomerId);
            try
            {
                custToUpd.Name = cust.Name;
                custToUpd.Phone = cust.Phone;
                custToUpd.Email = cust.Email;
                custToUpd.Address = cust.Address;
                custToUpd.City = cust.City;
                custToUpd.State = cust.State;
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer record with ID:\t" + cust.CustomerId + " to update. Details: " +
                    ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return;
            }
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Customer with ID:\t" + cust.CustomerId);
        }

        #region Gets

        public Customer Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Customer with ID:\t" + id);
            var cust = _context.Customers.SingleOrDefault(c => c.CustomerId == id);
            if (cust == null) StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + id);
            return cust;
        }

        public IEnumerable<Customer> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Customers...");
            var custs = _context.Customers.ToList();
            StaticLogger.LogInfo(GetType(), custs.Count().ToString() + " records found.");
            return custs;
        }

        public IEnumerable<Customer> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Customer> custList = null)
        {
            var pageScope = custList == null ? "all" : "filtered";
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from " + pageScope + " Customer records.");

            var custs = custList == null ? _context.Customers.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList() :
                                                        custList.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + custs.Count().ToString() + " records.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFiltered(string searchBy)
        {
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Customer records by:\t" + searchBy);
            var custs = new List<Customer>();

            var searchTerms = searchBy.Split(' ');
            foreach (var term in searchTerms)
            {
                var termToState = StateDictionary.States.SingleOrDefault(x => x.Key == term).Value;

                custs.AddRange
                    (
                        _context.Customers.Where(c =>
                            //(StateDictionary.States.ContainsKey(term) &&
                            //c.State.ToUpper().Contains(StateDictionary.States[term].ToUpper())) ||
                            c.State.ToUpper() == termToState ||
                            c.Name.ToUpper().Contains(term) ||
                            c.Email != null && c.Email.ToUpper().Contains(term) ||
                            c.State.ToUpper().Contains(term) ||
                            c.City.ToUpper().Contains(term) ||
                            c.Phone.Contains(term)||
                            c.Address.Contains(term))
                    );
            }

            custs = custs.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + custs.Count() + " Customers that match: " + searchBy);
            return custs.OrderBy(c => c.CustomerId);

        }

        public IEnumerable<Customer> GetAllSorted(string sortBy, bool desc=false, IEnumerable<Customer> custList = null)
        {
            var custs = new List<Customer>();
            var custScope = custList == null ? _context.Customers : custList;

            var log = new StringBuilder("Repo fetching ");
            if (custList == null) log.Append("all");
            log.Append(" Customers");
            if (custList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return custScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "NAME":
                    custs = desc ? custScope.OrderByDescending(c => c.Name).ToList() :
                        custScope.OrderBy(c => c.Name).ToList();
                    log.Append("Name");
                    break;
                case "ADR":
                case "ADDRESS":
                    custs = desc ? custScope.OrderByDescending(c => c.Address).ToList() :
                        custScope.OrderBy(c => c.Address).ToList();
                    log.Append("Address");
                    break;
                case "PHONE":
                case "PHONENUMBER":
                case "NUMBER":
                case "PHONENO":
                case "PHONE_NO":
                case "PHONE_NUMBER":
                case "TELEPHONE":
                    custs = desc ? custScope.OrderByDescending(c => c.Phone).ToList() :
                        custScope.OrderBy(c => c.Phone).ToList();
                    log.Append("Phone");
                    break;
                case "EMAIL":
                case "E-MAIL":
                case "MAIL":
                    custs = desc ? custScope.OrderByDescending(c => c.Email).ToList() :
                        custScope.OrderBy(c => c.Email).ToList();
                    log.Append("E-mail");
                    break;
                case "CITY":
                case "TOWN":
                    custs = desc ? custScope.OrderByDescending(c => c.City).ToList() :
                        custScope.OrderBy(c => c.City).ToList();
                    log.Append("City");
                    break;
                case "COUNTY":
                case "STATE":
                    custs = desc ? custScope.OrderByDescending(c => c.State).ToList() :
                        custScope.OrderBy(c => c.State).ToList();
                    log.Append("State/County");
                    break;
                default:
                    custs = desc ? custScope.OrderByDescending(c => c.CustomerId).ToList() :
                        custScope.OrderBy(c => c.CustomerId).ToList();
                    log.Append("ID");
                    break;
            }


            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), custs.Count().ToString() + " records found.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Customers, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var custs = GetAllFiltered(searchBy);
            if (custs.Count() == 0) return custs;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Customer records.");
            custs = custs.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + custs.Count().ToString() + " records.");
            return custs;
        }

        public IEnumerable<Customer> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var descTxt = desc ? " descending" : " ascending";
            StaticLogger.LogInfo(GetType(), "Repo Sorting Customers by " + sortBy + descTxt + 
                ", then retrieving page " + pgIndex + " of size " + pgSize + ".");
            var custs = GetAllPaginated(pgSize, pgIndex, GetAllSorted(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo got " + custs.Count() + " results.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering results by: " + searchBy + ", then sorting the result by: " + sortBy + ".");
            var custs = GetAllSorted(sortBy, desc, GetAllFiltered(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo found " + custs.Count() + " results.");
            return custs;
        }

        public IEnumerable<Customer> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Customer records filtered by: " + searchBy + ", sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order. Page: " + pgIndex + " of size: " + pgSize + ".");
            var custs = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(),"Repo retrieved " + custs.Count() + " Customer records.");
            return custs;
        }

        #endregion

        public bool CustomerIdExists(int id) => _context.Customers.Any(c => c.CustomerId == id);

        public bool EmailExists(string email) => _context.Customers.SingleOrDefault(c => c.Email == email) != null;

        public bool PhoneExists(string phone) => _context.Customers.SingleOrDefault(c => c.Phone == phone) != null;

        public int? FindIdByEmail(string email)
        {
            try
            {
                return _context.Customers.SingleOrDefault(c => c.Email == email).CustomerId;
            }

            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to fetch Account by Email: " + email + ", but found no Customer using the given E-mail Address. Please check the repo. \nException details: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                return null;
            }
            catch (InvalidOperationException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to fetch Account by Email: " + email+", but found multiple Customers using the same E-mail Address. Please check the repo. \nException details: " + ex.Message + "\nStack Trace: "+ex.StackTrace);
                return null;
            }
        }

        public string GetPassByCustomerId(int custId)
        {
            try
            {
                return _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Phone.Substring(0, 3);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to extract an Account's password by the Customer ID: " + custId + ", but could not find said ID.\nException Details: " + ex.Message + "\n Stack Trace: " + ex.StackTrace);
                return null;
            }
        }

        #region Async

        #region Gets

        public async Task<Customer> GetAsync(int id)
        {
            return Get(id);
            //StaticLogger.LogInfo(GetType(), "Repo fetching Customer with ID:\t" + id);
            //var cust = await _context.Customers.FindAsync(id);
            //if (cust == null) StaticLogger.LogWarn(GetType(), "Repo found no Customer with ID:\t" + id);
            //return cust;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Customers...");
            var custs = _context.Customers;
            StaticLogger.LogInfo(GetType(), custs.Count().ToString() + " records found.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<Customer> custList = null)
        {
            var pageScope = custList == null ? "all" : "filtered";
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from " + pageScope + " Customer records.");

            if (custList == null) custList = await GetAllAsync();

            var custs = GetAllPaginated(pgSize, pgIndex, custList);

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + custs.Count().ToString() + " records.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAsync(string searchBy)
        {
            return GetAllFiltered(searchBy);
            //searchBy = searchBy.ToUpper();
            //StaticLogger.LogInfo(GetType(), "Repo searching Customer records by:\t" + searchBy);
            //var custs = new List<Customer>();

            //var searchTerms = searchBy.Split(' ');
            //foreach (var term in searchTerms)
            //{
            //    var termToState = StateDictionary.States.SingleOrDefault(x => x.Key == term).Value;

            //    custs = await _context.Customers.Where(c =>
            //               c.State.ToUpper() == termToState ||
            //               c.Name.ToUpper().Contains(term) ||
            //               c.Email != null && c.Email.ToUpper().Contains(term) ||
            //               c.State.ToUpper().Contains(term) ||
            //               c.City.ToUpper().Contains(term) ||
            //               c.Phone.Contains(term) ||
            //               c.Address.Contains(term)
            //        ).ToListAsync();
            //}

            //StaticLogger.LogInfo(GetType(), "Repo found " + custs.Count() + " Customers that match: " + searchBy);
            //return custs.OrderBy(c => c.CustomerId);
        }

        public async Task<IEnumerable<Customer>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<Customer> custList = null)
        {
            var custScope = custList == null ? await GetAllAsync() : custList;

            var log = new StringBuilder("Repo fetching ");
            if (custList == null) log.Append("all");
            log.Append(" Customers");
            if (custList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            StaticLogger.LogInfo(GetType(), log.ToString());
            return GetAllSorted(sortBy, desc, custScope);
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Customers, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var custs = await GetAllFilteredAsync(searchBy);
            if (custs.Count() == 0) return custs;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Customer records.");
            custs = await GetAllPaginatedAsync(pgSize, pgIndex, custs);
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + custs.Count().ToString() + " records.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredAndSortedAsync(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering results by: " + searchBy + ", then sorting the result by: " + sortBy + ".");
            var custs = await GetAllSortedAsync(sortBy, desc, await GetAllFilteredAsync(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo found " + custs.Count() + " results.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var descTxt = desc ? " descending" : " ascending";
            StaticLogger.LogInfo(GetType(), "Repo Sorting Customers by " + sortBy + descTxt + ", then retrieving page " + pgIndex + " of size " + pgSize + ".");
            var custs = await GetAllPaginatedAsync(pgSize, pgIndex, await GetAllSortedAsync(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo got " + custs.Count() + " results.");
            return custs;
        }

        public async Task<IEnumerable<Customer>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Customer records filtered by: " + searchBy + ", sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order. Page: " + pgIndex + " of size: " + pgSize + ".");
            var custs = await GetAllPaginatedAsync(pgSize, pgIndex,
                        await GetAllFilteredAndSortedAsync(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + custs.Count() + " Customer records.");
            return custs;
        }

        #endregion

        public async Task CreateAsync(Customer cust)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for Customer named:\t" + cust.Name);
            _context.Customers.Add(cust);
            StaticLogger.LogInfo(GetType(), "Customer:/t" + cust.Name + " succesfully registered.");
        }

        public async Task UpdateAsync(Customer cust)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Customer with ID:\t" + cust.CustomerId);
            var custToUpd = await GetAsync(cust.CustomerId);
            try
            {
                custToUpd.Name = cust.Name;
                custToUpd.Phone = cust.Phone;
                custToUpd.Email = cust.Email;
                custToUpd.Address = cust.Address;
                custToUpd.City = cust.City;
                custToUpd.State = cust.State;
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer record with ID:\t" + cust.CustomerId + " to update. Details: " +
                    ex.Message + "\nStack Trace:\t" + ex.StackTrace);
                return;
            }
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Customer with ID:\t" + cust.CustomerId);
        }

        public async Task<bool> CustomerIdExistsAsync(int id)
            => CustomerIdExists(id);
        //=> await _context.Customers.FindAsync(id) != null;

        public async Task<bool> EmailExistsAsync(string email)
        => _context.Customers.SingleOrDefault(c => c.Email == email) != null;

        public async Task<bool> PhoneExistsAsync(string phone)
        => _context.Customers.SingleOrDefault(c => c.Phone==phone) != null;

        // Main Window Methods

        public async Task<int?> FindIdByEmailAsync(string email)
        {
            try
            {
                //var cust = await _context.Customers.SingleOrDefaultAsync(c => c.Email == email);   
                var cust = _context.Customers.SingleOrDefault(c => c.Email == email);
                return cust.CustomerId;
            }

            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to fetch Account by Email: " + email + ", but found no Customer using the given E-mail Address. Please check the repo. \nException details: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                return null;
            }
            catch (InvalidOperationException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to fetch Account by Email: " + email + ", but found multiple Customers using the same E-mail Address. Please check the repo. \nException details: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
                return null;
            }
        }

        public async Task<string> GetPassByCustomerIdAsync(int custId)
        {
            try
            {
                var cust = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);
                return cust.Phone.Substring(0, 3);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo tried to extract an Account's password by the Customer ID: " + custId + ", but could not find said ID.\nException Details: " + ex.Message + "\n Stack Trace: " + ex.StackTrace);
                return null;
            }
        }

        #endregion
    }
}
