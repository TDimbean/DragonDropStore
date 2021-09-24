using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.BLL.DataServices
{
    public class OrderDataService : IOrderDataService
    {
        private IOrderRepository _repo;

        public OrderDataService(IOrderRepository repo)=> _repo = repo;

        public string Create(Order ord, bool requiresErrList = false)
        {
            var validation = ValidateOrder(ord);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Order failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Service requesting creation of record for Order by Repo.");
            _repo.Create(ord);
            StaticLogger.LogInfo(GetType(), "Repo returned to Data Service.");
            return null;
        }

        public string Update(Order ord, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            var validation = ValidateOrder(ord);

            if (!_repo.OrderIdExists(ord.OrderId))
            {
                idError = "Order ID must exist within the Repo for an update to be performed; Order Data Service could not find any Order with ID: " + ord.OrderId + " in Repo.\n";
                idExists = false;
            }

            if(!validation.isValid || !idExists)
            {
                StaticLogger.LogError(GetType(), "Order failed Data Service validation and will not be submitted to the Repo for update. Reason: " + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Service requesting update for record with ID:\t" + ord.OrderId + " from Repo.");
            _repo.Update(ord);
            StaticLogger.LogInfo(GetType(), "Repo returned to Order Data Service");
            return null;
        }

        #region Gets

        public Order Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Order with ID:\t" + id + " from Repo.");
            var ord = _repo.Get(id);
            if (ord == null) StaticLogger.LogWarn(GetType(), "Order Data Service found no Order with ID:\t" + id + " in Repo.");
            return ord;
        }

        public IEnumerable<Order> GetAllUnprocessed()
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Unprocessed Orders from Repo.");
            var ords = _repo.GetAllUnprocessed();
            StaticLogger.LogInfo(GetType(), "Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllProcessed()
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Processed Orders from Repo.");
            var ords = _repo.GetAllProcessed();
            StaticLogger.LogInfo(GetType(), "Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Orders from Repo.");
            var ords = _repo.GetAll();
            StaticLogger.LogInfo(GetType(),"Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllPaginated(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting page " + pgIndex + " of size " + pgSize + " from all records in Repo.");
            var ords = _repo.GetAllPaginated(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllFiltered(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Order records filtered by:\t" + searchBy);
            var ords = _repo.GetAllFiltered(searchBy);
            StaticLogger.LogInfo(GetType(), "Order Data Service got " + ords.Count() + " records back from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Orders by \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo.");
            var ords = _repo.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        #region ByCustomerId

        public IEnumerable<Order> GetAllByCustomerId(int id)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting all Orders placed by Customer with ID: " + id + " from Repo.");
            if(!_repo.CustomerIdExists(id))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + id + " was found in Repo.");
                return new List<Order>();
            }

            var ords = _repo.GetAllByCustomerId(id);
            StaticLogger.LogInfo(GetType(), "Order Data Service got: " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex)
        {
            if(!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Servive requesting page: " + pgIndex + " of size: " + pgSize + " from Customer with ID: " + custId + "'s Orders.");
            var ords = _repo.GetAllByCustomerIdAndPaged(custId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service got: " + ords.Count() + " records from Repo.");

            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndFiltered(int custId, string searchBy)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + "and Filtered by: " + searchBy + " from Repo.");
            var ords = _repo.GetAllByCustomerIdAndFiltered(custId, searchBy);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: "+pgIndex+" of Size: " + pgSize +" of records from Customer with ID: " + custId + "and Filtered by: " + searchBy + " from Repo.");
            var ords = _repo.GetAllByCustomerIdFilteredAndPaged(custId, searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        #endregion

        #region Sorts

        public IEnumerable<Order> GetAllSorted(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllSorted(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: "+pgIndex+ "of Size: "+pgSize+" of records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records Filtered by: "+ searchBy + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllFilteredAndSorted(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllByCustomerIdAndSorted(custId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + ", Filtered by: " + searchBy + " and Sorted in "+(desc?"descending":"ascending") + " order by " + sortBy + " from Repo.");
            var ords = _repo.GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + "from records Filtered by:" + searchBy + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " from records placed by Customer with ID:" + custId + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllByCustomerIdSortedAndPaged(custId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!_repo.CustomerIdExists(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " from records placed by Customer with ID:" + custId + " Filtered by: "+searchBy+" and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = _repo.GetAllByCustomerIdFilteredSortedAndPaged(custId, searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        #endregion

        #endregion

        #region StatusAdvancements

        public void PromoteReceived(int ordId) => _repo.PromoteReceived(ordId);

        public void BatchPromoteReceived(List<int> ordIds) => _repo.BatchPromoteReceived(ordIds);

        public void PromoteProcessed(int ordId, DateTime shipDate) => _repo.PromoteProcessed(ordId, shipDate);

        #endregion

        #region Async

        public async Task<string> CreateAsync(Order ord, bool requiresErrList = false)
        {
            var validation = ValidateOrder(ord);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Order failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Service requesting creation of record for Order by Repo.");
            await _repo.CreateAsync(ord);
            StaticLogger.LogInfo(GetType(), "Repo returned to Data Service.");
            return null;
        }

        public async Task<string> UpdateAsync(Order ord, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            var validation = ValidateOrder(ord);

            if (!await _repo.OrderIdExistsAsync(ord.OrderId))
            {
                idError = "Order ID must exist within the Repo for an update to be performed; Order Data Service could not find any Order with ID: " + ord.OrderId + " in Repo.\n";
                idExists = false;
            }

            if (!validation.isValid || !idExists)
            {
                StaticLogger.LogError(GetType(), "Order failed Data Service validation and will not be submitted to the Repo for update. Reason: " + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Service requesting update for record with ID:\t" + ord.OrderId + " from Repo.");
            await _repo.UpdateAsync(ord);
            StaticLogger.LogInfo(GetType(), "Repo returned to Order Data Service");
            return null;
        }

        #region Gets

        public async Task<Order> GetAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Order with ID:\t" + id + " from Repo.");
            var ord = await _repo.GetAsync(id);
            if (ord == null) StaticLogger.LogWarn(GetType(), "Order Data Service found no Order with ID:\t" + id + " in Repo.");
            return ord;
        }

        public async Task<IEnumerable<Order>> GetAllUnprocessedAsync()
        {
                StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Unprocessed Orders from Repo.");
                var ords = await _repo.GetAllUnprocessedAsync();
                StaticLogger.LogInfo(GetType(), "Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
                return ords;
        }

        public async Task<IEnumerable<Order>> GetAllProcessedAsync()
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Processed Orders from Repo.");
            var ords = await _repo.GetAllProcessedAsync();
            StaticLogger.LogInfo(GetType(), "Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service retrieving all Orders from Repo.");
            var ords = await _repo.GetAllAsync();
            StaticLogger.LogInfo(GetType(), "Order Data Service  got " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllPaginatedAsync(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting page " + pgIndex + " of size " + pgSize + " from all records in Repo.");
            var ords = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAsync(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Order records filtered by:\t" + searchBy);
            var ords = await _repo.GetAllFilteredAsync(searchBy);
            StaticLogger.LogInfo(GetType(), "Order Data Service got " + ords.Count() + " records back from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Orders by \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo.");
            var ords = await _repo.GetAllFilteredAndPagedAsync(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count().ToString() + " records from Repo.");
            return ords;
        }

        #region ByCustomerId

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting all Orders placed by Customer with ID: " + id + " from Repo.");
            if (!await _repo.CustomerIdExistsAsync(id))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + id + " was found in Repo.");
                return new List<Order>();
            }

            var ords = await _repo.GetAllByCustomerIdAsync(id);
            StaticLogger.LogInfo(GetType(), "Order Data Service got: " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "Order Data Servive requesting page: " + pgIndex + " of size: " + pgSize + " from Customer with ID: " + custId + "'s Orders.");
            var ords = await _repo.GetAllByCustomerIdAndPagedAsync(custId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service got: " + ords.Count() + " records from Repo.");

            return ords;
        }


        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndFilteredAsync(int custId, string searchBy)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + "and Filtered by: " + searchBy + " from Repo.");
            var ords = await _repo.GetAllByCustomerIdAndFilteredAsync(custId, searchBy);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndPagedAsync(int custId, string searchBy, int pgSize, int pgIndex)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of records from Customer with ID: " + custId + "and Filtered by: " + searchBy + " from Repo.");
            var ords = await _repo.GetAllByCustomerIdFilteredAndPagedAsync(custId, searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        #endregion

        #region Sorts

        public async Task<IEnumerable<Order>> GetAllSortedAsync(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllSortedAsync(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + "of Size: " + pgSize + " of records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records Filtered by: " + searchBy + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllByCustomerIdAndSortedAsync(custId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting records from Customer with ID: " + custId + ", Filtered by: " + searchBy + " and Sorted in " + (desc ? "descending" : "ascending") + " order by " + sortBy + " from Repo.");
            var ords = await _repo.GetAllByCustomerIdFilteredAndSortedAsync(custId, searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + "from records Filtered by:" + searchBy + "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllFilteredSortedAndPagedAsync(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex +
                " of Size: " + pgSize + " from records placed by Customer with ID:" + custId +
                "and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                " order from Repo.");
            var ords = await _repo.GetAllByCustomerIdSortedAndPagedAsync
                (custId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            if (!await _repo.CustomerIdExistsAsync(custId))
            {
                StaticLogger.LogWarn(GetType(), "No Customer with ID: " + custId + " exists in Repo. Request for their Orders cannot proceed.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Order Data Service requesting Page: " + pgIndex +
                " of Size: " + pgSize + " from records placed by Customer with ID:" + custId +
                " Filtered by: " + searchBy + " and Sorted by: " + sortBy + " in " +
                (desc ? "descending" : "ascending") + " order from Repo.");
            var ords = await _repo.GetAllByCustomerIdFilteredSortedAndPagedAsync
                (custId, searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Data Service received " + ords.Count() + " records from Repo.");
            return ords;
        }

        #endregion

        #endregion


        #region StatusAdvancements

        public async Task PromoteReceivedAsync(int ordId) => await _repo.PromoteReceivedAsync(ordId);
              
        public async Task BatchPromoteReceivedAsync(List<int> ordIds) 
            => await _repo.BatchPromoteReceivedAsync(ordIds);
            
        public async Task PromoteProcessedAsync(int ordId, DateTime shipDate) 
            => await _repo.PromoteProcessedAsync(ordId, shipDate);

        #endregion

        #endregion

        #region Validates

        public (bool isValid, string errorList) ValidateOrder(Order ord)
        {
            var custVal = ValidateCustomerId(ord.CustomerId);
            var ordDateVal = ValidateOrderDate(ord.OrderDate);
            var shipDateVal = ValidateShippingDate(ord.ShippingDate, ord.OrderDate);
            var statVal = ValidateOrderStatusId(ord.OrderStatusId);
            var payMethVal = ValidatePaymentMethodId(ord.PaymentMethodId);
            var shipMethVal = ValidateShippingMethodId(ord.ShippingMethodId);
            var shipAndStatVal = ValidateShipWithStatus(ord.ShippingDate, ord.OrderStatusId);

            var isValid = custVal.isValid && ordDateVal.isValid
                        && shipDateVal.isValid && statVal.isValid
                        && payMethVal.isValid && shipMethVal.isValid
                        && shipAndStatVal.isValid;

            if (isValid) return (true, string.Empty);

            var errorList = new StringBuilder(custVal.errorList)
                            .AppendLine(ordDateVal.errorList)
                            .AppendLine(shipDateVal.errorList)
                            .AppendLine(statVal.errorList)
                            .AppendLine(payMethVal.errorList)
                            .AppendLine(shipMethVal.errorList)
                            .AppendLine(shipAndStatVal.errorList);

            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateCustomerId(int custId)
            => _repo.CustomerIdExists(custId) ? (true, string.Empty) :
                 (false, "No Customer with ID: " + custId +
                " found in Repo. Orders require an existing Customer to be associated with.");

        public (bool isValid, string errorList) ValidateOrderDate(DateTime date)
            => date <= DateTime.Now ? (true, string.Empty) :
                (false, "Order Date too far into the future; Must be no later than right now.");

        public (bool isValid, string errorList) ValidateShippingDate(DateTime? shipDate, DateTime ordDate)
            => shipDate != null && shipDate < ordDate ?
                (false, "Orders cannot be Shipped before being placed.") :
                (true, string.Empty);

        public (bool isValid, string errorList) ValidateOrderStatusId(int statId)
            => _repo.OrderStatusIdExists(statId) ? (true, string.Empty) :
                (false, "Orders must have a Status associated with them.");

        public (bool isValid, string errorList) ValidatePaymentMethodId(int methId)
            => _repo.PaymentMethodIdExists(methId) ? (true, string.Empty) :
                (false, "Orders must have a Payment Method associated with them.");

        public (bool isValid, string errorList) ValidateShippingMethodId(int shipId)
            => _repo.ShippingMethodIdExists(shipId) ? (true, string.Empty) :
                (false, "Orders must have a Shipping Method associated with them.");

        public (bool isValid, string errorList) ValidateShipWithStatus(DateTime? date, int statId)
            => date == null && statId > 1 ?
                (false, "An Order's Shipping Date cannot be missing if it has been Shipped or Delivered.") :
                (true, string.Empty);

        #endregion
    }
}
