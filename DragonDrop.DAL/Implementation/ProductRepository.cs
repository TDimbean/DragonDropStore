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
    public class ProductRepository : IProductRepository
    {
        private DragonDrop_DbContext _context;

        public ProductRepository(DragonDrop_DbContext context) => _context = context;

        public void Create(Product prod)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo creating record for Product named:\t" + prod.Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo cannot register Product as it is missing a Name.\n Details: " + ex.Message + "\nStackTrace:\t" + ex.StackTrace);
            }

            if (prod.CategoryId == null || prod.BarCode == null || prod.Name == null || prod.UnitPrice == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not create record. The Product data is incomplete.");
                return;
            }

            if (!IsUniqueBarCode(prod.BarCode))
            {
                StaticLogger.LogError(GetType(), "Repo could not create record bracuse of duplicate Bar Code:\t" + prod.BarCode.Substring(0, 1) + "-" + prod.BarCode.Substring(1, 5) + "~~" + prod.BarCode.Substring(6, 5) + "-" + prod.BarCode.Substring(11, 1));
                return;
            }
            _context.Products.Add(prod);
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "Product:/t" + prod.Name + " succesfully registered.");
        }

        public void Update(Product prod)
        {
            if (!ProductIdExists(prod.ProductId))
            {
                StaticLogger.LogError(GetType(), "Repo could not find any Product with ID:\t" + prod.ProductId + " to upate.");
                return;
            }

            try
            {
                StaticLogger.LogInfo(GetType(), "Repo updating record for Product with ID:\t" + prod.ProductId);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo cannot update Product because no Product with ID:\t" + prod.ProductId + " was found.\n Details: " + ex.Message + "\nStackTrace:\t" + ex.StackTrace);
                return;
            }

            if (prod.CategoryId == null || prod.BarCode == null || prod.Name == null || prod.UnitPrice == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not create record. The Product data is incomplete.");
                return;
            }

            if (_context.Products.Any(p => p.BarCode == prod.BarCode && p.ProductId != prod.ProductId))
            {
                StaticLogger.LogError(GetType(), "Repo could not create record bracuse of duplicate Bar Code:\t" + prod.BarCode.Substring(0, 1) + "-" + prod.BarCode.Substring(1, 5) + "~~" + prod.BarCode.Substring(6, 5) + "-" + prod.BarCode.Substring(11, 1));
                return;
            }
            var prodToUpd = Get(prod.ProductId);
            prodToUpd.Name = prod.Name;
            prodToUpd.Description = prod.Description;
            prodToUpd.BarCode = prod.BarCode;
            prodToUpd.CategoryId = prod.CategoryId;
            prodToUpd.Stock = prod.Stock;
            prodToUpd.UnitPrice = prod.UnitPrice;
            _context.SaveChanges();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Product with ID:\t" + prod.ProductId);
        }

        public void AddStock(int prodId, int qty)
        {
            if (qty == 0) return;
            StaticLogger.LogInfo(GetType(), "Repo " + (qty > 0 ? "adding" : "subtracting" + " " + Math.Abs(qty) +
                " Stock for Product with ID: " + prodId));
            var prod = _context.Products.SingleOrDefault(p => p.ProductId == prodId);
            if (prod == null)
            {
                StaticLogger.LogError(GetType(), "Repo found no Product with ID: " + prodId);
                return;
            }
            if(qty<0 && prod.Stock<Math.Abs(qty))
            {
                StaticLogger.LogError(GetType(), "Repo could not reduce the Stock for Product with ID: " +
                    prodId + "becuse it is insufficient.");
                return;
            }
            prod.Stock += qty;
            _context.SaveChanges();
        }

        #region Gets

        public Product Get(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Product with ID:\t" + id);
            var prod = _context.Products.SingleOrDefault(p => p.ProductId == id);
            if (prod == null) StaticLogger.LogWarn(GetType(), "Repo found no Product with ID:\t" + id);
            return prod;
        }

        public Product GetByBarcode(string code)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Product with Barcode:\t" + code);
            var prod = _context.Products.SingleOrDefault(p => p.BarCode == code);
            if (prod == null) StaticLogger.LogWarn(GetType(), "Repo found no Product with Barcode:\t" + code);
            return prod;
        }

        public IEnumerable<Product> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Products...");
            var prods = _context.Products.ToList();
            StaticLogger.LogInfo(GetType(), prods.Count().ToString() + " records found.");
            return prods;
        }

        public IEnumerable<Product> GetAllPaginated(int pgSize, int pgIndex, IEnumerable<Product> prodList = null)
        {
            if (pgIndex == 0) return GetAll();
            pgSize = Math.Abs(pgSize);
            pgIndex = Math.Abs(pgIndex);

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from "+ (prodList ==null?"all":"select")+" Product records.");

            var prodScope = prodList == null ? _context.Products : prodList;

            var prods = prodScope.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFiltered(string searchBy)
        {
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Product records by:\t" + searchBy);
            var prods = new List<Product>();

            var searchTerms = searchBy.Split(' ');

            var intVal = 0;
            var decVal = 0m;
            bool isInt = false;
            bool isDec = false;

            foreach (var term in searchTerms)
            {
                isInt = int.TryParse(term, out intVal);
                isDec = decimal.TryParse(term, out decVal);

                prods.AddRange
                    (
                        _context.Products.Where(p =>
                            isInt && (p.Stock == intVal || p.UnitPrice == intVal) ||
                            isDec && p.UnitPrice == decVal ||
                            p.Name.ToUpper().Contains(term) ||
                            p.Description.ToUpper().Contains(term) ||
                            p.BarCode.Contains(term) ||
                            p.BarCode.Substring(0,6).Contains(term) ||
                            _context.Categories.Any(c => c.Name.ToUpper().Contains(term) && c.CategoryId == p.CategoryId))
                    );
            }

            prods = prods.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match: " + searchBy);
            return prods.OrderBy(p => p.ProductId);
        }

        public IEnumerable<Product> GetAllFiltered(int stock, bool greater)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Repo searching Products with " + smlGrtText + " than: " + stock+ " items in stock.");
            var prods = greater ? _context.Products.Where(p => p.Stock > stock).ToList() :
                                _context.Products.Where(p => p.Stock < stock).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match criteria.");
            return prods.OrderBy(p => p.ProductId);
        }

        public IEnumerable<Product> GetAllFiltered(decimal price, bool greater)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Products " + smlGrtText + " : " + price + "$.");
            var prods = greater ? _context.Products.Where(p => p.UnitPrice > price).ToList() :
                                _context.Products.Where(p => p.UnitPrice < price).ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match criteria.");
            return prods.OrderBy(p => p.ProductId);
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Products, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = GetAllFiltered(searchBy);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(int stock, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Repo searching Products with " + smlGrtText + " than: " + stock + " items in stock.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = GetAllFiltered(stock, greater);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(decimal price, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Products " + smlGrtText + " : " + price + "$.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = GetAllFiltered(price, greater);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllSorted
            (string sortBy, bool desc = false, IEnumerable<Product> prodList = null)
        {
            var log = new StringBuilder("Repo sorting " + (prodList == null ? "all" : "select") + " Product records in " + (desc ? "descending" : "asccending") + " order by: ");

            var prodScope = prodList == null ? _context.Products : prodList;

            var prods = new List<Product>();

            if (sortBy == null) return prodScope;
            switch (sortBy.Trim().ToUpper())
            {
                case "NAME":
                case "PRODUCTNAME":
                case "PRODNAME":
                case "PRODUCT_NAME":
                case "PROD_NAME":
                    prods = desc ? prodScope.OrderByDescending(p => p.Name).ToList() :
                                   prodScope.OrderBy(p => p.Name).ToList();
                    log.Append("Name");
                    break;
                case "MANUFACTURER":
                case "MAN":
                case "MADEBY":
                case "MADE_BY":
                    prods = desc ? prodScope.OrderByDescending(p => p.Manufacturer).ToList() :
                                   prodScope.OrderBy(p => p.Manufacturer).ToList();
                    log.Append("Manufacturer");
                    break;
                case "BARCODE":
                case "BAR_CODE":
                case "CODE":
                case "UPC":
                case "UPCCODE":
                case "UPC_CODE":
                    prods = desc ? prodScope.OrderByDescending(p => p.BarCode).ToList() :
                                   prodScope.OrderBy(p => p.BarCode).ToList();
                    log.Append("Barcode");
                    break;
                case "STOCK":
                case "STK":
                    prods = desc ? prodScope.OrderByDescending(p => p.Stock).ToList() :
                                   prodScope.OrderBy(p => p.Stock).ToList();
                    log.Append("Stock");
                    break;
                case "PRICE":
                case "COST":
                case "UNITPRICE":
                case "UNIT_PRICE":
                    prods = desc ? prodScope.OrderByDescending(p => p.UnitPrice).ToList() :
                                   prodScope.OrderBy(p => p.UnitPrice).ToList();
                    log.Append("Price");
                    break;
                case "CAT":
                case "CATEGORY":
                    prods = desc ? prodScope.OrderByDescending(p => p.CategoryId).ToList() :
                                   prodScope.OrderBy(p => p.CategoryId).ToList();
                    log.Append("Category");
                    break;
                case "DESC":
                case "DESCRIPTION":
                    prods = desc ? prodScope.OrderByDescending(p => p.Description.Length).ToList() :
                                   prodScope.OrderBy(p => p.Description.Length).ToList();
                    log.Append("Description");
                    break;
                default:
                    prods = desc ? prodScope.OrderByDescending(p => p.ProductId).ToList() :
                                                       prodScope.OrderBy(p => p.ProductId).ToList();
                    log.Append("ID");
                    break;
            }

            log.Append(".");

            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), "Repo sorted: " +prods.Count() + " records.");
            return prods;
        }
          
        public IEnumerable<Product> GetAllSortedAndPaged
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo sorting all Product records in " + (desc ? "descending" : "ascending") + " order by: " + sortBy + " and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = GetAllPaginated(pgSize, pgIndex, GetAllSorted(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + prods.Count() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndSorted
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Product records by: " + searchBy + " then sorting the results in " + (desc ? "descending" : "ascending") + " order by: " + sortBy + ".");

            var prods = GetAllSorted(sortBy, desc, GetAllFiltered(searchBy));

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count() + " records.");
            return prods;
        }
    
        public IEnumerable<Product> GetAllFilteredAndSorted
            (int stock, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(),"Repo fetching records for Products with " + (greater?"over":"under") +" " +stock +" pieces available and Sorting them in "+ (desc?"descending":"ascending")+" order by "+sortBy+".");

            var prods = GetAllSorted(sortBy, desc, GetAllFiltered(stock, greater));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndSorted
            (decimal price, bool greater, string sortBy, bool desc = false)

        {
            StaticLogger.LogInfo(GetType(),"Repo fetching Products "+(greater?"over":"under")+" "+price.ToString("0:0.##")+"$ in value and Sorting them in "+(desc?"descending":"ascending")+" order by: "+sortBy+".");

            var prods = GetAllSorted(sortBy, desc, GetAllFiltered(price, greater));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Product records by: " + searchBy + ", sorting the results in " + (desc ? "descending" : "ascending") + " order by: " + sortBy + " and showing Page: "+pgIndex+" of Size: "+pgSize+".");

            var prods = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(searchBy, sortBy, desc));

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged
            (int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching records for Products with " + (greater ? "over" : "under") + " " + stock + " pieces available and Sorting them in " + (desc? "descending":"ascending") + " order by " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(stock, greater, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged
            (decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Products " + (greater ? "over" : "under") + " " + price.ToString("0:0.##") + "$ in value and Sorting them in " + (desc ? "descending" : "ascending") + " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = GetAllPaginated(pgSize, pgIndex, GetAllFilteredAndSorted(price, greater, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        #endregion

        #region IdVerifiers

        public bool IsUniqueBarCode(string barCode, int? prodId = null) 
            => !_context.Products.Any(p => p.BarCode == barCode && p.ProductId != prodId);

        public bool ProductIdExists(int prodId) => _context.Products.Any(p => p.ProductId == prodId);

        public bool CategoryIdExists(int catId) => _context.Categories.Any(c => c.CategoryId == catId);

        #endregion

        #region Async

        public async Task CreateAsync(Product prod)
        {
            try
            {
                StaticLogger.LogInfo(GetType(), "Repo creating record for Product named:\t" + prod.Name);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo cannot register Product as it is missing a Name.\n Details: " + ex.Message + "\nStackTrace:\t" + ex.StackTrace);
            }

            if (prod.CategoryId == null || prod.BarCode == null || prod.Name == null || prod.UnitPrice == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not create record. The Product data is incomplete.");
                return;
            }

            if (!await IsUniqueBarCodeAsync(prod.BarCode))
            {
                StaticLogger.LogError(GetType(), "Repo could not create record bracuse of duplicate Bar Code:\t" + prod.BarCode.Substring(0, 1) + "-" + prod.BarCode.Substring(1, 5) + "~~" + prod.BarCode.Substring(6, 5) + "-" + prod.BarCode.Substring(11, 1));
                return;
            }
            _context.Products.Add(prod);
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "Product:/t" + prod.Name + " succesfully registered.");
        }

        public async Task UpdateAsync(Product prod)
        {
            if (!ProductIdExists(prod.ProductId))
            {
                StaticLogger.LogError(GetType(), "Repo could not find any Product with ID:\t" + prod.ProductId + " to upate.");
                return;
            }

            try
            {
                StaticLogger.LogInfo(GetType(), "Repo updating record for Product with ID:\t" + prod.ProductId);
            }
            catch (NullReferenceException ex)
            {
                StaticLogger.LogError(GetType(), "Repo cannot update Product because no Product with ID:\t" + prod.ProductId + " was found.\n Details: " + ex.Message + "\nStackTrace:\t" + ex.StackTrace);
                return;
            }

            if (prod.CategoryId == null || prod.BarCode == null || prod.Name == null || prod.UnitPrice == null)
            {
                StaticLogger.LogError(GetType(), "Repo could not create record. The Product data is incomplete.");
                return;
            }

            if ((await _context.Products.Where(p => p.BarCode == prod.BarCode && p.ProductId != prod.ProductId).ToListAsync()).Count() > 0)
            {
                StaticLogger.LogError(GetType(), "Repo could not create record because of duplicate Bar Code:\t" + prod.BarCode.Substring(0, 1) + "-" + prod.BarCode.Substring(1, 5) + "~~" + prod.BarCode.Substring(6, 5) + "-" + prod.BarCode.Substring(11, 1));
                return;
            }
            var prodToUpd = await GetAsync(prod.ProductId);
            prodToUpd.Name = prod.Name;
            prodToUpd.Description = prod.Description;
            prodToUpd.BarCode = prod.BarCode;
            prodToUpd.CategoryId = prod.CategoryId;
            prodToUpd.Stock = prod.Stock;
            prodToUpd.UnitPrice = prod.UnitPrice;
            await _context.SaveChangesAsync();
            StaticLogger.LogInfo(GetType(), "Repo succesfully updated Product with ID:\t" + prod.ProductId);
        }

        public async Task AddStockAsync(int prodId, int qty)
        {
            if (qty == 0) return;
            StaticLogger.LogInfo(GetType(), "Repo " + (qty > 0 ? "adding" : "subtracting" + " " + Math.Abs(qty) +
                " Stock for Product with ID: " + prodId));
            var prod = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == prodId);
            if (prod == null)
            {
                StaticLogger.LogError(GetType(), "Repo found no Product with ID: " + prodId);
                return;
            }
            if (qty < 0 && prod.Stock < Math.Abs(qty))
            {
                StaticLogger.LogError(GetType(), "Repo could not reduce the Stock for Product with ID: " +
                    prodId + "becuse it is insufficient.");
                return;
            }
            prod.Stock += qty;
            await _context.SaveChangesAsync();
        }

        #region Gets

        public async Task<Product> GetByBarcodeAsync(string code)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Product with Barcode:\t" + code);
            var prod = await _context.Products.SingleOrDefaultAsync(p => p.BarCode == code);
            if (prod == null) StaticLogger.LogWarn(GetType(), "Repo found no Product with Barcode:\t" + code);
            return prod;
        }

        public async Task<Product> GetAsync(int id)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Product with ID:\t" + id);
            var prod = await _context.Products.FindAsync(id);
            if (prod == null) StaticLogger.LogWarn(GetType(), "Repo found no Product with ID:\t" + id);
            return prod;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching all Products...");
            var prods = await _context.Products.ToListAsync();
            StaticLogger.LogInfo(GetType(), prods.Count().ToString() + " records found.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllPaginatedAsync
            (int pgSize, int pgIndex, IEnumerable<Product> prodList=null)
        {
            if (pgIndex == 0) return prodList;
            pgSize = Math.Abs(pgSize);
            pgIndex = Math.Abs(pgIndex);

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + 
                pgSize + " from " + (prodList == null ? "all" : "select") + " Product records.");

            var prodScope = prodList == null ? await GetAllAsync() : prodList;

            var prods = prodScope.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(string searchBy)
        {
            searchBy = searchBy.ToUpper();
            StaticLogger.LogInfo(GetType(), "Repo searching Product records by:\t" + searchBy);
            var prods = new List<Product>();

            var searchTerms = searchBy.Split(' ');

            var intVal = 0;
            var decVal = 0m;
            bool isInt = false;
            bool isDec = false;

            foreach (var term in searchTerms)
            {
                isInt = int.TryParse(term, out intVal);
                isDec = decimal.TryParse(term, out decVal);

                prods.AddRange
                    (
                        await _context.Products.Where(p =>
                            isInt && (p.Stock == intVal || p.UnitPrice == intVal) ||
                            isDec && p.UnitPrice == decVal ||
                            p.Name.ToUpper().Contains(term) ||
                            p.Description.ToUpper().Contains(term) ||
                            p.BarCode.Contains(term) ||
                            _context.Categories.Any(c => c.Name.ToUpper().Contains(term) && c.CategoryId == p.CategoryId))
                            .ToListAsync()
                    );
            }

            prods = prods.Distinct().ToList();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match: " + searchBy);
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(int stock, bool greater)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Repo searching Products with " + smlGrtText + " than: " + stock + " items in stock.");
            var prods = greater ? await _context.Products.Where(p => p.Stock > stock).ToListAsync() :
                                await _context.Products.Where(p => p.Stock < stock).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match criteria.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(decimal price, bool greater)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Products " + smlGrtText + " : " + price + "$.");
            var prods = greater ? await _context.Products.Where(p => p.UnitPrice > price).ToListAsync() :
                                await _context.Products.Where(p => p.UnitPrice < price).ToListAsync();
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " Products that match criteria.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering and paging Products, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = await GetAllFilteredAsync(searchBy);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync(int stock, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Repo searching Products with " + smlGrtText + " than: " + stock + " items in stock.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = await GetAllFilteredAsync(stock, greater);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync(decimal price, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Repo searching Products " + smlGrtText + " : " + price + "$.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = await GetAllFilteredAsync(price, greater);
            if (prods.Count() == 0) return prods;

            StaticLogger.LogInfo(GetType(), "Repo fetching page " + pgIndex + " of size " + pgSize + " from filtered Product records.");
            prods = prods.Skip(pgSize * (pgIndex - 1)).Take(pgSize).ToList();
            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count().ToString() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllSortedAsync
           (string sortBy, bool desc = false, IEnumerable<Product> prodList = null)
        {
            var log = new StringBuilder("Repo sorting " + (prodList == null ? "all" : "select") +
                " Product records in " + (desc ? "descending" : "asccending") + " order by: ");

            if (prodList != null)return GetAllSorted(sortBy, desc, prodList);

            var prods = new List<Product>();

            switch (sortBy.Trim().ToUpper())
            {
                case "NAME":
                case "PRODUCTNAME":
                case "PRODNAME":
                case "PRODUCT_NAME":
                case "PROD_NAME":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.Name).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.Name).ToListAsync();
                    log.Append("Name");
                    break;
                case "MANUFACTURER":
                case "MAN":
                case "MADEBY":
                case "MADE_BY":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.Manufacturer).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.Manufacturer).ToListAsync();
                    log.Append("Manufacturer");
                    break;
                case "BARCODE":
                case "BAR_CODE":
                case "CODE":
                case "UPC":
                case "UPCCODE":
                case "UPC_CODE":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.BarCode).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.BarCode).ToListAsync();
                    log.Append("Barcode");
                    break;
                case "STOCK":
                case "STK":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.Stock).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.Stock).ToListAsync();
                    log.Append("Stock");
                    break;
                case "PRICE":
                case "COST":
                case "UNITPRICE":
                case "UNIT_PRICE":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.UnitPrice).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.UnitPrice).ToListAsync();
                    log.Append("Price");
                    break;
                case "CAT":
                case "CATEGORY":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.CategoryId).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.CategoryId).ToListAsync();
                    log.Append("Category");
                    break;
                case "DESC":
                case "DESCRIPTION":
                    prods = desc ? await _context.Products.OrderByDescending(p => p.Description.Length).ToListAsync() :
                                   await _context.Products.OrderBy(p => p.Description.Length).ToListAsync();
                    log.Append("Description");
                    break;
                default:
                    prods = desc ? await _context.Products.OrderByDescending(p => p.ProductId).ToListAsync() :
                                                       await _context.Products.OrderBy(p => p.ProductId).ToListAsync();
                    log.Append("ID");
                    break;
            }

            log.Append(".");

            StaticLogger.LogInfo(GetType(), log.ToString());
            StaticLogger.LogInfo(GetType(), "Repo sorted: " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo sorting all Product records in " +
                (desc ? "descending" : "ascending") + " order by: " + sortBy + 
                " and displaying Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = await GetAllPaginatedAsync(pgSize, pgIndex, 
                        await GetAllSortedAsync(sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo retrieved: " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Product records by: " +
                searchBy + " then sorting the results in " + 
                (desc ? "descending" : "ascending") + " order by: " + sortBy + ".");

            var prods = await GetAllSortedAsync(sortBy, desc, 
                        await GetAllFilteredAsync(searchBy));

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (int stock, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching records for Products with " +
                (greater ? "over" : "under") + " " + stock + " pieces available and Sorting them in " 
                + (desc ? "descending" : "ascending") + " order by " + sortBy + ".");

            var prods = await GetAllSortedAsync(sortBy, desc, 
                        await GetAllFilteredAsync(stock, greater));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (decimal price, bool greater, string sortBy, bool desc = false)

        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Products " + (greater ? "over" : "under") +
                " " + price.ToString("0:0.##") + "$ in value and Sorting them in " + 
                (desc ? "descending" : "ascending") + " order by: " + sortBy + ".");

            var prods = await GetAllSortedAsync(sortBy, desc, 
                        await GetAllFilteredAsync(price, greater));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo filtering Product records by: " + 
                searchBy + ", sorting the results in " + (desc ? "descending" : "ascending") +
                " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = await GetAllPaginatedAsync(pgSize, pgIndex, 
                        await GetAllFilteredAndSortedAsync(searchBy, sortBy, desc));

            StaticLogger.LogInfo(GetType(), "Repo retrieved " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching records for Products with " +
                (greater ? "over" : "under") + " " + stock + " pieces available and Sorting them in "
                + (desc ? "descending" : "ascending") + " order by " + sortBy + " and showing Page: " 
                + pgIndex + " of Size: " + pgSize + ".");

            var prods = await GetAllPaginatedAsync(pgSize, pgIndex, 
                        await GetAllFilteredAndSortedAsync(stock, greater, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Repo fetching Products " + 
                (greater ? "over" : "under") + " " + price.ToString("0:0.##") +
                "$ in value and Sorting them in " + (desc ? "descending" : "ascending") + 
                " order by: " + sortBy + " and showing Page: " + pgIndex + " of Size: " + pgSize + ".");

            var prods = await GetAllPaginatedAsync(pgSize, pgIndex, 
                        await GetAllFilteredAndSortedAsync(price, greater, sortBy, desc));
            StaticLogger.LogInfo(GetType(), "Repo found " + prods.Count() + " records.");
            return prods;
        }

        #endregion

        #region IdVerifiers

        public async Task<bool> IsUniqueBarCodeAsync(string barCode, int? prodId = null) 
            => (await _context.Products.Where(p => 
                p.BarCode == barCode && 
                p.ProductId != prodId)
                .ToListAsync()).Count() == 0;

        public async Task<bool> ProductIdExistsAsync(int prodId)
            => await _context.Products.FindAsync(prodId) != null;

        public async Task<bool> CategoryIdExistsAsync(int catId) 
            => await _context.Categories.FindAsync(catId) != null;

        #endregion

        #endregion

        #region LeftOvers

        //public Product GetByBarcode(string barcode)
        //{
        //    StaticLogger.LogInfo(GetType(), "Repo fetching Product with Barcode:\t" + barcode);
        //    var prod = _context.Products.SingleOrDefault(p => p.BarCode == barcode);
        //    if (prod == null) StaticLogger.LogWarn(GetType(), "Repo found no Product with Barcode:\t" + barcode);
        //    return prod;
        //}

        #endregion
    }
}
