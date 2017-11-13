namespace System.Web.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AdapterDictionary : OrderedDictionary
    {
        public string this[string key]
        {
            get => 
                ((string) base[key]);
            set
            {
                base[key] = value;
            }
        }
    }
}

