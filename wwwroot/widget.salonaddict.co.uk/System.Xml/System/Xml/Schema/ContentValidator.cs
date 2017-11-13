namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class ContentValidator
    {
        public static readonly ContentValidator Any = new ContentValidator(XmlSchemaContentType.Mixed, true, true);
        private XmlSchemaContentType contentType;
        public static readonly ContentValidator Empty = new ContentValidator(XmlSchemaContentType.Empty);
        private bool isEmptiable;
        private bool isOpen;
        public static readonly ContentValidator Mixed = new ContentValidator(XmlSchemaContentType.Mixed);
        public static readonly ContentValidator TextOnly = new ContentValidator(XmlSchemaContentType.TextOnly, false, false);

        public ContentValidator(XmlSchemaContentType contentType)
        {
            this.contentType = contentType;
            this.isEmptiable = true;
        }

        protected ContentValidator(XmlSchemaContentType contentType, bool isOpen, bool isEmptiable)
        {
            this.contentType = contentType;
            this.isOpen = isOpen;
            this.isEmptiable = isEmptiable;
        }

        public virtual bool CompleteValidation(ValidationState context) => 
            true;

        public virtual ArrayList ExpectedElements(ValidationState context, bool isRequiredOnly) => 
            null;

        public virtual ArrayList ExpectedParticles(ValidationState context, bool isRequiredOnly) => 
            null;

        public virtual void InitValidation(ValidationState context)
        {
        }

        public virtual object ValidateElement(XmlQualifiedName name, ValidationState context, out int errorCode)
        {
            if ((this.contentType == XmlSchemaContentType.TextOnly) || (this.contentType == XmlSchemaContentType.Empty))
            {
                context.NeedValidateChildren = false;
            }
            errorCode = -1;
            return null;
        }

        public XmlSchemaContentType ContentType =>
            this.contentType;

        public virtual bool IsEmptiable =>
            this.isEmptiable;

        public bool IsOpen
        {
            get => 
                (((this.contentType != XmlSchemaContentType.TextOnly) && (this.contentType != XmlSchemaContentType.Empty)) && this.isOpen);
            set
            {
                this.isOpen = value;
            }
        }

        public bool PreserveWhitespace
        {
            get
            {
                if (this.contentType != XmlSchemaContentType.TextOnly)
                {
                    return (this.contentType == XmlSchemaContentType.Mixed);
                }
                return true;
            }
        }
    }
}

