namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class OpenTicketDB : BaseEntity
    {
        public string CustomerDisplayText { get; set; }

        public DateTime? OpenedOnUtc { get; set; }

        public string OpenUserDisplayText { get; set; }

        public Guid? OpenUserId { get; set; }

        public decimal RowTotal { get; set; }

        public Guid SalonId { get; set; }

        public string ServiceDisplayText { get; set; }

        public DateTime? StartDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public Guid TicketId { get; set; }
    }
}

