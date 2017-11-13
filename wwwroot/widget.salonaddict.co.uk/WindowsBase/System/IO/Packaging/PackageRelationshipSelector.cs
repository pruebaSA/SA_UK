namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections.Generic;

    public sealed class PackageRelationshipSelector
    {
        private string _selectionCriteria;
        private PackageRelationshipSelectorType _selectorType;
        private Uri _sourceUri;

        public PackageRelationshipSelector(Uri sourceUri, PackageRelationshipSelectorType selectorType, string selectionCriteria)
        {
            if (sourceUri == null)
            {
                throw new ArgumentNullException("sourceUri");
            }
            if (selectionCriteria == null)
            {
                throw new ArgumentNullException("selectionCriteria");
            }
            if (Uri.Compare(sourceUri, PackUriHelper.PackageRootUri, UriComponents.SerializationInfoString, UriFormat.UriEscaped, StringComparison.Ordinal) != 0)
            {
                sourceUri = PackUriHelper.ValidatePartUri(sourceUri);
            }
            if (selectorType == PackageRelationshipSelectorType.Type)
            {
                InternalRelationshipCollection.ThrowIfInvalidRelationshipType(selectionCriteria);
            }
            else
            {
                if (selectorType != PackageRelationshipSelectorType.Id)
                {
                    throw new ArgumentOutOfRangeException("selectorType");
                }
                InternalRelationshipCollection.ThrowIfInvalidXsdId(selectionCriteria);
            }
            this._sourceUri = sourceUri;
            this._selectionCriteria = selectionCriteria;
            this._selectorType = selectorType;
        }

        public List<PackageRelationship> Select(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            List<PackageRelationship> list = new List<PackageRelationship>(0);
            switch (this.SelectorType)
            {
                case PackageRelationshipSelectorType.Id:
                    if (!this.SourceUri.Equals(PackUriHelper.PackageRootUri))
                    {
                        if (package.PartExists(this.SourceUri))
                        {
                            PackagePart part = package.GetPart(this.SourceUri);
                            if (part.RelationshipExists(this.SelectionCriteria))
                            {
                                list.Add(part.GetRelationship(this.SelectionCriteria));
                            }
                        }
                        return list;
                    }
                    if (package.RelationshipExists(this.SelectionCriteria))
                    {
                        list.Add(package.GetRelationship(this.SelectionCriteria));
                    }
                    return list;

                case PackageRelationshipSelectorType.Type:
                    if (this.SourceUri.Equals(PackUriHelper.PackageRootUri))
                    {
                        foreach (PackageRelationship relationship in package.GetRelationshipsByType(this.SelectionCriteria))
                        {
                            list.Add(relationship);
                        }
                        return list;
                    }
                    if (package.PartExists(this.SourceUri))
                    {
                        foreach (PackageRelationship relationship2 in package.GetPart(this.SourceUri).GetRelationshipsByType(this.SelectionCriteria))
                        {
                            list.Add(relationship2);
                        }
                    }
                    return list;
            }
            return list;
        }

        public string SelectionCriteria =>
            this._selectionCriteria;

        public PackageRelationshipSelectorType SelectorType =>
            this._selectorType;

        public Uri SourceUri =>
            this._sourceUri;
    }
}

