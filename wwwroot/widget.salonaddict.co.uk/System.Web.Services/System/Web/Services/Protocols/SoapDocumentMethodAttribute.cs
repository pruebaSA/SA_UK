namespace System.Web.Services.Protocols
{
    using System;
    using System.Web.Services.Description;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SoapDocumentMethodAttribute : Attribute
    {
        private string action;
        private string binding;
        private bool oneWay;
        private string requestName;
        private string requestNamespace;
        private string responseName;
        private string responseNamespace;
        private SoapParameterStyle style;
        private SoapBindingUse use;

        public SoapDocumentMethodAttribute()
        {
        }

        public SoapDocumentMethodAttribute(string action)
        {
            this.action = action;
        }

        public string Action
        {
            get => 
                this.action;
            set
            {
                this.action = value;
            }
        }

        public string Binding
        {
            get
            {
                if (this.binding != null)
                {
                    return this.binding;
                }
                return string.Empty;
            }
            set
            {
                this.binding = value;
            }
        }

        public bool OneWay
        {
            get => 
                this.oneWay;
            set
            {
                this.oneWay = value;
            }
        }

        public SoapParameterStyle ParameterStyle
        {
            get => 
                this.style;
            set
            {
                this.style = value;
            }
        }

        public string RequestElementName
        {
            get
            {
                if (this.requestName != null)
                {
                    return this.requestName;
                }
                return string.Empty;
            }
            set
            {
                this.requestName = value;
            }
        }

        public string RequestNamespace
        {
            get => 
                this.requestNamespace;
            set
            {
                this.requestNamespace = value;
            }
        }

        public string ResponseElementName
        {
            get
            {
                if (this.responseName != null)
                {
                    return this.responseName;
                }
                return string.Empty;
            }
            set
            {
                this.responseName = value;
            }
        }

        public string ResponseNamespace
        {
            get => 
                this.responseNamespace;
            set
            {
                this.responseNamespace = value;
            }
        }

        public SoapBindingUse Use
        {
            get => 
                this.use;
            set
            {
                this.use = value;
            }
        }
    }
}

