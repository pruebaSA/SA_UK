namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Web.Resources;
    using System.Xml.Serialization;

    internal class MetadataSource
    {
        private string m_Address;
        private string m_Protocol;
        private int m_SourceId;

        public MetadataSource()
        {
            this.m_Address = string.Empty;
            this.m_Protocol = string.Empty;
        }

        public MetadataSource(string protocol, string address, int sourceId)
        {
            if (protocol == null)
            {
                throw new ArgumentNullException("protocol");
            }
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            if (protocol.Length == 0)
            {
                throw new ArgumentException(WCFModelStrings.ReferenceGroup_EmptyProtocol);
            }
            if (address == null)
            {
                throw new ArgumentException(WCFModelStrings.ReferenceGroup_EmptyAddress);
            }
            this.m_Protocol = protocol;
            this.m_Address = address;
            if (sourceId < 0)
            {
                throw new ArgumentException(WCFModelStrings.ReferenceGroup_InvalidSourceId);
            }
            this.m_SourceId = sourceId;
        }

        [XmlAttribute]
        public string Address
        {
            get => 
                this.m_Address;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.m_Address = value;
            }
        }

        [XmlAttribute]
        public string Protocol
        {
            get => 
                this.m_Protocol;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.m_Protocol = value;
            }
        }

        [XmlAttribute]
        public int SourceId
        {
            get => 
                this.m_SourceId;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(WCFModelStrings.ReferenceGroup_InvalidSourceId);
                }
                this.m_SourceId = value;
            }
        }
    }
}

