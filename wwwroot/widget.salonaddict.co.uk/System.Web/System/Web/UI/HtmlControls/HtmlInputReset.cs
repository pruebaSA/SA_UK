namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultEvent(""), SupportsEventValidation, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlInputReset : HtmlInputButton
    {
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler ServerClick
        {
            add
            {
                base.ServerClick += value;
            }
            remove
            {
                base.ServerClick -= value;
            }
        }

        public HtmlInputReset() : base("reset")
        {
        }

        public HtmlInputReset(string type) : base(type)
        {
        }

        internal override void RenderAttributesInternal(HtmlTextWriter writer)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool CausesValidation
        {
            get => 
                base.CausesValidation;
            set
            {
                base.CausesValidation = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string ValidationGroup
        {
            get => 
                base.ValidationGroup;
            set
            {
                base.ValidationGroup = value;
            }
        }
    }
}

