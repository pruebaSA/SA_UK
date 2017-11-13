namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class WSPUpgradeXML : BaseEntity
    {
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

        public string BillingPhoneNumberType { get; set; }

        public string BillStartDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CurrencyCode { get; set; }

        public int ExcessFeeWT { get; set; }

        public int ExcessLimitWT { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceType { get; set; }

        public string PaymentDueDate { get; set; }

        public string PlanDescription { get; set; }

        public string PlanEndDate { get; set; }

        public int PlanFee { get; set; }

        public Guid? PlanParentId { get; set; }

        public string PlanStartDate { get; set; }

        public string PlanType { get; set; }

        public Guid SalonId { get; set; }

        public string Status { get; set; }

        public int SubtotalExclTax { get; set; }

        public string TaxRate { get; set; }

        public int TotalAmountDue { get; set; }

        public int TotalExclTax { get; set; }

        public int TotalInclTax { get; set; }

        public int TotalOverdue { get; set; }

        public int TotalPlan { get; set; }

        public int TotalTax { get; set; }

        public string VATNumber { get; set; }
    }
}

