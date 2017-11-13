namespace System.Xml.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class XmlSerializerVersionAttribute : Attribute
    {
        private string mvid;
        private string ns;
        private string serializerVersion;
        private System.Type type;

        public XmlSerializerVersionAttribute()
        {
        }

        public XmlSerializerVersionAttribute(System.Type type)
        {
            this.type = type;
        }

        public string Namespace
        {
            get => 
                this.ns;
            set
            {
                this.ns = value;
            }
        }

        public string ParentAssemblyId
        {
            get => 
                this.mvid;
            set
            {
                this.mvid = value;
            }
        }

        public System.Type Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }

        public string Version
        {
            get => 
                this.serializerVersion;
            set
            {
                this.serializerVersion = value;
            }
        }
    }
}

