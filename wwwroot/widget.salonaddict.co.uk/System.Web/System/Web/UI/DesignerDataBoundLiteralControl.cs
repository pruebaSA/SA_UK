namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DesignerDataBoundLiteralControl : Control
    {
        private string _text;

        public DesignerDataBoundLiteralControl()
        {
            base.PreventAutoID();
        }

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                this._text = (string) savedState;
            }
        }

        protected internal override void Render(HtmlTextWriter output)
        {
            output.Write(this._text);
        }

        protected override object SaveViewState() => 
            this._text;

        public string Text
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

