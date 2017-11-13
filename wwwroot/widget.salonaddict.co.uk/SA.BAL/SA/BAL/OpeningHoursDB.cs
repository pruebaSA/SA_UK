namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class OpeningHoursDB : BaseEntity
    {
        public bool FriClosed { get; set; }

        public TimeSpan? FriEnd1 { get; set; }

        public TimeSpan? FriEnd2 { get; set; }

        public TimeSpan? FriStart1 { get; set; }

        public TimeSpan? FriStart2 { get; set; }

        public bool MonClosed { get; set; }

        public TimeSpan? MonEnd1 { get; set; }

        public TimeSpan? MonEnd2 { get; set; }

        public TimeSpan? MonStart1 { get; set; }

        public TimeSpan? MonStart2 { get; set; }

        public Guid OpeningHoursId { get; set; }

        public Guid SalonId { get; set; }

        public bool SatClosed { get; set; }

        public TimeSpan? SatEnd1 { get; set; }

        public TimeSpan? SatEnd2 { get; set; }

        public TimeSpan? SatStart1 { get; set; }

        public TimeSpan? SatStart2 { get; set; }

        public bool ShowOnMobile { get; set; }

        public bool ShowOnWeb { get; set; }

        public bool ShowOnWidget { get; set; }

        public bool SunClosed { get; set; }

        public TimeSpan? SunEnd1 { get; set; }

        public TimeSpan? SunEnd2 { get; set; }

        public TimeSpan? SunStart1 { get; set; }

        public TimeSpan? SunStart2 { get; set; }

        public bool ThuClosed { get; set; }

        public TimeSpan? ThuEnd1 { get; set; }

        public TimeSpan? ThuEnd2 { get; set; }

        public TimeSpan? ThuStart1 { get; set; }

        public TimeSpan? ThuStart2 { get; set; }

        public bool TueClosed { get; set; }

        public TimeSpan? TueEnd1 { get; set; }

        public TimeSpan? TueEnd2 { get; set; }

        public TimeSpan? TueStart1 { get; set; }

        public TimeSpan? TueStart2 { get; set; }

        public bool WedClosed { get; set; }

        public TimeSpan? WedEnd1 { get; set; }

        public TimeSpan? WedEnd2 { get; set; }

        public TimeSpan? WedStart1 { get; set; }

        public TimeSpan? WedStart2 { get; set; }
    }
}

