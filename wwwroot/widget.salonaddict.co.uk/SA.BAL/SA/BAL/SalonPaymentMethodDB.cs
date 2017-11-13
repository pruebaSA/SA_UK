namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonPaymentMethodDB : BaseEntity
    {
        public bool Active { get; set; }

        public string Alias { get; set; }

        public string CardCvv2 { get; set; }

        public string CardExpirationMonth { get; set; }

        public string CardExpirationYear { get; set; }

        public string CardName { get; set; }

        public string CardNumber { get; set; }

        public string CardType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsPrimary { get; set; }

        public string MaskedCardNumber { get; set; }

        public string RealexCardRef { get; set; }

        public string RealexPayerRef { get; set; }

        public Guid SalonId { get; set; }

        public Guid SalonPaymentMethodId { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}

