namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xml;

    internal class InternalRelationshipCollection : IEnumerable<PackageRelationship>, IEnumerable
    {
        private bool _dirty;
        private Package _package;
        private PackagePart _relationshipPart;
        private List<PackageRelationship> _relationships;
        private PackagePart _sourcePart;
        private XmlWriter _streamingXmlWriter;
        private static readonly int _timestampLength = 0x10;
        private Uri _uri;
        private static readonly string IdAttributeName = "Id";
        private static readonly string[] RelationshipKnownNamespaces = new string[] { PackagingUtilities.RelationshipNamespaceUri };
        private static readonly string RelationshipsTagName = "Relationships";
        private static readonly string RelationshipTagName = "Relationship";
        private static readonly string TargetAttributeName = "Target";
        private static readonly string TargetModeAttributeName = "TargetMode";
        private static readonly string TypeAttributeName = "Type";
        private static readonly string XmlBaseAttributeName = "xml:base";

        internal InternalRelationshipCollection(Package package) : this(package, null)
        {
        }

        internal InternalRelationshipCollection(PackagePart part) : this(part.Package, part)
        {
        }

        private InternalRelationshipCollection(Package package, PackagePart part)
        {
            this._package = package;
            this._sourcePart = part;
            this._uri = GetRelationshipPartUri(this._sourcePart);
            this._relationships = new List<PackageRelationship>(4);
            if ((package.FileOpenAccess != FileAccess.Write) && package.PartExists(this._uri))
            {
                this._relationshipPart = package.GetPart(this._uri);
                this.ThrowIfIncorrectContentType(this._relationshipPart.ValidatedContentType);
                this.ParseRelationshipPart(this._relationshipPart);
            }
            this._dirty = false;
        }

        internal PackageRelationship Add(Uri targetUri, TargetMode targetMode, string relationshipType, string id) => 
            this.Add(targetUri, targetMode, relationshipType, id, false);

        private PackageRelationship Add(Uri targetUri, TargetMode targetMode, string relationshipType, string id, bool parsing)
        {
            if (targetUri == null)
            {
                throw new ArgumentNullException("targetUri");
            }
            if (relationshipType == null)
            {
                throw new ArgumentNullException("relationshipType");
            }
            ThrowIfInvalidRelationshipType(relationshipType);
            if ((targetMode < TargetMode.Internal) || (targetMode > TargetMode.External))
            {
                throw new ArgumentOutOfRangeException("targetMode");
            }
            if ((targetMode == TargetMode.Internal) && targetUri.IsAbsoluteUri)
            {
                throw new ArgumentException(System.Windows.SR.Get("RelationshipTargetMustBeRelative"), "targetUri");
            }
            if ((!targetUri.IsAbsoluteUri && (targetMode != TargetMode.External)) || (targetUri.IsAbsoluteUri && (targetUri.Scheme == PackUriHelper.UriSchemePack)))
            {
                Uri resolvedTargetUri = this.GetResolvedTargetUri(targetUri, targetMode);
                if ((resolvedTargetUri != null) && PackUriHelper.IsRelationshipPartUri(resolvedTargetUri))
                {
                    throw new ArgumentException(System.Windows.SR.Get("RelationshipToRelationshipIllegal"), "targetUri");
                }
            }
            if (id == null)
            {
                id = this.GenerateUniqueRelationshipId();
            }
            else
            {
                this.ValidateUniqueRelationshipId(id);
            }
            this.EnsureRelationshipPart();
            PackageRelationship item = new PackageRelationship(this._package, this._sourcePart, targetUri, targetMode, relationshipType, id);
            this._relationships.Add(item);
            this._dirty = !parsing;
            return item;
        }

        internal void Clear()
        {
            Invariant.Assert(!this._package.InStreamingCreation);
            this._relationships.Clear();
            this._dirty = true;
        }

        internal void CloseInStreamingCreationMode()
        {
            this.FlushRelationshipsToPiece(true);
        }

        internal void Delete(string id)
        {
            Invariant.Assert(!this._package.InStreamingCreation);
            int relationshipIndex = this.GetRelationshipIndex(id);
            if (relationshipIndex != -1)
            {
                this._relationships.RemoveAt(relationshipIndex);
                this._dirty = true;
            }
        }

        private void EnsureRelationshipPart()
        {
            if ((this._relationshipPart == null) || this._relationshipPart.IsDeleted)
            {
                if (!this._package.InStreamingCreation && this._package.PartExists(this._uri))
                {
                    this._relationshipPart = this._package.GetPart(this._uri);
                    this.ThrowIfIncorrectContentType(this._relationshipPart.ValidatedContentType);
                }
                else
                {
                    CompressionOption compressionOption = (this._sourcePart == null) ? CompressionOption.NotCompressed : this._sourcePart.CompressionOption;
                    this._relationshipPart = this._package.CreatePart(this._uri, PackagingUtilities.RelationshipPartContentType.ToString(), compressionOption);
                }
            }
        }

        internal void Flush()
        {
            if (this._dirty)
            {
                if (this._package.InStreamingCreation)
                {
                    this.FlushRelationshipsToPiece(false);
                }
                else if (this._relationships.Count == 0)
                {
                    if (this._package.PartExists(this._uri))
                    {
                        this._package.DeletePart(this._uri);
                    }
                    this._relationshipPart = null;
                }
                else
                {
                    this.EnsureRelationshipPart();
                    this.WriteRelationshipPart(this._relationshipPart);
                }
                this._dirty = false;
            }
        }

        private void FlushRelationshipsToPiece(bool isLastPiece)
        {
            if (this._dirty)
            {
                Invariant.Assert(this._relationships.Count > 0);
                WriteRelationshipsAsXml(this.StreamingXmlWriter, this._relationships, false, true);
                if (!isLastPiece)
                {
                    this.StreamingXmlWriter.Flush();
                }
                this._dirty = false;
            }
            if (isLastPiece && (this.StreamingXmlWriter.WriteState != WriteState.Closed))
            {
                this.StreamingXmlWriter.WriteEndElement();
                this.StreamingXmlWriter.WriteEndDocument();
                this.StreamingXmlWriter.Close();
            }
        }

        private string GenerateRelationshipId() => 
            ("R" + Guid.NewGuid().ToString("N").Substring(0, _timestampLength));

        private string GenerateUniqueRelationshipId()
        {
            string str;
            do
            {
                str = this.GenerateRelationshipId();
            }
            while (!this._package.InStreamingCreation && (this.GetRelationship(str) != null));
            return str;
        }

        public List<PackageRelationship>.Enumerator GetEnumerator() => 
            this._relationships.GetEnumerator();

        internal PackageRelationship GetRelationship(string id)
        {
            Invariant.Assert(!this._package.InStreamingCreation);
            int relationshipIndex = this.GetRelationshipIndex(id);
            if (relationshipIndex == -1)
            {
                return null;
            }
            return this._relationships[relationshipIndex];
        }

        private int GetRelationshipIndex(string id)
        {
            for (int i = 0; i < this._relationships.Count; i++)
            {
                if (string.Equals(this._relationships[i].Id, id, StringComparison.Ordinal))
                {
                    return i;
                }
            }
            return -1;
        }

        private static Uri GetRelationshipPartUri(PackagePart part)
        {
            Uri packageRootUri;
            if (part == null)
            {
                packageRootUri = PackUriHelper.PackageRootUri;
            }
            else
            {
                packageRootUri = part.Uri;
            }
            return PackUriHelper.GetRelationshipPartUri(packageRootUri);
        }

        private Uri GetResolvedTargetUri(Uri target, TargetMode targetMode)
        {
            if (targetMode == TargetMode.Internal)
            {
                if (this._sourcePart == null)
                {
                    return PackUriHelper.ResolvePartUri(PackUriHelper.PackageRootUri, target);
                }
                return PackUriHelper.ResolvePartUri(this._sourcePart.Uri, target);
            }
            if (target.IsAbsoluteUri && (string.CompareOrdinal(target.Scheme, PackUriHelper.UriSchemePack) == 0))
            {
                return PackUriHelper.GetPartUri(target);
            }
            return target;
        }

        private void ParseRelationshipPart(PackagePart part)
        {
            using (Stream stream = part.GetStream(FileMode.Open, FileAccess.Read))
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    reader.ProhibitDtd = true;
                    using (XmlCompatibilityReader reader2 = new XmlCompatibilityReader(reader, RelationshipKnownNamespaces))
                    {
                        PackagingUtilities.PerformInitailReadAndVerifyEncoding(reader);
                        reader2.MoveToContent();
                        if (((reader2.NodeType != XmlNodeType.Element) || (reader2.Depth != 0)) || ((string.CompareOrdinal(RelationshipsTagName, reader2.LocalName) != 0) || (string.CompareOrdinal(PackagingUtilities.RelationshipNamespaceUri, reader2.NamespaceURI) != 0)))
                        {
                            throw new XmlException(System.Windows.SR.Get("ExpectedRelationshipsElementTag"), null, reader2.LineNumber, reader2.LinePosition);
                        }
                        this.ThrowIfXmlBaseAttributeIsPresent(reader2);
                        if (PackagingUtilities.GetNonXmlnsAttributeCount(reader2) <= 0)
                        {
                            goto Label_018B;
                        }
                        throw new XmlException(System.Windows.SR.Get("RelationshipsTagHasExtraAttributes"), null, reader2.LineNumber, reader2.LinePosition);
                    Label_00A8:
                        reader2.MoveToContent();
                        if (reader2.NodeType != XmlNodeType.None)
                        {
                            if (((reader2.NodeType == XmlNodeType.Element) && (reader2.Depth == 1)) && ((string.CompareOrdinal(RelationshipTagName, reader2.LocalName) == 0) && (string.CompareOrdinal(PackagingUtilities.RelationshipNamespaceUri, reader2.NamespaceURI) == 0)))
                            {
                                this.ThrowIfXmlBaseAttributeIsPresent(reader2);
                                int num = 3;
                                if (reader2.GetAttribute(TargetModeAttributeName) != null)
                                {
                                    num++;
                                }
                                if (PackagingUtilities.GetNonXmlnsAttributeCount(reader2) != num)
                                {
                                    throw new XmlException(System.Windows.SR.Get("RelationshipTagDoesntMatchSchema"), null, reader2.LineNumber, reader2.LinePosition);
                                }
                                this.ProcessRelationshipAttributes(reader2);
                                if (!reader2.IsEmptyElement)
                                {
                                    this.ProcessEndElementForRelationshipTag(reader2);
                                }
                            }
                            else if ((string.CompareOrdinal(RelationshipsTagName, reader2.LocalName) != 0) || (reader2.NodeType != XmlNodeType.EndElement))
                            {
                                throw new XmlException(System.Windows.SR.Get("UnknownTagEncountered"), null, reader2.LineNumber, reader2.LinePosition);
                            }
                        }
                    Label_018B:
                        if (reader2.Read())
                        {
                            goto Label_00A8;
                        }
                    }
                }
            }
        }

        private void ProcessEndElementForRelationshipTag(XmlCompatibilityReader reader)
        {
            reader.Read();
            reader.MoveToContent();
            if ((reader.NodeType != XmlNodeType.EndElement) || (string.CompareOrdinal(RelationshipTagName, reader.LocalName) != 0))
            {
                throw new XmlException(System.Windows.SR.Get("ElementIsNotEmptyElement", new object[] { RelationshipTagName }), null, reader.LineNumber, reader.LinePosition);
            }
        }

        private void ProcessRelationshipAttributes(XmlCompatibilityReader reader)
        {
            string attribute = reader.GetAttribute(TargetModeAttributeName);
            TargetMode targetMode = TargetMode.Internal;
            if (attribute != null)
            {
                try
                {
                    targetMode = (TargetMode) Enum.Parse(typeof(TargetMode), attribute, false);
                }
                catch (ArgumentNullException exception)
                {
                    this.ThrowForInvalidAttributeValue(reader, TargetModeAttributeName, exception);
                }
                catch (ArgumentException exception2)
                {
                    this.ThrowForInvalidAttributeValue(reader, TargetModeAttributeName, exception2);
                }
            }
            string uriString = reader.GetAttribute(TargetAttributeName);
            switch (uriString)
            {
                case null:
                case string.Empty:
                    throw new XmlException(System.Windows.SR.Get("RequiredRelationshipAttributeMissing", new object[] { TargetAttributeName }), null, reader.LineNumber, reader.LinePosition);
            }
            Uri targetUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            string relationshipType = reader.GetAttribute(TypeAttributeName);
            switch (relationshipType)
            {
                case null:
                case string.Empty:
                    throw new XmlException(System.Windows.SR.Get("RequiredRelationshipAttributeMissing", new object[] { TypeAttributeName }), null, reader.LineNumber, reader.LinePosition);
            }
            string id = reader.GetAttribute(IdAttributeName);
            switch (id)
            {
                case null:
                case string.Empty:
                    throw new XmlException(System.Windows.SR.Get("RequiredRelationshipAttributeMissing", new object[] { IdAttributeName }), null, reader.LineNumber, reader.LinePosition);
            }
            this.Add(targetUri, targetMode, relationshipType, id, true);
        }

        IEnumerator<PackageRelationship> IEnumerable<PackageRelationship>.GetEnumerator() => 
            this._relationships.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._relationships.GetEnumerator();

        private void ThrowForInvalidAttributeValue(XmlCompatibilityReader reader, string attributeName, Exception ex)
        {
            throw new XmlException(System.Windows.SR.Get("InvalidValueForTheAttribute", new object[] { attributeName }), ex, reader.LineNumber, reader.LinePosition);
        }

        private void ThrowIfIncorrectContentType(ContentType contentType)
        {
            if (!contentType.AreTypeAndSubTypeEqual(PackagingUtilities.RelationshipPartContentType))
            {
                throw new FileFormatException(System.Windows.SR.Get("RelationshipPartIncorrectContentType"));
            }
        }

        internal static void ThrowIfInvalidRelationshipType(string relationshipType)
        {
            if (relationshipType.Trim() == string.Empty)
            {
                throw new ArgumentException(System.Windows.SR.Get("InvalidRelationshipType"));
            }
        }

        internal static void ThrowIfInvalidXsdId(string id)
        {
            Invariant.Assert(id != null, "id should not be null");
            try
            {
                XmlConvert.VerifyNCName(id);
            }
            catch (XmlException exception)
            {
                throw new XmlException(System.Windows.SR.Get("NotAValidXmlIdString", new object[] { id }), exception);
            }
        }

        private void ThrowIfXmlBaseAttributeIsPresent(XmlCompatibilityReader reader)
        {
            if (reader.GetAttribute(XmlBaseAttributeName) != null)
            {
                throw new XmlException(System.Windows.SR.Get("InvalidXmlBaseAttributePresent", new object[] { XmlBaseAttributeName }), null, reader.LineNumber, reader.LinePosition);
            }
        }

        private void ValidateUniqueRelationshipId(string id)
        {
            ThrowIfInvalidXsdId(id);
            if (this.GetRelationshipIndex(id) >= 0)
            {
                throw new XmlException(System.Windows.SR.Get("NotAUniqueRelationshipId", new object[] { id }));
            }
        }

        private void WriteRelationshipPart(PackagePart part)
        {
            using (IgnoreFlushAndCloseStream stream = new IgnoreFlushAndCloseStream(part.GetStream()))
            {
                stream.SetLength(0L);
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(RelationshipsTagName, PackagingUtilities.RelationshipNamespaceUri);
                    WriteRelationshipsAsXml(writer, this._relationships, false, false);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        internal static void WriteRelationshipsAsXml(XmlWriter writer, IEnumerable<PackageRelationship> relationships, bool alwaysWriteTargetModeAttribute, bool inStreamingProduction)
        {
            foreach (PackageRelationship relationship in relationships)
            {
                if (!inStreamingProduction || !relationship.Saved)
                {
                    writer.WriteStartElement(RelationshipTagName);
                    writer.WriteAttributeString(TypeAttributeName, relationship.RelationshipType);
                    writer.WriteAttributeString(TargetAttributeName, relationship.TargetUri.OriginalString);
                    if (alwaysWriteTargetModeAttribute || (relationship.TargetMode == TargetMode.External))
                    {
                        writer.WriteAttributeString(TargetModeAttributeName, relationship.TargetMode.ToString());
                    }
                    writer.WriteAttributeString(IdAttributeName, relationship.Id);
                    writer.WriteEndElement();
                    if (inStreamingProduction)
                    {
                        relationship.Saved = true;
                    }
                }
            }
        }

        private XmlWriter StreamingXmlWriter
        {
            get
            {
                if (this._streamingXmlWriter == null)
                {
                    this.EnsureRelationshipPart();
                    StreamingZipPartStream w = (StreamingZipPartStream) this._relationshipPart.GetStream(FileMode.CreateNew, FileAccess.Write);
                    this._streamingXmlWriter = new XmlTextWriter(w, Encoding.UTF8);
                    this.StreamingXmlWriter.WriteStartDocument();
                    this.StreamingXmlWriter.WriteStartElement(RelationshipsTagName, PackagingUtilities.RelationshipNamespaceUri);
                }
                return this._streamingXmlWriter;
            }
        }
    }
}

