﻿namespace System.Web.UI.Design.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls.WebParts;

    [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
    public class ProxyWebPartManagerDesigner : ControlDesigner
    {
        public override string GetDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml();

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(ProxyWebPartManager));
            base.Initialize(component);
        }

        protected override bool UsePreviewControl =>
            true;
    }
}

