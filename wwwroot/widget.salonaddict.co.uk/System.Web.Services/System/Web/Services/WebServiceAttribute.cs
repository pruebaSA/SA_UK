namespace System.Web.Services
{
    using System;

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public sealed class WebServiceAttribute : System.Attribute
    {
        public const string DefaultNamespace = "http://tempuri.org/";
        private string description;
        private string name;
        private string ns = "http://tempuri.org/";

        public string Description
        {
            get
            {
                if (this.description != null)
                {
                    return this.description;
                }
                return string.Empty;
            }
            set
            {
                this.description = value;
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
            get => 
                this.ns;
            set
            {
                this.ns = value;
            }
        }
    }
}

