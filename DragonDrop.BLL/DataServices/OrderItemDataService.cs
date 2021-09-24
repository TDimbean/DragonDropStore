using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.BLL.DataServices
{
    public class OrderItemDataService : IOrderItemDataService
    {
        private IOrderItemRepository _repo;

        public OrderItemDataService(IOrderItemRepository repo)
        {
            _repo = repo;
        }

        public string Create(OrderItem item, bool requiresErrList = false)
        {
            var entryExists = false;
            var entryError = "";

            if (_repo.Get(item.OrderId, item.ProductId) != null)
            {
                entryError = "OrderItem already exists. If you wish to change the Quantity, please use the Update.";
                entryExists = true;
            }

            var validation = ValidateOrderItem(item);

            if (entryExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "OrderItem failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList + entryError);
                if (requiresErrList) return validation.errorList + entryError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting record creation for OrderItem with Order ID:\t" + item.OrderId + " and Product ID:\t" + item.ProductId);
            _repo.Create(item);
            StaticLogger.LogInfo(GetType(), "OrderItem Repo returned to Data Service.");
            return null;
        }

        public string Update(OrderItem item, bool requiresErrList = false)
        {
            var entryExist = true;
            var entryError = "";

            if (_repo.Get(item.OrderId, item.ProductId) == null)
            {
                entryError = "\nOrder Item with Order/Product IDs: " + item.OrderId + "/" + item.ProductId + " was not found.";
                entryExist = false;
            }

            var validation = ValidateOrderItem(item);

            if (!entryExist || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "OrderItem failed Data Service validation and will not be sent to the Repo. Reason: " + validation.errorList + entryError);
                if (requiresErrList) return validation.errorList + entryError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting update for record with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId + " from Repo.");
            _repo.Update(item);
            StaticLogger.LogInfo(GetType(), "Repo finished update and returned to OrderItem Data Service.");
            return null;
        }

        #region Gets

        public OrderItem Get(int ordId, int prodId)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting OrderItem with Order ID:\t" + ordId + " and Product ID:\t" + prodId + " from Repo.");
            var item = _repo.Get(ordId, prodId);
            if (item == null) StaticLogger.LogWarn(GetType(), "OrderItem Data Service found no OrderItem with Order/Product IDs:\t" + ordId + "/" + prodId);
            return item;
        }

        public IEnumerable<OrderItem> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting fetching all from Repo.");
            var items = _repo.GetAll();
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " +items.Count().ToString() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllPaginated(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting page " + pgIndex + " of size " + pgSize + " from all OrderItem records in Repo.");
            var items = _repo.GetAllPaginated(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count().ToString() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderId(int ordId)
        {
            if (!_repo.OrderIdExists(ordId))
            {
                StaticLogger.LogError(GetType(), "No order with ID:\t" + ordId + " was found in Repo, so OrderItem Data Service cannot request its associated items.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting all OrderItem with Order ID:\t" + ordId +" from Repo.");
            var items = _repo.GetAllByOrderId(ordId);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductId(int prodId)
        {
            if (!_repo.ProductIdExists(prodId))
            {
                StaticLogger.LogError(GetType(), "No product with ID:\t" + prodId + " was found in Repo, so OrderItem Data Service cannot request its associated OrderItems.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting all OrderItem for Product with ID:\t" + prodId+" from Repo.");
            var items = _repo.GetAllByProductId(prodId);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdAndPaged(int ordId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting OrderItems, according to \n ORDER ID: " + ordId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo.");
            var items = _repo.GetAllByOrderIdAndPaged(ordId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service received " + items.Count().ToString() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdAndPaged(int prodId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting OrderItems, according to \n PRODUCT ID: " + prodId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize+" from Repo.");
            var items = _repo.GetAllByProductIdAndPaged(prodId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service received " + items.Count().ToString() + " records from Repo.");
            return items;
        }

        //NEWS

        public IEnumerable<OrderItem> GetAllSorted(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting records sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllSorted(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: " + pgIndex + " of Size: "+pgSize+" of records sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdAndSorted(int ordId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting Order Items for Order with ID: " + ordId + " sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllByOrderIdAndSorted(ordId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdAndSorted(int prodId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Order Items for Product with ID: " + prodId + " Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllByProductIdAndSorted(prodId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdSortedAndPaged(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of Order Items for Order with ID: " + ordId + ", Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllByOrderIdSortedAndPaged(ordId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdSortedAndPaged(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: "+ pgIndex + " of Size: " + pgSize + "of Order Items for Product with ID: " + prodId + ", Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = _repo.GetAllByProductIdSortedAndPaged(prodId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        #endregion

        #region Async
        public async Task<string> CreateAsync(OrderItem item, bool requiresErrList = false)
        {
            var entryExists = false;
            var entryError = "";

            if (await _repo.GetAsync(item.OrderId, item.ProductId) != null)
            {
                entryError = "OrderItem already exists. If you wish to change the Quantity, please use the Update.";
                entryExists = true;
            }

            var validation = ValidateOrderItem(item);

            if (entryExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "OrderItem failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList + entryError);
                if (requiresErrList) return validation.errorList + entryError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting record creation for OrderItem with Order ID:\t" + item.OrderId + " and Product ID:\t" + item.ProductId);
            await _repo.CreateAsync(item);
            StaticLogger.LogInfo(GetType(), "OrderItem Repo returned to Data Service.");
            return null;
        }

        public async Task<string> UpdateAsync(OrderItem item, bool requiresErrList = false)
        {
            var entryExist = true;
            var entryError = "";

            if (await _repo.GetAsync(item.OrderId, item.ProductId) == null)
            {
                entryError = "\nOrder Item with Order/Product IDs: " + item.OrderId + "/" + item.ProductId + " was not found.";
                entryExist = false;
            }

            var validation = ValidateOrderItem(item);

            if (!entryExist || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "OrderItem failed Data Service validation and will not be sent to the Repo. Reason: " + validation.errorList + entryError);
                if (requiresErrList) return validation.errorList + entryError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting update for record with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId + " from Repo.");
            await _repo.UpdateAsync(item);
            StaticLogger.LogInfo(GetType(), "Repo finished update and returned to OrderItem Data Service.");
            return null;
        }

        #region Gets

        public async Task<OrderItem> GetAsync(int ordId, int prodId)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting OrderItem with Order ID:\t" + ordId + " and Product ID:\t" + prodId + " from Repo.");
            var item = await _repo.GetAsync(ordId, prodId);
            if (item == null) StaticLogger.LogWarn(GetType(), "OrderItem Data Service found no OrderItem with Order/Product IDs:\t" + ordId + "/" + prodId);
            return item;
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting fetching all from Repo.");
            var items = await _repo.GetAllAsync();
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count().ToString() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllPaginatedAsync(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting page " + pgIndex + " of size " + pgSize + " from all OrderItem records in Repo.");
            var items = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count().ToString() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(int ordId)
        {
            if (!await _repo.OrderIdExistsAsync(ordId))
            {
                StaticLogger.LogError(GetType(), "No order with ID:\t" + ordId + " was found in Repo, so OrderItem Data Service cannot request its associated items.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting all OrderItem with Order ID:\t" + ordId + " from Repo.");
            var items = await _repo.GetAllByOrderIdAsync(ordId);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAsync(int prodId)
        {
            if (!await _repo.ProductIdExistsAsync(prodId))
            {
                StaticLogger.LogError(GetType(), "No product with ID:\t" + prodId + " was found in Repo, so OrderItem Data Service cannot request its associated OrderItems.");
                return null;
            }

            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting all OrderItem for Product with ID:\t" + prodId + " from Repo.");
            var items = await _repo.GetAllByProductIdAsync(prodId);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service got " + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAndPagedAsync
            (int ordId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service requesting OrderItems, according to \n ORDER ID: " +
                ordId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo.");
            var items = await _repo.GetAllByOrderIdAndPagedAsync(ordId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service received " +
                items.Count().ToString() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAndPagedAsync
            (int prodId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), 
                "OrderItem Data Service requesting OrderItems, according to \n PRODUCT ID: " +
                prodId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize + " from Repo.");
            var items = await _repo.GetAllByProductIdAndPagedAsync(prodId, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "OrderItem Data Service received " +
                items.Count().ToString() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllSortedAsync
            (string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting records sorted by: " +
                sortBy + " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = await _repo.GetAllSortedAsync(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " 
                + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of records sorted by: " + sortBy +
                " in " + (desc ? "descending" : "ascending") + " order from Repo.");
            var items = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + 
                items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAndSortedAsync
            (int ordId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), 
                "OrderItem Data Service requesting Order Items for Order with ID: " +
                ordId + " sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + 
                " order from Repo.");
            var items = await _repo.GetAllByOrderIdAndSortedAsync(ordId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAndSortedAsync
            (int prodId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), 
                "Order Item Data Service requesting Order Items for Product with ID: " +
                prodId + " Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + 
                " order from Repo.");
            var items = await _repo.GetAllByProductIdAndSortedAsync(prodId, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdSortedAndPagedAsync
            (int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of Order Items for Order with ID: " + 
                ordId + ", Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + 
                " order from Repo.");
            var items = await _repo.GetAllByOrderIdSortedAndPagedAsync
                (ordId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Order Item Data Service received " + items.Count() + " records from Repo.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdSortedAndPagedAsync
            (int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Order Item Data Service requesting Page: " 
                + pgIndex + " of Size: " + pgSize + "of Order Items for Product with ID: " 
                + prodId + ", Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending")
                + " order from Repo.");
            var items = await _repo.GetAllByProductIdSortedAndPagedAsync
                (prodId, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        #endregion

        #endregion

        public (bool isValid, string errorList) ValidateOrderItem(OrderItem item)
        {
            var ordVal = ValidateOrderId(item.OrderId);
            var prodVal = ValidateProductId(item.ProductId);
            var qtyVal = ValidateQuantity(item.Quantity);

            var isValid = ordVal.isValid && prodVal.isValid && qtyVal.isValid;

            var errorList = new StringBuilder(ordVal.error, 3);
            errorList.Append(prodVal.error);
            errorList.Append(qtyVal.error);
            errorList.Replace("\r\n", "");

            return (isValid, errorList.ToString());
        }

        public (bool isValid, string error) ValidateQuantity(int qty)
        {
            if (qty <= 0)
                return (false, "OrderItem must have a quantity greater than 0.");
            else return (true, string.Empty);
        }

        public (bool isValid, string error) ValidateOrderId(int ordId)
        {
            if (!_repo.OrderIdExists(ordId))
                return(false, "OrderItem needs to target a valid Order Id.");
            else return (true, string.Empty);
        }

        public (bool isValid, string error) ValidateProductId(int prodId)
        {
            if (!_repo.ProductIdExists(prodId))
                return (false, "OrderItem needs to target a valid Product Id.");
            else return (true, string.Empty);
        }
    }
}
