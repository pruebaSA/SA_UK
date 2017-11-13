namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class AtomFeed
    {
        public long? Count { get; set; }

        public IEnumerable<AtomEntry> Entries { get; set; }

        public Uri NextLink { get; set; }
    }
}

