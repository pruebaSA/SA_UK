namespace Microsoft.Transactions.Wsat.Recovery
{
    using System;
    using System.IO;

    internal abstract class LogEntryHeaderDeserializer
    {
        protected MemoryStream mem;

        protected LogEntryHeaderDeserializer(MemoryStream mem)
        {
            this.mem = mem;
        }

        public abstract LogEntry DeserializeHeader();
    }
}

