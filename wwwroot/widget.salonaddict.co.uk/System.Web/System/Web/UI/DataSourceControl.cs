namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [Bindable(false), Designer("System.Web.UI.Design.DataSourceDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), NonVisualControl, ControlBuilder(typeof(DataSourceControlBuilder)), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class DataSourceControl : Control, IDataSource, IListSource
    {
        private static readonly object EventDataSourceChanged = new object();
        private static readonly object EventDataSourceChangedInternal = new object();

        internal event EventHandler DataSourceChangedInternal
        {
            add
            {
                base.Events.AddHandler(EventDataSourceChangedInternal, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventDataSourceChangedInternal, value);
            }
        }

        event EventHandler IDataSource.DataSourceChanged
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

        protected DataSourceControl()
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

        protected abstract DataSourceView GetView(string viewName);
        protected virtual ICollection GetViewNames() => 
            null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool HasControls() => 
            base.HasControls();

        private void OnDataSourceChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventDataSourceChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnDataSourceChangedInternal(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventDataSourceChangedInternal];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseDataSourceChangedEvent(EventArgs e)
        {
            this.OnDataSourceChangedInternal(e);
            this.OnDataSourceChanged(e);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RenderControl(HtmlTextWriter writer)
        {
            base.RenderControl(writer);
        }

        IList IListSource.GetList()
        {
            if (base.DesignMode)
            {
                return null;
            }
            return ListSourceHelper.GetList(this);
        }

        DataSourceView IDataSource.GetView(string viewName) => 
            this.GetView(viewName);

        ICollection IDataSource.GetViewNames() => 
            this.GetViewNames();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ClientID =>
            base.ClientID;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DefaultValue(false)]
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

        bool IListSource.ContainsListCollection
        {
            get
            {
                if (base.DesignMode)
                {
                    return false;
                }
                return ListSourceHelper.ContainsListCollection(this);
            }
        }

        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
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

