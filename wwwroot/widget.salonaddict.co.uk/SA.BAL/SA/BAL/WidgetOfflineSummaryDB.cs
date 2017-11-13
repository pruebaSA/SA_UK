namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class WidgetOfflineSummaryDB : BaseEntity
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public Guid SalonId { get; set; }
    }
}

