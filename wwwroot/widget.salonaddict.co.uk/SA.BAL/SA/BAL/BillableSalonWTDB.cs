namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class BillableSalonWTDB : BaseEntity
    {
        public DateTime AppointmentDate { get; set; }

        public Guid AppointmentId { get; set; }

        public TimeSpan AppointmentTime { get; set; }

        public string FirstName { get; set; }

        public int ItemPrice { get; set; }

        public string LastName { get; set; }

        public string ServiceName { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}

