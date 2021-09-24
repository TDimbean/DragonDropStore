using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.UnitTests.MockRepoes
{
    public class TestOrderRepo : IOrderRepository
    {
        private TestDb _context;

        public TestOrderRepo(TestDb context) => _context = context;

        public Order Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order with ID:\t" + id);
            var ord = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (ord == null) StaticLogger.LogWarn(GetType(), "Repo found no Order with ID:\t" + id);
            return ord;
        }

        public void Create(Order ord)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo creating record for Order placed by:\t"
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == ord.CustomerId).Name
                    + " at:/t" + ord.OrderDate);
            }

            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Order because Customer was null. Details: " + ex.Message
                    + "\n Stack Trace:\t" + ex.StackTrace);
            }

            if(ord.CustomerId<0||ord.OrderDate==null||ord.PaymentMethodId==0||ord.ShippingMethodId==0)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Order because the data submitted was incomplete.");
                return;
            }

            _context.Orders.Add(ord);
            StaticLogger.LogInfo(GetType(), "Order succesfully registered at: " + ord.OrderDate + ".");
        }

        public void Update(Order ord)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Order with ID:\t" + ord.OrderId);
            var ordToUpd = Get(ord.OrderId);

            if (ordToUpd==null)
            {
                StaticLogger.LogError(GetType(), "Repo failed to update record. No Order with ID:\t" + ord.OrderId + " was found.");
                return;
            }

            ordToUpd.CustomerId = ord.CustomerId;
            ordToUpd.OrderDate = ord.OrderDate;
            ordToUpd.OrderStatusId = ord.OrderStatusId;
            ordToUpd.PaymentMethodId = ord.PaymentMethodId;
            ordToUpd.ShippingMethodId = ord.ShippingMethodId;
            ordToUpd.ShippingDate = ord.ShippingDate;
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Order with ID:\t" + ord.OrderId);
        }

        #region Gets
        
        public IEnumerable<Order> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Orders...");
            var ords = _context.Orders.ToList();
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public IEnumerable<Order> GetAllUnprocessed()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Unprocessed Orders...");
            var ords = _context.Orders.Where(o=>o.OrderStatusId==0).OrderBy(o => o.OrderDate).ToList();
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public IEnumerable<Order> GetAllProcessed()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Processed Orders...");
            var ords = _context.Orders.Where(o => o.OrderStatusId == 1).OrderBy(o => o.OrderDate).ToList();
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public IEnumerable<Order> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Order> ordList = null)
        {
            var pageScope = ordList == null ? "all" : "filtered";
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from " + pageScope + " Order records.");

            var ords = ordList == null ? _context.Orders.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList() :
                                                 ordList.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + ords.Count().ToString() + " records.");
            return ords;
        }

        public IEnumerable<Order> GetAllFiltered(string searchBy, IEnumerable<Order> ordList = null)
        {
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Order records by:\t" + searchBy);
            var ords = new List<Order>();

            var ordScope = ordList == null ? _context.Orders : ordList;

            var searchTerms = searchBy.Split(' ');
            var date = new DateTime(1600, 1, 1);
            foreach (var term in searchTerms)
            {
                ords.AddRange
                    (
                        ordScope.Where(o =>
                            _context.Customers.Any(c => c.Name.ToUpper()
                            .Contains(term) && c.CustomerId == o.CustomerId) ||
                            _context.ShippingMethods.Any(s => s.Name.ToUpper()
                            .Contains(term) && s.ShippingMethodId == o.ShippingMethodId) ||
                            _context.PaymentMethods.Any(p => p.Name.ToUpper()
                            .Contains(term) && p.PaymentMethodId == o.PaymentMethodId) ||
                            _context.OrderStatuses.Any(os => os.Name.ToUpper()
                            .Contains(term) && os.OrderStatusId == o.OrderStatusId))
                    );
                if (DateTime.TryParse(term.Trim(), out date)) ords.AddRange(ordScope
                    .Where(o => o.OrderDate == date || o.ShippingDate == date));
            }

            ords = ords.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " Orders that match: " + searchBy);
            return ords.OrderBy(o => o.OrderId);

        }

        public IEnumerable<Order> GetAllSorted(string sortBy, bool desc = false, IEnumerable<Order> ordList = null)
        {
            var ords = new List<Order>();
            var ordScope = ordList == null ? _context.Orders : ordList;

            var log = new StringBuilder("Repo fetching ");
            if (ordList == null) log.Append("all");
            log.Append(" Orders");
            if (ordList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }


            if (sortBy == null) return ordScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "CUST":
                case "CUSTID":
                case "CUSTOMER":
                case "CUSTOMERID":
                    ords = desc ? ordScope.OrderByDescending(c => c.CustomerId).ToList() :
                        ordScope.OrderBy(c => c.CustomerId).ToList();
                    log.Append("Customer");
                    break;
                case "SHIP":
                case "SHIPPING":
                case "SHIPPINGMETHOD":
                case "SHIPMETHOD":
                case "DELIVERY":
                case "DELIVERYMETHOD":
                case "SHIPMETH":
                case "SHIP_METH":
                case "SHIPPING_METHOD":
                case "SHIP_METHOD":
                    ords = desc ? ordScope.OrderByDescending(c => c.ShippingMethodId).ToList() :
                        ordScope.OrderBy(c => c.ShippingMethodId).ToList();
                    log.Append("Shipping Method");
                    break;
                case "PAY":
                case "PAYMENT":
                case "PAYMETHOD":
                case "PAYMENTMETHOD":
                    ords = desc ? ordScope.OrderByDescending(c => c.PaymentMethodId).ToList() :
                        ordScope.OrderBy(c => c.PaymentMethodId).ToList();
                    log.Append("Payment Method");
                    break;
                case "ORDERDATE":
                case "ORDDATE":
                case "ORDER_DATE":
                case "ORD_DATE":
                case "PLACED":
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderDate).ToList() :
                        ordScope.OrderBy(c => c.OrderDate).ToList();
                    log.Append("Order Date");
                    break;
                case "SHIPPINGDATE":
                case "SHIPPING_DATE":
                case "SHIPDATE":
                case "SHIP_DATE":
                case "SHIPPED":
                    ords = desc ? ordScope.OrderByDescending(c => c.ShippingDate).ToList() :
                        ordScope.OrderBy(c => c.ShippingDate).ToList();
                    log.Append("Shipping Date");
                    break;
                case "STATUS":
                case "STAT":
                case "ORDERSTATUS":
                case "ORDSTATUS":
                case "ORDER_STATUS":
                case "ORD_STATUS":
                case "ORDSTAT":
                case "ORD_STAT":
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderStatusId).ToList() :
                        ordScope.OrderBy(c => c.OrderStatusId).ToList();
                    log.Append("Status");
                    break;
                default:
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderId).ToList() :
                        ordScope.OrderBy(c => c.OrderId).ToList();
                    log.Append("ID");
                    break;
            }


            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public IEnumerable<Order> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var descTxt = desc ? " descending" : " ascending";
            StaticLogger.LogInfo(GetType(), "Repo Sorting Orders by " + sortBy + descTxt + ", then retrieving page " + pgIndex + " of size " + pgSize + ".");
            var ords = GetAllPaginated(pgSize, pgIndex, GetAllSorted(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo got " + ords.Count() + " results.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Orders, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var ords = GetAllFiltered(searchBy);
            if (ords.Count() == 0) return ords;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Order records.");
            ords = ords.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + ords.Count().ToString() + " records.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering results by: " + searchBy + ", then sorting the result by: " + sortBy + ".");
            var ords = GetAllSorted(sortBy, desc, GetAllFiltered(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " results.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerId(int id)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retrieving all orders placed by Customer:\t" + id + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == id).Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + id + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = _context.Orders.Where(o => o.CustomerId == id).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " orders placed by Customer with ID: " + id);

            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndPaged(int custId, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " from Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllByCustomerId(custId).Skip(pgSize * (pgIndex - 1)).Take(pgSize);
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " Orders on requested page.");

            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndSorted(int custId, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retrieving Orders placed by: " + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " then sorting them by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order.");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllSorted(sortBy, desc, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " records and sorted them.");
            return ords;
        }

        public IEnumerable<Order> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order records filtered by: " + searchBy + ", sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order. Page: " + pgIndex + " of size: " + pgSize + ".");
            var ords = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved and sorted " + ords.Count() + " records.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdSortedAndPaged(int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo fetching all Orders by: " + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " to sort by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order. Looking at Page: " + pgIndex + " of Size: " + pgSize + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdAndSorted(custId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved and sorted " + ords.Count() + " records.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdAndFiltered(int custId, string searchBy)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + " and filtering them by: " + searchBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllFiltered(searchBy, GetAllByCustomerId(custId));
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " Orders that match criteria.");

            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredAndPaged(int custId, string searchBy, int pgSize, int pgIndex)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", filtering them by: " + searchBy + " then displaying Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdAndFiltered(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredAndSorted(int custId, string searchBy, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", filtering them by: " + searchBy + " then sorting them by: " + sortBy + " " + (desc ? "descending" : "ascending") + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllSorted(sortBy, desc, GetAllByCustomerIdAndFiltered(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        public IEnumerable<Order> GetAllByCustomerIdFilteredSortedAndPaged(int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + _context.Customers.SingleOrDefault(c => c.CustomerId == custId).Name + ", filtering them by: " + searchBy + ", sorting by: " + sortBy + " " + (desc ? "descending" : "ascending") + " then displaying Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = GetAllPaginated(pgSize, pgIndex, GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        #endregion

        #region IdVerifiers

        public bool CustomerIdExists(int custId) => _context.Customers.Any(c => c.CustomerId == custId);

        public bool ShippingMethodIdExists(int shipId) => _context.ShippingMethods.Any(s => s.ShippingMethodId == shipId);

        public bool PaymentMethodIdExists(int payId) => _context.PaymentMethods.Any(p => p.PaymentMethodId == payId);

        public bool OrderStatusIdExists(int statId) => _context.OrderStatuses.Any(os => os.OrderStatusId == statId);

        public bool OrderIdExists(int ordId) => _context.Orders.Any(o => o.OrderId == ordId);

        #endregion

        #region StatusAdvancements

        public void PromoteReceived(int ordId)
        {
            var ord = _context.Orders.SingleOrDefault(o => o.OrderId == ordId);
            if (ord == null) return;
            if (ord.OrderStatusId == 0) ord.OrderStatusId = 1;
        }

        public void PromoteProcessed(int ordId, DateTime shippingDate)
        {
            StaticLogger.LogInfo(GetType(),"Repo promoting Order with ID: " + ordId + " from Received to Processed.");
            var ord = _context.Orders.SingleOrDefault(o => o.OrderId == ordId);
            if (ord == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not find any Order with ID: " + ordId + " to promote.");
                return;
            }
            if (ord.OrderStatusId == 1 && ord.ShippingDate == null && shippingDate >= ord.OrderDate)
            {
                ord.OrderStatusId = 2;
                ord.ShippingDate = shippingDate;
            }
            else
                StaticLogger.LogError(GetType(), "Repo could not promote Order with ID: " + ordId + ";"+
                    "Either the Order Status is different than \"Received\" or the Shipping Date has already been set;"+
                    "Please modify manually.");
        }

        public void BatchPromoteReceived(List<int> ordIds)
        {
            var ords = _context.Orders.Where(o => ordIds.Contains(o.OrderId));
            foreach (var ord in ords) if (ord.OrderStatusId == 0) ord.OrderStatusId = 1;
        }

        #endregion

        #region Async

        public async Task CreateAsync(Order ord)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo creating record for Order placed by:\t"
                    //+ (await _context.Customers.FindAsync(ord.CustomerId)).Name
                    + (_context.Customers.Find(o=>o.CustomerId==ord.CustomerId)).Name
                    + " at:/t" + ord.OrderDate);
            }

            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Order because Customer was null. Details: " + ex.Message
                    + "\n Stack Trace:\t" + ex.StackTrace);
            }

            if (ord.CustomerId < 0 || ord.OrderDate == null || ord.PaymentMethodId == 0 || ord.ShippingMethodId == 0)
            {
                StaticLogger.LogError(GetType(), "Repo could not register Order because the data submitted was incomplete.");
                return;
            }

            _context.Orders.Add(ord);
            StaticLogger.LogInfo(GetType(), "Order succesfully registered at: " + ord.OrderDate + ".");
        }

        public async Task UpdateAsync(Order ord)
        {
            StaticLogger.LogInfo(GetType(), "Repo updating record for Order with ID:\t" + ord.OrderId);
            var ordToUpd = await GetAsync(ord.OrderId);

            if (ordToUpd == null)
            {
                StaticLogger.LogError(GetType(), "Repo failed to update record. No Order with ID:\t" + ord.OrderId + " was found.");
                return;
            }

            ordToUpd.CustomerId = ord.CustomerId;
            ordToUpd.OrderDate = ord.OrderDate;
            ordToUpd.OrderStatusId = ord.OrderStatusId;
            ordToUpd.PaymentMethodId = ord.PaymentMethodId;
            ordToUpd.ShippingMethodId = ord.ShippingMethodId;
            ordToUpd.ShippingDate = ord.ShippingDate;
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Order with ID:\t" + ord.OrderId);
        }

        #region Gets

        public async Task<Order> GetAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order with ID:\t" + id);
            //var ord = await _context.Orders.FindAsync(id);
            var ord = _context.Orders.Find(o=>o.OrderId==id);
            if (ord == null) StaticLogger.LogWarn(GetType(), "Repo found no Order with ID:\t" + id);
            return ord;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Orders...");
            //var ords = await _context.Orders.ToListAsync();
            var ords = _context.Orders;
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllUnprocessedAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Unprocessed Orders...");
            //var ords = await _context.Orders.Where(o=>o.OrderStatus==0).OrderBy(o=>o.OrderDate).ToListAsync();
            var ords = _context.Orders.Where(o => o.OrderStatusId == 0).OrderBy(o => o.OrderDate);
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllProcessedAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Processed Orders...");
            //var ords = await _context.Orders.Where(o=>o.OrderStatuId==1).OrderBy(o=>o.OrderDate).ToListAsync();
            var ords = _context.Orders.Where(o => o.OrderStatusId == 1).OrderBy(o => o.OrderDate);
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllPaginatedAsync(int pgSize, int pgIndex, IEnumerable<Order> ordList = null)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from all Order records.");
            if (ordList == null) ordList = await GetAllAsync();

            var ords = GetAllPaginated(pgSize, pgIndex, ordList);

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + ords.Count().ToString() + " records.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAsync(string searchBy, IEnumerable<Order> ordList = null)
        {
            return GetAllFiltered(searchBy, ordList);
            /*
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Order records by:\t" + searchBy);
            var ords = new List<Order>();

            var searchTerms = searchBy.Split(' ');
            var date = new DateTime(1600, 1, 1);
            foreach (var term in searchTerms)
            {
                var fetch = await
                        _context.Orders.Where(o =>
                            _context.Customers.Any(c => c.Name.ToUpper()
                            .Contains(term) && c.CustomerId == o.CustomerId) ||
                            _context.ShippingMethods.Any(s => s.Name.ToUpper()
                            .Contains(term) && s.ShippingMethodId == o.ShippingMethodId) ||
                            _context.PaymentMethods.Any(p => p.Name.ToUpper()
                            .Contains(term) && p.PaymentMethodId == o.PaymentMethodId) ||
                            _context.OrderStatuses.Any(os => os.Name.ToUpper()
                            .Contains(term) && os.OrderStatusId == o.OrderStatusId))
                            .ToListAsync();

                ords.AddRange(fetch);

                if (DateTime.TryParse(term.Trim(), out date)) ords.AddRange(await _context.Orders
                    .Where(o => o.OrderDate == date || o.ShippingDate == date).ToListAsync());
            }

            ords = ords.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " Orders that match: " + searchBy);
            return ords;
            */
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Orders, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var ords = await GetAllFilteredAsync(searchBy);
            if (ords.Count() == 0) return ords;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Order records.");
            ords = ords.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + ords.Count().ToString() + " records.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAsync(int id)
        {
            return GetAllByCustomerId(id);
            /*
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retrieving all orders placed by Customer:\t" + id + "\tNAME: "
                    + (await _context.Customers.FindAsync(id)).Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + id + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            //var ords = await _context.Orders.Where(o => o.CustomerId == id).ToListAsync();
            var ords =  _context.Orders.Where(o => o.CustomerId == id).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " orders placed by Customer with ID: " + id);

            return ords;
            */
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndPagedAsync(int custId, int pgSize, int pgIndex)
        {
            return GetAllByCustomerIdAndPaged(custId, pgSize, pgIndex);
            /*
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo retrieving Orders placed by Customer:\t" + custId + "\tNAME: "
                    + (await _context.Customers.FindAsync(custId)).Name + " from Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = (await GetAllByCustomerIdAsync(custId)).Skip(pgSize * (pgIndex - 1)).Take(pgSize);
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " Orders on requested page.");

            return ords;*/
        }

        public async Task<IEnumerable<Order>> GetAllSortedAsync(string sortBy, bool desc = false, IEnumerable<Order> ordList = null)
        {
            var ords = new List<Order>();
            var ordScope = ordList == null ? await GetAllAsync() : ordList;

            var log = new StringBuilder("Repo fetching ");
            if (ordList == null) log.Append("all");
            log.Append(" Orders");
            if (ordList == null) log.Append(".");
            else
            {
                log.Append(", sorted by: " + sortBy);
                var descTxt = desc ? " descending." : " ascending.";
                log.Append(descTxt);
            }

            if (sortBy == null) return ordScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "CUST":
                case "CUSTOMER":
                case "CUSTOMERID":
                case "CUSTID":
                case "CUSTOMER_ID":
                case "CUST_ID":
                    ords = desc ? ordScope.OrderByDescending(c => c.CustomerId).ToList() :
                        ordScope.OrderBy(c => c.CustomerId).ToList();
                    log.Append("Customer");
                    break;
                case "SHIP":
                case "SHIPPING":
                case "SHIPPINGMETHOD":
                case "SHIPMETHOD":
                case "DELIVERY":
                case "DELIVERYMETHOD":
                case "SHIPMETH":
                case "SHIP_METH":
                case "SHIPPING_METHOD":
                case "SHIP_METHOD":
                    ords = desc ? ordScope.OrderByDescending(c => c.ShippingMethodId).ToList() :
                        ordScope.OrderBy(c => c.ShippingMethodId).ToList();
                    log.Append("Shipping Method");
                    break;
                case "PAY":
                case "PAYMENT":
                case "PAYMETHOD":
                case "PAYMENTMETHOD":
                case "PAYMETH":
                case "PAY_METHOD":
                case "PAYMENT_METHOD":
                case "PAY_METH":
                    ords = desc ? ordScope.OrderByDescending(c => c.PaymentMethodId).ToList() :
                        ordScope.OrderBy(c => c.PaymentMethodId).ToList();
                    log.Append("Payment Method");
                    break;
                case "ORDERDATE":
                case "ORDDATE":
                case "ORDER_DATE":
                case "ORD_DATE":
                case "PLACED":
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderDate).ToList() :
                        ordScope.OrderBy(c => c.OrderDate).ToList();
                    log.Append("Order Date");
                    break;
                case "SHIPPINGDATE":
                case "SHIPPING_DATE":
                case "SHIPDATE":
                case "SHIP_DATE":
                case "SHIPPED":
                    ords = desc ? ordScope.OrderByDescending(c => c.ShippingDate).ToList() :
                        ordScope.OrderBy(c => c.ShippingDate).ToList();
                    log.Append("Shipping Date");
                    break;
                case "STATUS":
                case "STAT":
                case "ORDERSTATUS":
                case "ORDSTATUS":
                case "ORDER_STATUS":
                case "ORD_STATUS":
                case "ORDSTAT":
                case "ORD_STAT":
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderStatusId).ToList() :
                        ordScope.OrderBy(c => c.OrderStatusId).ToList();
                    log.Append("Status");
                    break;
                default:
                    ords = desc ? ordScope.OrderByDescending(c => c.OrderId).ToList() :
                       ordScope.OrderBy(c => c.OrderId).ToList();
                    log.Append("ID");
                    break;
            }


            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), ords.Count().ToString() + " records found.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllSortedAndPagedAsync(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var descTxt = desc ? " descending" : " ascending";
            StaticLogger.LogInfo(GetType(), "Repo Sorting Orders by " + sortBy + descTxt + ", then retrieving page " + pgIndex + " of size " + pgSize + ".");
            var ords = await GetAllPaginatedAsync(pgSize, pgIndex,
                await GetAllSortedAsync(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo got " + ords.Count() + " results.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredAndSortedAsync(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering results by: " + searchBy + ", then sorting the result by: " + sortBy + ".");
            var ords = await GetAllSortedAsync(sortBy, desc,
                       await GetAllFilteredAsync(searchBy));
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " results.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Order records filtered by: " + searchBy + ", sorted by: " + sortBy + " in " + (desc ? "descending" : "ascending") + " order. Page: " + pgIndex + " of size: " + pgSize + ".");
            var ords = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllFilteredAndSortedAsync(searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved and sorted " + ords.Count() + " records.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndSortedAsync
            (int custId, string sortBy, bool desc = false)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name =  _context.Customers.SingleOrDefault(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo retrieving Orders placed by: " +
                    name.Name + " then sorting them by: " + sortBy + " in " +
                    (desc ? "descending" : "ascending") + " order.");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllSortedAsync(sortBy, desc,
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo found " + ords.Count() + " records and sorted them.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdSortedAndPagedAsync
            (int custId, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo fetching all Orders by: " + name.Name +
                    " to sort by: " + sortBy + " in " + (desc ? "descending" : "ascending") +
                    " order. Looking at Page: " + pgIndex + " of Size: " + pgSize + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllByCustomerIdAndSortedAsync(custId, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved and sorted " + ords.Count() + " records.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdAndFilteredAsync(int custId, string searchBy)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" +
                    custId + "\tNAME: " + name.Name + " and filtering them by: " + searchBy);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllFilteredAsync(searchBy,
                       await GetAllByCustomerIdAsync(custId));
            StaticLogger.LogInfo(GetType(), "Repo found: " + ords.Count() + " Orders that match criteria.");

            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);
                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" +
                    custId + "\tNAME: " + name.Name + ", filtering them by: " + searchBy +
                    " then displaying Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllByCustomerIdAndFilteredAsync(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredAndSortedAsync
            (int custId, string searchBy, string sortBy, bool desc = false)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);

                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" +
                    custId + "\tNAME: " + name.Name + ", filtering them by: " + searchBy +
                    " then sorting them by: " + sortBy + " " +
                    (desc ? "descending" : "ascending") + ".");
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllSortedAsync(sortBy, desc,
                       await GetAllByCustomerIdAndFilteredAsync(custId, searchBy));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        public async Task<IEnumerable<Order>> GetAllByCustomerIdFilteredSortedAndPagedAsync
            (int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            try
            {
                //var name = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerId == custId);
                var name = _context.Customers.SingleOrDefault(c => c.CustomerId == custId);

                StaticLogger.LogInfo(GetType(), "Repo retreiving Orders placed by Customer:\t" +
                    custId + "\tNAME: " + name.Name + ", filtering them by: " + searchBy +
                    ", sorting by: " + sortBy + " " + (desc ? "descending" : "ascending") +
                    " then displaying Page: " + pgIndex + " of Size: " + pgSize);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo found no Customer with ID:\t" + custId + " and hence no orders by said Customer."
                    + "\n Details: " + ex.Message + "\n Stack Trace:\t" + ex.StackTrace);
                return null;
            }

            var ords = await GetAllPaginatedAsync(pgSize, pgIndex,
                       await GetAllByCustomerIdFilteredAndSortedAsync(custId, searchBy, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + ords.Count() + " records matching critera.");
            return ords;
        }

        #endregion

        #region IdVerifiers

        public async Task<bool> CustomerIdExistsAsync(int custId)
            =>  _context.Customers.Find(c=>c.CustomerId==custId) != null;

        public async Task<bool> ShippingMethodIdExistsAsync(int shipId)
            =>  _context.ShippingMethods.Find(s=>s.ShippingMethodId==shipId) != null;
        //=>  _context.ShippingMethods.Find(shipId) != null;

        public async Task<bool> PaymentMethodIdExistsAsync(int payId)
            =>  _context.PaymentMethods.Find(p=>p.PaymentMethodId==payId) != null;
        //=>  _context.PaymentMethods.Find(payId) != null;

        public async Task<bool> OrderStatusIdExistsAsync(int statId)
            =>  _context.OrderStatuses.Find(s=>s.OrderStatusId==statId) != null;
        //=>  _context.OrderStatuses.Find(statId) != null;

        public async Task<bool> OrderIdExistsAsync(int ordId)
            =>  _context.Orders.Find(o=>o.OrderId==ordId) != null;
        //=>  _context.Orders.Find(ordId) != null;

        #endregion

        #region StatusAdvancements

        public async Task PromoteReceivedAsync(int ordId)
        {
            var ord = _context.Orders.SingleOrDefault(o => o.OrderId == ordId);
            if (ord == null) return;
            if (ord.OrderStatusId == 0) ord.OrderStatusId = 1;
        }

        public async Task PromoteProcessedAsync(int ordId, DateTime shippingDate)
        {
            var ord = _context.Orders.SingleOrDefault(o => o.OrderId == ordId);
            if (ord == null) return;
            if (ord.OrderStatusId == 1 && ord.ShippingDate == null && shippingDate >= ord.OrderDate)
            {
                ord.OrderStatusId = 2;
                ord.ShippingDate = shippingDate;
            }
        }

        public async Task BatchPromoteReceivedAsync(List<int> ordIds)
        {
            var ords = _context.Orders.Where(o => ordIds.Contains(o.OrderId)).ToList();
            foreach (var ord in ords) if (ord.OrderStatusId == 0) ord.OrderStatusId = 1;
        }

        #endregion

        #endregion
    }
}
