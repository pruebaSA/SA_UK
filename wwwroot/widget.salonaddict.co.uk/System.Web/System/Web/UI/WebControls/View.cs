namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ParseChildren(false), Designer("System.Web.UI.Design.WebControls.ViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxData("<{0}:View runat=\"server\"></{0}:View>"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class View : Control
    {
        private bool _active;
        private static readonly object _eventActivate = new object();
        private static readonly object _eventDeactivate = new object();

        [WebSysDescription("View_Activate"), WebCategory("Action")]
        public event EventHandler Activate
        {
            add
            {
                base.Events.AddHandler(_eventActivate, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventActivate, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("View_Deactivate")]
        public event EventHandler Deactivate
        {
            add
            {
                base.Events.AddHandler(_eventDeactivate, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventDeactivate, value);
            }
        }

        protected internal virtual void OnActivate(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[_eventActivate];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal virtual void OnDeactivate(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[_eventDeactivate];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal bool Active
        {
            get => 
                this._active;
            set
            {
                this._active = value;
                base.Visible = true;
            }
        }

        [Browsable(true)]
        public override bool EnableTheming
        {
            get => 
                base.EnableTheming;
            set
            {
                base.EnableTheming = value;
            }
        }

        [Browsable(false), WebCategory("Behavior"), WebSysDescription("Control_Visible"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Visible
        {
            get
            {
                if (this.Parent == null)
                {
                    return this.Active;
                }
                return (this.Active && this.Parent.Visible);
            }
            set
            {
                if (!base.DesignMode)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("View_CannotSetVisible"));
                }
            }
        }
    }
}

