namespace Microsoft.Transactions.Wsat.Recovery
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.IO;

    internal class LogEntryHeaderSerializer
    {
        private LogEntry logEntry;

        public LogEntryHeaderSerializer(LogEntry logEntry)
        {
            this.logEntry = logEntry;
        }

        public MemoryStream WriteHeader()
        {
            MemoryStream mem = new MemoryStream();
            mem.WriteByte(1);
            if (CoordinationContext.IsNativeIdentifier(this.logEntry.RemoteTransactionId, this.logEntry.LocalTransactionId))
            {
                mem.WriteByte(1);
            }
            else
            {
                mem.WriteByte(0);
                SerializationUtils.WriteString(mem, this.logEntry.RemoteTransactionId);
            }
            Guid localEnlistmentId = this.logEntry.LocalEnlistmentId;
            SerializationUtils.WriteGuid(mem, ref localEnlistmentId);
            return mem;
        }
    }
}

