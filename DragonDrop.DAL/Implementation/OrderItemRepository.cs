using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.DAL.Implementation
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private DragonDrop_DbContext _context;

        public OrderItemRepository(DragonDrop_DbContext context) => _context = context;

        public void Create(OrderItem item)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for OrderItem for Order:\t" + item.OrderId + " for Product:\t" + item.ProductId);

            //Check that all the OrderItems data is Valid
            if (!_context.Orders.Any(o => o.OrderId == item.OrderId) ||
                !_context.Products.Any(p => p.ProductId == item.ProductId) ||
                item.Quantity < 1)
            {
                StaticLogger.LogError(GetType(), "Repo could not register OrderItem because either the Order or Product ID was non-existent or the requested quantity was 0.\nOrder ID:\t" + item.OrderId + " Product ID:\t" + item.ProductId);
                return;
            }

            if (_context.OrderItems.Any(i => i.OrderId == item.OrderId && i.ProductId == item.ProductId))
            {
                StaticLogger.LogError(GetType(), "Repo failed to add item as Orderd ID:" + item.OrderId + " already contains an entry for Product ID:\t" + item.ProductId + "\n If you wish to edit the quantity, please Update the OrderItem rather than re-adding it.");
                return;
            }

            _context.OrderItems.Add(item);
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "OrderItem succesfully registered.");
        }

        public void Update(OrderItem item)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for OrderItem with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId);
            var itemToUpd = Get(item.OrderId, item.ProductId);

            if (item.Quantity < 1)
            {
                StaticLogger.LogError(GetType(), "Repo failed to update OrderItem because requested quantity was 0.");
                return;
            }

            itemToUpd.Quantity = item.Quantity;
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated OrderItem with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId);
        }

        #region Gets

        public OrderItem Get(int ordId, int prodId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching OrderItem for Order ID:\t" + ordId + " of Product ID:\t" + prodId);
            var item = _context.OrderItems.SingleOrDefault(i => i.OrderId == ordId && i.ProductId == prodId);
            if (item == null) StaticLogger.LogWarn(GetType(), "Repo found no OrderItem with Order/Product IDs:\t" + ordId + "/" + prodId);
            return item;
        }

        public IEnumerable<OrderItem> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItems...");
            var items = _context.OrderItems.ToList();
            StaticLogger.LogInfo(GetType(), items.Count().ToString() + " records found.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<OrderItem> itemList = null)
        {
            if (pgIndex == 0) return GetAll();
            pgSize = Math.Abs(pgSize);
            pgIndex = Math.Abs(pgIndex);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from "+ (itemList==null ? "all":"selected")+" OrderItem records.");

            var itemScope = itemList == null ? _context.OrderItems : itemList;

            var items = itemScope.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderId(int ordId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Order ID:\t" + ordId);
            var items = _context.OrderItems.Where(i => i.OrderId == ordId).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Order ID:\t" + ordId);
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductId(int prodId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Product ID:\t" + prodId);
            var items = _context.OrderItems.Where(i => i.ProductId == prodId).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Product ID:\t" + prodId);
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdAndPaged(int ordId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n ORDER ID: " + ordId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var items = GetAllByOrderId(ordId);
            if (items.Count() == 0) return items;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            items = items.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdAndPaged(int prodId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n PRODUCT ID: " + prodId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var items = GetAllByProductId(prodId);
            if (items.Count() == 0) return items;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            items = items.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllSorted(string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null)
        {
            var items = new List<OrderItem>();
            var itemScope = itemList == null ? _context.OrderItems : itemList;

            var log = new StringBuilder("Repo fetching ");
            if (itemList == null) log.Append("all");
            log.Append(" OrderItems");
            if (itemList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return itemScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "ORDER":
                case "ORD":
                case "ORDERID":
                case "ORDID":
                case "ORDER_ID":
                case "ORD_ID":
                    items = desc ? itemScope.OrderByDescending(i => i.OrderId).ToList() :
                        itemScope.OrderBy(i => i.OrderId).ToList();
                    log.Append("Order ID");
                    break;
                case "PRODUCT":
                case "PROD":
                case "PRODUCTID":
                case "PRODID":
                case "PRODUCT_ID":
                case "PROD_ID":
                    items = desc ? itemScope.OrderByDescending(i => i.ProductId).ToList() :
                        itemScope.OrderBy(i => i.ProductId).ToList();
                    log.Append("Product ID");
                    break;
                case "QUANTITY":
                case "QTY":
                    items = desc ? itemScope.OrderByDescending(i => i.Quantity).ToList() :
                        itemScope.OrderBy(i => i.Quantity).ToList();
                    log.Append("Name");
                    break;
                default:
                    return GetAll();
            }


            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), items.Count().ToString() + " records found.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null)
        {
            StaticLogger.LogInfo(GetType(),"Repo getting OrderItems sorted by: " + sortBy + " " + (desc ? "descending" : "ascending") + ", Page: " + pgIndex + " of Size: " + pgSize + ".");

            var itemScope = itemList == null ? _context.OrderItems : itemList;

            var items = GetAllPaginated(pgSize, pgIndex, GetAllSorted(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdAndSorted(int ordId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Order with ID: " + ordId + " and sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order.");
            var items = GetAllSorted(sortBy, desc, GetAllByOrderId(ordId));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdAndSorted(int prodId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Product with ID: " + prodId + " and sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order.");
            var items = GetAllSorted(sortBy, desc, GetAllByProductId(prodId));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByOrderIdSortedAndPaged(int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Order with ID: " + ordId + ", sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order, and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");
            var items = GetAllPaginated(pgSize, pgIndex, GetAllByOrderIdAndSorted(ordId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public IEnumerable<OrderItem> GetAllByProductIdSortedAndPaged(int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Product with ID: " + prodId + ", sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order, and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");
            var items = GetAllPaginated(pgSize, pgIndex, GetAllByProductIdAndSorted(prodId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        #endregion

        public bool ProductIdExists(int prodId) => _context.Products.Any(p => p.ProductId == prodId);

        public bool OrderIdExists(int ordId) => _context.Orders.Any(p => p.OrderId == ordId);

        #region Async

        public async Task<OrderItem> GetAsync(int ordId, int prodId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching OrderItem for Order ID:\t" + ordId + " of Product ID:\t" + prodId);
            var item = await _context.OrderItems.FindAsync(ordId, prodId);
            if (item == null) StaticLogger.LogWarn(GetType(), "Repo found no OrderItem with Order/Product IDs:\t" + ordId + "/" + prodId);
            return item;
        }

        public async Task CreateAsync(OrderItem item)
        {
            StaticLogger.LogInfo(GetType(), "Repo creating record for OrderItem for Order:\t" + item.OrderId + " for Product:\t" + item.ProductId);

            //Check that all the OrderItems data is Valid
            if ((await _context.Orders.FindAsync(item.OrderId)) == null ||
                ( await _context.Products.FindAsync(item.ProductId)) == null ||
                item.Quantity < 1)
            {
                StaticLogger.LogError(GetType(), "Repo could not register OrderItem because either the Order or Product ID was non-existent or the requested quantity was 0.\nOrder ID:\t" + item.OrderId + " Product ID:\t" + item.ProductId);
                return;
            }

            if ((await _context.OrderItems.FindAsync(item.OrderId, item.ProductId)) != null)
            {
                StaticLogger.LogError(GetType(), "Repo failed to add item as Orderd ID:" + item.OrderId + " already contains an entry for Product ID:\t" + item.ProductId + "\n If you wish to edit the quantity, please Update the OrderItem rather than re-adding it.");
                return;
            }

            _context.Set<OrderItem>().Add(item);
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "OrderItem succesfully registered.");
        }

        public async Task UpdateAsync(OrderItem item)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for OrderItem with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId);
            var itemToUpd = await GetAsync(item.OrderId, item.ProductId);

            if (item.Quantity < 1)
            {
                StaticLogger.LogError(GetType(), "Repo failed to update OrderItem because requested quantity was 0.");
                return;
            }

            itemToUpd.Quantity = item.Quantity;
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated OrderItem with Order/Product IDs:\t" + item.OrderId + "/" + item.ProductId);
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItems...");
            var items = await _context.OrderItems.ToListAsync();
            StaticLogger.LogInfo(GetType(), items.Count().ToString() + " records found.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(int ordId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Order ID:\t" + ordId);
            var items = await _context.OrderItems.Where(i => i.OrderId == ordId).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Order ID:\t" + ordId);
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAsync(int prodId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Product ID:\t" + prodId);
            var items = await _context.OrderItems.Where(i => i.ProductId == prodId).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Product ID:\t" + prodId);
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAndPagedAsync
            (int ordId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n ORDER ID: " +
                ordId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            var items = await _context.OrderItems.Where(i => i.OrderId == ordId)
                .Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAndPagedAsync
            (int prodId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n PRODUCT ID: " +
                prodId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            var items = await _context.OrderItems.Where(i => i.ProductId == prodId)
                .Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllPaginatedAsync
            (int pgSize, int pgIndex, IEnumerable<OrderItem> itemList=null)
        {
            if (pgIndex == 0) return await GetAllAsync();
            pgSize = Math.Abs(pgSize);
            pgIndex = Math.Abs(pgIndex);
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from " + (itemList == null ? "all" : "selected") + " OrderItem records.");

            var items = new List<OrderItem>();

            items = itemList == null ? await _context.OrderItems.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToListAsync() :
                 itemList.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderAsync(int ordId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Order ID:\t" + ordId);
            var items = await _context.OrderItems.Where(i => i.OrderId == ordId).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Order ID:\t" + ordId);
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductAsync(int prodId)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all OrderItem for Product ID:\t" + prodId);
            var items = await _context.OrderItems.Where(i => i.ProductId == prodId).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " OrderItems in Product ID:\t" + prodId);
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderAndPagedAsync(int ordId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n ORDER ID: " + ordId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var items = await GetAllByOrderAsync(ordId);
            if (items.Count() == 0) return items;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            items = items.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductAndPagedAsync(int prodId, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging OrderItems, according to \n PRODUCT ID: " 
                + prodId + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered OrderItem records.");
            var items = await GetAllPaginatedAsync(pgSize, pgIndex,
                    await GetAllByProductAsync(prodId));
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + items.Count().ToString() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllSortedAsync
            (string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null)
        {
            var itemScope = itemList == null ? await GetAllAsync() : itemList;

            var log = new StringBuilder("Repo fetching ");
            if (itemList == null) log.Append("all");
            log.Append(" OrderItems");
            if (itemList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return itemScope;

            var items = GetAllSorted(sortBy, desc, itemScope);

            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), items.Count().ToString() + " records found.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false, IEnumerable<OrderItem> itemList = null)
        {
            StaticLogger.LogInfo(GetType(), "Repo getting OrderItems sorted by: " + sortBy +
                " " + (desc ? "descending" : "ascending") + ", Page: " + 
                pgIndex + " of Size: " + pgSize + ".");

            var itemScope = itemList == null ? await GetAllAsync() : itemList;

            var items = await GetAllPaginatedAsync(pgSize, pgIndex,
                        await GetAllSortedAsync(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAndSortedAsync
            (int ordId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Order with ID: "
                + ordId + " and sorting them by: " + sortBy + " in " +
                (desc ? "descending" : "ascending") + " order.");
            var items = await GetAllSortedAsync(sortBy, desc,
                        await GetAllByOrderIdAsync(ordId));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdAndSortedAsync
            (int prodId, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Product with ID: " + 
                prodId + " and sorting them by: " + sortBy + " in " +
                (desc ? "descending" : "ascending") + " order.");
            var items = await GetAllSortedAsync(sortBy, desc, 
                        await GetAllByProductIdAsync(prodId));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByOrderIdSortedAndPagedAsync
            (int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Order with ID: " +
                ordId + ", sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                " order, and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");
            var items = await GetAllPaginatedAsync(pgSize, pgIndex,
                        await GetAllByOrderIdAndSortedAsync(ordId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public async Task<IEnumerable<OrderItem>> GetAllByProductIdSortedAndPagedAsync
            (int prodId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order Items for Product with ID: " +
                prodId + ", sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                " order, and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");
            var items = await GetAllPaginatedAsync(pgSize, pgIndex, 
                        await GetAllByProductIdAndSortedAsync(prodId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + items.Count() + " records.");
            return items;
        }

        public async Task<bool> ProductIdExistsAsync(int prodId) => await _context.Products.FindAsync(prodId) != null;

        public async Task<bool> OrderIdExistsAsync(int ordId) => await _context.Orders.FindAsync(ordId) != null;

        #endregion
    }
}
