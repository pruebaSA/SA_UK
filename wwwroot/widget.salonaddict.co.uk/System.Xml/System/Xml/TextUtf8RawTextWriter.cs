﻿namespace System.Xml
{
    using System;
    using System.IO;
    using System.Text;

    internal class TextUtf8RawTextWriter : XmlUtf8RawTextWriter
    {
        public TextUtf8RawTextWriter(Stream stream, Encoding encoding, XmlWriterSettings settings, bool closeOutput) : base(stream, encoding, settings, closeOutput)
        {
        }

        internal override void StartElementContent()
        {
        }

        public override void WriteCData(string text)
        {
            base.WriteRaw(text);
        }

        public override void WriteCharEntity(char ch)
        {
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            if (!base.inAttributeValue)
            {
                base.WriteRaw(buffer, index, count);
            }
        }

        public override void WriteComment(string text)
        {
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
        }

        public override void WriteEndAttribute()
        {
            base.inAttributeValue = false;
        }

        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
        }

        public override void WriteEntityRef(string name)
        {
        }

        internal override void WriteFullEndElement(string prefix, string localName, string ns)
        {
        }

        internal override void WriteNamespaceDeclaration(string prefix, string ns)
        {
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
        }

        public override void WriteRaw(string data)
        {
            if (!base.inAttributeValue)
            {
                base.WriteRaw(data);
            }
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            if (!base.inAttributeValue)
            {
                base.WriteRaw(buffer, index, count);
            }
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            base.inAttributeValue = true;
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
        }

        public override void WriteString(string textBlock)
        {
            if (!base.inAttributeValue)
            {
                base.WriteRaw(textBlock);
            }
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
        }

        public override void WriteWhitespace(string ws)
        {
            if (!base.inAttributeValue)
            {
                base.WriteRaw(ws);
            }
        }

        internal override void WriteXmlDeclaration(string xmldecl)
        {
        }

        internal override void WriteXmlDeclaration(XmlStandalone standalone)
        {
        }
    }
}

