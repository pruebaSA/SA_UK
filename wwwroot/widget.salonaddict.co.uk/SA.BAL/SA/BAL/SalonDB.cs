namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonDB : BaseEntity
    {
        public string Abbreviation { get; set; }

        public bool Active { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string AddressLine5 { get; set; }

        public string AdminComment { get; set; }

        public bool BookOnMobile { get; set; }

        public bool BookOnWeb { get; set; }

        public bool BookOnWidget { get; set; }

        public string CityTown { get; set; }

        public string Company { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string CurrencyCode { get; set; }

        public bool Deleted { get; set; }

        public string Directions { get; set; }

        public string Email { get; set; }

        public string FaxNumber { get; set; }

        public string FullDescription { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string Mobile { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? PictureId { get; set; }

        public bool Published { get; set; }

        public string RealexPayerRef { get; set; }

        public Guid SalonId { get; set; }

        public string SEName { get; set; }

        public string ShortDescription { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }

        public string StateProvince { get; set; }

        public double TotalRatingSum { get; set; }

        public double TotalRatingVotes { get; set; }

        public int TotalReviews { get; set; }

        public int TotalTicketCount { get; set; }

        public string VatNumber { get; set; }

        public string VatNumberStatus { get; set; }

        public string Website { get; set; }

        public string ZipPostalCode { get; set; }
    }
}

