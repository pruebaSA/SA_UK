namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class WidgetApiKeyDB : BaseEntity
    {
        public bool Active { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public bool Deleted { get; set; }

        public string HttpReferer { get; set; }

        public Guid KeyId { get; set; }

        public Guid SalonId { get; set; }

        public string VerificationToken { get; set; }
    }
}

