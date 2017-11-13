namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("HotSpots"), SupportsEventValidation, ParseChildren(true, "HotSpots"), DefaultEvent("Click"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ImageMap : Image, IPostBackEventHandler
    {
        private bool _hasHotSpots;
        private HotSpotCollection _hotSpots;
        private static readonly object EventClick = new object();

        [WebSysDescription("ImageMap_Click"), Category("Action")]
        public event ImageMapEventHandler Click
        {
            add
            {
                base.Events.AddHandler(EventClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventClick, value);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            if (this._hasHotSpots)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Usemap, "#ImageMap" + this.ClientID, false);
            }
        }

        protected override void LoadViewState(object savedState)
        {
            object obj2 = null;
            object[] objArray = null;
            if (savedState != null)
            {
                objArray = (object[]) savedState;
                if (objArray.Length != 2)
                {
                    throw new ArgumentException(System.Web.SR.GetString("ViewState_InvalidViewState"));
                }
                obj2 = objArray[0];
            }
            base.LoadViewState(obj2);
            if ((objArray != null) && (objArray[1] != null))
            {
                ((IStateManager) this.HotSpots).LoadViewState(objArray[1]);
            }
        }

        protected virtual void OnClick(ImageMapEventArgs e)
        {
            ImageMapEventHandler handler = (ImageMapEventHandler) base.Events[EventClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {
            base.ValidateEvent(this.UniqueID, eventArgument);
            string postBackValue = null;
            if ((eventArgument != null) && (this._hotSpots != null))
            {
                int num = int.Parse(eventArgument, CultureInfo.InvariantCulture);
                if ((num >= 0) && (num < this._hotSpots.Count))
                {
                    HotSpot spot = this._hotSpots[num];
                    System.Web.UI.WebControls.HotSpotMode hotSpotMode = spot.HotSpotMode;
                    switch (hotSpotMode)
                    {
                        case System.Web.UI.WebControls.HotSpotMode.NotSet:
                            hotSpotMode = this.HotSpotMode;
                            break;

                        case System.Web.UI.WebControls.HotSpotMode.PostBack:
                            postBackValue = spot.PostBackValue;
                            break;
                    }
                }
            }
            if (postBackValue != null)
            {
                this.OnClick(new ImageMapEventArgs(postBackValue));
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (this.Enabled && !base.IsEnabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            this._hasHotSpots = (this._hotSpots != null) && (this._hotSpots.Count > 0);
            base.Render(writer);
            if (this._hasHotSpots)
            {
                string str = "ImageMap" + this.ClientID;
                writer.AddAttribute(HtmlTextWriterAttribute.Name, str);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, str);
                writer.RenderBeginTag(HtmlTextWriterTag.Map);
                System.Web.UI.WebControls.HotSpotMode hotSpotMode = this.HotSpotMode;
                if (hotSpotMode == System.Web.UI.WebControls.HotSpotMode.NotSet)
                {
                    hotSpotMode = System.Web.UI.WebControls.HotSpotMode.Navigate;
                }
                int num = 0;
                string target = this.Target;
                foreach (HotSpot spot in this._hotSpots)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Shape, spot.MarkupName, false);
                    writer.AddAttribute(HtmlTextWriterAttribute.Coords, spot.GetCoordinates());
                    System.Web.UI.WebControls.HotSpotMode mode2 = spot.HotSpotMode;
                    switch (mode2)
                    {
                        case System.Web.UI.WebControls.HotSpotMode.NotSet:
                            mode2 = hotSpotMode;
                            break;

                        case System.Web.UI.WebControls.HotSpotMode.PostBack:
                        {
                            if (this.Page != null)
                            {
                                this.Page.VerifyRenderingInServerForm(this);
                            }
                            string argument = num.ToString(CultureInfo.InvariantCulture);
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, this.Page.ClientScript.GetPostBackClientHyperlink(this, argument, true));
                            break;
                        }
                        case System.Web.UI.WebControls.HotSpotMode.Navigate:
                        {
                            string str4 = base.ResolveClientUrl(spot.NavigateUrl);
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, str4);
                            string str5 = spot.Target;
                            if (str5.Length == 0)
                            {
                                str5 = target;
                            }
                            if (str5.Length > 0)
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Target, str5);
                            }
                            break;
                        }
                        case System.Web.UI.WebControls.HotSpotMode.Inactive:
                            writer.AddAttribute("nohref", "true");
                            break;
                    }
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, spot.AlternateText);
                    writer.AddAttribute(HtmlTextWriterAttribute.Alt, spot.AlternateText);
                    string accessKey = spot.AccessKey;
                    if (accessKey.Length > 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
                    }
                    int tabIndex = spot.TabIndex;
                    if (tabIndex != 0)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(NumberFormatInfo.InvariantInfo));
                    }
                    writer.RenderBeginTag(HtmlTextWriterTag.Area);
                    writer.RenderEndTag();
                    num++;
                }
                writer.RenderEndTag();
            }
        }

        protected override object SaveViewState()
        {
            object obj2 = base.SaveViewState();
            object obj3 = null;
            if ((this._hotSpots != null) && (this._hotSpots.Count > 0))
            {
                obj3 = ((IStateManager) this._hotSpots).SaveViewState();
            }
            if ((obj2 == null) && (obj3 == null))
            {
                return null;
            }
            return new object[] { obj2, obj3 };
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._hotSpots != null)
            {
                ((IStateManager) this._hotSpots).TrackViewState();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [WebCategory("Behavior"), WebSysDescription("HotSpot_HotSpotMode"), DefaultValue(0)]
        public virtual System.Web.UI.WebControls.HotSpotMode HotSpotMode
        {
            get
            {
                object obj2 = this.ViewState["HotSpotMode"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.HotSpotMode) obj2;
                }
                return System.Web.UI.WebControls.HotSpotMode.NotSet;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.HotSpotMode.NotSet) || (value > System.Web.UI.WebControls.HotSpotMode.Inactive))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["HotSpotMode"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty), NotifyParentProperty(true), WebCategory("Behavior"), WebSysDescription("ImageMap_HotSpots"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public HotSpotCollection HotSpots
        {
            get
            {
                if (this._hotSpots == null)
                {
                    this._hotSpots = new HotSpotCollection();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._hotSpots).TrackViewState();
                    }
                }
                return this._hotSpots;
            }
        }

        [WebCategory("Behavior"), WebSysDescription("HotSpot_Target"), DefaultValue("")]
        public virtual string Target
        {
            get
            {
                object obj2 = this.ViewState["Target"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["Target"] = value;
            }
        }
    }
}

