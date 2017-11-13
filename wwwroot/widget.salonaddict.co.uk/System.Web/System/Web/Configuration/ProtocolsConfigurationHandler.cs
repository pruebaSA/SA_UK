namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Xml;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProtocolsConfigurationHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContextObj, XmlNode section) => 
            new ProtocolsConfiguration(section);
    }
}

