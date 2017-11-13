namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class ServiceDB : BaseEntity
    {
        public bool Active { get; set; }

        public bool BookOnMobile { get; set; }

        public bool BookOnWeb { get; set; }

        public bool BookOnWidget { get; set; }

        public Guid? CategoryId1 { get; set; }

        public Guid? CategoryId2 { get; set; }

        public Guid? CategoryId3 { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string CurrencyCode { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }

        public int Duration { get; set; }

        public string FullDescription { get; set; }

        public bool IsTaxExempt { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string Name { get; set; }

        public decimal OldPrice { get; set; }

        public Guid? PictureId { get; set; }

        public decimal Price { get; set; }

        public Guid SalonId { get; set; }

        public string SEName { get; set; }

        public Guid ServiceId { get; set; }

        public string ShortDescription { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }
    }
}

