namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class BillingTotalsDB : BaseEntity
    {
        public int CountThisMonth { get; set; }

        public int CountThisWeek { get; set; }

        public int CountThisYear { get; set; }

        public int CountToday { get; set; }

        public int SumThisMonth { get; set; }

        public int SumThisWeek { get; set; }

        public int SumThisYear { get; set; }

        public int SumToday { get; set; }
    }
}

