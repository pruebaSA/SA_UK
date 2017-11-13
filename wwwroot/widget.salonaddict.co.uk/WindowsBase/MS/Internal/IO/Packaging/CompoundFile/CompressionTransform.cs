namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections;
    using System.IO;
    using System.IO.Packaging;

    internal class CompressionTransform : IDataTransform
    {
        private static readonly VersionPair _currentFeatureVersion = new VersionPair(1, 0);
        private static readonly string _featureName = "Microsoft.Metadata.CompressionTransform";
        private const long _highWaterMark = 0xa00000L;
        private const long _lowWaterMark = 0x19000L;
        private static readonly VersionPair _minimumReaderVersion = new VersionPair(1, 0);
        private static readonly VersionPair _minimumUpdaterVersion = new VersionPair(1, 0);
        private TransformEnvironment _transformEnvironment;
        private VersionedStreamOwner _versionedStreamOwner;

        public CompressionTransform(TransformEnvironment myEnvironment)
        {
            this._transformEnvironment = myEnvironment;
            this._versionedStreamOwner = new VersionedStreamOwner(this._transformEnvironment.GetPrimaryInstanceData(), new FormatVersion(_featureName, _minimumReaderVersion, _minimumUpdaterVersion, _currentFeatureVersion));
        }

        Stream IDataTransform.GetTransformedStream(Stream encodedStream, IDictionary transformContext)
        {
            Stream tempStream = new SparseMemoryStream(0x19000L, 0xa00000L);
            return new VersionedStream(new CompressEmulationStream(encodedStream, tempStream, 0L, new CompoundFileDeflateTransform()), this._versionedStreamOwner);
        }

        internal static string ClassTransformIdentifier =>
            "{86DE7F2B-DDCE-486d-B016-405BBE82B8BC}";

        public bool FixedSettings =>
            true;

        public bool IsReady =>
            true;

        public object TransformIdentifier =>
            ClassTransformIdentifier;
    }
}

