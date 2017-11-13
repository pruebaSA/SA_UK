namespace System.Web.Services.Protocols
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public sealed class SoapHeaderAttribute : Attribute
    {
        private SoapHeaderDirection direction = SoapHeaderDirection.In;
        private string memberName;
        private bool required = true;

        public SoapHeaderAttribute(string memberName)
        {
            this.memberName = memberName;
        }

        public SoapHeaderDirection Direction
        {
            get => 
                this.direction;
            set
            {
                this.direction = value;
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

        [Obsolete("This property will be removed from a future version. The presence of a particular header in a SOAP message is no longer enforced", false)]
        public bool Required
        {
            get => 
                this.required;
            set
            {
                this.required = value;
            }
        }
    }
}

