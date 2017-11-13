namespace System.Web.Services
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple=true)]
    public sealed class WebServiceBindingAttribute : System.Attribute
    {
        private WsiProfiles claims;
        private bool emitClaims;
        private string location;
        private string name;
        private string ns;

        public WebServiceBindingAttribute()
        {
        }

        public WebServiceBindingAttribute(string name)
        {
            this.name = name;
        }

        public WebServiceBindingAttribute(string name, string ns)
        {
            this.name = name;
            this.ns = ns;
        }

        public WebServiceBindingAttribute(string name, string ns, string location)
        {
            this.name = name;
            this.ns = ns;
            this.location = location;
        }

        public WsiProfiles ConformsTo
        {
            get => 
                this.claims;
            set
            {
                this.claims = value;
            }
        }

        public bool EmitConformanceClaims
        {
            get => 
                this.emitClaims;
            set
            {
                this.emitClaims = value;
            }
        }

        public string Location
        {
            get
            {
                if (this.location != null)
                {
                    return this.location;
                }
                return string.Empty;
            }
            set
            {
                this.location = value;
            }
        }

        public string Name
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

        public string Namespace
        {
            get
            {
                if (this.ns != null)
                {
                    return this.ns;
                }
                return string.Empty;
            }
            set
            {
                this.ns = value;
            }
        }
    }
}

