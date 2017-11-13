namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [Designer("System.Web.UI.Design.HierarchicalDataSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Bindable(false), ControlBuilder(typeof(DataSourceControlBuilder)), NonVisualControl, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class HierarchicalDataSourceControl : Control, IHierarchicalDataSource
    {
        private static readonly object EventDataSourceChanged = new object();

        event EventHandler IHierarchicalDataSource.DataSourceChanged
        {
            add
            {
                base.Events.AddHandler(EventDataSourceChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDataSourceChanged, value);
            }
        }

        protected HierarchicalDataSourceControl()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ApplyStyleSheetSkin(Page page)
        {
            base.ApplyStyleSheetSkin(page);
        }

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Control FindControl(string id) => 
            base.FindControl(id);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Focus()
        {
            throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
        }

        protected abstract HierarchicalDataSourceView GetHierarchicalView(string viewPath);
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool HasControls() => 
            base.HasControls();

        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventDataSourceChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);
        }

        HierarchicalDataSourceView IHierarchicalDataSource.GetHierarchicalView(string viewPath) => 
            this.GetHierarchicalView(viewPath);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ClientID =>
            base.ClientID;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool EnableTheming
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string SkinID
        {
            get => 
                string.Empty;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Visible
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("ControlNonVisual", new object[] { base.GetType().Name }));
            }
        }
    }
}

