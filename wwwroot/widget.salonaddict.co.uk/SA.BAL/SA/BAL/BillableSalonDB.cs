namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class BillableSalonDB : BaseEntity
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public string BillEndDate { get; set; }

        public string Name { get; set; }

        public Guid SalonId { get; set; }
    }
}

