namespace System.Xml.Serialization
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class XmlEnumAttribute : Attribute
    {
        private string name;

        public XmlEnumAttribute()
        {
        }

        public XmlEnumAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

