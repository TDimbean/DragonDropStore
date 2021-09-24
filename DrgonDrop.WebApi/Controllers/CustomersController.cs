using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using System.Web.Http;

namespace DrgonDrop.WebApi.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        private ICustomerDataService _service;

        public CustomersController(ICustomerDataService service) => _service = service;


        // GET api/customers/{id}
        public IHttpActionResult Get([FromUri]int custId)
        {
            var cust = _service.Get(custId);

            if (cust == null) return NotFound();

            return Ok(cust);
        }

        // POST api/customers/{returnErrors}
        //[HttpPost]
        //[Route("{returnErrors:bool}")]
        public IHttpActionResult Create(Customer cust, [FromUri] bool returnErrors = false)
        {
            var result = _service.Create(cust, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Customer registration failed.");
            }

            return Ok("Customer successfully registered.");
        }

        // PUT: api/customers/{returnErrors}
        [HttpPut]
        public IHttpActionResult Update(Customer cust,[FromUri] bool returnErrors=false)
        {
            var result = _service.Update(cust, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Customer update failed.");
            }

            return Ok("Customer successfully updated.");
        }

        // GET api/customers
        public IHttpActionResult GetAll() => Ok(_service.GetAll());

        // GET api/customers?pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllPaginated([FromUri] int pgSize, int pgIndex) => Ok(_service.GetAllPaginated(pgSize, pgIndex));

        // GET api/customers?searchBy={searchBy}
        public IHttpActionResult GetAllFiltered([FromUri] string searchBy) => Ok(_service.GetAllFiltered(searchBy));

        // GET api/customers?searchBy={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllFilteredAndPaged([FromUri] string searchBy, int pgSize, int pgIndex) 
            => Ok(_service.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex));

      ////  ///  SORTING doesn't work
      
        // GET api/customers?sort={sortBy}&&desc={desc?}
        public IHttpActionResult GetAllSorted([FromUri] string sortBy, bool desc = false)
            => Ok(_service.GetAllSorted(sortBy, desc));

        // GET api/customers?pgSize={pgSize}&&pgIndex={pgIndex}&&sort={sortBy}&&desc={desc?}
        public IHttpActionResult GetAllSortedAndPaged([FromUri]int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc));

        // GET api/customers?search={searchBy}&&sort={sortBy}&&desc={bool?}
        public IHttpActionResult GetAllFilteredAndSorted([FromUri] string searchBy, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredAndSorted(searchBy, sortBy, desc));

        // GET api/customers?search={searchBy}&&pgSize={pgSize}&&pgIndex={pgIndex}&&sort={sortBy}&&desc={desc?}
        public IHttpActionResult GetAllFilteredSortedAndPaged([FromUri] string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex,sortBy,desc));
    }
}