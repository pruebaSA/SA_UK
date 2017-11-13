namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class DataPagerField : IStateManager
    {
        private System.Web.UI.WebControls.DataPager _dataPager;
        private StateBag _stateBag = new StateBag();
        private bool _trackViewState;

        internal event EventHandler FieldChanged;

        protected DataPagerField()
        {
        }

        protected internal DataPagerField CloneField()
        {
            DataPagerField newField = this.CreateField();
            this.CopyProperties(newField);
            return newField;
        }

        protected virtual void CopyProperties(DataPagerField newField)
        {
            newField.Visible = this.Visible;
        }

        public abstract void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex);
        protected abstract DataPagerField CreateField();
        protected string GetQueryStringNavigateUrl(int pageNumber) => 
            this.DataPager.GetQueryStringNavigateUrl(pageNumber);

        public abstract void HandleEvent(CommandEventArgs e);
        protected virtual void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] objArray = (object[]) savedState;
                if (objArray[0] != null)
                {
                    ((IStateManager) this.ViewState).LoadViewState(objArray[0]);
                }
            }
        }

        protected virtual void OnFieldChanged()
        {
            if (this.FieldChanged != null)
            {
                this.FieldChanged(this, EventArgs.Empty);
            }
        }

        protected virtual object SaveViewState()
        {
            object obj2 = ((IStateManager) this.ViewState).SaveViewState();
            if (obj2 != null)
            {
                return new object[] { obj2 };
            }
            return null;
        }

        internal void SetDataPager(System.Web.UI.WebControls.DataPager dataPager)
        {
            this._dataPager = dataPager;
        }

        internal void SetDirty()
        {
            this._stateBag.SetDirty(true);
        }

        void IStateManager.LoadViewState(object state)
        {
            this.LoadViewState(state);
        }

        object IStateManager.SaveViewState() => 
            this.SaveViewState();

        void IStateManager.TrackViewState()
        {
            this.TrackViewState();
        }

        protected virtual void TrackViewState()
        {
            this._trackViewState = true;
            ((IStateManager) this.ViewState).TrackViewState();
        }

        protected System.Web.UI.WebControls.DataPager DataPager =>
            this._dataPager;

        protected bool IsTrackingViewState =>
            this._trackViewState;

        protected bool QueryStringHandled
        {
            get => 
                this.DataPager.QueryStringHandled;
            set
            {
                this.DataPager.QueryStringHandled = value;
            }
        }

        protected string QueryStringValue =>
            this.DataPager.QueryStringValue;

        bool IStateManager.IsTrackingViewState =>
            this.IsTrackingViewState;

        protected StateBag ViewState =>
            this._stateBag;

        [Category("Behavior"), DefaultValue(true), ResourceDescription("DataPagerField_Visible")]
        public bool Visible
        {
            get
            {
                object obj2 = this.ViewState["Visible"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                if (value != this.Visible)
                {
                    this.ViewState["Visible"] = value;
                    this.OnFieldChanged();
                }
            }
        }
    }
}

