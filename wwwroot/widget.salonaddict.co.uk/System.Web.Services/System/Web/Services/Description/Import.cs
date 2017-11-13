﻿namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtensionPoint("Extensions")]
    public sealed class Import : DocumentableItem
    {
        private ServiceDescriptionFormatExtensionCollection extensions;
        private string location;
        private string ns;
        private System.Web.Services.Description.ServiceDescription parent;

        internal void SetParent(System.Web.Services.Description.ServiceDescription parent)
        {
            this.parent = parent;
        }

        [XmlIgnore]
        public override ServiceDescriptionFormatExtensionCollection Extensions
        {
            get
            {
                if (this.extensions == null)
                {
                    this.extensions = new ServiceDescriptionFormatExtensionCollection(this);
                }
                return this.extensions;
            }
        }

        [XmlAttribute("location")]
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

        [XmlAttribute("namespace")]
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

        public System.Web.Services.Description.ServiceDescription ServiceDescription =>
            this.parent;
    }
}

