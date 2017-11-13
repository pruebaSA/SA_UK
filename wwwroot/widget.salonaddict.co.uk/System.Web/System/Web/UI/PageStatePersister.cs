namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class PageStatePersister
    {
        private object _controlState;
        private System.Web.UI.Page _page;
        private IStateFormatter _stateFormatter;
        private object _viewState;

        protected PageStatePersister(System.Web.UI.Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page", System.Web.SR.GetString("PageStatePersister_PageCannotBeNull"));
            }
            this._page = page;
        }

        public abstract void Load();
        public abstract void Save();

        public object ControlState
        {
            get => 
                this._controlState;
            set
            {
                this._controlState = value;
            }
        }

        protected System.Web.UI.Page Page
        {
            get => 
                this._page;
            set
            {
                this._page = value;
            }
        }

        protected IStateFormatter StateFormatter
        {
            get
            {
                if (this._stateFormatter == null)
                {
                    this._stateFormatter = this.Page.CreateStateFormatter();
                }
                return this._stateFormatter;
            }
        }

        public object ViewState
        {
            get => 
                this._viewState;
            set
            {
                this._viewState = value;
            }
        }
    }
}

