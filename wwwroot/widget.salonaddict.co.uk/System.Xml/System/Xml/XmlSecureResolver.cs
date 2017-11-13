namespace System.Xml
{
    using System;
    using System.Net;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class XmlSecureResolver : XmlResolver
    {
        private PermissionSet permissionSet;
        private XmlResolver resolver;

        public XmlSecureResolver(XmlResolver resolver, PermissionSet permissionSet)
        {
            this.resolver = resolver;
            this.permissionSet = permissionSet;
        }

        public XmlSecureResolver(XmlResolver resolver, Evidence evidence) : this(resolver, SecurityManager.ResolvePolicy(evidence))
        {
        }

        public XmlSecureResolver(XmlResolver resolver, string securityUrl) : this(resolver, CreateEvidenceForUrl(securityUrl))
        {
        }

        public static Evidence CreateEvidenceForUrl(string securityUrl)
        {
            Evidence evidence = new Evidence();
            if ((securityUrl != null) && (securityUrl.Length > 0))
            {
                evidence.AddHost(new Url(securityUrl));
                evidence.AddHost(Zone.CreateFromUrl(securityUrl));
                Uri uri = new Uri(securityUrl, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri && !uri.IsFile)
                {
                    evidence.AddHost(Site.CreateFromUrl(securityUrl));
                }
            }
            return evidence;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            this.permissionSet.PermitOnly();
            return this.resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri) => 
            this.resolver.ResolveUri(baseUri, relativeUri);

        public override ICredentials Credentials
        {
            set
            {
                this.resolver.Credentials = value;
            }
        }
    }
}

