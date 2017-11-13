namespace System.Deployment.Application.Manifest
{
    using System;
    using System.Deployment.Application;
    using System.Deployment.Internal.Isolation.Manifest;

    internal class Description
    {
        private readonly Uri _errorReportUri;
        private readonly string _filteredProduct;
        private readonly string _filteredPublisher;
        private readonly string _filteredSuiteName;
        private readonly string _iconFile;
        private readonly string _iconFileFS;
        private readonly string _product;
        private readonly string _publisher;
        private readonly string _suiteName;
        private readonly Uri _supportUri;

        public Description(System.Deployment.Internal.Isolation.Manifest.DescriptionMetadataEntry descriptionMetadataEntry)
        {
            this._publisher = descriptionMetadataEntry.Publisher;
            this._product = descriptionMetadataEntry.Product;
            this._suiteName = descriptionMetadataEntry.SuiteName;
            if (this._suiteName == null)
            {
                this._suiteName = "";
            }
            this._supportUri = AssemblyManifest.UriFromMetadataEntry(descriptionMetadataEntry.SupportUrl, "Ex_DescriptionSupportUrlNotValid");
            this._errorReportUri = AssemblyManifest.UriFromMetadataEntry(descriptionMetadataEntry.ErrorReportUrl, "Ex_DescriptionErrorReportUrlNotValid");
            this._iconFile = descriptionMetadataEntry.IconFile;
            if (this._iconFile != null)
            {
                this._iconFileFS = UriHelper.NormalizePathDirectorySeparators(this._iconFile);
            }
            this._filteredPublisher = PathTwiddler.FilterString(this._publisher, ' ', false);
            this._filteredProduct = PathTwiddler.FilterString(this._product, ' ', false);
            this._filteredSuiteName = PathTwiddler.FilterString(this._suiteName, ' ', false);
        }

        public Uri ErrorReportUri =>
            this._errorReportUri;

        public string ErrorReportUrl =>
            this._errorReportUri?.AbsoluteUri;

        public string FilteredProduct =>
            this._filteredProduct;

        public string FilteredPublisher =>
            this._filteredPublisher;

        public string FilteredSuiteName =>
            this._filteredSuiteName;

        public string IconFile =>
            this._iconFile;

        public string IconFileFS =>
            this._iconFileFS;

        public string Product =>
            this._product;

        public string Publisher =>
            this._publisher;

        public Uri SupportUri =>
            this._supportUri;

        public string SupportUrl =>
            this._supportUri?.AbsoluteUri;
    }
}

