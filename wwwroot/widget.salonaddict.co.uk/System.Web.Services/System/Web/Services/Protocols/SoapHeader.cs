namespace System.Web.Services.Protocols
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web.Services;
    using System.Xml.Serialization;

    [SoapType(IncludeInSchema=false), XmlType(IncludeInSchema=false)]
    public abstract class SoapHeader
    {
        private string actor;
        private bool didUnderstand;
        private bool mustUnderstand;
        private bool relay;
        internal SoapProtocolVersion version;

        protected SoapHeader()
        {
        }

        [XmlAttribute("actor", Namespace="http://schemas.xmlsoap.org/soap/envelope/"), SoapAttribute("actor", Namespace="http://schemas.xmlsoap.org/soap/envelope/"), DefaultValue("")]
        public string Actor
        {
            get
            {
                if (this.version == SoapProtocolVersion.Soap12)
                {
                    return "";
                }
                return this.InternalActor;
            }
            set
            {
                this.InternalActor = value;
            }
        }

        [XmlIgnore, SoapIgnore]
        public bool DidUnderstand
        {
            get => 
                this.didUnderstand;
            set
            {
                this.didUnderstand = value;
            }
        }

        [SoapAttribute("mustUnderstand", Namespace="http://schemas.xmlsoap.org/soap/envelope/"), DefaultValue("0"), XmlAttribute("mustUnderstand", Namespace="http://schemas.xmlsoap.org/soap/envelope/")]
        public string EncodedMustUnderstand
        {
            get
            {
                if ((this.version != SoapProtocolVersion.Soap12) && this.MustUnderstand)
                {
                    return "1";
                }
                return "0";
            }
            set
            {
                switch (value)
                {
                    case "false":
                    case "0":
                        this.MustUnderstand = false;
                        return;

                    case "true":
                    case "1":
                        this.MustUnderstand = true;
                        return;
                }
                throw new ArgumentException(Res.GetString("WebHeaderInvalidMustUnderstand", new object[] { value }));
            }
        }

        [XmlAttribute("mustUnderstand", Namespace="http://www.w3.org/2003/05/soap-envelope"), SoapAttribute("mustUnderstand", Namespace="http://www.w3.org/2003/05/soap-envelope"), DefaultValue("0"), ComVisible(false)]
        public string EncodedMustUnderstand12
        {
            get
            {
                if ((this.version != SoapProtocolVersion.Soap11) && this.MustUnderstand)
                {
                    return "1";
                }
                return "0";
            }
            set
            {
                this.EncodedMustUnderstand = value;
            }
        }

        [SoapAttribute("relay", Namespace="http://www.w3.org/2003/05/soap-envelope"), DefaultValue("0"), XmlAttribute("relay", Namespace="http://www.w3.org/2003/05/soap-envelope"), ComVisible(false)]
        public string EncodedRelay
        {
            get
            {
                if ((this.version != SoapProtocolVersion.Soap11) && this.Relay)
                {
                    return "1";
                }
                return "0";
            }
            set
            {
                switch (value)
                {
                    case "false":
                    case "0":
                        this.Relay = false;
                        return;

                    case "true":
                    case "1":
                        this.Relay = true;
                        return;
                }
                throw new ArgumentException(Res.GetString("WebHeaderInvalidRelay", new object[] { value }));
            }
        }

        internal virtual string InternalActor
        {
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            get
            {
                if (this.actor != null)
                {
                    return this.actor;
                }
                return string.Empty;
            }
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            set
            {
                this.actor = value;
            }
        }

        internal virtual bool InternalMustUnderstand
        {
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            get => 
                this.mustUnderstand;
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            set
            {
                this.mustUnderstand = value;
            }
        }

        internal virtual bool InternalRelay
        {
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            get => 
                this.relay;
            [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
            set
            {
                this.relay = value;
            }
        }

        [SoapIgnore, XmlIgnore]
        public bool MustUnderstand
        {
            get => 
                this.InternalMustUnderstand;
            set
            {
                this.InternalMustUnderstand = value;
            }
        }

        [ComVisible(false), XmlIgnore, SoapIgnore]
        public bool Relay
        {
            get => 
                this.InternalRelay;
            set
            {
                this.InternalRelay = value;
            }
        }

        [ComVisible(false), XmlAttribute("role", Namespace="http://www.w3.org/2003/05/soap-envelope"), DefaultValue(""), SoapAttribute("role", Namespace="http://www.w3.org/2003/05/soap-envelope")]
        public string Role
        {
            get
            {
                if (this.version == SoapProtocolVersion.Soap11)
                {
                    return "";
                }
                return this.InternalActor;
            }
            set
            {
                this.InternalActor = value;
            }
        }
    }
}

