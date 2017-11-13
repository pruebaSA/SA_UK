namespace System.IO.Packaging
{
    using System;
    using System.Collections;
    using System.IO;

    internal interface IDataTransform
    {
        Stream GetTransformedStream(Stream encodedDataStream, IDictionary transformContext);

        bool FixedSettings { get; }

        bool IsReady { get; }

        object TransformIdentifier { get; }
    }
}

