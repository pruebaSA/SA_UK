﻿namespace System.Xml
{
    using System;
    using System.IO;
    using System.Text;

    internal class HtmlEncodedRawTextWriterIndent : HtmlEncodedRawTextWriter
    {
        private int endBlockPos;
        private string indentChars;
        private int indentLevel;
        private bool newLineOnAttributes;

        public HtmlEncodedRawTextWriterIndent(TextWriter writer, XmlWriterSettings settings) : base(writer, settings)
        {
            this.Init(settings);
        }

        public HtmlEncodedRawTextWriterIndent(Stream stream, Encoding encoding, XmlWriterSettings settings, bool closeOutput) : base(stream, encoding, settings, closeOutput)
        {
            this.Init(settings);
        }

        protected override void FlushBuffer()
        {
            this.endBlockPos = (this.endBlockPos == base.bufPos) ? 1 : 0;
            base.FlushBuffer();
        }

        private void Init(XmlWriterSettings settings)
        {
            this.indentLevel = 0;
            this.indentChars = settings.IndentChars;
            this.newLineOnAttributes = settings.NewLineOnAttributes;
        }

        internal override void StartElementContent()
        {
            base.bufChars[base.bufPos++] = '>';
            base.contentPos = base.bufPos;
            if ((base.currentElementProperties & ElementProperties.HEAD) != ElementProperties.DEFAULT)
            {
                this.WriteIndent();
                base.WriteMetaElement();
                this.endBlockPos = base.bufPos;
            }
            else if ((base.currentElementProperties & ElementProperties.BLOCK_WS) != ElementProperties.DEFAULT)
            {
                this.endBlockPos = base.bufPos;
            }
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            base.WriteDocType(name, pubid, sysid, subset);
            this.endBlockPos = base.bufPos;
        }

        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
            this.indentLevel--;
            bool flag = (base.currentElementProperties & ElementProperties.BLOCK_WS) != ElementProperties.DEFAULT;
            if ((flag && (this.endBlockPos == base.bufPos)) && (base.contentPos != base.bufPos))
            {
                this.WriteIndent();
            }
            base.WriteEndElement(prefix, localName, ns);
            base.contentPos = 0;
            if (flag)
            {
                this.endBlockPos = base.bufPos;
            }
        }

        private void WriteIndent()
        {
            base.RawText(base.newLineChars);
            for (int i = this.indentLevel; i > 0; i--)
            {
                base.RawText(this.indentChars);
            }
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (this.newLineOnAttributes)
            {
                base.RawText(base.newLineChars);
                this.indentLevel++;
                this.WriteIndent();
                this.indentLevel--;
            }
            base.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (base.trackTextContent && base.inTextContent)
            {
                base.ChangeTextContentMark(false);
            }
            base.elementScope.Push((byte) base.currentElementProperties);
            if (ns.Length == 0)
            {
                base.currentElementProperties = (ElementProperties) HtmlEncodedRawTextWriter.elementPropertySearch.FindCaseInsensitiveString(localName);
                if ((this.endBlockPos == base.bufPos) && ((base.currentElementProperties & ElementProperties.BLOCK_WS) != ElementProperties.DEFAULT))
                {
                    this.WriteIndent();
                }
                this.indentLevel++;
                base.bufChars[base.bufPos++] = '<';
            }
            else
            {
                base.currentElementProperties = ElementProperties.BLOCK_WS | ElementProperties.HAS_NS;
                if (this.endBlockPos == base.bufPos)
                {
                    this.WriteIndent();
                }
                this.indentLevel++;
                base.bufChars[base.bufPos++] = '<';
                if (prefix.Length != 0)
                {
                    base.RawText(prefix);
                    base.bufChars[base.bufPos++] = ':';
                }
            }
            base.RawText(localName);
            base.attrEndPos = base.bufPos;
        }
    }
}

