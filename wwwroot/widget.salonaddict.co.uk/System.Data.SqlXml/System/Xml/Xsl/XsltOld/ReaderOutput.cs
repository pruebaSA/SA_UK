namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Utils;

    internal class ReaderOutput : XmlReader, RecordOutput
    {
        private int attributeCount;
        private ArrayList attributeList;
        private BuilderInfo attributeValue;
        private RecordBuilder builder;
        private int currentIndex;
        private BuilderInfo currentInfo;
        private XmlEncoder encoder = new XmlEncoder();
        private bool haveRecord;
        private BuilderInfo mainNode;
        private OutputScopeManager manager;
        private XmlNameTable nameTable;
        private Processor processor;
        private static BuilderInfo s_DefaultInfo = new BuilderInfo();
        private System.Xml.ReadState state;
        private XmlCharType xmlCharType = XmlCharType.Instance;

        internal ReaderOutput(Processor processor)
        {
            this.processor = processor;
            this.nameTable = processor.NameTable;
            this.Reset();
        }

        [Conditional("DEBUG")]
        private void CheckCurrentInfo()
        {
        }

        public override void Close()
        {
            this.processor = null;
            this.state = System.Xml.ReadState.Closed;
            this.Reset();
        }

        private bool FindAttribute(string name, out int attrIndex)
        {
            if (name == null)
            {
                name = string.Empty;
            }
            for (int i = 0; i < this.attributeCount; i++)
            {
                BuilderInfo info = (BuilderInfo) this.attributeList[i];
                if (info.Name == name)
                {
                    attrIndex = i;
                    return true;
                }
            }
            attrIndex = -1;
            return false;
        }

        private bool FindAttribute(string localName, string namespaceURI, out int attrIndex)
        {
            if (namespaceURI == null)
            {
                namespaceURI = string.Empty;
            }
            if (localName == null)
            {
                localName = string.Empty;
            }
            for (int i = 0; i < this.attributeCount; i++)
            {
                BuilderInfo info = (BuilderInfo) this.attributeList[i];
                if ((info.NamespaceURI == namespaceURI) && (info.LocalName == localName))
                {
                    attrIndex = i;
                    return true;
                }
            }
            attrIndex = -1;
            return false;
        }

        public override string GetAttribute(int i) => 
            this.GetBuilderInfo(i).Value;

        public override string GetAttribute(string name)
        {
            int num;
            if (this.FindAttribute(name, out num))
            {
                return ((BuilderInfo) this.attributeList[num]).Value;
            }
            return null;
        }

        public override string GetAttribute(string localName, string namespaceURI)
        {
            int num;
            if (this.FindAttribute(localName, namespaceURI, out num))
            {
                return ((BuilderInfo) this.attributeList[num]).Value;
            }
            return null;
        }

        private BuilderInfo GetBuilderInfo(int attrib)
        {
            if ((attrib < 0) || (this.attributeCount <= attrib))
            {
                throw new ArgumentOutOfRangeException("attrib");
            }
            return (BuilderInfo) this.attributeList[attrib];
        }

        public override string LookupNamespace(string prefix)
        {
            prefix = this.nameTable.Get(prefix);
            if ((this.manager != null) && (prefix != null))
            {
                return this.manager.ResolveNamespace(prefix);
            }
            return null;
        }

        public override void MoveToAttribute(int i)
        {
            if ((i < 0) || (this.attributeCount <= i))
            {
                throw new ArgumentOutOfRangeException("i");
            }
            this.SetAttribute(i);
        }

        public override bool MoveToAttribute(string name)
        {
            int num;
            if (this.FindAttribute(name, out num))
            {
                this.SetAttribute(num);
                return true;
            }
            return false;
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            int num;
            if (this.FindAttribute(localName, namespaceURI, out num))
            {
                this.SetAttribute(num);
                return true;
            }
            return false;
        }

        public override bool MoveToElement()
        {
            if ((this.NodeType != XmlNodeType.Attribute) && (this.currentInfo != this.attributeValue))
            {
                return false;
            }
            this.SetMainNode();
            return true;
        }

        public override bool MoveToFirstAttribute()
        {
            if (this.attributeCount <= 0)
            {
                return false;
            }
            this.SetAttribute(0);
            return true;
        }

        public override bool MoveToNextAttribute()
        {
            if ((this.currentIndex + 1) < this.attributeCount)
            {
                this.SetAttribute(this.currentIndex + 1);
                return true;
            }
            return false;
        }

        public override bool Read()
        {
            if (this.state != System.Xml.ReadState.Interactive)
            {
                if (this.state != System.Xml.ReadState.Initial)
                {
                    return false;
                }
                this.state = System.Xml.ReadState.Interactive;
            }
        Label_001C:
            if (this.haveRecord)
            {
                this.processor.ResetOutput();
                this.haveRecord = false;
            }
            this.processor.Execute();
            if (!this.haveRecord)
            {
                this.state = System.Xml.ReadState.EndOfFile;
                this.Reset();
                goto Label_00AD;
            }
            XmlNodeType nodeType = this.NodeType;
            if (nodeType != XmlNodeType.Text)
            {
                if (nodeType == XmlNodeType.Whitespace)
                {
                    goto Label_007B;
                }
                goto Label_00AD;
            }
            if (!this.xmlCharType.IsOnlyWhitespace(this.Value))
            {
                goto Label_00AD;
            }
            this.currentInfo.NodeType = XmlNodeType.Whitespace;
        Label_007B:
            if (this.Value.Length == 0)
            {
                goto Label_001C;
            }
            if (this.XmlSpace == System.Xml.XmlSpace.Preserve)
            {
                this.currentInfo.NodeType = XmlNodeType.SignificantWhitespace;
            }
        Label_00AD:
            return this.haveRecord;
        }

        public override bool ReadAttributeValue()
        {
            if ((this.ReadState != System.Xml.ReadState.Interactive) || (this.NodeType != XmlNodeType.Attribute))
            {
                return false;
            }
            if (this.attributeValue == null)
            {
                this.attributeValue = new BuilderInfo();
                this.attributeValue.NodeType = XmlNodeType.Text;
            }
            if (this.currentInfo == this.attributeValue)
            {
                return false;
            }
            this.attributeValue.Value = this.currentInfo.Value;
            this.attributeValue.Depth = this.currentInfo.Depth + 1;
            this.currentInfo = this.attributeValue;
            return true;
        }

        public override string ReadInnerXml()
        {
            if (this.ReadState == System.Xml.ReadState.Interactive)
            {
                if ((this.NodeType == XmlNodeType.Element) && !this.IsEmptyElement)
                {
                    StringOutput output = new StringOutput(this.processor);
                    output.OmitXmlDecl();
                    int depth = this.Depth;
                    this.Read();
                    while (depth < this.Depth)
                    {
                        output.RecordDone(this.builder);
                        this.Read();
                    }
                    this.Read();
                    output.TheEnd();
                    return output.Result;
                }
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    return this.encoder.AtributeInnerXml(this.Value);
                }
                this.Read();
            }
            return string.Empty;
        }

        public override string ReadOuterXml()
        {
            if (this.ReadState == System.Xml.ReadState.Interactive)
            {
                if (this.NodeType == XmlNodeType.Element)
                {
                    StringOutput output = new StringOutput(this.processor);
                    output.OmitXmlDecl();
                    bool isEmptyElement = this.IsEmptyElement;
                    int depth = this.Depth;
                    output.RecordDone(this.builder);
                    this.Read();
                    while (depth < this.Depth)
                    {
                        output.RecordDone(this.builder);
                        this.Read();
                    }
                    if (!isEmptyElement)
                    {
                        output.RecordDone(this.builder);
                        this.Read();
                    }
                    output.TheEnd();
                    return output.Result;
                }
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    return this.encoder.AtributeOuterXml(this.Name, this.Value);
                }
                this.Read();
            }
            return string.Empty;
        }

        public override string ReadString()
        {
            string str = string.Empty;
            if (((this.NodeType == XmlNodeType.Element) || (this.NodeType == XmlNodeType.Attribute)) || (this.currentInfo == this.attributeValue))
            {
                if (this.mainNode.IsEmptyTag)
                {
                    return str;
                }
                if (!this.Read())
                {
                    throw new InvalidOperationException(System.Xml.Utils.Res.GetString("Xml_InvalidOperation"));
                }
            }
            StringBuilder builder = null;
            bool flag = true;
        Label_0051:
            switch (this.NodeType)
            {
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    if (flag)
                    {
                        str = this.Value;
                        flag = false;
                    }
                    else
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(str);
                        }
                        builder.Append(this.Value);
                    }
                    if (!this.Read())
                    {
                        throw new InvalidOperationException(System.Xml.Utils.Res.GetString("Xml_InvalidOperation"));
                    }
                    goto Label_0051;
            }
            if (builder != null)
            {
                return builder.ToString();
            }
            return str;
        }

        public Processor.OutputResult RecordDone(RecordBuilder record)
        {
            this.builder = record;
            this.mainNode = record.MainNode;
            this.attributeList = record.AttributeList;
            this.attributeCount = record.AttributeCount;
            this.manager = record.Manager;
            this.haveRecord = true;
            this.SetMainNode();
            return Processor.OutputResult.Interrupt;
        }

        private void Reset()
        {
            this.currentIndex = -1;
            this.currentInfo = s_DefaultInfo;
            this.mainNode = s_DefaultInfo;
            this.manager = null;
        }

        public override void ResolveEntity()
        {
            if (this.NodeType != XmlNodeType.EntityReference)
            {
                throw new InvalidOperationException(System.Xml.Utils.Res.GetString("Xml_InvalidOperation"));
            }
        }

        private void SetAttribute(int attrib)
        {
            this.currentIndex = attrib;
            this.currentInfo = (BuilderInfo) this.attributeList[attrib];
        }

        private void SetMainNode()
        {
            this.currentIndex = -1;
            this.currentInfo = this.mainNode;
        }

        public void TheEnd()
        {
        }

        public override int AttributeCount =>
            this.attributeCount;

        public override string BaseURI =>
            string.Empty;

        public override int Depth =>
            this.currentInfo.Depth;

        public override bool EOF =>
            (this.state == System.Xml.ReadState.EndOfFile);

        public override bool HasValue =>
            XmlReader.HasValueInternal(this.NodeType);

        public override bool IsDefault =>
            false;

        public override bool IsEmptyElement =>
            this.currentInfo.IsEmptyTag;

        public override string this[int i] =>
            this.GetAttribute(i);

        public override string this[string name] =>
            this.GetAttribute(name);

        public override string this[string name, string namespaceURI] =>
            this.GetAttribute(name, namespaceURI);

        public override string LocalName =>
            this.currentInfo.LocalName;

        public override string Name
        {
            get
            {
                string prefix = this.Prefix;
                string localName = this.LocalName;
                if ((prefix == null) || (prefix.Length <= 0))
                {
                    return localName;
                }
                if (localName.Length > 0)
                {
                    return (prefix + ":" + localName);
                }
                return prefix;
            }
        }

        public override string NamespaceURI =>
            this.currentInfo.NamespaceURI;

        public override XmlNameTable NameTable =>
            this.nameTable;

        public override XmlNodeType NodeType =>
            this.currentInfo.NodeType;

        public override string Prefix =>
            this.currentInfo.Prefix;

        public override char QuoteChar =>
            this.encoder.QuoteChar;

        public override System.Xml.ReadState ReadState =>
            this.state;

        public override string Value =>
            this.currentInfo.Value;

        public override string XmlLang =>
            this.manager?.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this.manager?.XmlSpace;

        private class XmlEncoder
        {
            private StringBuilder buffer;
            private XmlTextEncoder encoder;

            public string AtributeInnerXml(string value)
            {
                if (this.encoder == null)
                {
                    this.Init();
                }
                this.buffer.Length = 0;
                this.encoder.StartAttribute(false);
                this.encoder.Write(value);
                this.encoder.EndAttribute();
                return this.buffer.ToString();
            }

            public string AtributeOuterXml(string name, string value)
            {
                if (this.encoder == null)
                {
                    this.Init();
                }
                this.buffer.Length = 0;
                this.buffer.Append(name);
                this.buffer.Append('=');
                this.buffer.Append(this.QuoteChar);
                this.encoder.StartAttribute(false);
                this.encoder.Write(value);
                this.encoder.EndAttribute();
                this.buffer.Append(this.QuoteChar);
                return this.buffer.ToString();
            }

            private void Init()
            {
                this.buffer = new StringBuilder();
                this.encoder = new XmlTextEncoder(new StringWriter(this.buffer, CultureInfo.InvariantCulture));
            }

            public char QuoteChar =>
                '"';
        }
    }
}

