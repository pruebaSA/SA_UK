namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [ToolboxItem(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LiteralControl : Control, ITextControl
    {
        internal string _text;

        public LiteralControl()
        {
            base.PreventAutoID();
            base.SetEnableViewStateInternal(false);
        }

        public LiteralControl(string text) : this()
        {
            this._text = (text != null) ? text : string.Empty;
        }

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        internal override void InitRecursive(Control namingContainer)
        {
            this.ResolveAdapter();
            if (base._adapter != null)
            {
                base._adapter.OnInit(EventArgs.Empty);
            }
            else
            {
                this.OnInit(EventArgs.Empty);
            }
        }

        internal override void LoadRecursive()
        {
            if (base._adapter != null)
            {
                base._adapter.OnLoad(EventArgs.Empty);
            }
            else
            {
                this.OnLoad(EventArgs.Empty);
            }
        }

        internal override void PreRenderRecursiveInternal()
        {
            if (base._adapter != null)
            {
                base._adapter.OnPreRender(EventArgs.Empty);
            }
            else
            {
                this.OnPreRender(EventArgs.Empty);
            }
        }

        protected internal override void Render(HtmlTextWriter output)
        {
            output.Write(this._text);
        }

        internal override void UnloadRecursive(bool dispose)
        {
            if (base._adapter != null)
            {
                base._adapter.OnUnload(EventArgs.Empty);
            }
            else
            {
                this.OnUnload(EventArgs.Empty);
            }
            if (dispose)
            {
                this.Dispose();
            }
        }

        public virtual string Text
        {
            get => 
                this._text;
            set
            {
                this._text = (value != null) ? value : string.Empty;
            }
        }
    }
}

