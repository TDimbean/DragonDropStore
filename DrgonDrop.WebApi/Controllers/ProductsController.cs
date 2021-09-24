using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure.Helpers;
using System.Web.Http;

namespace DrgonDrop.WebApi.Controllers
{
    public class ProductsController : ApiController
    {
        private IProductDataService _service;

        public ProductsController(IProductDataService service) => _service = service;

        // GET api/products?prodId={prodId:int}
        public IHttpActionResult Get([FromUri]int prodId)
        {
            var prod = _service.Get(prodId);

            if (prod == null) return NotFound();
            
            return Ok(prod);
        }

        // POST api/products/{returnErrors}
        public IHttpActionResult Create(Product prod, [FromUri] bool returnErrors = false)
        {
            var result = _service.Create(prod, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Product registration failed.");
            }

            return Ok("Product successfully registered.");
        }

        // PUT: api/products/{returnErrors}
        [HttpPut]
        public IHttpActionResult Update(Product prod, [FromUri] bool returnErrors = false)
        {
            var result = _service.Update(prod, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Product update failed.");
            }

            return Ok("Product successfully updated.");
        }

        // GET api/products
        public IHttpActionResult GetAll() => Ok(_service.GetAll());

        // GET api/products?pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllPaginated([FromUri] int pgSize, int pgIndex) => Ok(_service.GetAllPaginated(pgSize, pgIndex));

        // GET api/products?search={searchBy}
        public IHttpActionResult GetAllFiltered([FromUri] string searchBy) => Ok(_service.GetAllFiltered(searchBy));

        // GET api/products?search={searchBy}&&pgSize={pgSize}&&pgIndex={pgIndex}
        public IHttpActionResult GetAllFilteredAndPaged([FromUri] string searchBy, int pgSize, int pgIndex)
            => Ok(_service.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex));

        // GET api/products?stock={stock:int}&over={over:bool}
        public IHttpActionResult GetAllByStock([FromUri] int stock, bool over) 
            => Ok(_service.GetAllFiltered(stock, over));

        // GET api/products?price={price}&over={over}
        public IHttpActionResult GetAllByPrice([FromUri]string price, bool over)
        {
            var cost = price.ToDecimal();
            if (cost == null) return BadRequest("Prices are represented with decimal values, please use only numbers in your query.");

            return Ok(_service.GetAllFiltered(cost.GetValueOrDefault(), over));
        }

        // GET api/products?stock={stock}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByStockAndPaged([FromUri] int stock, bool over, int pgSize, int pgIndex)
            => Ok(_service.GetAllFilteredAndPaged(stock, over, pgSize, pgIndex));

        // GET api/products?price={price}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByPriceAndPaged([FromUri] string price, bool over, int pgSize, int pgIndex)
        {
            var cost = price.ToDecimal();
            if (cost == null) return BadRequest("Prices are represented with decimal values, please use only numbers in your query.");

            return Ok(_service.GetAllFilteredAndPaged(cost.GetValueOrDefault(), over, pgSize, pgIndex));
        }

        // GET api/products?sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllSorted([FromUri] string sortBy, bool desc = false) => Ok(_service.GetAllSorted(sortBy, desc));

        // GET api/products?pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllSortedAndPaged([FromUri] int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc));

        // GET api/products?searchBy={searchBy}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllFilteredAndSorted([FromUri] string searchBy, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredAndSorted(searchBy, sortBy, desc));

        // GET api/products?stock={stock}&over={over}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByStockAndSorted([FromUri] int stock, bool over, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredAndSorted(stock, over, sortBy, desc));

        // GET api/products?price={price}&over={over}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByPriceAndSorted([FromUri] string price, bool over, string sortBy, bool desc = false)
        {
            var cost = price.ToDecimal();
            if (cost == null) return BadRequest("Product Prices are represented with Decimal Values, please use only numbers in your query.");

            return Ok(_service.GetAllFilteredAndSorted(cost.GetValueOrDefault(), over, sortBy, desc));
        }

        // GET api/products?searchBy={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllFilteredSortedAndPaged([FromUri] string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc));

        // GET api/products?stock={stock}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByStockSortedAndPaged([FromUri] int stock, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredSortedAndPaged(stock, over, pgSize, pgIndex, sortBy, desc));

        // GET api/products?price={price}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByPriceSortedAndPaged([FromUri] string price, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var cost = price.ToDecimal();
            if (cost == null) return BadRequest("Product Prices are represented with Decimal Values, please use only numbers in your query.");

            return Ok(_service.GetAllFilteredSortedAndPaged(cost.GetValueOrDefault(), over, pgSize, pgIndex, sortBy, desc));
        }
    }
}