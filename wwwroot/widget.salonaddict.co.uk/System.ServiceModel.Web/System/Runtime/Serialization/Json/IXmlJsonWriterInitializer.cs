namespace System.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using System.Text;

    public interface IXmlJsonWriterInitializer
    {
        void SetOutput(Stream stream, Encoding encoding, bool ownsStream);
    }
}

