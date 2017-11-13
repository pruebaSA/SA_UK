namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class BillingPeriodListItemDB : BaseEntity
    {
        public string BillEndDate { get; set; }

        public Guid InvoiceId { get; set; }
    }
}

