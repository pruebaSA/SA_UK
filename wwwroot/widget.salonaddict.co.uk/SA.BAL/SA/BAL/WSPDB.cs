namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class WSPDB : BaseEntity
    {
        public bool Active { get; set; }

        public string BillEndDate { get; set; }

        public string BillStartDate { get; set; }

        public string CancelDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Description { get; set; }

        public int ExcessFeeWT { get; set; }

        public int ExcessLimitWT { get; set; }

        public Guid? ParentId { get; set; }

        public string PlanEndDate { get; set; }

        public Guid PlanId { get; set; }

        public int PlanPrice { get; set; }

        public string PlanStartDate { get; set; }

        public string PlanType { get; set; }

        public bool Prorate { get; set; }

        public Guid SalonId { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}

