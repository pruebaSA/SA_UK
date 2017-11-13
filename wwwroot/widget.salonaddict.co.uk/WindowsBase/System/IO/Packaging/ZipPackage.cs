namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using MS.Internal.IO.Zip;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows;
    using System.Xml;

    public sealed class ZipPackage : Package
    {
        private Stream _containerStream;
        private ContentTypeHelper _contentTypeHelper;
        private static readonly ExtensionEqualityComparer _extensionEqualityComparer = new ExtensionEqualityComparer();
        private static readonly string _forwardSlash = "/";
        private IgnoredItemHelper _ignoredItemHelper;
        private const int _initialPartListSize = 50;
        private const int _initialPieceNameListSize = 50;
        private ZipArchive _zipArchive;

        internal ZipPackage(Stream s, FileMode mode, FileAccess access, bool streaming) : base(access, streaming)
        {
            this._containerStream = s;
            this._zipArchive = ZipArchive.OpenOnStream(s, mode, access, streaming);
            this._ignoredItemHelper = new IgnoredItemHelper(this._zipArchive);
            this._contentTypeHelper = new ContentTypeHelper(this._zipArchive, this._ignoredItemHelper);
        }

        internal ZipPackage(string path, FileMode mode, FileAccess access, FileShare share, bool streaming) : base(access, streaming)
        {
            this._containerStream = null;
            this._zipArchive = ZipArchive.OpenOnFile(path, mode, access, share, streaming);
            this._ignoredItemHelper = new IgnoredItemHelper(this._zipArchive);
            this._contentTypeHelper = new ContentTypeHelper(this._zipArchive, this._ignoredItemHelper);
        }

        protected override PackagePart CreatePartCore(Uri partUri, string contentType, CompressionOption compressionOption)
        {
            CompressionMethodEnum enum2;
            DeflateOptionEnum enum3;
            partUri = PackUriHelper.ValidatePartUri(partUri);
            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }
            Package.ThrowIfCompressionOptionInvalid(compressionOption);
            GetZipCompressionMethodFromOpcCompressionOption(compressionOption, out enum2, out enum3);
            if (base.InStreamingCreation)
            {
                this._contentTypeHelper.AddContentType((PackUriHelper.ValidatedPartUri) partUri, new ContentType(contentType), enum2, enum3, true);
                return new ZipPackagePart(this, this._zipArchive, (PackUriHelper.ValidatedPartUri) partUri, contentType, compressionOption);
            }
            this._ignoredItemHelper.Delete((PackUriHelper.ValidatedPartUri) partUri);
            string zipFileName = ((PackUriHelper.ValidatedPartUri) partUri).PartUriString.Substring(1);
            ZipFileInfo zipFileInfo = this._zipArchive.AddFile(zipFileName, enum2, enum3);
            this._contentTypeHelper.AddContentType((PackUriHelper.ValidatedPartUri) partUri, new ContentType(contentType), enum2, enum3, false);
            return new ZipPackagePart(this, zipFileInfo, (PackUriHelper.ValidatedPartUri) partUri, contentType, compressionOption);
        }

        private static void DeleteInterleavedPartOrStream(List<PieceInfo> sortedPieceInfoList)
        {
            if (sortedPieceInfoList.Count > 0)
            {
                ZipArchive zipArchive = sortedPieceInfoList[0].ZipFileInfo.ZipArchive;
                foreach (PieceInfo info in sortedPieceInfoList)
                {
                    zipArchive.DeleteFile(info.ZipFileInfo.Name);
                }
            }
        }

        protected override void DeletePartCore(Uri partUri)
        {
            partUri = PackUriHelper.ValidatePartUri(partUri);
            string zipItemNameFromOpcName = GetZipItemNameFromOpcName(PackUriHelper.GetStringForPartUri(partUri));
            if (this._zipArchive.FileExists(zipItemNameFromOpcName))
            {
                this._zipArchive.DeleteFile(zipItemNameFromOpcName);
            }
            else if (base.PartExists(partUri))
            {
                ZipPackagePart part = base.GetPart(partUri) as ZipPackagePart;
                List<PieceInfo> pieceDescriptors = part.PieceDescriptors;
                if (pieceDescriptors != null)
                {
                    DeleteInterleavedPartOrStream(pieceDescriptors);
                }
            }
            this._ignoredItemHelper.Delete((PackUriHelper.ValidatedPartUri) partUri);
            this._contentTypeHelper.DeleteContentType((PackUriHelper.ValidatedPartUri) partUri);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (base.InStreamingCreation)
                    {
                        this._contentTypeHelper.CloseContentTypes();
                    }
                    else
                    {
                        this._contentTypeHelper.SaveToFile();
                    }
                    this._zipArchive.Close();
                    this._containerStream = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override void FlushCore()
        {
            if (!base.InStreamingCreation)
            {
                this._contentTypeHelper.SaveToFile();
            }
            this._zipArchive.Flush();
        }

        private static CompressionOption GetCompressionOptionFromZipFileInfo(ZipFileInfo zipFileInfo)
        {
            CompressionOption notCompressed = CompressionOption.NotCompressed;
            if (zipFileInfo.CompressionMethod == CompressionMethodEnum.Deflated)
            {
                switch (zipFileInfo.DeflateOption)
                {
                    case DeflateOptionEnum.Normal:
                        return CompressionOption.Normal;

                    case ((DeflateOptionEnum) 1):
                    case ((DeflateOptionEnum) 3):
                    case ((DeflateOptionEnum) 5):
                        return notCompressed;

                    case DeflateOptionEnum.Maximum:
                        return CompressionOption.Maximum;

                    case DeflateOptionEnum.Fast:
                        return CompressionOption.Fast;

                    case DeflateOptionEnum.SuperFast:
                        return CompressionOption.SuperFast;

                    case DeflateOptionEnum.None:
                        return notCompressed;
                }
            }
            return notCompressed;
        }

        internal static string GetOpcNameFromZipItemName(string zipItemName) => 
            (_forwardSlash + zipItemName);

        protected override PackagePart GetPartCore(Uri partUri) => 
            null;

        protected override PackagePart[] GetPartsCore()
        {
            List<PackagePart> parts = new List<PackagePart>(50);
            SortedDictionary<PieceInfo, object> pieceDictionary = new SortedDictionary<PieceInfo, object>(PieceNameHelper.PieceNameComparer);
            foreach (ZipFileInfo info in (IEnumerable) this._zipArchive.GetFiles())
            {
                if (IsValidFileItem(info))
                {
                    if (this.IsZipItemValidOpcPartOrPiece(info.Name))
                    {
                        PieceInfo info2;
                        if (PieceNameHelper.TryCreatePieceInfo(info, out info2))
                        {
                            if (pieceDictionary.ContainsKey(info2))
                            {
                                throw new FormatException(System.Windows.SR.Get("DuplicatePiecesFound"));
                            }
                            if (info2.PartUri != null)
                            {
                                pieceDictionary.Add(info2, null);
                            }
                        }
                        else
                        {
                            PackUriHelper.ValidatedPartUri uri2;
                            Uri partUri = new Uri(GetOpcNameFromZipItemName(info.Name), UriKind.Relative);
                            if (PackUriHelper.TryValidatePartUri(partUri, out uri2))
                            {
                                ContentType contentType = this._contentTypeHelper.GetContentType(uri2);
                                if (contentType != null)
                                {
                                    parts.Add(new ZipPackagePart(this, info, uri2, contentType.ToString(), GetCompressionOptionFromZipFileInfo(info)));
                                }
                                else
                                {
                                    this._ignoredItemHelper.AddItemForAtomicPart(uri2, info.Name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    PackUriHelper.ValidatedPartUri uri4;
                    Uri uri3 = new Uri(GetOpcNameFromZipItemName(info.Name), UriKind.Relative);
                    if (PackUriHelper.TryValidatePartUri(uri3, out uri4))
                    {
                        this._ignoredItemHelper.AddItemForAtomicPart(uri4, info.Name);
                    }
                }
            }
            this.ProcessPieces(pieceDictionary, parts);
            return parts.ToArray();
        }

        internal static void GetZipCompressionMethodFromOpcCompressionOption(CompressionOption compressionOption, out CompressionMethodEnum compressionMethod, out DeflateOptionEnum deflateOption)
        {
            switch (compressionOption)
            {
                case CompressionOption.Normal:
                    compressionMethod = CompressionMethodEnum.Deflated;
                    deflateOption = DeflateOptionEnum.Normal;
                    return;

                case CompressionOption.Maximum:
                    compressionMethod = CompressionMethodEnum.Deflated;
                    deflateOption = DeflateOptionEnum.Maximum;
                    return;

                case CompressionOption.Fast:
                    compressionMethod = CompressionMethodEnum.Deflated;
                    deflateOption = DeflateOptionEnum.Fast;
                    return;

                case CompressionOption.SuperFast:
                    compressionMethod = CompressionMethodEnum.Deflated;
                    deflateOption = DeflateOptionEnum.SuperFast;
                    return;
            }
            compressionMethod = CompressionMethodEnum.Stored;
            deflateOption = DeflateOptionEnum.None;
        }

        internal static string GetZipItemNameFromOpcName(string opcName) => 
            opcName.Substring(1);

        private static bool IsValidFileItem(ZipFileInfo info) => 
            (!info.FolderFlag && !info.VolumeLabelFlag);

        private bool IsZipItemValidOpcPartOrPiece(string zipItemName)
        {
            if (zipItemName.StartsWith(ContentTypeHelper.ContentTypeFileName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (zipItemName.StartsWith(_forwardSlash, StringComparison.Ordinal))
            {
                return false;
            }
            if (zipItemName.EndsWith(_forwardSlash, StringComparison.Ordinal))
            {
                return false;
            }
            return true;
        }

        private void ProcessPieces(SortedDictionary<PieceInfo, object> pieceDictionary, List<PackagePart> parts)
        {
            if (pieceDictionary.Count != 0)
            {
                string strB = null;
                int startIndex = 0;
                List<PieceInfo> pieces = new List<PieceInfo>(pieceDictionary.Keys);
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (strB == null)
                    {
                        if (pieces[i].PieceNumber != 0)
                        {
                            this._ignoredItemHelper.AddItemForStrayPiece(pieces[i]);
                            continue;
                        }
                        startIndex = i;
                        strB = pieces[i].NormalizedPrefixName;
                    }
                    else
                    {
                        if (string.CompareOrdinal(pieces[i].NormalizedPrefixName, strB) != 0)
                        {
                            if (pieces[i].PieceNumber == 0)
                            {
                                this._ignoredItemHelper.AddItemsForInvalidSequence(strB, pieces, startIndex, i - startIndex);
                                startIndex = i;
                                strB = pieces[i].NormalizedPrefixName;
                                goto Label_00DF;
                            }
                            this._ignoredItemHelper.AddItemsForInvalidSequence(strB, pieces, startIndex, (i - startIndex) + 1);
                            strB = null;
                            continue;
                        }
                        if (pieces[i].PieceNumber != (i - startIndex))
                        {
                            this._ignoredItemHelper.AddItemsForInvalidSequence(strB, pieces, startIndex, (i - startIndex) + 1);
                            strB = null;
                            continue;
                        }
                    }
                Label_00DF:
                    if (pieces[i].IsLastPiece)
                    {
                        this.RecordValidSequence(strB, pieces, startIndex, (i - startIndex) + 1, parts);
                        strB = null;
                    }
                }
                if (strB != null)
                {
                    this._ignoredItemHelper.AddItemsForInvalidSequence(strB, pieces, startIndex, pieces.Count - startIndex);
                }
            }
        }

        private void RecordValidSequence(string normalizedPrefixNameForCurrentSequence, List<PieceInfo> pieces, int startIndex, int numItems, List<PackagePart> parts)
        {
            PackUriHelper.ValidatedPartUri partUri = pieces[startIndex].PartUri;
            ContentType contentType = this._contentTypeHelper.GetContentType(partUri);
            if (contentType == null)
            {
                this._ignoredItemHelper.AddItemsForInvalidSequence(normalizedPrefixNameForCurrentSequence, pieces, startIndex, numItems);
            }
            else
            {
                parts.Add(new ZipPackagePart(this, this._zipArchive, pieces.GetRange(startIndex, numItems), partUri, contentType.ToString(), GetCompressionOptionFromZipFileInfo(pieces[startIndex].ZipFileInfo)));
            }
        }

        private class ContentTypeHelper
        {
            private CompressionMethodEnum _cachedCompressionMethod;
            private DeflateOptionEnum _cachedDeflateOption;
            private ZipFileInfo _contentTypeFileInfo;
            private static readonly string _contentTypesFile = "[Content_Types].xml";
            private static readonly string _contentTypesFileUpperInvariant = "[CONTENT_TYPES].XML";
            private bool _contentTypeStreamExists;
            private List<PieceInfo> _contentTypeStreamPieces;
            private Dictionary<string, ContentType> _defaultDictionary;
            private static readonly int _defaultDictionaryInitialSize = 0x10;
            private bool _dirty;
            private ZipPackage.IgnoredItemHelper _ignoredItemHelper;
            private Dictionary<PackUriHelper.ValidatedPartUri, ContentType> _overrideDictionary;
            private static readonly int _overrideDictionaryInitialSize = 8;
            private XmlWriter _streamingXmlWriter;
            private ZipArchive _zipArchive;
            private static readonly string ContentTypeAttributeName = "ContentType";
            private static readonly string DefaultTagName = "Default";
            private static readonly string ExtensionAttributeName = "Extension";
            private static readonly string OverrideTagName = "Override";
            private static readonly string PartNameAttributeName = "PartName";
            private static readonly string TemporaryPartNameWithoutExtension = "/tempfiles/sample.";
            private static readonly string TypesNamespaceUri = "http://schemas.openxmlformats.org/package/2006/content-types";
            private static readonly string TypesTagName = "Types";

            internal ContentTypeHelper(ZipArchive zipArchive, ZipPackage.IgnoredItemHelper ignoredItemHelper)
            {
                this._zipArchive = zipArchive;
                this._defaultDictionary = new Dictionary<string, ContentType>(_defaultDictionaryInitialSize, ZipPackage._extensionEqualityComparer);
                this._ignoredItemHelper = ignoredItemHelper;
                if ((this._zipArchive.OpenAccess == FileAccess.Read) || (this._zipArchive.OpenAccess == FileAccess.ReadWrite))
                {
                    this.ParseContentTypesFile(this._zipArchive.GetFiles());
                }
                this._dirty = false;
            }

            internal void AddContentType(PackUriHelper.ValidatedPartUri partUri, ContentType contentType, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption, bool inStreamingCreation)
            {
                if (inStreamingCreation)
                {
                    this.EnsureStreamingXmlWriter(compressionMethod, deflateOption);
                    this._contentTypeStreamExists = true;
                }
                else if (!this._contentTypeStreamExists)
                {
                    this._cachedCompressionMethod = compressionMethod;
                    this._cachedDeflateOption = deflateOption;
                }
                bool flag = false;
                string partUriExtension = partUri.PartUriExtension;
                if ((inStreamingCreation || (partUriExtension.Length == 0)) || (this._defaultDictionary.ContainsKey(partUriExtension) && !(flag = this._defaultDictionary[partUriExtension].AreTypeAndSubTypeEqual(contentType))))
                {
                    this.AddOverrideElement(partUri, contentType, inStreamingCreation);
                }
                else if (!flag)
                {
                    this.AddDefaultElement(partUriExtension, contentType, inStreamingCreation);
                    this._ignoredItemHelper.DeleteItemsWithSimilarExtention(partUriExtension);
                }
            }

            private void AddDefaultElement(string extension, ContentType contentType, bool inStreamingCreation)
            {
                this._defaultDictionary.Add(extension, contentType);
                if (inStreamingCreation)
                {
                    this.WriteDefaultElement(this._streamingXmlWriter, extension, contentType);
                    this._streamingXmlWriter.Flush();
                }
                else
                {
                    this._dirty = true;
                }
            }

            private void AddOverrideElement(PackUriHelper.ValidatedPartUri partUri, ContentType contentType, bool inStreamingCreation)
            {
                if (inStreamingCreation)
                {
                    this.WriteOverrideElement(this._streamingXmlWriter, partUri, contentType);
                    this._streamingXmlWriter.Flush();
                }
                else
                {
                    this.DeleteContentType(partUri);
                    this.EnsureOverrideDictionary();
                    this._overrideDictionary.Add(partUri, contentType);
                    this._dirty = true;
                }
            }

            internal void CloseContentTypes()
            {
                if (this._streamingXmlWriter != null)
                {
                    this._streamingXmlWriter.WriteEndElement();
                    this._streamingXmlWriter.WriteEndDocument();
                    this._streamingXmlWriter.Close();
                }
            }

            internal void DeleteContentType(PackUriHelper.ValidatedPartUri partUri)
            {
                if ((this._overrideDictionary != null) && this._overrideDictionary.Remove(partUri))
                {
                    this._dirty = true;
                }
            }

            private void EnsureOverrideDictionary()
            {
                if (this._overrideDictionary == null)
                {
                    this._overrideDictionary = new Dictionary<PackUriHelper.ValidatedPartUri, ContentType>(_overrideDictionaryInitialSize);
                }
            }

            private void EnsureStreamingXmlWriter(CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption)
            {
                if (this._streamingXmlWriter == null)
                {
                    StreamingZipPartStream w = new StreamingZipPartStream(ZipPackage.GetOpcNameFromZipItemName(ContentTypeFileName), this._zipArchive, compressionMethod, deflateOption, FileMode.Create, FileAccess.Write);
                    this._streamingXmlWriter = new XmlTextWriter(w, Encoding.UTF8);
                    this._streamingXmlWriter.WriteStartDocument();
                    this._streamingXmlWriter.WriteStartElement(TypesTagName, TypesNamespaceUri);
                }
            }

            internal ContentType GetContentType(PackUriHelper.ValidatedPartUri partUri)
            {
                if ((this._overrideDictionary != null) && this._overrideDictionary.ContainsKey(partUri))
                {
                    return this._overrideDictionary[partUri];
                }
                string partUriExtension = partUri.PartUriExtension;
                if (this._defaultDictionary.ContainsKey(partUriExtension))
                {
                    return this._defaultDictionary[partUriExtension];
                }
                return null;
            }

            private Stream OpenContentTypeStream(ZipFileInfoCollection zipFiles)
            {
                SortedDictionary<PieceInfo, ZipFileInfo> dictionary = null;
                foreach (ZipFileInfo info in (IEnumerable) zipFiles)
                {
                    if (info.Name.ToUpperInvariant().StartsWith(_contentTypesFileUpperInvariant, StringComparison.Ordinal))
                    {
                        if (info.Name.Length == ContentTypeFileName.Length)
                        {
                            this._contentTypeFileInfo = info;
                        }
                        else
                        {
                            PieceInfo info2;
                            if (PieceNameHelper.TryCreatePieceInfo(info, out info2))
                            {
                                if (dictionary == null)
                                {
                                    dictionary = new SortedDictionary<PieceInfo, ZipFileInfo>(PieceNameHelper.PieceNameComparer);
                                }
                                dictionary.Add(info2, info);
                            }
                        }
                    }
                }
                List<PieceInfo> pieces = null;
                if (dictionary != null)
                {
                    pieces = new List<PieceInfo>(dictionary.Keys);
                    if (pieces[0].PieceNumber != 0)
                    {
                        this._ignoredItemHelper.AddItemsForInvalidSequence(_contentTypesFileUpperInvariant, pieces, 0, pieces.Count);
                        dictionary = null;
                        pieces = null;
                    }
                    else
                    {
                        int num = -1;
                        for (int i = 0; i < pieces.Count; i++)
                        {
                            if (pieces[i].PieceNumber != i)
                            {
                                this._ignoredItemHelper.AddItemsForInvalidSequence(_contentTypesFileUpperInvariant, pieces, 0, pieces.Count);
                                dictionary = null;
                                pieces = null;
                                break;
                            }
                            if (pieces[i].IsLastPiece)
                            {
                                num = i;
                                break;
                            }
                        }
                        if (pieces != null)
                        {
                            if (num == -1)
                            {
                                this._ignoredItemHelper.AddItemsForInvalidSequence(_contentTypesFileUpperInvariant, pieces, 0, pieces.Count);
                                dictionary = null;
                                pieces = null;
                            }
                            else if (num < (pieces.Count - 1))
                            {
                                this._ignoredItemHelper.AddItemsForInvalidSequence(_contentTypesFileUpperInvariant, pieces, num + 1, (pieces.Count - num) - 1);
                                pieces.RemoveRange(num + 1, (pieces.Count - num) - 1);
                            }
                        }
                    }
                }
                if ((this._contentTypeFileInfo != null) && (pieces != null))
                {
                    throw new FormatException(System.Windows.SR.Get("BadPackageFormat"));
                }
                if (this._contentTypeFileInfo != null)
                {
                    this._contentTypeStreamExists = true;
                    return this._contentTypeFileInfo.GetStream(FileMode.Open, FileAccess.Read);
                }
                if (pieces != null)
                {
                    this._contentTypeStreamExists = true;
                    this._contentTypeStreamPieces = pieces;
                    return new InterleavedZipPartStream(pieces[0].PrefixName, pieces, FileMode.Open, FileAccess.Read);
                }
                return null;
            }

            private void ParseContentTypesFile(ZipFileInfoCollection zipFiles)
            {
                Stream input = this.OpenContentTypeStream(zipFiles);
                if (input != null)
                {
                    using (input)
                    {
                        using (XmlTextReader reader = new XmlTextReader(input))
                        {
                            reader.WhitespaceHandling = WhitespaceHandling.None;
                            reader.ProhibitDtd = true;
                            PackagingUtilities.PerformInitailReadAndVerifyEncoding(reader);
                            reader.MoveToContent();
                            if (((reader.NodeType != XmlNodeType.Element) || (reader.Depth != 0)) || ((string.CompareOrdinal(reader.NamespaceURI, TypesNamespaceUri) != 0) || (string.CompareOrdinal(reader.Name, TypesTagName) != 0)))
                            {
                                throw new XmlException(System.Windows.SR.Get("TypesElementExpected"), null, reader.LineNumber, reader.LinePosition);
                            }
                            if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) <= 0)
                            {
                                goto Label_016E;
                            }
                            throw new XmlException(System.Windows.SR.Get("TypesTagHasExtraAttributes"), null, reader.LineNumber, reader.LinePosition);
                        Label_009A:
                            reader.MoveToContent();
                            if (reader.NodeType != XmlNodeType.None)
                            {
                                if (((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1)) && ((string.CompareOrdinal(reader.NamespaceURI, TypesNamespaceUri) == 0) && (string.CompareOrdinal(reader.Name, DefaultTagName) == 0)))
                                {
                                    this.ProcessDefaultTagAttributes(reader);
                                }
                                else if (((reader.NodeType == XmlNodeType.Element) && (reader.Depth == 1)) && ((string.CompareOrdinal(reader.NamespaceURI, TypesNamespaceUri) == 0) && (string.CompareOrdinal(reader.Name, OverrideTagName) == 0)))
                                {
                                    this.ProcessOverrideTagAttributes(reader);
                                }
                                else if (((reader.NodeType != XmlNodeType.EndElement) || (reader.Depth != 0)) || (string.CompareOrdinal(reader.Name, TypesTagName) != 0))
                                {
                                    throw new XmlException(System.Windows.SR.Get("TypesXmlDoesNotMatchSchema"), null, reader.LineNumber, reader.LinePosition);
                                }
                            }
                        Label_016E:
                            if (reader.Read())
                            {
                                goto Label_009A;
                            }
                        }
                    }
                }
            }

            private void ProcessDefaultTagAttributes(XmlTextReader reader)
            {
                if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != 2)
                {
                    throw new XmlException(System.Windows.SR.Get("DefaultTagDoesNotMatchSchema"), null, reader.LineNumber, reader.LinePosition);
                }
                string attribute = reader.GetAttribute(ExtensionAttributeName);
                this.ValidateXmlAttribute(ExtensionAttributeName, attribute, DefaultTagName, reader);
                string attributeValue = reader.GetAttribute(ContentTypeAttributeName);
                this.ThrowIfXmlAttributeMissing(ContentTypeAttributeName, attributeValue, DefaultTagName, reader);
                PackUriHelper.ValidatedPartUri uri = PackUriHelper.ValidatePartUri(new Uri(TemporaryPartNameWithoutExtension + attribute, UriKind.Relative));
                this._defaultDictionary.Add(uri.PartUriExtension, new ContentType(attributeValue));
                if (!reader.IsEmptyElement)
                {
                    this.ProcessEndElement(reader, DefaultTagName);
                }
            }

            private void ProcessEndElement(XmlTextReader reader, string elementName)
            {
                reader.Read();
                reader.MoveToContent();
                if ((reader.NodeType != XmlNodeType.EndElement) || (string.CompareOrdinal(elementName, reader.LocalName) != 0))
                {
                    throw new XmlException(System.Windows.SR.Get("ElementIsNotEmptyElement", new object[] { elementName }), null, reader.LineNumber, reader.LinePosition);
                }
            }

            private void ProcessOverrideTagAttributes(XmlTextReader reader)
            {
                if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != 2)
                {
                    throw new XmlException(System.Windows.SR.Get("OverrideTagDoesNotMatchSchema"), null, reader.LineNumber, reader.LinePosition);
                }
                string attribute = reader.GetAttribute(PartNameAttributeName);
                this.ValidateXmlAttribute(PartNameAttributeName, attribute, OverrideTagName, reader);
                string attributeValue = reader.GetAttribute(ContentTypeAttributeName);
                this.ThrowIfXmlAttributeMissing(ContentTypeAttributeName, attributeValue, OverrideTagName, reader);
                PackUriHelper.ValidatedPartUri key = PackUriHelper.ValidatePartUri(new Uri(attribute, UriKind.Relative));
                this.EnsureOverrideDictionary();
                this._overrideDictionary.Add(key, new ContentType(attributeValue));
                if (!reader.IsEmptyElement)
                {
                    this.ProcessEndElement(reader, OverrideTagName);
                }
            }

            internal void SaveToFile()
            {
                if (this._dirty)
                {
                    if (!this._contentTypeStreamExists)
                    {
                        this._contentTypeFileInfo = this._zipArchive.AddFile(_contentTypesFile, this._cachedCompressionMethod, this._cachedDeflateOption);
                        this._contentTypeStreamExists = true;
                    }
                    else if (this._contentTypeStreamPieces != null)
                    {
                        CompressionMethodEnum compressionMethod = this._contentTypeStreamPieces[0].ZipFileInfo.CompressionMethod;
                        DeflateOptionEnum deflateOption = this._contentTypeStreamPieces[0].ZipFileInfo.DeflateOption;
                        ZipPackage.DeleteInterleavedPartOrStream(this._contentTypeStreamPieces);
                        this._contentTypeStreamPieces = null;
                        this._contentTypeFileInfo = this._zipArchive.AddFile(_contentTypesFile, compressionMethod, deflateOption);
                    }
                    using (Stream stream = this._contentTypeFileInfo.GetStream(FileMode.Create, this._zipArchive.OpenAccess))
                    {
                        using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                        {
                            writer.WriteStartDocument();
                            writer.WriteStartElement(TypesTagName, TypesNamespaceUri);
                            foreach (string str in this._defaultDictionary.Keys)
                            {
                                this.WriteDefaultElement(writer, str, this._defaultDictionary[str]);
                            }
                            if (this._overrideDictionary != null)
                            {
                                foreach (PackUriHelper.ValidatedPartUri uri in this._overrideDictionary.Keys)
                                {
                                    this.WriteOverrideElement(writer, uri, this._overrideDictionary[uri]);
                                }
                            }
                            writer.WriteEndElement();
                            writer.WriteEndDocument();
                            this._dirty = false;
                        }
                    }
                }
            }

            private void ThrowIfXmlAttributeMissing(string attributeName, string attributeValue, string tagName, XmlTextReader reader)
            {
                if (attributeValue == null)
                {
                    throw new XmlException(System.Windows.SR.Get("RequiredAttributeMissing", new object[] { tagName, attributeName }), null, reader.LineNumber, reader.LinePosition);
                }
            }

            private void ValidateXmlAttribute(string attributeName, string attributeValue, string tagName, XmlTextReader reader)
            {
                this.ThrowIfXmlAttributeMissing(attributeName, attributeValue, tagName, reader);
                if (attributeValue == string.Empty)
                {
                    throw new XmlException(System.Windows.SR.Get("RequiredAttributeEmpty", new object[] { tagName, attributeName }), null, reader.LineNumber, reader.LinePosition);
                }
            }

            private void WriteDefaultElement(XmlWriter xmlWriter, string extension, ContentType contentType)
            {
                xmlWriter.WriteStartElement(DefaultTagName);
                xmlWriter.WriteAttributeString(ExtensionAttributeName, extension);
                xmlWriter.WriteAttributeString(ContentTypeAttributeName, contentType.ToString());
                xmlWriter.WriteEndElement();
            }

            private void WriteOverrideElement(XmlWriter xmlWriter, PackUriHelper.ValidatedPartUri partUri, ContentType contentType)
            {
                xmlWriter.WriteStartElement(OverrideTagName);
                xmlWriter.WriteAttributeString(PartNameAttributeName, partUri.PartUriString);
                xmlWriter.WriteAttributeString(ContentTypeAttributeName, contentType.ToString());
                xmlWriter.WriteEndElement();
            }

            internal static string ContentTypeFileName =>
                _contentTypesFile;
        }

        private sealed class ExtensionEqualityComparer : IEqualityComparer<string>
        {
            bool IEqualityComparer<string>.Equals(string extensionA, string extensionB)
            {
                Invariant.Assert(extensionA != null, "extenstion should not be null");
                Invariant.Assert(extensionB != null, "extenstion should not be null");
                return (string.CompareOrdinal(extensionA.ToUpperInvariant(), extensionB.ToUpperInvariant()) == 0);
            }

            int IEqualityComparer<string>.GetHashCode(string extension)
            {
                Invariant.Assert(extension != null, "extenstion should not be null");
                return extension.ToUpperInvariant().GetHashCode();
            }
        }

        private class IgnoredItemHelper
        {
            private const int _dictionaryInitialSize = 8;
            private Dictionary<string, List<string>> _extensionDictionary = new Dictionary<string, List<string>>(8, ZipPackage._extensionEqualityComparer);
            private Dictionary<string, List<string>> _ignoredItemDictionary = new Dictionary<string, List<string>>(8, StringComparer.Ordinal);
            private const int _listInitialSize = 1;
            private ZipArchive _zipArchive;

            internal IgnoredItemHelper(ZipArchive zipArchive)
            {
                this._zipArchive = zipArchive;
            }

            private void AddItem(PackUriHelper.ValidatedPartUri partUri, string normalizedPrefixName, string zipFileName)
            {
                if (!this._ignoredItemDictionary.ContainsKey(normalizedPrefixName))
                {
                    this._ignoredItemDictionary.Add(normalizedPrefixName, new List<string>(1));
                }
                this._ignoredItemDictionary[normalizedPrefixName].Add(zipFileName);
                if (partUri != null)
                {
                    this.UpdateExtensionDictionary(partUri, normalizedPrefixName);
                }
            }

            internal void AddItemForAtomicPart(PackUriHelper.ValidatedPartUri partUri, string zipFileName)
            {
                this.AddItem(partUri, partUri.NormalizedPartUriString, zipFileName);
            }

            internal void AddItemForStrayPiece(PieceInfo pieceInfo)
            {
                this.AddItem(pieceInfo.PartUri, pieceInfo.NormalizedPrefixName, pieceInfo.ZipFileInfo.Name);
            }

            internal void AddItemsForInvalidSequence(string normalizedPrefixNameForThisSequence, List<PieceInfo> pieces, int startIndex, int count)
            {
                List<string> list;
                if (this._ignoredItemDictionary.ContainsKey(normalizedPrefixNameForThisSequence))
                {
                    list = this._ignoredItemDictionary[normalizedPrefixNameForThisSequence];
                }
                else
                {
                    list = new List<string>(count);
                    this._ignoredItemDictionary.Add(normalizedPrefixNameForThisSequence, list);
                }
                for (int i = startIndex; i < (startIndex + count); i++)
                {
                    list.Add(pieces[i].ZipFileInfo.Name);
                }
                if (pieces[startIndex].PartUri != null)
                {
                    this.UpdateExtensionDictionary(pieces[startIndex].PartUri, pieces[startIndex].NormalizedPrefixName);
                }
            }

            internal void Delete(PackUriHelper.ValidatedPartUri partUri)
            {
                string normalizedPartUriString = partUri.NormalizedPartUriString;
                if (this._ignoredItemDictionary.ContainsKey(normalizedPartUriString))
                {
                    foreach (string str2 in this._ignoredItemDictionary[normalizedPartUriString])
                    {
                        this._zipArchive.DeleteFile(str2);
                    }
                    this._ignoredItemDictionary.Remove(normalizedPartUriString);
                }
            }

            internal void DeleteItemsWithSimilarExtention(string extension)
            {
                if (this._extensionDictionary.ContainsKey(extension))
                {
                    foreach (string str in this._extensionDictionary[extension])
                    {
                        if (this._ignoredItemDictionary.ContainsKey(str))
                        {
                            foreach (string str2 in this._ignoredItemDictionary[str])
                            {
                                this._zipArchive.DeleteFile(str2);
                            }
                            this._ignoredItemDictionary.Remove(str);
                        }
                    }
                    this._extensionDictionary.Remove(extension);
                }
            }

            private void UpdateExtensionDictionary(PackUriHelper.ValidatedPartUri partUri, string normalizedPrefixName)
            {
                string partUriExtension = partUri.PartUriExtension;
                if (!this._extensionDictionary.ContainsKey(partUriExtension))
                {
                    this._extensionDictionary.Add(partUriExtension, new List<string>(1));
                }
                this._extensionDictionary[partUriExtension].Add(normalizedPrefixName);
            }
        }
    }
}

