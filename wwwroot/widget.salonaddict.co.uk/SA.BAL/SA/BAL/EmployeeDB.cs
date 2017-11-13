namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class EmployeeDB : BaseEntity
    {
        public bool BookOnMobile { get; set; }

        public bool BookOnWeb { get; set; }

        public bool BookOnWidget { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? DateOfHire { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }

        public string DisplayText { get; set; }

        public string Email { get; set; }

        public Guid EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaTitle { get; set; }

        public string Mobile { get; set; }

        public string MobilePIN { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? PictureId { get; set; }

        public Guid SalonId { get; set; }

        public string SEName { get; set; }

        public string Sex { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }

        public int TotalRatingSum { get; set; }

        public int TotalRatingVotes { get; set; }

        public int TotalReviews { get; set; }

        public string VatNumber { get; set; }

        public string VatNumberStatus { get; set; }
    }
}

