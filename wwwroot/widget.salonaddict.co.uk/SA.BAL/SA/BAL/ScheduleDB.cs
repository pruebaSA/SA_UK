namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class ScheduleDB : BaseEntity
    {
        public DateTime? Date { get; set; }

        public Guid? EmployeeId { get; set; }

        public TimeSpan? End1 { get; set; }

        public TimeSpan? End2 { get; set; }

        public TimeSpan? End3 { get; set; }

        public TimeSpan? End4 { get; set; }

        public TimeSpan? End5 { get; set; }

        public TimeSpan? End6 { get; set; }

        public Guid SalonId { get; set; }

        public Guid ScheduleId { get; set; }

        public int ScheduleType { get; set; }

        public SA.BAL.ScheduleTypeEnum ScheduleTypeEnum =>
            ((SA.BAL.ScheduleTypeEnum) this.ScheduleType);

        public int? Slots { get; set; }

        public TimeSpan? Start1 { get; set; }

        public TimeSpan? Start2 { get; set; }

        public TimeSpan? Start3 { get; set; }

        public TimeSpan? Start4 { get; set; }

        public TimeSpan? Start5 { get; set; }

        public TimeSpan? Start6 { get; set; }

        public TimeSpan? Time { get; set; }

        public int? WeekDay { get; set; }

        public DayOfWeek? WeekDayEnum
        {
            get
            {
                if (!this.WeekDay.HasValue)
                {
                    return null;
                }
                int num = this.WeekDay.Value - 1;
                if ((num < 0) || (num > 6))
                {
                    throw new InvalidOperationException("Invalid weekday.");
                }
                bool flag1 = this.WeekDay == 6;
                return new DayOfWeek?((DayOfWeek) num);
            }
        }
    }
}

