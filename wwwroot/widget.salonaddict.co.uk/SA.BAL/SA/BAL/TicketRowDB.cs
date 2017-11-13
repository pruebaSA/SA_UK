namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class TicketRowDB : BaseEntity
    {
        public string EmployeeDisplayText { get; set; }

        public Guid? EmployeeId { get; set; }

        public DateTime? EndDate { get; set; }

        public TimeSpan? EndTime { get; set; }

        public bool MultiDay { get; set; }

        public decimal Price { get; set; }

        public int RowOrder { get; set; }

        public decimal RowTotal { get; set; }

        public string ServiceDisplayText { get; set; }

        public Guid? ServiceId { get; set; }

        public DateTime? StartDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public bool Taxable { get; set; }

        public decimal TaxRate { get; set; }

        public Guid TicketId { get; set; }

        public Guid TicketRowId { get; set; }

        public int TicketRowType { get; set; }

        public SA.BAL.TicketRowTypeEnum TicketRowTypeEnum =>
            ((SA.BAL.TicketRowTypeEnum) this.TicketRowType);

        public decimal TotalTax { get; set; }
    }
}

