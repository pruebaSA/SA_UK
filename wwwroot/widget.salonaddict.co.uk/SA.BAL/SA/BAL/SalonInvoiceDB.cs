namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonInvoiceDB : BaseEntity
    {
        public string AdminComment { get; set; }

        public string AuthorizationTransactionCode { get; set; }

        public string AuthorizationTransactionId { get; set; }

        public string AuthorizationTransactionResult { get; set; }

        public string BillEndDate { get; set; }

        public string BillingAddressLine1 { get; set; }

        public string BillingAddressLine2 { get; set; }

        public string BillingAddressLine3 { get; set; }

        public string BillingAddressLine4 { get; set; }

        public string BillingAddressLine5 { get; set; }

        public string BillingCompany { get; set; }

        public string BillingEmail { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

        public string BillingMobile { get; set; }

        public string BillingPhoneNumber { get; set; }

        public string BillingPhoneType { get; set; }

        public string BillStartDate { get; set; }

        public string CaptureTransactionId { get; set; }

        public string CaptureTransactionResult { get; set; }

        public string CardAlias { get; set; }

        public string CardName { get; set; }

        public string CardNumber { get; set; }

        public string CardType { get; set; }

        public string Comment { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CurrencyCode { get; set; }

        public bool Deleted { get; set; }

        public Guid InvoiceId { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceType { get; set; }

        public string MaskedCardNumber { get; set; }

        public DateTime? PaidOn { get; set; }

        public Guid? ParentId { get; set; }

        public string PaymentDueDate { get; set; }

        public string PlanDescription { get; set; }

        public string PlanEndDate { get; set; }

        public int PlanFee { get; set; }

        public Guid PlanId { get; set; }

        public string PlanStartDate { get; set; }

        public string PlanType { get; set; }

        public bool Published { get; set; }

        public Guid SalonId { get; set; }

        public string Status { get; set; }

        public int SubtotalExclTax { get; set; }

        public string TaxRate { get; set; }

        public int TotalAdjustment { get; set; }

        public int TotalAmountDue { get; set; }

        public int TotalExclTax { get; set; }

        public int TotalInclTax { get; set; }

        public int TotalOverdue { get; set; }

        public int TotalPaid { get; set; }

        public int TotalPlan { get; set; }

        public int TotalTax { get; set; }

        public int TotalWidget { get; set; }

        public int TotalWidgetCount { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string VATNumber { get; set; }
    }
}

