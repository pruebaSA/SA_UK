namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class SalonReviewDB : BaseEntity
    {
        public string AppointmentDate { get; set; }

        public Guid? AppointmentId { get; set; }

        public string AppointmentTime { get; set; }

        public string ApproverDate { get; set; }

        public string ApproverDescription { get; set; }

        public string ApproverTime { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string EmployeeDescription { get; set; }

        public Guid? EmployeeId { get; set; }

        public float EmployeeRating { get; set; }

        public float EmployeeRatingSum { get; set; }

        public float EmployeeRatingTotal { get; set; }

        public bool GoAgain { get; set; }

        public string ReviewerDate { get; set; }

        public string ReviewerDescription { get; set; }

        public string ReviewerTime { get; set; }

        public string SalonDescription { get; set; }

        public Guid SalonId { get; set; }

        public float SalonRating { get; set; }

        public float SalonRatingSum { get; set; }

        public float SalonRatingTotal { get; set; }

        public Guid SalonReviewId { get; set; }

        public float SatisfactionRating { get; set; }

        public float SatisfactionRatingSum { get; set; }

        public float SatisfactionRatingTotal { get; set; }

        public string ServiceDescription { get; set; }

        public Guid? ServiceId { get; set; }

        public float ServiceRating { get; set; }

        public float ServiceRatingSum { get; set; }

        public float ServiceRatingTotal { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UserDescription { get; set; }

        public Guid? UserId { get; set; }

        public float ValueRating { get; set; }

        public float ValueRatingSum { get; set; }

        public float ValueRatingTotal { get; set; }

        public float WeightedRating { get; set; }

        public float WeightedSum { get; set; }

        public float WeightedTotal { get; set; }
    }
}

