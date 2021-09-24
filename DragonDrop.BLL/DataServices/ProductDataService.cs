using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.DAL.Interfaces;
using DragonDrop.Infrastructure;
using DragonDrop.Infrastructure.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonDrop.BLL.DataServices
{
    public class ProductDataService : IProductDataService
    {
        private IProductRepository _repo;

        public ProductDataService(IProductRepository repo) => _repo = repo;

        public Product Get(int id)
        {
            if (!_repo.ProductIdExists(id))
            {
                StaticLogger.LogWarn(GetType(), "Product Data Service found no record with ID: " + id + " in Repo.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product with ID:\t" + id + " from Repo.");
            var prod = _repo.Get(id);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return prod;
        }

        public Product GetByBarcode(string code)
        {
            //if (!_repo.ProductIdExists(id))
            //{
            //    StaticLogger.LogWarn(GetType(), "Product Data Service found no record with ID: " + id + " in Repo.");
            //    return null;
            //}
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product with Barcode:\t" + code + " from Repo.");
            var prod = _repo.GetByBarcode(code);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return prod;
        }

        public string Create(Product prod, bool requiresErrList = false)
        {
            var validation = ValidateProduct(prod);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Product failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Product Data Service requesting record creation for Product named:\t" + prod.Name);
            _repo.Create(prod);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return null;
        }

        public string Update(Product prod, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            if (!_repo.ProductIdExists(prod.ProductId))
            {
                idError = "Product Data Service could not find any Product with ID:\t" + prod.ProductId + " in Repo.";
                idExists = false;
            }

            var validation = ValidateProduct(prod);

            if (!idExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Prouct failed Data Service validation and will not be sent to Repo for update. Reason:\n" + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Product Data Service requesting update for record with ID:\t" + prod.ProductId + " from Repo.");
            _repo.Update(prod);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return null;
        }

        public void AddStock(int prodId, int qty) => _repo.AddStock(prodId, qty);

        #region Gets

        public IEnumerable<Product> GetAll()
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting all Products from Repo.");
            var prods = _repo.GetAll();
            StaticLogger.LogInfo(GetType(), "Product Data Service retrieved " + prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllPaginated(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting page " + pgIndex + " of size " + pgSize + " from all Product records in Repo.");
            var prods = _repo.GetAllPaginated(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        #region Filters

        public IEnumerable<Product> GetAllFiltered(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product records by:\t" + searchBy + " from Repo.");
            var prods = _repo.GetAllFiltered(searchBy);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " + prods.Count() + " Products from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFiltered(int stock, bool greater)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products with " + smlGrtText + " than: " + stock + " items in stock from Repo.");
            var prods = _repo.GetAllFiltered(stock, greater);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " + prods.Count() + " Products back from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFiltered(decimal price, bool greater)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products " + smlGrtText + " : " + price + "$ from Repo.");
            var prods = _repo.GetAllFiltered(price, greater);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " + prods.Count() + " Products back from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products from Repo, according to \n SEARCH: " + searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = _repo.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(int stock, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products with " + smlGrtText + " than: " + stock + " items in stock from Repo.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = _repo.GetAllFilteredAndPaged(stock, greater, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndPaged(decimal price, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products " + smlGrtText + " : " + price + "$ from Repo.\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = _repo.GetAllFilteredAndPaged(price, greater, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        #endregion

        #region Sorts

        public IEnumerable<Product> GetAllSorted(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllSorted(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllSortedAndPaged(int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: "+pgIndex+" of Size: " + pgSize +" of records Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndSorted(string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting records Filtered by: "+ searchBy + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredAndSorted(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndSorted(int stock, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting records with stock " + (greater?"over":"under") + stock + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredAndSorted(stock, greater, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredAndSorted(decimal price, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting records with price " + (greater ? "over" : "under") + price.ToString("0.0:##") + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredAndSorted(price, greater, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged(string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: "+pgIndex +" of Size: "+pgSize+" of records Filtered by: " + searchBy + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged(int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of records with stock " + (greater ? "over" : "under") + stock + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredSortedAndPaged(stock, greater, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        public IEnumerable<Product> GetAllFilteredSortedAndPaged(decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " + pgIndex + " of Size: " + pgSize + " of records with price " + (greater ? "over" : "under") + price.ToString("0.0:##") + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = _repo.GetAllFilteredSortedAndPaged(price, greater, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + prods.Count() + " records from Repo.");
            return prods;
        }

        #endregion

        #endregion

        #region Validators

        public (bool isValid, string errorList) ValidateProduct(Product prod)
        {
            var errorList = new StringBuilder();

            var nameVal = ValidateName(prod.Name);
            var descVal = ValidateDescription(prod.Description);
            var barVal = ValidateBarCode(prod.BarCode, prod.ProductId);
            var priceVal = ValidateUnitPrice(prod.UnitPrice);

            var isValid = nameVal.isValid && descVal.isValid && barVal.isValid && priceVal.isValid;
            errorList.Append(nameVal.errorList)
                    .Append(descVal.errorList)
                    .Append(barVal.errorList)
                    .Append(priceVal.errorList);

            if (prod.CategoryId == null || !_repo.CategoryIdExists(prod.CategoryId.GetValueOrDefault()))
            {
                errorList.AppendLine("Products require a valid Category to be placed under\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
                isValid = false;
            }

            if (prod.Stock < 0)
            {
                errorList.AppendLine("Product Stock may not be negative.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");

            return (isValid, errorList.ToString());
        }

        public (bool isValid, string errorList) ValidateUnitPrice(decimal? price)
        {
            if (price == null || price <= 0m)
                return(false, "A Product's Unit Price must be greter than 0$.");
            return (true, string.Empty);
        }

        public (bool isValid, string errorList) ValidateDescription(string desc)
        {
            if (desc != null && desc.Length > 360)
                return(false, "Product Description is limited to 360 characters.");
            return (true, string.Empty);
        }

        public (bool isValid, string errorList) ValidateName(string name)
        {
            if (name == null || name.Trim().Length == 0) return(false, "Products require a name.");
            if (name.Length > 50) return(false, "Product Name limited to 50 characters.");
            return (true, string.Empty);
        }

        public (bool isValid, string errorList) ValidateBarCode(string barCode, int prodId)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (barCode == null)
            {
                errorList.AppendLine("Products require a Barcode.");
                isValid = false;
            }
            else
            {
                var barCodeValidation = ValidateBarCodeFormat(barCode);

                if (!barCodeValidation.isValid.GetValueOrDefault())
                {
                    errorList.AppendLine(barCodeValidation.errorList);
                    barCodeValidation = (null, null);
                    isValid = false;
                }
            }

            if (!_repo.IsUniqueBarCode(barCode, prodId))
            {
                errorList.Append("Products must have a unique Bar Code.");
                isValid = false;
            }

            return (isValid, errorList.ToString());
        }

        public (bool? isValid, string errorList) ValidateBarCodeFormat(string barCode)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (barCode.Length != 12)
            {
                errorList.AppendLine("Product Barcode must be precisely 12 characters long.");
                isValid = false;
            }

            else
            {

                if (!barCode.IsDigitsOnly())
                {
                    errorList.AppendLine("Product Barcode must be made up entirely of digits.");
                    isValid = false;
                }

                else
                {
                    #region Format Check

                    var oddSum = 0;
                    var evenSum = 0;

                    for (int i = 0; i < 11; i++)
                    {
                        var digit = int.Parse(barCode[i].ToString());

                        oddSum += i % 2 == 0 ? digit : 0;
                        evenSum += i % 2 != 0 ? digit : 0;
                    }

                    var totalSum = oddSum * 3 + evenSum;
                    var check = 0;

                    while (true)
                    {
                        if ((check + totalSum) % 10 == 0) break;
                        else check++;
                    }

                    var validFormat = check == int.Parse(barCode.Substring(11, 1));

                    #endregion

                    if (!validFormat)
                    {
                        errorList.AppendLine("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
                        isValid = false;
                    }
                }
            }

            return (isValid, errorList.ToString());
        }

        #endregion

        public bool BarcodeExists(string code) => !_repo.IsUniqueBarCode(code);

        #region Async

        public async Task<Product> GetAsync(int id)
        {
            if (!await _repo.ProductIdExistsAsync(id))
            {
                StaticLogger.LogWarn(GetType(), "Product Data Service found no record with ID: " + id + " in Repo.");
                return null;
            }
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product with ID:\t" + id + " from Repo.");
            var prod = await _repo.GetAsync(id);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return prod;
        }

        public async Task<Product> GetByBarcodeAsync(string code)
        {
            //if (!_repo.ProductIdExists(id))
            //{
            //    StaticLogger.LogWarn(GetType(), "Product Data Service found no record with ID: " + id + " in Repo.");
            //    return null;
            //}
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product with Barcode:\t" + code + " from Repo.");
            var prod = await _repo.GetByBarcodeAsync(code);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return prod;
        }


        public async Task<string> CreateAsync(Product prod, bool requiresErrList = false)
        {
            var validation = await ValidateProductAsync(prod);

            if (!validation.isValid)
            {
                StaticLogger.LogWarn(GetType(), "Product failed Data Service validation and will not be submitted to the Repo. Details:\n" + validation.errorList);
                if (requiresErrList) return validation.errorList;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Product Data Service requesting record creation for Product named:\t" + prod.Name);
            await _repo.CreateAsync(prod);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return null;
        }

        public async Task AddStockAsync(int prodId, int qty) => await _repo.AddStockAsync(prodId, qty);

        public async Task<string> UpdateAsync(Product prod, bool requiresErrList = false)
        {
            var idExists = true;
            var idError = "";

            if (!await _repo.ProductIdExistsAsync(prod.ProductId))
            {
                idError = "Product Data Service could not find any Product with ID:\t" + 
                    prod.ProductId + " in Repo.";
                idExists = false;
            }

            var validation = await ValidateProductAsync(prod);

            if (!idExists || !validation.isValid)
            {
                StaticLogger.LogWarn(GetType(),
                    "Prouct failed Data Service validation and will not be sent to Repo for update. Reason:\n" 
                    + validation.errorList + idError);
                if (requiresErrList) return validation.errorList + idError;
                else return null;
            }

            StaticLogger.LogInfo(GetType(), "Product Data Service requesting update for record with ID:\t" +
                prod.ProductId + " from Repo.");
            await _repo.UpdateAsync(prod);
            StaticLogger.LogInfo(GetType(), "Repo finished and returned to Product Data Service.");
            return null;
        }

        #region Gets

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting all Products from Repo.");
            var prods = await _repo.GetAllAsync();
            StaticLogger.LogInfo(GetType(), "Product Data Service retrieved " + 
                prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllPaginatedAsync(int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting page " + 
                pgIndex + " of size " + pgSize + " from all Product records in Repo.");
            var prods = await _repo.GetAllPaginatedAsync(pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        #region Filters

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(string searchBy)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Product records by:\t" + 
                searchBy + " from Repo.");
            var prods = await _repo.GetAllFilteredAsync(searchBy);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " + 
                prods.Count() + " Products from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(int stock, bool greater)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products with " + 
                smlGrtText + " than: " + stock + " items in stock from Repo.");
            var prods = await _repo.GetAllFilteredAsync(stock, greater);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " + 
                prods.Count() + " Products back from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAsync(decimal price, bool greater)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products " + 
                smlGrtText + " : " + price + "$ from Repo.");
            var prods = await _repo.GetAllFilteredAsync(price, greater);
            StaticLogger.LogInfo(GetType(), "Product Data Service got " +
                prods.Count() + " Products back from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync(string searchBy, int pgSize, int pgIndex)
        {
            StaticLogger.LogInfo(GetType(), 
                "Product Data Service requesting Products from Repo, according to \n SEARCH: " +
                searchBy + "\n Page Index: " + pgIndex + "; Page Size: " + pgSize);
            var prods = await _repo.GetAllFilteredAndPagedAsync(searchBy, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "more" : "less";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products with " +
                smlGrtText + " than: " + stock + " items in stock from Repo.\n Page Index: " +
                pgIndex + "; Page Size: " + pgSize);
            var prods = await _repo.GetAllFilteredAndPagedAsync(stock, greater, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex)
        {
            var smlGrtText = greater ? "over" : "under";
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Products " +
                smlGrtText + " : " + price + "$ from Repo.\n Page Index: " + 
                pgIndex + "; Page Size: " + pgSize);
            var prods = await _repo.GetAllFilteredAndPagedAsync(price, greater, pgSize, pgIndex);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count().ToString() + " records from Repo.");
            return prods;
        }

        #endregion

        #region Sorts

        public async Task<IEnumerable<Product>> GetAllSortedAsync(string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting records Sorted by: " + 
                sortBy + " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = await _repo.GetAllSortedAsync(sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + 
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllSortedAndPagedAsync
            (int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " + 
                pgIndex + " of Size: " + pgSize + " of records Sorted by: " + sortBy +
                " in " + (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = await _repo.GetAllSortedAndPagedAsync(pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (string searchBy, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), 
                "Product Data Service requesting records Filtered by: " +
                searchBy + " and Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = await _repo.GetAllFilteredAndSortedAsync(searchBy, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (int stock, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(),
                "Product Data Service requesting records with stock " + (greater ? "over" : "under") +
                stock + " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") + 
                " order from Repo.");
            var prods = await _repo.GetAllFilteredAndSortedAsync(stock, greater, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredAndSortedAsync
            (decimal price, bool greater, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(),
                "Product Data Service requesting records with price " + 
                (greater ? "over" : "under") + price.ToString("0.0:##") + 
                " and Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = await _repo.GetAllFilteredAndSortedAsync(price, greater, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " + 
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " + 
                pgIndex + " of Size: " + pgSize + " of records Filtered by: " + 
                searchBy + " and Sorted by: " + sortBy + " in " + 
                (desc ? "descending" : "ascening") + " order from Repo.");
            var prods = await _repo.GetAllFilteredSortedAndPagedAsync
                (searchBy, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (int stock, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of records with stock " + 
                (greater ? "over" : "under") + stock + " and Sorted by: " + 
                sortBy + " in " + (desc ? "descending" : "ascening") + 
                " order from Repo.");
            var prods = await _repo.GetAllFilteredSortedAndPagedAsync
                (stock, greater, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        public async Task<IEnumerable<Product>> GetAllFilteredSortedAndPagedAsync
            (decimal price, bool greater, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            StaticLogger.LogInfo(GetType(), "Product Data Service requesting Page: " +
                pgIndex + " of Size: " + pgSize + " of records with price " + 
                (greater ? "over" : "under") + price.ToString("0.0:##") +
                " and Sorted by: " + sortBy + " in " + (desc ? "descending" : "ascening") +
                " order from Repo.");
            var prods = await _repo.GetAllFilteredSortedAndPagedAsync
                (price, greater, pgSize, pgIndex, sortBy, desc);
            StaticLogger.LogInfo(GetType(), "Product Data Service received " +
                prods.Count() + " records from Repo.");
            return prods;
        }

        #endregion

        #endregion

        #region Validators

        public async Task<(bool isValid, string errorList)> ValidateProductAsync(Product prod)
        {
            var errorList = new StringBuilder();

            var nameVal = await ValidateNameAsync(prod.Name);
            var descVal = await ValidateDescriptionAsync(prod.Description);
            var barVal = await ValidateBarCodeAsync(prod.BarCode, prod.ProductId);
            var priceVal = await ValidateUnitPriceAsync(prod.UnitPrice);

            var isValid = nameVal.isValid && descVal.isValid && barVal.isValid && priceVal.isValid;
            errorList.Append(nameVal.errorList)
                    .Append(descVal.errorList)
                    .Append(barVal.errorList)
                    .Append(priceVal.errorList);

            if (prod.CategoryId == null || !await _repo.CategoryIdExistsAsync(prod.CategoryId.GetValueOrDefault()))
            {
                errorList.AppendLine("Products require a valid Category to be placed under;"+
                    "\nIf unsure of which Category a Product falls under, you may use \"Miscellaneous\", ID: 0.");
                isValid = false;
            }

            if (prod.Stock < 0)
            {
                errorList.AppendLine("Product Stock may not be negative.");
                isValid = false;
            }

            errorList.Replace("\r\n", "");

            return (isValid, errorList.ToString());
        }

        public async Task<(bool isValid, string errorList)> ValidateUnitPriceAsync(decimal? price)
        {
            if (price == null || price <= 0m)
                return (false, "A Product's Unit Price must be greter than 0$.");
            return (true, string.Empty);
        }

        public async Task<(bool isValid, string errorList)> ValidateDescriptionAsync(string desc)
        {
            if (desc != null && desc.Length > 360)
                return (false, "Product Description is limited to 360 characters.");
            return (true, string.Empty);
        }

        public async Task<(bool isValid, string errorList)> ValidateNameAsync(string name)
        {
            if (name == null || name.Trim().Length == 0) return (false, "Products require a name.");
            if (name.Length > 50) return (false, "Product Name limited to 50 characters.");
            return (true, string.Empty);
        }

        public async Task<(bool isValid, string errorList)> ValidateBarCodeAsync(string barCode, int prodId)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (barCode == null)
            {
                errorList.AppendLine("Products require a Barcode.");
                isValid = false;
            }
            else
            {
                var barCodeValidation = await ValidateBarCodeFormatAsync(barCode);

                if (!barCodeValidation.isValid.GetValueOrDefault())
                {
                    errorList.AppendLine(barCodeValidation.errorList);
                    barCodeValidation = (null, null);
                    isValid = false;
                }

            if (!await _repo.IsUniqueBarCodeAsync(barCode, prodId))
            {
                errorList.Append("Products must have a unique Bar Code.");
                isValid = false;
            }
            }

            return (isValid, errorList.ToString());
        }

        public async Task<(bool? isValid, string errorList)> ValidateBarCodeFormatAsync(string barCode)
        {
            var isValid = true;
            var errorList = new StringBuilder();

            if (barCode.Length != 12)
            {
                errorList.AppendLine("Product Barcode must be precisely 12 characters long.");
                isValid = false;
            }

            else
            {

                if (!barCode.IsDigitsOnly())
                {
                    errorList.AppendLine("Product Barcode must be made up entirely of digits.");
                    isValid = false;
                }

                else
                {
                    #region Format Check

                    var oddSum = 0;
                    var evenSum = 0;

                    for (int i = 0; i < 11; i++)
                    {
                        var digit = int.Parse(barCode[i].ToString());

                        oddSum += i % 2 == 0 ? digit : 0;
                        evenSum += i % 2 != 0 ? digit : 0;
                    }

                    var totalSum = oddSum * 3 + evenSum;
                    var check = 0;

                    while (true)
                    {
                        if ((check + totalSum) % 10 == 0) break;
                        else check++;
                    }

                    var validFormat = check == int.Parse(barCode.Substring(11, 1));

                    #endregion

                    if (!validFormat)
                    {
                        errorList.AppendLine("Barcode must be in a valid format\n(X-XXXXX-YYYYY-Z,"+
                            "\n X=> Manufacturer Code;\n Y=> Product Code;\n Z=> Check Digit).");
                        isValid = false;
                    }
                }
            }

            return (isValid, errorList.ToString());
        }

        #endregion

        public async Task<bool> BarcodeExistsAsync(string code) => !await _repo.IsUniqueBarCodeAsync(code);

        #endregion
    }
}
