namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class BookingTotalsDB : BaseEntity
    {
        public int CountThisMonth { get; set; }

        public int CountThisWeek { get; set; }

        public int CountThisYear { get; set; }

        public int CountToday { get; set; }

        public decimal SumThisMonth { get; set; }

        public decimal SumThisWeek { get; set; }

        public decimal SumThisYear { get; set; }

        public decimal SumToday { get; set; }
    }
}

