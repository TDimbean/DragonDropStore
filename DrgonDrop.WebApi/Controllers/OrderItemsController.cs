using DragonDrop.BLL.Interfaces.DataServices;
using DragonDrop.DAL.Entities;
using System.Web.Http;

namespace DrgonDrop.WebApi.Controllers
{
    //[RoutePrefix("api/orderitems")]   -- Unnecessary but might come in handy if Controller expanded to require [Route("")] Attributes
    public class OrderItemsController : ApiController
    {
        private IOrderItemDataService _service;

        public OrderItemsController(IOrderItemDataService service) => _service = service;

        // GET api/orderitems?ordId={ordId}&prodId={prodId}
        public IHttpActionResult Get([FromUri]int ordId, int prodId)
        {
            var item = _service.Get(ordId, prodId);

            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST api/orderitems?returnErrors={returnErrors?}
        public IHttpActionResult Create(OrderItem item, [FromUri] bool returnErrors = false)
        {
            var result = _service.Create(item, true);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("OrderItem registration failed.");
            }

            return Ok("OrderItem successfully registered.");
        }

        // PUT: api/orderitems?returnErrors={returnErrors?}
        [HttpPut]
        public IHttpActionResult Update(OrderItem item, [FromUri] bool returnErrors = false)
        {
            var result = _service.Update(item, returnErrors);

            if (!string.IsNullOrEmpty(result))
            {
                if (returnErrors) return BadRequest(result);
                return BadRequest("OrderItem update failed.");
            }

            return Ok("OrderItem successfully updated.");
        }

        // GET api/orderitems
        public IHttpActionResult GetAll() => Ok(_service.GetAll());

        // GET api/orderitems?pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllPaginated([FromUri] int pgSize, int pgIndex) => Ok(_service.GetAllPaginated(pgSize, pgIndex));

        // GET api/orderitems?ordId={ordId}
        public IHttpActionResult GetAllByOrderId([FromUri]int ordId) => Ok(_service.GetAllByOrderId(ordId));

        // GET api/orderitems?ordId={ordId}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByOrderIdAndPaged([FromUri]int ordId, int pgSize, int pgIndex)
            => Ok(_service.GetAllByOrderIdAndPaged(ordId, pgSize, pgIndex));

        // GET api/orderitems?prodId={prodId}
        public IHttpActionResult GetAllByProductId([FromUri] int prodId) => Ok(_service.GetAllByProductId(prodId));

        // GET api/orderitems?prodId={prodId}&pgSize={pgSize}&pgIndex={pgIndex}
        public IHttpActionResult GetAllByProductIdAndPaged([FromUri] int prodId,int pgSize, int pgIndex)
            => Ok(_service.GetAllByProductIdAndPaged(prodId, pgSize, pgIndex));

        // GET api/orderItems?sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllSorted([FromUri] string sortBy, bool desc = false)
            => Ok(_service.GetAllSorted(sortBy, desc));

        // GET api/orderItems?pgSize={pgSize:int}&pgIndex={pgIndex:int}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllSortedAndPaged([FromUri] int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllSortedAndPaged(pgSize, pgIndex, sortBy, desc));

        // GET api/orderItems?ordId={ordId:int}&sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByOrderIdAndSorted([FromUri] int ordId, string sortBy, bool desc = false)
            => Ok(_service.GetAllByOrderIdAndSorted(ordId, sortBy, desc));

        // GET api/orderItems?prodId={prodId:int}&sortBy={sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByProductIdAndSorted([FromUri] int prodId, string sortBy, bool desc = false)
            => Ok(_service.GetAllByProductIdAndSorted(prodId, sortBy, desc));

        // GET api/orderItems?ordId={ordId:int}&pgSize={pgSize:int}&pgIndex={pgIndex:int}sortBy}&desc={desc:bool?}
        public IHttpActionResult GetAllByOrderIdSortedAndPaged([FromUri] int ordId, int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllByOrderIdSortedAndPaged(ordId, pgSize, pgIndex, sortBy, desc));

        // GET api/orderItems?prodId={prodId:int}&sortBy={sortBy}&pgSize={pgSize:int}&pgIndex={pgIndex:int}&desc={desc:bool?}
        public IHttpActionResult GetAllByProductIdSortedAndPaged([FromUri] int prodId,int pgSize, int pgIndex, string sortBy, bool desc = false)
            => Ok(_service.GetAllByProductIdSortedAndPaged(prodId, pgSize, pgIndex, sortBy, desc));
    }
}