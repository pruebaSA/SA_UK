namespace SA.BAL
{
    using System;
    using System.Runtime.CompilerServices;

    public class QueuedMessageDB : BaseEntity
    {
        public string Bcc { get; set; }

        public string Body { get; set; }

        public string Cc { get; set; }

        public DateTime CreatedOn { get; set; }

        public string MessageType { get; set; }

        public Guid QueuedMessageId { get; set; }

        public string Recipient { get; set; }

        public string RecipientDisplayName { get; set; }

        public string Sender { get; set; }

        public string SenderDisplayName { get; set; }

        public int SendTries { get; set; }

        public DateTime? SentOn { get; set; }

        public string Subject { get; set; }
    }
}

