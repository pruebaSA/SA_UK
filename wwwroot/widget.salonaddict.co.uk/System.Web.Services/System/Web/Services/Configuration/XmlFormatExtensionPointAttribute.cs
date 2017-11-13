namespace System.Web.Services.Configuration
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionPointAttribute : Attribute
    {
        private bool allowElements = true;
        private string name;

        public XmlFormatExtensionPointAttribute(string memberName)
        {
            this.name = memberName;
        }

        public bool AllowElements
        {
            get => 
                this.allowElements;
            set
            {
                this.allowElements = value;
            }
        }

        public string MemberName
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }
                return string.Empty;
            }
            set
            {
                this.name = value;
            }
        }
    }
}

