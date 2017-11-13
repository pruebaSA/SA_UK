﻿namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;

    internal class CanonicalXmlSignificantWhitespace : XmlSignificantWhitespace, ICanonicalizableNode
    {
        private bool m_isInNodeSet;

        public CanonicalXmlSignificantWhitespace(string strData, XmlDocument doc, bool defaultNodeSetInclusionState) : base(strData, doc)
        {
            this.m_isInNodeSet = defaultNodeSetInclusionState;
        }

        public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (this.IsInNodeSet && (docPos == DocPosition.InRootElement))
            {
                strBuilder.Append(System.Security.Cryptography.Xml.Utils.EscapeWhitespaceData(this.Value));
            }
        }

        public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
        {
            if (this.IsInNodeSet && (docPos == DocPosition.InRootElement))
            {
                byte[] bytes = new UTF8Encoding(false).GetBytes(System.Security.Cryptography.Xml.Utils.EscapeWhitespaceData(this.Value));
                hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
            }
        }

        public bool IsInNodeSet
        {
            get => 
                this.m_isInNodeSet;
            set
            {
                this.m_isInNodeSet = value;
            }
        }
    }
}

