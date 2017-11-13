namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class ClosingDayDB : BaseEntity
    {
        public bool Active { get; set; }

        public string Category { get; set; }

        public Guid ClosingDayId { get; set; }

        public int CycleLength { get; set; }

        public int CyclePeriodType { get; set; }

        public SA.BAL.CyclePeriodTypeEnum CyclePeriodTypeEnum =>
            ((SA.BAL.CyclePeriodTypeEnum) this.CyclePeriodType);

        public DateTime Date
        {
            get
            {
                if (this.CyclePeriodTypeEnum != SA.BAL.CyclePeriodTypeEnum.Years)
                {
                    throw new InvalidOperationException("Cycle period is not supported.");
                }
                DateTime startDateUtc = this.StartDateUtc;
                while (DateTime.UtcNow.Year > startDateUtc.Year)
                {
                    startDateUtc = startDateUtc.AddYears(this.CycleLength);
                }
                return startDateUtc.Date;
            }
        }

        public string Description { get; set; }

        public Guid SalonId { get; set; }

        public DateTime StartDateUtc { get; set; }

        public int? TotalCycles { get; set; }
    }
}

