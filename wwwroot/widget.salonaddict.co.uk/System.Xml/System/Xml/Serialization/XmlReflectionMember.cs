namespace System.Xml.Serialization
{
    using System;

    public class XmlReflectionMember
    {
        private bool isReturnValue;
        private string memberName;
        private bool overrideIsNullable;
        private System.Xml.Serialization.SoapAttributes soapAttributes = new System.Xml.Serialization.SoapAttributes();
        private Type type;
        private System.Xml.Serialization.XmlAttributes xmlAttributes = new System.Xml.Serialization.XmlAttributes();

        public bool IsReturnValue
        {
            get => 
                this.isReturnValue;
            set
            {
                this.isReturnValue = value;
            }
        }

        public string MemberName
        {
            get
            {
                if (this.memberName != null)
                {
                    return this.memberName;
                }
                return string.Empty;
            }
            set
            {
                this.memberName = value;
            }
        }

        public Type MemberType
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }

        public bool OverrideIsNullable
        {
            get => 
                this.overrideIsNullable;
            set
            {
                this.overrideIsNullable = value;
            }
        }

        public System.Xml.Serialization.SoapAttributes SoapAttributes
        {
            get => 
                this.soapAttributes;
            set
            {
                this.soapAttributes = value;
            }
        }

        public System.Xml.Serialization.XmlAttributes XmlAttributes
        {
            get => 
                this.xmlAttributes;
            set
            {
                this.xmlAttributes = value;
            }
        }
    }
}

