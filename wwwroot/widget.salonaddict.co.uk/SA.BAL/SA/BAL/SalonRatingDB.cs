namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonRatingDB : BaseEntity
    {
        public float EmployeeOverallRating { get; set; }

        public float EmployeeOverallSum { get; set; }

        public float EmployeeOverallTotal { get; set; }

        public float GoAgainOverallRating { get; set; }

        public float GoAgainOverallSum { get; set; }

        public float GoAgainOverallTotal { get; set; }

        public string LastReviewerDate { get; set; }

        public string LastReviewerDescription { get; set; }

        public string LastReviewerTime { get; set; }

        public int ReviewCount { get; set; }

        public Guid SalonId { get; set; }

        public float SalonOverallRating { get; set; }

        public float SalonOverallSum { get; set; }

        public float SalonOverallTotal { get; set; }

        public Guid SalonRatingId { get; set; }

        public float SatisfactionOverallRating { get; set; }

        public float SatisfactionOverallSum { get; set; }

        public float SatisfactionOverallTotal { get; set; }

        public float ServiceOverallRating { get; set; }

        public float ServiceOverallSum { get; set; }

        public float ServiceOverallTotal { get; set; }

        public float ValueOverallRating { get; set; }

        public float ValueOverallSum { get; set; }

        public float ValueOverallTotal { get; set; }

        public float WeightedOverallRating { get; set; }

        public float WeightedOverallSum { get; set; }

        public float WeightedOverallTotal { get; set; }
    }
}

