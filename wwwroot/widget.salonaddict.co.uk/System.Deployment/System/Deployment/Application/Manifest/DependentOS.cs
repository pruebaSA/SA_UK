namespace System.Deployment.Application.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation.Manifest;

    internal class DependentOS
    {
        private readonly ushort _buildNumber;
        private readonly ushort _majorVersion;
        private readonly ushort _minorVersion;
        private readonly byte _servicePackMajor;
        private readonly byte _servicePackMinor;
        private readonly Uri _supportUrl;

        public DependentOS(System.Deployment.Internal.Isolation.Manifest.DependentOSMetadataEntry dependentOSMetadataEntry)
        {
            this._majorVersion = dependentOSMetadataEntry.MajorVersion;
            this._minorVersion = dependentOSMetadataEntry.MinorVersion;
            this._buildNumber = dependentOSMetadataEntry.BuildNumber;
            this._servicePackMajor = dependentOSMetadataEntry.ServicePackMajor;
            this._servicePackMinor = dependentOSMetadataEntry.ServicePackMinor;
            this._supportUrl = AssemblyManifest.UriFromMetadataEntry(dependentOSMetadataEntry.SupportUrl, "Ex_DependentOSSupportUrlNotValid");
        }

        public ushort BuildNumber =>
            this._buildNumber;

        public ushort MajorVersion =>
            this._majorVersion;

        public ushort MinorVersion =>
            this._minorVersion;

        public byte ServicePackMajor =>
            this._servicePackMajor;

        public byte ServicePackMinor =>
            this._servicePackMinor;

        public Uri SupportUrl =>
            this._supportUrl;
    }
}

