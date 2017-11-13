namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class LocationSalonMappingDB : BaseEntity
    {
        public Guid LocationId { get; set; }

        public Guid MappingId { get; set; }

        public Guid SalonId { get; set; }
    }
}

