namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class TemplatePagerField : DataPagerField
    {
        private EventHandlerList _events;
        private int _maximumRows;
        private ITemplate _pagerTemplate;
        private int _startRowIndex;
        private int _totalRowCount;
        private static readonly object EventPagerCommand = new object();

        [ResourceDescription("TemplatePagerField_OnPagerCommand"), Category("Action")]
        public event EventHandler<DataPagerCommandEventArgs> PagerCommand
        {
            add
            {
                this.Events.AddHandler(EventPagerCommand, value);
            }
            remove
            {
                this.Events.RemoveHandler(EventPagerCommand, value);
            }
        }

        protected override void CopyProperties(DataPagerField newField)
        {
            ((TemplatePagerField) newField).PagerTemplate = this.PagerTemplate;
            base.CopyProperties(newField);
        }

        public override void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex)
        {
            this._startRowIndex = startRowIndex;
            this._maximumRows = maximumRows;
            this._totalRowCount = totalRowCount;
            if (this._pagerTemplate != null)
            {
                this._pagerTemplate.InstantiateIn(container);
            }
        }

        protected override DataPagerField CreateField() => 
            new TemplatePagerField();

        public override void HandleEvent(CommandEventArgs e)
        {
            DataPagerFieldItem item = null;
            DataPagerFieldCommandEventArgs args = e as DataPagerFieldCommandEventArgs;
            if (args != null)
            {
                item = args.Item;
            }
            DataPagerCommandEventArgs args2 = new DataPagerCommandEventArgs(this, this._totalRowCount, e, item);
            this.OnPagerCommand(args2);
            if (args2.NewStartRowIndex != -1)
            {
                base.DataPager.SetPageProperties(args2.NewStartRowIndex, args2.NewMaximumRows, true);
            }
        }

        protected virtual void OnPagerCommand(DataPagerCommandEventArgs e)
        {
            EventHandler<DataPagerCommandEventArgs> handler = (EventHandler<DataPagerCommandEventArgs>) this.Events[EventPagerCommand];
            if (handler == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.TemplatePagerField_UnhandledEvent, new object[] { "PagerCommand" }));
            }
            handler(this, e);
        }

        private EventHandlerList Events
        {
            get
            {
                if (this._events == null)
                {
                    this._events = new EventHandlerList();
                }
                return this._events;
            }
        }

        [TemplateContainer(typeof(DataPagerFieldItem), BindingDirection.TwoWay), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), DefaultValue((string) null), ResourceDescription("TemplatePagerField_PagerTemplate")]
        public virtual ITemplate PagerTemplate
        {
            get => 
                this._pagerTemplate;
            set
            {
                this._pagerTemplate = value;
                this.OnFieldChanged();
            }
        }
    }
}

