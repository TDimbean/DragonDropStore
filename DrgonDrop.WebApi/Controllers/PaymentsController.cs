using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using DragonDrop.Infrastructure.Helpers;
using System.Web.Http;

namespace DrgonDrop.WebApi.Controllers
{
    [RoutePrefix("api/payments")]
    public class PaymentsController : ApiController
    {
        private IPaymentDataService _service;

        public PaymentsController(IPaymentDataService service) => _service = service;

        // GET api/payments?payId={payId:int}
        public IHttpActionResult Get([FromUri]int payId)
        {
            var pay = _service.Get(payId);

            if (pay == null) return NotFound();

            return Ok(pay);
        }

        // POST api/payments?returnErrors={returnErrors}
        public IHttpActionResult Create(Payment pay, [FromUri] bool returnErrors = false)
        {
            var result = _service.Create(pay, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Payment registration failed.");
            }

            return Ok("Payment successfully registered.");
        }

        // PUT: api/payments?returnErrors={returnErrors?}
        [HttpPut]
        public IHttpActionResult Update(Payment pay, [FromUri] bool returnErrors = false)
        {
            var result = _service.Update(pay, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("Payment update failed.");
            }

            return Ok("Payment successfully updated.");
        }

        // GET api/payments
        public IHttpActionResult GetAll() => Ok(_service.GetAll());

        // GET api/payments?pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllPaginated([FromUri] int pgSize, int pgIndex) => Ok(_service.GetAllPaginated(pgSize, pgIndex));

        // GET api/payments?search={searchBy}
        public IHttpActionResult GetAllFiltered([FromUri] string searchBy) => Ok(_service.GetAllFiltered(searchBy));

        // GET api/payments?searchBy={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllFilteredAndPaged([FromUri] string searchBy, int pgSize, int pgIndex)
            => Ok(_service.GetAllFilteredAndPaged(searchBy, pgSize, pgIndex));

        // GET api/payments?date={date}&before={before}
        public IHttpActionResult GetAllByDate([FromUri] string date, bool before)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllFiltered(when.GetValueOrDefault(), before));
        }

        // GET api/payments?amount={amount}&over={over}
        public IHttpActionResult GetAllByAmount([FromUri] string amount, bool over)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllFiltered(amt.GetValueOrDefault(), over));
        }

        // Get api/payments?date={date}&before={before}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByDateAndPaged([FromUri] string date, bool before, int pgSize, int pgIndex)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllFilteredAndPaged(when.GetValueOrDefault(), before, pgSize, pgIndex));
        }

        // GET api/payments?amount={amount}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByAmountAndPaged([FromUri] string amount, bool over, int pgSize, int pgIndex)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllFilteredAndPaged(amt.GetValueOrDefault(), over, pgSize, pgIndex));
        }

        // GET api/payments?custId={custId}
        public IHttpActionResult GetAllByCustomerId([FromUri]int custId) => Ok(_service.GetAllByCustomerId(custId));

        // GET api/payments?custId={custId}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByCustomerIdAndPaged([FromUri]int custId, int pgSize, int pgIndex)
            => Ok(_service.GetAllByCustomerIdAndPaged(custId, pgSize, pgIndex));

        // GET api/payments?sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllSorted([FromUri]string sortBy, bool desc=false)
            => Ok(_service.GetAllSorted(sortBy, desc));

        // GET api/payments?pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllSortedAndPaged([FromUri]int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc));

        // GET api/payments?searchBy={searchBy}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllFilteredAndSorted([FromUri]string searchBy, string sortBy, bool desc=false)
            => Ok(_service.GetAllFilteredAndSorted(searchBy, sortBy, desc));

        // GET api/payments?date={date}&before={before}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByDateAndSorted([FromUri]string date, bool before, string sortBy, bool desc=false)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllFilteredAndSorted(when.GetValueOrDefault(), before, sortBy, desc));
        }

        // GET api/payments?amount={amount}&over={over}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByAmountAndSorted([FromUri]string amount, bool over, string sortBy, bool desc=false)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllFilteredAndSorted(amt.GetValueOrDefault(), over, sortBy, desc));
        }

        // GET api/payments?custId={custId}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdAndSorted([FromUri]int custId, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdAndSorted(custId, sortBy, desc));

        // GET api/payments?custId={custId}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdSortedAndPaged([FromUri]int custId, int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdSortedAndPaged(custId, pgSize, pgIndex, sortBy, desc));

        // GET api/payments?custId={custId}&searchBy={searchBy}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdFilteredSortedAndPaged([FromUri]int custId, string searchBy, int pgSize, int pgIndex, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdFilteredSortedAndPaged(custId, searchBy, pgSize, pgIndex, sortBy, desc));

        // GET api/payments?custId={custId}&date={date}&before={before}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdByDateSortedAndPaged([FromUri]int custId, string date, bool before, int pgSize, int pgIndex, string sortBy, bool desc=false)
        {
            var when = date.ToDate();
            if (date == null) return BadRequest("Valid Date required for this kind of filter.");

            return Ok(_service.GetAllByCustomerIdFilteredSortedAndPaged(custId, when.GetValueOrDefault(), before, pgSize, pgIndex, sortBy, desc));
        }

        // GET api/payments?custId={custId}&amount={amount}&over={over}&pgSize={pgSize}&pgIndex={pgIndex}&sortBy={sortBy}&desc={desc?}
        public IHttpActionResult GetAllByCustomerIdByAmountSortedAndPaged([FromUri]int custId, string amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc=false)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal Amount required for this kind of filter.");

            return Ok(_service.GetAllByCustomerIdFilteredSortedAndPaged(custId, amt.GetValueOrDefault(), over, pgSize, pgIndex, sortBy, desc));
        }

        // GET api/payments?searchBy={searchBy}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByDateSortedAndPaged([FromUri] string searchBy, int pgSize, int pgIndex, string sortBy, bool desc = false)
        => Ok(_service.GetAllFilteredSortedAndPaged(searchBy, pgSize, pgIndex, sortBy, desc));

        // GET api/payments?date={date}&before={before:bool}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByDateSortedAndPaged([FromUri] string date, bool before, int pgSize, int pgIndex, string sortBy, bool desc=false)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllFilteredSortedAndPaged(when.GetValueOrDefault(), before, pgSize, pgIndex, sortBy, desc));
        }

        // GET api/payments?amount={amount}&over={over:bool}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByAmountSortedAndPaged([FromUri]string amount, bool over, int pgSize, int pgIndex, string sortBy, bool desc = false)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllFilteredSortedAndPaged(amt.GetValueOrDefault(), over, pgSize, pgIndex, sortBy, desc));
        }

        // GET api/payments?custId={custId:int}&date={date}&before={before:bool}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByCustomerIdByDateAndPaged([FromUri] int custId, string date, bool before, int pgSize, int pgIndex)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdFilteredAndPaged(custId, when.GetValueOrDefault(), before, pgSize, pgIndex));
        }

        // GET api/payments?custId={custId:int}&amount={amount}&over={over:bool}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByCustomerIdByAmountAndPaged([FromUri] int custId, string amount, bool over, int pgSize, int pgIndex)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdFilteredAndPaged(custId, amt.GetValueOrDefault(), over, pgSize, pgIndex));
        }

        // GET api/payments?custId={custId:int}&searchBy={searchBy}&pgSize={pgSize:int}&pgIndex={pgIndex:int}
        public IHttpActionResult GetAllByCustomerIdFilteredAndPaged([FromUri] int custId, string searchBy, int pgSize, int pgIndex)
            => Ok(_service.GetAllByCustomerIdFilteredAndPaged(custId, searchBy, pgSize, pgIndex));
        
        // GET api/payments?custId={custId:int}&searchBy={searchBy}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByCustomerIdFilteredAndSorted([FromUri] int custId, string searchBy, string sortBy, bool desc=false)
            => Ok(_service.GetAllByCustomerIdFilteredAndSorted(custId, searchBy, sortBy, desc));

        // GET api/payments?custId={custId:int}&amount={amount}&over={over:bool}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByCustomerIdByAmountAndSorted([FromUri] int custId, string amount, bool over, string sortBy, bool desc = false)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdFilteredAndSorted(custId, amt.GetValueOrDefault(), over, sortBy, desc));
        }

        // GET api/payments?custId={custId:int}&date={date}&before={before:bool}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByCustomerIdByDateAndPaged([FromUri] int custId, string date, bool before, string sortBy, bool desc = false)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdFilteredAndSorted(custId, when.GetValueOrDefault(), before, sortBy, desc));
        }
        
        // GET api/payments?custId={custId:int}&searchBy={searchBy}
        public IHttpActionResult GetAllByCustomerIdAndFiltered([FromUri] int custId, string searchBy)
            => Ok(_service.GetAllByCustomerIdAndFiltered(custId, searchBy));

        // GET api/payments?custId={custId:int}&amount={amount}&over={over:bool}
        public IHttpActionResult GetAllByCustomerIdAndByAmount([FromUri] int custId, string amount, bool over)
        {
            var amt = amount.ToDecimal();
            if (amt == null) return BadRequest("Decimal amount required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdAndFiltered(custId, amt.GetValueOrDefault(), over));
        }

        // GET api/payments?custId={custId:int}&date={date}&before={before:bool}
        public IHttpActionResult GetAllByCustomerIdAndByDate([FromUri] int custId, string date, bool before)
        {
            var when = date.ToDate();
            if (when == null) return BadRequest("Valid Date required for this type of Filter.");

            return Ok(_service.GetAllByCustomerIdAndFiltered(custId, when.GetValueOrDefault(), before));
        }
    }
}