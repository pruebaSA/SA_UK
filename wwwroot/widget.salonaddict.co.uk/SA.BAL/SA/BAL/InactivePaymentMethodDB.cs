namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class InactivePaymentMethodDB : BaseEntity
    {
        public string Alias { get; set; }

        public string CardExpirationMonth { get; set; }

        public string CardExpirationYear { get; set; }

        public string MaskedCardNumber { get; set; }

        public Guid SalonId { get; set; }

        public string SalonName { get; set; }

        public Guid SalonPaymentMethodId { get; set; }
    }
}

