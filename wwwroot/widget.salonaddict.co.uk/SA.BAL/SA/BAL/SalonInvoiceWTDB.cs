namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonInvoiceWTDB : BaseEntity
    {
        public string AppointmentDate { get; set; }

        public Guid AppointmentId { get; set; }

        public string AppointmentTime { get; set; }

        public string FirstName { get; set; }

        public Guid InvoiceId { get; set; }

        public Guid InvoiceItemId { get; set; }

        public int ItemPrice { get; set; }

        public string LastName { get; set; }

        public string ServiceName { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}

