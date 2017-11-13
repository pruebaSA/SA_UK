namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class LocationSalonSummaryDB : BaseEntity
    {
        public Guid LocationId { get; set; }

        public Guid MappingId { get; set; }

        public string Name { get; set; }

        public Guid? ParentLocationId { get; set; }

        public string ParentName { get; set; }
    }
}

