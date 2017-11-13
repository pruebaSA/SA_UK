namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class WSPExpiringDB
    {
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public string Name { get; set; }

        public string PlanDescription { get; set; }

        public string PlanEndDate { get; set; }

        public Guid PlanId { get; set; }

        public int PlanPrice { get; set; }

        public string PlanStartDate { get; set; }

        public string PlanType { get; set; }

        public Guid SalonId { get; set; }
    }
}

