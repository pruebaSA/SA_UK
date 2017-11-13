namespace MS.Internal.IO.Packaging
{
    using System;
    using System.Runtime.InteropServices;

    internal static class XTable
    {
        private static readonly TableEntry[] _table = new TableEntry[] { 
            new TableEntry(ID.OpcSignatureNamespace, "http://schemas.openxmlformats.org/package/2006/digital-signature"), new TableEntry(ID.OpcSignatureNamespacePrefix, "opc"), new TableEntry(ID.OpcSignatureNamespaceAttribute, "xmlns:opc"), new TableEntry(ID.W3CSignatureNamespaceRoot, "http://www.w3.org/2000/09/xmldsig#"), new TableEntry(ID.RelationshipsTransformName, "http://schemas.openxmlformats.org/package/2006/RelationshipTransform"), new TableEntry(ID.SignatureTagName, "Signature"), new TableEntry(ID.OpcSignatureAttrValue, "SignatureIdValue"), new TableEntry(ID.SignedInfoTagName, "SignedInfo"), new TableEntry(ID.ReferenceTagName, "Reference"), new TableEntry(ID.SignatureMethodTagName, "SignatureMethod"), new TableEntry(ID.ObjectTagName, "Object"), new TableEntry(ID.KeyInfoTagName, "KeyInfo"), new TableEntry(ID.ManifestTagName, "Manifest"), new TableEntry(ID.TransformTagName, "Transform"), new TableEntry(ID.TransformsTagName, "Transforms"), new TableEntry(ID.AlgorithmAttrName, "Algorithm"),
            new TableEntry(ID.SourceIdAttrName, "SourceId"), new TableEntry(ID.OpcAttrValue, "idPackageObject"), new TableEntry(ID.OpcLinkAttrValue, "#idPackageObject"), new TableEntry(ID.TargetAttrName, "Target"), new TableEntry(ID.SignatureValueTagName, "SignatureValue"), new TableEntry(ID.UriAttrName, "URI"), new TableEntry(ID.DigestMethodTagName, "DigestMethod"), new TableEntry(ID.DigestValueTagName, "DigestValue"), new TableEntry(ID.SignaturePropertiesTagName, "SignatureProperties"), new TableEntry(ID.SignaturePropertyTagName, "SignatureProperty"), new TableEntry(ID.SignatureTimeTagName, "SignatureTime"), new TableEntry(ID.SignatureTimeFormatTagName, "Format"), new TableEntry(ID.SignatureTimeValueTagName, "Value"), new TableEntry(ID.RelationshipsTagName, "Relationships"), new TableEntry(ID.RelationshipReferenceTagName, "RelationshipReference"), new TableEntry(ID.RelationshipsGroupReferenceTagName, "RelationshipsGroupReference"),
            new TableEntry(ID.SourceTypeAttrName, "SourceType"), new TableEntry(ID.SignaturePropertyIdAttrName, "Id"), new TableEntry(ID.SignaturePropertyIdAttrValue, "idSignatureTime"), new TableEntry(ID._IllegalValue, "")
        };

        internal static string Get(ID i) => 
            _table[(int) i].s;

        internal enum ID
        {
            OpcSignatureNamespace,
            OpcSignatureNamespacePrefix,
            OpcSignatureNamespaceAttribute,
            W3CSignatureNamespaceRoot,
            RelationshipsTransformName,
            SignatureTagName,
            OpcSignatureAttrValue,
            SignedInfoTagName,
            ReferenceTagName,
            SignatureMethodTagName,
            ObjectTagName,
            KeyInfoTagName,
            ManifestTagName,
            TransformTagName,
            TransformsTagName,
            AlgorithmAttrName,
            SourceIdAttrName,
            OpcAttrValue,
            OpcLinkAttrValue,
            TargetAttrName,
            SignatureValueTagName,
            UriAttrName,
            DigestMethodTagName,
            DigestValueTagName,
            SignaturePropertiesTagName,
            SignaturePropertyTagName,
            SignatureTimeTagName,
            SignatureTimeFormatTagName,
            SignatureTimeValueTagName,
            RelationshipsTagName,
            RelationshipReferenceTagName,
            RelationshipsGroupReferenceTagName,
            SourceTypeAttrName,
            SignaturePropertyIdAttrName,
            SignaturePropertyIdAttrValue,
            _IllegalValue
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TableEntry
        {
            internal XTable.ID id;
            internal string s;
            internal TableEntry(XTable.ID index, string str)
            {
                this.id = index;
                this.s = str;
            }
        }
    }
}

