namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PagerSettings : IStateManager
    {
        private bool _isTracking;
        private StateBag _viewState = new StateBag();

        [Browsable(false)]
        public event EventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, EventArgs.Empty);
            }
        }

        void IStateManager.LoadViewState(object state)
        {
            if (state != null)
            {
                ((IStateManager) this.ViewState).LoadViewState(state);
            }
        }

        object IStateManager.SaveViewState() => 
            ((IStateManager) this.ViewState).SaveViewState();

        void IStateManager.TrackViewState()
        {
            this._isTracking = true;
            this.ViewState.TrackViewState();
        }

        public override string ToString() => 
            string.Empty;

        [NotifyParentProperty(true), WebSysDescription("PagerSettings_FirstPageImageUrl"), WebCategory("Appearance"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string FirstPageImageUrl
        {
            get
            {
                object obj2 = this.ViewState["FirstPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (this.FirstPageImageUrl != value)
                {
                    this.ViewState["FirstPageImageUrl"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [NotifyParentProperty(true), WebSysDescription("PagerSettings_FirstPageText"), WebCategory("Appearance"), DefaultValue("&lt;&lt;")]
        public string FirstPageText
        {
            get
            {
                object obj2 = this.ViewState["FirstPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "&lt;&lt;";
            }
            set
            {
                if (this.FirstPageText != value)
                {
                    this.ViewState["FirstPageText"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        internal bool IsPagerOnBottom
        {
            get
            {
                PagerPosition position = this.Position;
                if (position != PagerPosition.Bottom)
                {
                    return (position == PagerPosition.TopAndBottom);
                }
                return true;
            }
        }

        internal bool IsPagerOnTop
        {
            get
            {
                PagerPosition position = this.Position;
                if (position != PagerPosition.Top)
                {
                    return (position == PagerPosition.TopAndBottom);
                }
                return true;
            }
        }

        [DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebSysDescription("PagerSettings_LastPageImageUrl"), NotifyParentProperty(true), WebCategory("Appearance"), UrlProperty]
        public string LastPageImageUrl
        {
            get
            {
                object obj2 = this.ViewState["LastPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (this.LastPageImageUrl != value)
                {
                    this.ViewState["LastPageImageUrl"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [NotifyParentProperty(true), WebSysDescription("PagerSettings_LastPageText"), WebCategory("Appearance"), DefaultValue("&gt;&gt;")]
        public string LastPageText
        {
            get
            {
                object obj2 = this.ViewState["LastPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "&gt;&gt;";
            }
            set
            {
                if (this.LastPageText != value)
                {
                    this.ViewState["LastPageText"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [NotifyParentProperty(true), WebSysDescription("PagerSettings_Mode"), DefaultValue(1), WebCategory("Appearance")]
        public PagerButtons Mode
        {
            get
            {
                object obj2 = this.ViewState["PagerMode"];
                if (obj2 != null)
                {
                    return (PagerButtons) obj2;
                }
                return PagerButtons.Numeric;
            }
            set
            {
                if ((value < PagerButtons.NextPrevious) || (value > PagerButtons.NumericFirstLast))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (this.Mode != value)
                {
                    this.ViewState["PagerMode"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [WebSysDescription("PagerSettings_NextPageImageUrl"), WebCategory("Appearance"), DefaultValue(""), NotifyParentProperty(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string NextPageImageUrl
        {
            get
            {
                object obj2 = this.ViewState["NextPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (this.NextPageImageUrl != value)
                {
                    this.ViewState["NextPageImageUrl"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [WebCategory("Appearance"), WebSysDescription("PagerSettings_NextPageText"), DefaultValue("&gt;"), NotifyParentProperty(true)]
        public string NextPageText
        {
            get
            {
                object obj2 = this.ViewState["NextPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "&gt;";
            }
            set
            {
                if (this.NextPageText != value)
                {
                    this.ViewState["NextPageText"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [DefaultValue(10), WebCategory("Behavior"), WebSysDescription("PagerSettings_PageButtonCount"), NotifyParentProperty(true)]
        public int PageButtonCount
        {
            get
            {
                object obj2 = this.ViewState["PageButtonCount"];
                if (obj2 != null)
                {
                    return (int) obj2;
                }
                return 10;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (this.PageButtonCount != value)
                {
                    this.ViewState["PageButtonCount"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [WebCategory("Layout"), WebSysDescription("PagerStyle_Position"), DefaultValue(0), NotifyParentProperty(true)]
        public PagerPosition Position
        {
            get
            {
                object obj2 = this.ViewState["Position"];
                if (obj2 != null)
                {
                    return (PagerPosition) obj2;
                }
                return PagerPosition.Bottom;
            }
            set
            {
                if ((value < PagerPosition.Bottom) || (value > PagerPosition.TopAndBottom))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["Position"] = value;
            }
        }

        [NotifyParentProperty(true), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebSysDescription("PagerSettings_PreviousPageImageUrl"), WebCategory("Appearance")]
        public string PreviousPageImageUrl
        {
            get
            {
                object obj2 = this.ViewState["PreviousPageImageUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                if (this.PreviousPageImageUrl != value)
                {
                    this.ViewState["PreviousPageImageUrl"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        [WebCategory("Appearance"), WebSysDescription("PagerSettings_PreviousPageText"), DefaultValue("&lt;"), NotifyParentProperty(true)]
        public string PreviousPageText
        {
            get
            {
                object obj2 = this.ViewState["PreviousPageText"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "&lt;";
            }
            set
            {
                if (this.PreviousPageText != value)
                {
                    this.ViewState["PreviousPageText"] = value;
                    this.OnPropertyChanged();
                }
            }
        }

        bool IStateManager.IsTrackingViewState =>
            this._isTracking;

        private StateBag ViewState =>
            this._viewState;

        [WebCategory("Appearance"), DefaultValue(true), NotifyParentProperty(true), WebSysDescription("PagerStyle_Visible")]
        public bool Visible
        {
            get
            {
                object obj2 = this.ViewState["PagerVisible"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["PagerVisible"] = value;
            }
        }
    }
}

