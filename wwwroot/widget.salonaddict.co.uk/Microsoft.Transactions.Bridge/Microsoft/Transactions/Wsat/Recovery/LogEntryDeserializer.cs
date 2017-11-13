namespace Microsoft.Transactions.Wsat.Recovery
{
    using System;
    using System.IO;

    internal abstract class LogEntryDeserializer
    {
        protected LogEntry entry;
        protected MemoryStream mem;

        protected LogEntryDeserializer(MemoryStream mem, LogEntry entry)
        {
            this.entry = entry;
            this.mem = mem;
        }

        public LogEntry Deserialize()
        {
            this.DeserializeExtended();
            return this.entry;
        }

        protected abstract void DeserializeExtended();
    }
}

