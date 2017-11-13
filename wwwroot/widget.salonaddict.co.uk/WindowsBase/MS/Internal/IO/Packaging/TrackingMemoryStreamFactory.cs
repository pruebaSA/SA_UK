namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;

    internal class TrackingMemoryStreamFactory : ITrackingMemoryStreamFactory
    {
        private long _bufferedMemoryConsumption;

        public MemoryStream Create() => 
            new TrackingMemoryStream(this);

        public MemoryStream Create(int capacity) => 
            new TrackingMemoryStream(this, capacity);

        public void ReportMemoryUsageDelta(int delta)
        {
            this._bufferedMemoryConsumption += delta;
        }

        internal long CurrentMemoryConsumption =>
            this._bufferedMemoryConsumption;
    }
}

