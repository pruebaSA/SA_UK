namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PartManifestEntry
    {
        private System.Uri _owningPartUri;
        private System.Uri _uri;
        private MS.Internal.ContentType _contentType;
        private string _hashAlgorithm;
        private string _hashValue;
        private List<string> _transforms;
        private List<PackageRelationshipSelector> _relationshipSelectors;
        internal bool IsRelationshipEntry =>
            (this._relationshipSelectors != null);
        internal System.Uri Uri =>
            this._uri;
        internal MS.Internal.ContentType ContentType =>
            this._contentType;
        internal string HashAlgorithm =>
            this._hashAlgorithm;
        internal string HashValue =>
            this._hashValue;
        internal List<string> Transforms =>
            this._transforms;
        internal List<PackageRelationshipSelector> RelationshipSelectors =>
            this._relationshipSelectors;
        internal System.Uri OwningPartUri =>
            this._owningPartUri;
        internal PartManifestEntry(System.Uri uri, MS.Internal.ContentType contentType, string hashAlgorithm, string hashValue, List<string> transforms, List<PackageRelationshipSelector> relationshipSelectors)
        {
            Invariant.Assert(uri != null);
            Invariant.Assert(contentType != null);
            Invariant.Assert(hashAlgorithm != null);
            this._uri = uri;
            this._contentType = contentType;
            this._hashAlgorithm = hashAlgorithm;
            this._hashValue = hashValue;
            this._transforms = transforms;
            this._relationshipSelectors = relationshipSelectors;
            this._owningPartUri = null;
            if (this._relationshipSelectors != null)
            {
                Invariant.Assert(relationshipSelectors.Count > 0);
                this._owningPartUri = relationshipSelectors[0].SourceUri;
            }
        }
    }
}

