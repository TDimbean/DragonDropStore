using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using System.Web.Http;

namespace DrgonDrop.WebApi.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private IOrderDataService _service;

        public OrdersController(IOrderDataService service) => _service = service;

        // GET api/orders/id
        public IHttpActionResult Get([FromUri]int ordId)
        {
            var ord = _service.Get(ordId);

            if (ord == null) return NotFound();

            return Ok(ord);
        }

        // POST api/orders/{returnErrors}
        public IHttpActionResult Create(Order ord, [FromUri] bool returnErrors = false)
        {
            var result = _service.Create(ord, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Order registration failed.");
            }

            return Ok("Order successfully registered.");
        }

        // PUT: api/Orders/{returnErrors}
        [HttpPut]
        public IHttpActionResult Update(Order ord, [FromUri] bool returnErrors = false)
        {
            var result = _service.Update(ord, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Order update failed.");
            }

            return Ok("Order successfully updated.");
        }

        // GET api/orders
        public IHttpActionResult GetAll() => Ok(_service.GetAll());

        // GET api/orders?pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllPaginated([FromUri] int pgSize, int pgIndex) => Ok(_service.GetAllPaginated(pgSize, pgIndex));

        // GET api/orders?search={searchBy}
        public IHttpActionResult GetAllFiltered([FromUri] string searchBy) => Ok(_service.GetAllFiltered(searchBy));

        // GET api/orders?search={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllFilteredAndPaged([FromUri] string searchBy, int pgSize, int pgIndex)
            => Ok(_service.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex));

        // GET api/orders?sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllSorted([FromUri] string sortBy, bool desc = false)
            => Ok(_service.GetAllSorted(sortBy, desc));

        // GET api/orders?pgSize={pgSize:int}&pgIndex={pgIndex:int}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllSortedAndPaged([FromUri] int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc));

        // GET api/orders?searchBy={searchBy}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllFilteredAndSorted([FromUri]string searchBy, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredAndSorted(searchBy, sortBy, desc));

        // GET api/orders?searchBy={searchBy}&pgSize={pgSize:int}&pgIndex={pgIndex:int}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllFilteredSortedAndPaged([FromUri]string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc));

        // GET api/orders?custId={custId}
        public IHttpActionResult GetAllByCustomerId([FromUri]int custId) => Ok(_service.GetAllByCustomerId(custId));

        // GET api/orders/bycustomer?custId={custId}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByCustomerIdAndPaged([FromUri]int custId, int pgSize, int pgIndex)
            => Ok(_service.GetAllByCustomerIdAndPaged(custId, pgSize, pgIndex));

        // GET api/orders?custId={custId}&searchBy={searchBy}
        public IHttpActionResult GetAllByCustomerIdAndFiltered([FromUri]int custId, string searchBy) 
            => Ok(_service.GetAllByCustomerIdAndFiltered(custId, searchBy));

        // GET api/orders?custId={custId}&searchBy={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByCustomerIdFilteredAndPaged([FromUri]int custId, string searchBy, int pgSize, int pgIndex)
            => Ok(_service.GetAllByCustomerIdFilteredAndPaged(custId, searchBy, pgSize, pgIndex));

        // GET api/orders?custId={custId}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdAndSorted([FromUri]int custId, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdAndSorted(custId, sortBy, desc));

        // GET api/orders?custId={custId}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdSortedAndPaged([FromUri]int custId, int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdSortedAndPaged(custId, pgSize,pgIndex,sortBy,desc));

        // GET api/orders?custId={custId}&searchBy={searchBy}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdFilteredAndSorted([FromUri]int custId, string searchBy, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc));
        
        // GET api/orders?custId={custId}&searchBy={searchBy}&pgSize={pgSize}&pgIndex=pgIndex&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdFilteredSortedAndPaged([FromUri]int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdFilteredSortedAndPaged(custId, searchBy, pgSize, pgIndex, sortBy, desc));
    }
}