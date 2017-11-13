namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ParseChildren(false), PersistChildren(true), NonVisualControl, ToolboxBitmap(typeof(Rating), "Rating.Rating.ico"), ToolboxData("<{0}:Rating runat=\"server\"></{0}:Rating>"), Designer(typeof(RatingDesigner))]
    public class Rating : Panel, ICallbackEventHandler, IPostBackEventHandler
    {
        private Orientation _align;
        private AjaxControlToolkit.RatingDirection _direction;
        private RatingExtender _extender;
        private string _returnFromEvent;
        private static readonly object EventChange = new object();

        public event RatingEventHandler Changed
        {
            add
            {
                base.Events.AddHandler(EventChange, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventChange, value);
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            this._extender = new RatingExtender();
            if (!base.DesignMode)
            {
                this.Controls.Add(this._extender);
            }
        }

        public string GetCallbackResult() => 
            this._returnFromEvent;

        protected virtual void OnChanged(RatingEventArgs e)
        {
            RatingEventHandler handler = (RatingEventHandler) base.Events[EventChange];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Page.ClientScript.GetCallbackEventReference(this, "", "", "");
            this.EnsureChildControls();
            this._extender.CallbackID = this.UniqueID;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            RatingEventArgs e = new RatingEventArgs(eventArgument);
            this.OnChanged(e);
            this._returnFromEvent = e.CallbackResult;
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            RatingEventArgs e = new RatingEventArgs(eventArgument);
            this.OnChanged(e);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            int currentRating = this.CurrentRating;
            int maxRating = this.MaxRating;
            writer.AddAttribute("href", "javascript:void(0)");
            writer.AddAttribute("style", "text-decoration:none");
            writer.AddAttribute("id", this.ClientID + "_A");
            writer.AddAttribute("title", currentRating.ToString(CultureInfo.CurrentCulture));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            for (int i = 1; i < (this.MaxRating + 1); i++)
            {
                writer.AddAttribute("id", this.ClientID + "_Star_" + i.ToString(CultureInfo.InvariantCulture));
                if (this._align == Orientation.Horizontal)
                {
                    writer.AddStyleAttribute("float", "left");
                }
                if (this._direction == AjaxControlToolkit.RatingDirection.LeftToRightTopToBottom)
                {
                    if (i <= currentRating)
                    {
                        writer.AddAttribute("class", this.StarCssClass + " " + this.FilledStarCssClass);
                    }
                    else
                    {
                        writer.AddAttribute("class", this.StarCssClass + " " + this.EmptyStarCssClass);
                    }
                }
                else if (i <= (maxRating - currentRating))
                {
                    writer.AddAttribute("class", this.StarCssClass + " " + this.EmptyStarCssClass);
                }
                else
                {
                    writer.AddAttribute("class", this.StarCssClass + " " + this.FilledStarCssClass);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write("&nbsp;");
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        [DefaultValue(false), Category("Behavior"), Description("True to cause a postback on rating change")]
        public bool AutoPostBack
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.AutoPostBack;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.AutoPostBack = value;
            }
        }

        [DefaultValue(""), Browsable(true), Category("Behavior"), Description("BehaviorID")]
        public string BehaviorID
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.BehaviorID;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.BehaviorID = value;
            }
        }

        [Browsable(true), DefaultValue(3), Category("Behavior"), Description("Rating"), Bindable(true, BindingDirection.TwoWay)]
        public int CurrentRating
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.Rating;
            }
            set
            {
                if (value > this.MaxRating)
                {
                    throw new ArgumentOutOfRangeException("CurrentRating", "CurrentRating must be greater than MaxRating");
                }
                this.EnsureChildControls();
                this._extender.Rating = value;
            }
        }

        [Browsable(true), Themeable(true), Category("Behavior"), Description("EmptyStarCssClass"), DefaultValue("")]
        public string EmptyStarCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.EmptyStarCssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.EmptyStarCssClass = value;
            }
        }

        [Browsable(true), Themeable(true), Category("Behavior"), Description("FilledStarCssClass"), DefaultValue("")]
        public string FilledStarCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.FilledStarCssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.FilledStarCssClass = value;
            }
        }

        public override string ID
        {
            get => 
                base.ID;
            set
            {
                base.ID = value;
                this.EnsureChildControls();
                this._extender.ID = value + "_RatingExtender";
                this._extender.TargetControlID = value;
            }
        }

        [Category("Behavior"), Browsable(true), Description("MaxRating"), DefaultValue(5), Bindable(true, BindingDirection.TwoWay)]
        public int MaxRating
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.MaxRating;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("MaxRating", "MaxRating must be greater than zero");
                }
                this.EnsureChildControls();
                this._extender.MaxRating = value;
                if (this.CurrentRating > value)
                {
                    this.CurrentRating = this.MaxRating;
                }
            }
        }

        [Browsable(true), DefaultValue(0), Themeable(true), Category("Appearance"), Description("Rating Align")]
        public Orientation RatingAlign
        {
            get => 
                this._align;
            set
            {
                this._align = value;
            }
        }

        [DefaultValue(0), Browsable(true), Themeable(true), Category("Appearance"), Description("Rating Direction")]
        public AjaxControlToolkit.RatingDirection RatingDirection
        {
            get
            {
                this.EnsureChildControls();
                return this._direction;
            }
            set
            {
                this.EnsureChildControls();
                this._direction = value;
                this._extender.RatingDirection = (int) value;
            }
        }

        [Description("ReadOnly"), Browsable(true), Category("Behavior"), Bindable(true, BindingDirection.TwoWay), DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.ReadOnly;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.ReadOnly = value;
            }
        }

        [Description("StarCssClass"), Browsable(true), Themeable(true), Category("Behavior"), DefaultValue("")]
        public string StarCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.StarCssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.StarCssClass = value;
            }
        }

        [Bindable(true, BindingDirection.TwoWay), Browsable(true), Category("Behavior"), Description("Tag"), DefaultValue("")]
        public string Tag
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.Tag;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.Tag = value;
            }
        }

        [Browsable(true), Themeable(true), Category("Behavior"), Description("WaitingStarCssClass"), DefaultValue("")]
        public string WaitingStarCssClass
        {
            get
            {
                this.EnsureChildControls();
                return this._extender.WaitingStarCssClass;
            }
            set
            {
                this.EnsureChildControls();
                this._extender.WaitingStarCssClass = value;
            }
        }
    }
}

