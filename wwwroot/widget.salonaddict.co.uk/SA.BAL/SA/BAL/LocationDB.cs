namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class LocationDB : BaseEntity
    {
        public SA.BAL.LocationType GetLocationTypeEnum()
        {
            if (this.LocationType == 0)
            {
                throw new InvalidOperationException();
            }
            return (SA.BAL.LocationType) this.LocationType;
        }

        public bool Active { get; set; }

        public Guid? CityTown { get; set; }

        public Guid? Country { get; set; }

        public Guid? County { get; set; }

        public bool Deleted { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public string Latitude { get; set; }

        public Guid LocationId { get; set; }

        public int LocationType { get; set; }

        public string Longitude { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string Name { get; set; }

        public string SEName { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }

        public Guid? StateProvince { get; set; }
    }
}

