namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;

    internal sealed class XmlAttributeCache : XmlRawWriter, IRemovableWriter
    {
        private AttrNameVal[] arrAttrs;
        private const int DefaultCacheSize = 0x20;
        private int hashCodeUnion;
        private int idxLastName;
        private int numEntries;
        private OnRemoveWriter onRemove;
        private XmlRawWriter wrapped;

        public override void Close()
        {
            this.wrapped.Close();
        }

        private void EnsureAttributeCache()
        {
            if (this.arrAttrs == null)
            {
                this.arrAttrs = new AttrNameVal[0x20];
            }
            else if (this.numEntries >= this.arrAttrs.Length)
            {
                AttrNameVal[] destinationArray = new AttrNameVal[this.numEntries * 2];
                Array.Copy(this.arrAttrs, destinationArray, this.numEntries);
                this.arrAttrs = destinationArray;
            }
        }

        public override void Flush()
        {
            this.wrapped.Flush();
        }

        private void FlushAttributes()
        {
            int index = 0;
            while (index != this.numEntries)
            {
                int nextNameIndex = this.arrAttrs[index].NextNameIndex;
                if (nextNameIndex == 0)
                {
                    nextNameIndex = this.numEntries;
                }
                string localName = this.arrAttrs[index].LocalName;
                if (localName != null)
                {
                    string prefix = this.arrAttrs[index].Prefix;
                    string ns = this.arrAttrs[index].Namespace;
                    this.wrapped.WriteStartAttribute(prefix, localName, ns);
                    while (++index != nextNameIndex)
                    {
                        string text = this.arrAttrs[index].Text;
                        if (text != null)
                        {
                            this.wrapped.WriteString(text);
                        }
                        else
                        {
                            this.wrapped.WriteValue(this.arrAttrs[index].Value);
                        }
                    }
                    this.wrapped.WriteEndAttribute();
                }
                else
                {
                    index = nextNameIndex;
                }
            }
            if (this.onRemove != null)
            {
                this.onRemove(this.wrapped);
            }
        }

        public void Init(XmlRawWriter wrapped)
        {
            this.SetWrappedWriter(wrapped);
            this.numEntries = 0;
            this.idxLastName = 0;
            this.hashCodeUnion = 0;
        }

        private void SetWrappedWriter(XmlRawWriter writer)
        {
            IRemovableWriter writer2 = writer as IRemovableWriter;
            if (writer2 != null)
            {
                writer2.OnRemoveWriterEvent = new OnRemoveWriter(this.SetWrappedWriter);
            }
            this.wrapped = writer;
        }

        internal override void StartElementContent()
        {
            this.FlushAttributes();
            this.wrapped.StartElementContent();
        }

        public override void WriteComment(string text)
        {
        }

        public override void WriteEndAttribute()
        {
        }

        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
        }

        public override void WriteEntityRef(string name)
        {
        }

        internal override void WriteNamespaceDeclaration(string prefix, string ns)
        {
            this.FlushAttributes();
            this.wrapped.WriteNamespaceDeclaration(prefix, ns);
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            int index = 0;
            int hashCode = ((int) 1) << localName[0];
            if ((this.hashCodeUnion & hashCode) != 0)
            {
                do
                {
                    if (this.arrAttrs[index].IsDuplicate(localName, ns, hashCode))
                    {
                        break;
                    }
                    index = this.arrAttrs[index].NextNameIndex;
                }
                while (index != 0);
            }
            else
            {
                this.hashCodeUnion |= hashCode;
            }
            this.EnsureAttributeCache();
            if (this.numEntries != 0)
            {
                this.arrAttrs[this.idxLastName].NextNameIndex = this.numEntries;
            }
            this.idxLastName = this.numEntries++;
            this.arrAttrs[this.idxLastName].Init(prefix, localName, ns, hashCode);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
        }

        public override void WriteString(string text)
        {
            this.EnsureAttributeCache();
            this.arrAttrs[this.numEntries++].Init(text);
        }

        public override void WriteValue(object value)
        {
            this.EnsureAttributeCache();
            this.arrAttrs[this.numEntries++].Init((XmlAtomicValue) value);
        }

        public override void WriteValue(string value)
        {
            this.WriteValue(value);
        }

        public int Count =>
            this.numEntries;

        public OnRemoveWriter OnRemoveWriterEvent
        {
            get => 
                this.onRemove;
            set
            {
                this.onRemove = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AttrNameVal
        {
            private string localName;
            private string prefix;
            private string namespaceName;
            private string text;
            private XmlAtomicValue value;
            private int hashCode;
            private int nextNameIndex;
            public string LocalName =>
                this.localName;
            public string Prefix =>
                this.prefix;
            public string Namespace =>
                this.namespaceName;
            public string Text =>
                this.text;
            public XmlAtomicValue Value =>
                this.value;
            public int NextNameIndex
            {
                get => 
                    this.nextNameIndex;
                set
                {
                    this.nextNameIndex = value;
                }
            }
            public void Init(string prefix, string localName, string ns, int hashCode)
            {
                this.localName = localName;
                this.prefix = prefix;
                this.namespaceName = ns;
                this.hashCode = hashCode;
                this.nextNameIndex = 0;
            }

            public void Init(string text)
            {
                this.text = text;
                this.value = null;
            }

            public void Init(XmlAtomicValue value)
            {
                this.text = null;
                this.value = value;
            }

            public bool IsDuplicate(string localName, string ns, int hashCode)
            {
                if (((this.localName != null) && (this.hashCode == hashCode)) && (this.localName.Equals(localName) && this.namespaceName.Equals(ns)))
                {
                    this.localName = null;
                    return true;
                }
                return false;
            }
        }
    }
}

