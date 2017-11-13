namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class LogDB : BaseEntity
    {
        public DateTime CreatedOn { get; set; }

        public string Exception { get; set; }

        public Guid LogId { get; set; }

        public string LogType { get; set; }

        public string Message { get; set; }

        public string PageURL { get; set; }

        public string Params { get; set; }

        public string ReferrerURL { get; set; }

        public string UserHostAddress { get; set; }

        public Guid? UserId { get; set; }
    }
}

