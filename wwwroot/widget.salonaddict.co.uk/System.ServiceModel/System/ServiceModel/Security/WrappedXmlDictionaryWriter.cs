namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class WrappedXmlDictionaryWriter : XmlDictionaryWriter
    {
        private string id;
        private int index;
        private XmlDictionaryWriter innerWriter;
        private bool insertId;
        private bool isStrReferenceElement;

        public WrappedXmlDictionaryWriter(XmlDictionaryWriter writer, string id)
        {
            this.innerWriter = writer;
            this.index = 0;
            this.insertId = false;
            this.isStrReferenceElement = false;
            this.id = id;
        }

        public override void Close()
        {
            this.innerWriter.Close();
        }

        public override void Flush()
        {
            this.innerWriter.Flush();
        }

        public override string LookupPrefix(string ns) => 
            this.innerWriter.LookupPrefix(ns);

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            this.innerWriter.WriteBase64(buffer, index, count);
        }

        public override void WriteCData(string text)
        {
            this.innerWriter.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            this.innerWriter.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            this.innerWriter.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            this.innerWriter.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.innerWriter.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            this.innerWriter.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            this.innerWriter.WriteEndDocument();
        }

        public override void WriteEndElement()
        {
            this.innerWriter.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            this.innerWriter.WriteEntityRef(name);
        }

        public override void WriteFullEndElement()
        {
            this.innerWriter.WriteFullEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            this.innerWriter.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(string data)
        {
            this.innerWriter.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            this.innerWriter.WriteRaw(buffer, index, count);
        }

        public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
        {
            if ((this.isStrReferenceElement && this.insertId) && (localName == XD.UtilityDictionary.IdAttribute.Value))
            {
                this.insertId = false;
            }
            this.innerWriter.WriteStartAttribute(prefix, localName, namespaceUri);
        }

        public override void WriteStartDocument()
        {
            this.innerWriter.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            this.innerWriter.WriteStartDocument(standalone);
        }

        public override void WriteStartElement(string prefix, string localName, string namespaceUri)
        {
            if (this.isStrReferenceElement && this.insertId)
            {
                if (this.id != null)
                {
                    this.innerWriter.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, this.id);
                }
                this.isStrReferenceElement = false;
                this.insertId = false;
            }
            this.index++;
            if ((this.index == 1) && (localName == XD.SecurityJan2004Dictionary.SecurityTokenReference.Value))
            {
                this.insertId = true;
                this.isStrReferenceElement = true;
            }
            this.innerWriter.WriteStartElement(prefix, localName, namespaceUri);
        }

        public override void WriteString(string text)
        {
            this.innerWriter.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            this.innerWriter.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteWhitespace(string ws)
        {
            this.innerWriter.WriteWhitespace(ws);
        }

        public override System.Xml.WriteState WriteState =>
            this.innerWriter.WriteState;
    }
}

