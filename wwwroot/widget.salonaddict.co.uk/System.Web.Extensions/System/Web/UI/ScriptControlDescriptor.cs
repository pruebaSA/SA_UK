namespace System.Web.UI
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptControlDescriptor : ScriptComponentDescriptor
    {
        public ScriptControlDescriptor(string type, string elementID) : base(type, elementID)
        {
            base.RegisterDispose = false;
        }

        public override string ClientID =>
            this.ElementID;

        public string ElementID =>
            base.ElementIDInternal;

        public override string ID
        {
            get => 
                base.ID;
            set
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptControlDescriptor_IDNotSettable, new object[] { "ID", typeof(ScriptControlDescriptor).FullName }));
            }
        }
    }
}

