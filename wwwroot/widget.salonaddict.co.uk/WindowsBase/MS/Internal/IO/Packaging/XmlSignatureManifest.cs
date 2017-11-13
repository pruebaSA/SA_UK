namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Windows;
    using System.Xml;

    internal static class XmlSignatureManifest
    {
        private const string _contentTypeQueryStringPrefix = "?ContentType=";

        private static XmlNode GenerateDigestMethod(PackageDigitalSignatureManager manager, XmlDocument xDoc)
        {
            XmlElement element = xDoc.CreateElement(XTable.Get(XTable.ID.DigestMethodTagName), "http://www.w3.org/2000/09/xmldsig#");
            XmlAttribute node = xDoc.CreateAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
            node.Value = manager.HashAlgorithm;
            element.Attributes.Append(node);
            return element;
        }

        private static XmlNode GenerateDigestValueNode(XmlDocument xDoc, HashAlgorithm hashAlgorithm, Stream s, string transformName)
        {
            XmlElement element = xDoc.CreateElement(XTable.Get(XTable.ID.DigestValueTagName), "http://www.w3.org/2000/09/xmldsig#");
            XmlText newChild = xDoc.CreateTextNode(XmlDigitalSignatureProcessor.GenerateDigestValue(s, transformName, hashAlgorithm));
            element.AppendChild(newChild);
            return element;
        }

        internal static XmlNode GenerateManifest(PackageDigitalSignatureManager manager, XmlDocument xDoc, HashAlgorithm hashAlgorithm, IEnumerable<Uri> parts, IEnumerable<PackageRelationshipSelector> relationshipSelectors)
        {
            if (!hashAlgorithm.CanReuseTransform)
            {
                throw new ArgumentException(System.Windows.SR.Get("HashAlgorithmMustBeReusable"));
            }
            XmlNode manifest = xDoc.CreateNode(XmlNodeType.Element, XTable.Get(XTable.ID.ManifestTagName), "http://www.w3.org/2000/09/xmldsig#");
            if (parts != null)
            {
                foreach (Uri uri in parts)
                {
                    manifest.AppendChild(GeneratePartSigningReference(manager, xDoc, hashAlgorithm, uri));
                }
            }
            int num = 0;
            if (relationshipSelectors != null)
            {
                num = GenerateRelationshipSigningReferences(manager, xDoc, hashAlgorithm, relationshipSelectors, manifest);
            }
            if ((parts == null) && (num == 0))
            {
                throw new ArgumentException(System.Windows.SR.Get("NothingToSign"));
            }
            return manifest;
        }

        private static XmlNode GeneratePartSigningReference(PackageDigitalSignatureManager manager, XmlDocument xDoc, HashAlgorithm hashAlgorithm, Uri partName)
        {
            PackagePart part = manager.Package.GetPart(partName);
            XmlElement element = xDoc.CreateElement(XTable.Get(XTable.ID.ReferenceTagName), "http://www.w3.org/2000/09/xmldsig#");
            XmlAttribute node = xDoc.CreateAttribute(XTable.Get(XTable.ID.UriAttrName));
            node.Value = PackUriHelper.GetStringForPartUri(partName) + "?ContentType=" + part.ContentType;
            element.Attributes.Append(node);
            string transformName = string.Empty;
            if (manager.TransformMapping.ContainsKey(part.ContentType))
            {
                transformName = manager.TransformMapping[part.ContentType];
                XmlElement newChild = xDoc.CreateElement(XTable.Get(XTable.ID.TransformsTagName), "http://www.w3.org/2000/09/xmldsig#");
                XmlElement element3 = xDoc.CreateElement(XTable.Get(XTable.ID.TransformTagName), "http://www.w3.org/2000/09/xmldsig#");
                XmlAttribute attribute2 = xDoc.CreateAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
                attribute2.Value = transformName;
                element3.Attributes.Append(attribute2);
                newChild.AppendChild(element3);
                element.AppendChild(newChild);
            }
            element.AppendChild(GenerateDigestMethod(manager, xDoc));
            using (Stream stream = part.GetStream(FileMode.Open, FileAccess.Read))
            {
                element.AppendChild(GenerateDigestValueNode(xDoc, hashAlgorithm, stream, transformName));
            }
            return element;
        }

        private static XmlNode GenerateRelationshipSigningReference(PackageDigitalSignatureManager manager, XmlDocument xDoc, HashAlgorithm hashAlgorithm, Uri relationshipPartName, IEnumerable<PackageRelationshipSelector> relationshipSelectors)
        {
            string stringForPartUri;
            string key = PackagingUtilities.RelationshipPartContentType.ToString();
            XmlElement element = xDoc.CreateElement(XTable.Get(XTable.ID.ReferenceTagName), "http://www.w3.org/2000/09/xmldsig#");
            if (PackUriHelper.ComparePartUri(relationshipPartName, PackageRelationship.ContainerRelationshipPartName) == 0)
            {
                stringForPartUri = PackageRelationship.ContainerRelationshipPartName.ToString();
            }
            else
            {
                stringForPartUri = PackUriHelper.GetStringForPartUri(relationshipPartName);
            }
            XmlAttribute attribute = xDoc.CreateAttribute(XTable.Get(XTable.ID.UriAttrName));
            attribute.Value = stringForPartUri + "?ContentType=" + key;
            element.Attributes.Append(attribute);
            XmlElement newChild = xDoc.CreateElement(XTable.Get(XTable.ID.TransformsTagName), "http://www.w3.org/2000/09/xmldsig#");
            string namespaceURI = XTable.Get(XTable.ID.OpcSignatureNamespace);
            string prefix = XTable.Get(XTable.ID.OpcSignatureNamespacePrefix);
            XmlElement element3 = xDoc.CreateElement(XTable.Get(XTable.ID.TransformTagName), "http://www.w3.org/2000/09/xmldsig#");
            XmlAttribute attribute2 = xDoc.CreateAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
            attribute2.Value = XTable.Get(XTable.ID.RelationshipsTransformName);
            element3.Attributes.Append(attribute2);
            foreach (PackageRelationshipSelector selector in relationshipSelectors)
            {
                switch (selector.SelectorType)
                {
                    case PackageRelationshipSelectorType.Id:
                    {
                        XmlNode node = xDoc.CreateElement(prefix, XTable.Get(XTable.ID.RelationshipReferenceTagName), namespaceURI);
                        XmlAttribute attribute3 = xDoc.CreateAttribute(XTable.Get(XTable.ID.SourceIdAttrName));
                        attribute3.Value = selector.SelectionCriteria;
                        node.Attributes.Append(attribute3);
                        element3.AppendChild(node);
                        break;
                    }
                    case PackageRelationshipSelectorType.Type:
                    {
                        XmlNode node2 = xDoc.CreateElement(prefix, XTable.Get(XTable.ID.RelationshipsGroupReferenceTagName), namespaceURI);
                        XmlAttribute attribute4 = xDoc.CreateAttribute(XTable.Get(XTable.ID.SourceTypeAttrName));
                        attribute4.Value = selector.SelectionCriteria;
                        node2.Attributes.Append(attribute4);
                        element3.AppendChild(node2);
                        break;
                    }
                    default:
                        Invariant.Assert(false, "This option should never be executed");
                        break;
                }
            }
            newChild.AppendChild(element3);
            string transformName = null;
            if (manager.TransformMapping.ContainsKey(key))
            {
                transformName = manager.TransformMapping[key];
                if (((transformName == null) || (transformName.Length == 0)) || !XmlDigitalSignatureProcessor.IsValidXmlCanonicalizationTransform(transformName))
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("UnsupportedTransformAlgorithm"));
                }
                element3 = xDoc.CreateElement(XTable.Get(XTable.ID.TransformTagName), "http://www.w3.org/2000/09/xmldsig#");
                attribute2 = xDoc.CreateAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
                attribute2.Value = transformName;
                element3.Attributes.Append(attribute2);
                newChild.AppendChild(element3);
            }
            element.AppendChild(newChild);
            element.AppendChild(GenerateDigestMethod(manager, xDoc));
            using (Stream stream = XmlDigitalSignatureProcessor.GenerateRelationshipNodeStream(GetRelationships(manager, relationshipSelectors)))
            {
                element.AppendChild(GenerateDigestValueNode(xDoc, hashAlgorithm, stream, transformName));
            }
            return element;
        }

        private static int GenerateRelationshipSigningReferences(PackageDigitalSignatureManager manager, XmlDocument xDoc, HashAlgorithm hashAlgorithm, IEnumerable<PackageRelationshipSelector> relationshipSelectors, XmlNode manifest)
        {
            Dictionary<Uri, List<PackageRelationshipSelector>> dictionary = new Dictionary<Uri, List<PackageRelationshipSelector>>();
            foreach (PackageRelationshipSelector selector in relationshipSelectors)
            {
                List<PackageRelationshipSelector> list;
                Uri relationshipPartUri = PackUriHelper.GetRelationshipPartUri(selector.SourceUri);
                if (dictionary.ContainsKey(relationshipPartUri))
                {
                    list = dictionary[relationshipPartUri];
                }
                else
                {
                    list = new List<PackageRelationshipSelector>();
                    dictionary.Add(relationshipPartUri, list);
                }
                list.Add(selector);
            }
            ((XmlElement) manifest).SetAttribute(XTable.Get(XTable.ID.OpcSignatureNamespaceAttribute), XTable.Get(XTable.ID.OpcSignatureNamespace));
            int num = 0;
            foreach (Uri uri2 in dictionary.Keys)
            {
                manifest.AppendChild(GenerateRelationshipSigningReference(manager, xDoc, hashAlgorithm, uri2, dictionary[uri2]));
                num++;
            }
            return num;
        }

        private static IEnumerable<PackageRelationship> GetRelationships(PackageDigitalSignatureManager manager, IEnumerable<PackageRelationshipSelector> relationshipSelectorsWithSameSource)
        {
            SortedDictionary<string, PackageRelationship> dictionary = new SortedDictionary<string, PackageRelationship>(StringComparer.Ordinal);
            foreach (PackageRelationshipSelector selector in relationshipSelectorsWithSameSource)
            {
                foreach (PackageRelationship relationship in selector.Select(manager.Package))
                {
                    if (!dictionary.ContainsKey(relationship.Id))
                    {
                        dictionary.Add(relationship.Id, relationship);
                    }
                }
            }
            return dictionary.Values;
        }

        private static string ParseDigestAlgorithmTag(XmlReader reader)
        {
            if (((PackagingUtilities.GetNonXmlnsAttributeCount(reader) > 1) || (string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0)) || (reader.Depth != 3))
            {
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            string attribute = null;
            if (reader.HasAttributes)
            {
                attribute = reader.GetAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
            }
            if ((attribute == null) || (attribute.Length == 0))
            {
                throw new XmlException(System.Windows.SR.Get("UnsupportedHashAlgorithm"));
            }
            return attribute;
        }

        private static string ParseDigestValueTag(XmlReader reader)
        {
            if (((PackagingUtilities.GetNonXmlnsAttributeCount(reader) > 0) || (string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0)) || (reader.Depth != 3))
            {
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            if (reader.HasAttributes || (reader.Read() && (reader.MoveToContent() != XmlNodeType.Text)))
            {
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            return reader.ReadString();
        }

        internal static void ParseManifest(PackageDigitalSignatureManager manager, XmlReader reader, out List<Uri> partManifest, out List<PartManifestEntry> partEntryManifest, out List<PackageRelationshipSelector> relationshipManifest)
        {
            Invariant.Assert(manager != null);
            Invariant.Assert(reader != null);
            partManifest = new List<Uri>();
            partEntryManifest = new List<PartManifestEntry>();
            relationshipManifest = new List<PackageRelationshipSelector>();
            string strB = XTable.Get(XTable.ID.ReferenceTagName);
            int num = 0;
            while (reader.Read() && (reader.MoveToContent() == XmlNodeType.Element))
            {
                if (((string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0) || (string.CompareOrdinal(reader.LocalName, strB) != 0)) || (reader.Depth != 2))
                {
                    throw new XmlException(System.Windows.SR.Get("UnexpectedXmlTag", new object[] { reader.Name }));
                }
                PartManifestEntry item = ParseReference(reader);
                if (item.IsRelationshipEntry)
                {
                    foreach (PackageRelationshipSelector selector in item.RelationshipSelectors)
                    {
                        relationshipManifest.Add(selector);
                    }
                }
                else
                {
                    partManifest.Add(item.Uri);
                }
                partEntryManifest.Add(item);
                num++;
            }
            if (num == 0)
            {
                throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
            }
        }

        private static Uri ParsePartUri(XmlReader reader, out ContentType contentType)
        {
            contentType = ContentType.Empty;
            Uri uri = null;
            if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) == 1)
            {
                string attribute = reader.GetAttribute(XTable.Get(XTable.ID.UriAttrName));
                if (attribute != null)
                {
                    uri = ParsePartUriAttribute(attribute, out contentType);
                }
            }
            if (uri == null)
            {
                throw new XmlException(System.Windows.SR.Get("RequiredXmlAttributeMissing", new object[] { XTable.Get(XTable.ID.UriAttrName) }));
            }
            return uri;
        }

        private static Uri ParsePartUriAttribute(string attrValue, out ContentType contentType)
        {
            contentType = ContentType.Empty;
            int index = attrValue.IndexOf('?');
            Uri uri = null;
            if (index > 0)
            {
                try
                {
                    string str = attrValue.Substring(index);
                    if ((str.Length > "?ContentType=".Length) && str.StartsWith("?ContentType=", StringComparison.Ordinal))
                    {
                        contentType = new ContentType(str.Substring("?ContentType=".Length));
                    }
                    uri = PackUriHelper.ValidatePartUri(new Uri(attrValue.Substring(0, index), UriKind.Relative));
                }
                catch (ArgumentException exception)
                {
                    throw new XmlException(System.Windows.SR.Get("PartReferenceUriMalformed"), exception);
                }
            }
            if (contentType.ToString().Length <= 0)
            {
                throw new XmlException(System.Windows.SR.Get("PartReferenceUriMalformed"));
            }
            return uri;
        }

        private static PartManifestEntry ParseReference(XmlReader reader)
        {
            ContentType contentType = null;
            Uri partUri = ParsePartUri(reader, out contentType);
            List<PackageRelationshipSelector> relationshipSelectors = null;
            string hashAlgorithm = null;
            string hashValue = null;
            List<string> transforms = null;
            bool flag = false;
            while (reader.Read() && (reader.MoveToContent() == XmlNodeType.Element))
            {
                if ((string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0) || (reader.Depth != 3))
                {
                    throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                }
                if ((hashAlgorithm != null) || (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.DigestMethodTagName)) != 0))
                {
                    if ((hashValue != null) || (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.DigestValueTagName)) != 0))
                    {
                        if (flag || (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.TransformsTagName)) != 0))
                        {
                            throw new XmlException(System.Windows.SR.Get("PackageSignatureCorruption"));
                        }
                        transforms = ParseTransformsTag(reader, partUri, ref relationshipSelectors);
                        flag = true;
                        continue;
                    }
                    hashValue = ParseDigestValueTag(reader);
                }
                else
                {
                    hashAlgorithm = ParseDigestAlgorithmTag(reader);
                    continue;
                }
            }
            return new PartManifestEntry(partUri, contentType, hashAlgorithm, hashValue, transforms, relationshipSelectors);
        }

        private static void ParseRelationshipsTransform(XmlReader reader, Uri partUri, ref List<PackageRelationshipSelector> relationshipSelectors)
        {
            Uri sourcePartUriFromRelationshipPartUri = PackUriHelper.GetSourcePartUriFromRelationshipPartUri(partUri);
            while ((reader.Read() && (reader.MoveToContent() == XmlNodeType.Element)) && (reader.Depth == 5))
            {
                if ((reader.IsEmptyElement && (PackagingUtilities.GetNonXmlnsAttributeCount(reader) == 1)) && (string.CompareOrdinal(reader.NamespaceURI, XTable.Get(XTable.ID.OpcSignatureNamespace)) == 0))
                {
                    if (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.RelationshipReferenceTagName)) == 0)
                    {
                        string attribute = reader.GetAttribute(XTable.Get(XTable.ID.SourceIdAttrName));
                        if ((attribute == null) || (attribute.Length <= 0))
                        {
                            goto Label_00CD;
                        }
                        if (relationshipSelectors == null)
                        {
                            relationshipSelectors = new List<PackageRelationshipSelector>();
                        }
                        relationshipSelectors.Add(new PackageRelationshipSelector(sourcePartUriFromRelationshipPartUri, PackageRelationshipSelectorType.Id, attribute));
                        continue;
                    }
                    if (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.RelationshipsGroupReferenceTagName)) == 0)
                    {
                        string selectionCriteria = reader.GetAttribute(XTable.Get(XTable.ID.SourceTypeAttrName));
                        if ((selectionCriteria != null) && (selectionCriteria.Length > 0))
                        {
                            if (relationshipSelectors == null)
                            {
                                relationshipSelectors = new List<PackageRelationshipSelector>();
                            }
                            relationshipSelectors.Add(new PackageRelationshipSelector(sourcePartUriFromRelationshipPartUri, PackageRelationshipSelectorType.Type, selectionCriteria));
                            continue;
                        }
                    }
                }
            Label_00CD:;
                throw new XmlException(System.Windows.SR.Get("UnexpectedXmlTag", new object[] { reader.LocalName }));
            }
        }

        private static List<string> ParseTransformsTag(XmlReader reader, Uri partUri, ref List<PackageRelationshipSelector> relationshipSelectors)
        {
            if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != 0)
            {
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            List<string> list = null;
            bool flag = false;
            int count = 0;
            while (reader.Read() && (reader.MoveToContent() == XmlNodeType.Element))
            {
                string strA = null;
                if (((reader.Depth != 4) || (string.CompareOrdinal(reader.NamespaceURI, "http://www.w3.org/2000/09/xmldsig#") != 0)) || (string.CompareOrdinal(reader.LocalName, XTable.Get(XTable.ID.TransformTagName)) != 0))
                {
                    throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
                }
                if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) == 1)
                {
                    strA = reader.GetAttribute(XTable.Get(XTable.ID.AlgorithmAttrName));
                }
                if ((strA != null) && (strA.Length > 0))
                {
                    if (string.CompareOrdinal(strA, XTable.Get(XTable.ID.RelationshipsTransformName)) == 0)
                    {
                        if (flag)
                        {
                            throw new XmlException(System.Windows.SR.Get("MultipleRelationshipTransformsFound"));
                        }
                        ParseRelationshipsTransform(reader, partUri, ref relationshipSelectors);
                        if (list == null)
                        {
                            list = new List<string>();
                        }
                        list.Add(strA);
                        flag = true;
                        count = list.Count;
                        continue;
                    }
                    if (reader.IsEmptyElement)
                    {
                        if (list == null)
                        {
                            list = new List<string>();
                        }
                        if (!XmlDigitalSignatureProcessor.IsValidXmlCanonicalizationTransform(strA))
                        {
                            throw new InvalidOperationException(System.Windows.SR.Get("UnsupportedTransformAlgorithm"));
                        }
                        list.Add(strA);
                        continue;
                    }
                }
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            if (list.Count == 0)
            {
                throw new XmlException(System.Windows.SR.Get("XmlSignatureParseError"));
            }
            if (flag && (list.Count == count))
            {
                throw new XmlException(System.Windows.SR.Get("RelationshipTransformNotFollowedByCanonicalizationTransform"));
            }
            return list;
        }
    }
}

