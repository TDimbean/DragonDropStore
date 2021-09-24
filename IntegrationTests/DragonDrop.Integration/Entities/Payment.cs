using System;

namespace DragonDrop.Integration.Entities
{
    public class Payment
    {
        public int PaymentId;
        public int CustomerId;
        public DateTime? Date;
        public decimal? Amount;
        public int PaymentMethodId;
    }
}
