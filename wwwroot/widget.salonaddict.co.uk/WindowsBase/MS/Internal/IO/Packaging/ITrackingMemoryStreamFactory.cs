namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;

    internal interface ITrackingMemoryStreamFactory
    {
        MemoryStream Create();
        MemoryStream Create(int capacity);
        void ReportMemoryUsageDelta(int delta);
    }
}

