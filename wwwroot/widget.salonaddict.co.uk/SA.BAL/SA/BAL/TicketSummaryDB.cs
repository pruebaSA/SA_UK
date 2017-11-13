namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class TicketSummaryDB : BaseEntity
    {
        public string AdminComment { get; set; }

        public string AuthorizationTransactionCode { get; set; }

        public string AuthorizationTransactionId { get; set; }

        public string AuthorizationTransactionResult { get; set; }

        public string BillingDisplayText { get; set; }

        public string BillingEmail { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

        public string BillingMobile { get; set; }

        public string BillingPhone { get; set; }

        public bool BookedOnMobile { get; set; }

        public bool BookedOnWeb { get; set; }

        public bool BookedOnWidget { get; set; }

        public DateTime? CancelledOnUtc { get; set; }

        public string CaptureTransactionId { get; set; }

        public string CaptureTransactionResult { get; set; }

        public int? CardExpirationMonth { get; set; }

        public int? CardExpirationYear { get; set; }

        public string CardName { get; set; }

        public string CardNumberMasked { get; set; }

        public string CardType { get; set; }

        public DateTime? ClosedOnUtc { get; set; }

        public bool Confirmed { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string CurrencyCode { get; set; }

        public decimal CurrencyRate { get; set; }

        public string CustomerDisplayText { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerIPAddress { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerMobile { get; set; }

        public string CustomerPhone { get; set; }

        public DateTime? CustomerReminder { get; set; }

        public string CustomerSpecialRequest { get; set; }

        public bool Deleted { get; set; }

        public decimal DepositRate { get; set; }

        public bool DepositRequired { get; set; }

        public string Language { get; set; }

        public DateTime? OpenedOnUtc { get; set; }

        public string OpenUserDisplayText { get; set; }

        public Guid? OpenUserId { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal PaidAmountInCustomerCurrency { get; set; }

        public DateTime? PaidDateUtc { get; set; }

        public bool? RepeatCustomer { get; set; }

        public string SalonDisplayText { get; set; }

        public Guid SalonId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal TaxRate { get; set; }

        public Guid TicketId { get; set; }

        public string TicketNumber { get; set; }

        public int? TicketStatusType { get; set; }

        public SA.BAL.TicketStatusTypeEnum TicketStatusTypeEnum
        {
            get
            {
                if (!this.TicketStatusType.HasValue)
                {
                    return SA.BAL.TicketStatusTypeEnum.None;
                }
                return this.TicketStatusType.Value;
            }
        }

        public decimal Total { get; set; }

        public decimal TotalTax { get; set; }

        public Guid? UserId { get; set; }

        public DateTime? VoidedOnUtc { get; set; }

        public string VoidUserDisplayText { get; set; }

        public Guid? VoidUserId { get; set; }
    }
}

