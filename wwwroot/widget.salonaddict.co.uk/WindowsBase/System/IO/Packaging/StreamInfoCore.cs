namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;

    internal class StreamInfoCore
    {
        internal string dataSpaceLabel;
        internal object exposedStream;
        internal IStream safeIStream;
        internal string streamName;

        internal StreamInfoCore(string nameStream, string label) : this(nameStream, label, null)
        {
        }

        internal StreamInfoCore(string nameStream, string label, IStream s)
        {
            this.streamName = nameStream;
            this.dataSpaceLabel = label;
            this.safeIStream = s;
            this.exposedStream = null;
        }
    }
}

