namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class Employee_Service_MappingDB : BaseEntity
    {
        public int Duration { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid MappingId { get; set; }

        public decimal Price { get; set; }

        public Guid ServiceId { get; set; }
    }
}

