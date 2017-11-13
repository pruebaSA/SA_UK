namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class UserControlControlBuilder : ControlBuilder
    {
        private string _innerText;

        public override object BuildObject()
        {
            object obj2 = base.BuildObject();
            if (base.InDesigner)
            {
                IUserControlDesignerAccessor accessor = (IUserControlDesignerAccessor) obj2;
                accessor.TagName = base.TagName;
                if (this._innerText != null)
                {
                    accessor.InnerText = this._innerText;
                }
            }
            return obj2;
        }

        public override bool NeedsTagInnerText() => 
            base.InDesigner;

        public override void SetTagInnerText(string text)
        {
            this._innerText = text;
        }
    }
}

