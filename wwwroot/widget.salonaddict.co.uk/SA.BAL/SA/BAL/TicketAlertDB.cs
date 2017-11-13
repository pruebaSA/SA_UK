namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class TicketAlertDB : BaseEntity
    {
        public bool Active { get; set; }

        public Guid AlertId { get; set; }

        public bool ByEmail { get; set; }

        public bool BySMS { get; set; }

        public string DisplayText { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Guid SalonId { get; set; }
    }
}

