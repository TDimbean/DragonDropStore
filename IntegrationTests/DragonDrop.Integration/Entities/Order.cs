using System;

namespace DragonDrop.Integration.Entities
{
    public class Order
    {
        public int OrderId;
        public int CustomerId;
        public DateTime OrderDate;
        public DateTime? ShippingDate;
        public int OrderStatusId;
        public int ShippingMethodId;
        public int PaymentMethodId;
    }
}
