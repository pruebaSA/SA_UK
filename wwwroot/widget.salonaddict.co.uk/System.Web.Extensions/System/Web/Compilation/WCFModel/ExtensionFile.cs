namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Xml.Serialization;

    internal class ExtensionFile : ExternalFile
    {
        private byte[] m_ContentBuffer;
        private string m_Name;

        public ExtensionFile()
        {
            this.m_Name = string.Empty;
        }

        public ExtensionFile(string name, string fileName, byte[] content) : base(fileName)
        {
            this.Name = name;
            this.m_ContentBuffer = content;
            base.IsExistingFile = false;
        }

        internal void CleanUpContent()
        {
            base.ErrorInLoading = null;
            this.m_ContentBuffer = null;
        }

        [XmlIgnore]
        public byte[] ContentBuffer
        {
            get => 
                this.m_ContentBuffer;
            set
            {
                this.m_ContentBuffer = value;
                base.ErrorInLoading = null;
            }
        }

        internal bool IsBufferValid =>
            (this.m_ContentBuffer != null);

        [XmlAttribute]
        public string Name
        {
            get => 
                this.m_Name;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.m_Name = value;
            }
        }
    }
}

