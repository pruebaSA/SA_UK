namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class TimeBlockDB : BaseEntity
    {
        public Guid BlockId { get; set; }

        public TimeBlockTypeEnum BlockType =>
            ((TimeBlockTypeEnum) this.BlockTypeId);

        public int BlockTypeId { get; set; }

        public int CycleLength { get; set; }

        public int CyclePeriodType { get; set; }

        public SA.BAL.CyclePeriodTypeEnum CyclePeriodTypeEnum =>
            ((SA.BAL.CyclePeriodTypeEnum) this.CyclePeriodType);

        public DateTime Date =>
            this.StartDateUtc;

        public string EmployeeDisplayText { get; set; }

        public Guid? EmployeeId { get; set; }

        public DateTime? EndDateUtc { get; set; }

        public TimeSpan EndTime { get; set; }

        public Guid SalonId { get; set; }

        public string ServiceDisplayText { get; set; }

        public Guid? ServiceId { get; set; }

        public int? Slots { get; set; }

        public DateTime StartDateUtc { get; set; }

        public TimeSpan StartTime { get; set; }

        public Guid? TicketId { get; set; }

        public int? TotalCycles { get; set; }

        public int? WeekDay { get; set; }
    }
}

