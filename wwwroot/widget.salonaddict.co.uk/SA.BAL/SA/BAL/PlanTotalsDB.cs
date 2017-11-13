namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class PlanTotalsDB : BaseEntity
    {
        public string PlanType { get; set; }

        public int Total { get; set; }
    }
}

