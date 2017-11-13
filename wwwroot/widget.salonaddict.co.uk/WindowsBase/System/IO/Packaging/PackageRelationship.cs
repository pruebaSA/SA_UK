namespace System.IO.Packaging
{
    using System;

    public class PackageRelationship
    {
        private static readonly Uri _containerRelationshipPartName = PackUriHelper.CreatePartUri(new Uri("/_rels/.rels", UriKind.Relative));
        private string _id;
        private System.IO.Packaging.Package _package;
        private string _relationshipType;
        private bool _saved;
        private PackagePart _source;
        private System.IO.Packaging.TargetMode _targetMode;
        private Uri _targetUri;

        internal PackageRelationship(System.IO.Packaging.Package package, PackagePart sourcePart, Uri targetUri, System.IO.Packaging.TargetMode targetMode, string relationshipType, string id)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            if (targetUri == null)
            {
                throw new ArgumentNullException("targetUri");
            }
            if (relationshipType == null)
            {
                throw new ArgumentNullException("relationshipType");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            this._package = package;
            this._source = sourcePart;
            this._targetUri = targetUri;
            this._relationshipType = relationshipType;
            this._targetMode = targetMode;
            this._id = id;
        }

        internal static Uri ContainerRelationshipPartName =>
            _containerRelationshipPartName;

        public string Id =>
            this._id;

        public System.IO.Packaging.Package Package =>
            this._package;

        public string RelationshipType =>
            this._relationshipType;

        internal bool Saved
        {
            get => 
                this._saved;
            set
            {
                this._saved = value;
            }
        }

        public Uri SourceUri =>
            this._source?.Uri;

        public System.IO.Packaging.TargetMode TargetMode =>
            this._targetMode;

        public Uri TargetUri =>
            this._targetUri;
    }
}

