namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonInvoiceADDB : BaseEntity
    {
        public Guid AdjustmentId { get; set; }

        public int Amount { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Description { get; set; }

        public Guid InvoiceId { get; set; }
    }
}

