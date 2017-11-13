namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal class XmlExpressionDumper : ExpressionDumper
    {
        private XmlWriter _writer;

        internal XmlExpressionDumper(Stream stream) : this(stream, DefaultEncoding, true)
        {
        }

        internal XmlExpressionDumper(Stream stream, Encoding encoding, bool indent)
        {
            XmlWriterSettings settings = new XmlWriterSettings {
                CheckCharacters = false,
                Indent = true,
                Encoding = encoding
            };
            this._writer = XmlWriter.Create(stream, settings);
            this._writer.WriteStartDocument(true);
        }

        internal override void Begin(string name, Dictionary<string, object> attrs)
        {
            this._writer.WriteStartElement(name);
            if (attrs != null)
            {
                foreach (KeyValuePair<string, object> pair in attrs)
                {
                    this._writer.WriteAttributeString(pair.Key, (pair.Value == null) ? "" : pair.Value.ToString());
                }
            }
        }

        internal void Close()
        {
            this._writer.WriteEndDocument();
            this._writer.Flush();
            this._writer.Close();
        }

        internal override void End(string name)
        {
            this._writer.WriteEndElement();
        }

        internal static Encoding DefaultEncoding =>
            Encoding.UTF8;
    }
}

