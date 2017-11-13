namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Point.js", LoadOrder=5), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Spring.js", LoadOrder=13), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Viewport.js", LoadOrder=14), Designer("AjaxControlToolkit.SeadragonDesigner, AjaxControlToolkit"), ToolboxData("<{0}:Seadragon runat=server></{0}:Seadragon>"), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.TileSource.js", LoadOrder=8), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Utils.js", LoadOrder=0), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Buttons.js", LoadOrder=1), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Config.js", LoadOrder=2), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.js", LoadOrder=3), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.MouseTracker.js", LoadOrder=4), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Strings.js", LoadOrder=6), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Drawer.js", LoadOrder=7), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Profiler.js", LoadOrder=12), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.DeepZoom.js", LoadOrder=9), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.Rect.js", LoadOrder=10), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.ImageLoader.js", LoadOrder=11), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.Seadragon.Viewer", "Seadragon.Seadragon.DisplayRect.js", LoadOrder=15), PersistChildren(false)]
    public class Seadragon : ScriptControlBase
    {
        private List<SeadragonControl> _controls;
        private ArrayList _controlsDescriptor;
        private List<SeadragonOverlay> _overlays;
        private ArrayList _overlaysDescriptor;

        protected override void CreateChildControls()
        {
            this._controlsDescriptor = new ArrayList();
            this._overlaysDescriptor = new ArrayList();
            foreach (SeadragonControl control in this.ControlsCollection)
            {
                this.Controls.Add(control);
                this._controlsDescriptor.Add(new { 
                    id = control.ClientID,
                    anchor = control.Anchor
                });
            }
            foreach (SeadragonOverlay overlay in this.OverlaysCollection)
            {
                this.Controls.Add(overlay);
                if (overlay is SeadragonFixedOverlay)
                {
                    SeadragonFixedOverlay overlay2 = overlay as SeadragonFixedOverlay;
                    this._overlaysDescriptor.Add(new { 
                        id = overlay2.ClientID,
                        point = overlay2.Point,
                        placement = overlay2.Placement
                    });
                }
                else
                {
                    SeadragonScalableOverlay overlay3 = overlay as SeadragonScalableOverlay;
                    this._overlaysDescriptor.Add(new { 
                        id = overlay3.ClientID,
                        rect = overlay3.Rect
                    });
                }
            }
        }

        protected override ControlCollection CreateControlCollection() => 
            base.CreateControlCollection();

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddProperty("controls", this._controlsDescriptor);
            descriptor.AddProperty("overlays", this._overlaysDescriptor);
            descriptor.AddProperty("xmlPath", base.ResolveClientUrl(this.SourceUrl));
            descriptor.AddProperty("prefixUrl", this.Page.Request.ApplicationPath);
        }

        protected V GetPropertyValue<V>(string propertyName, V nullValue)
        {
            if (this.ViewState[propertyName] == null)
            {
                return nullValue;
            }
            return (V) this.ViewState[propertyName];
        }

        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            this.ViewState[propertyName] = value;
        }

        [ExtenderControlProperty, ClientPropertyName("alwaysBlend"), DefaultValue(false)]
        public bool AlwaysBlend
        {
            get => 
                this.GetPropertyValue<bool>("AlwaysBlend", false);
            set
            {
                this.SetPropertyValue<bool>("AlwaysBlend", value);
            }
        }

        [ClientPropertyName("animationTime"), ExtenderControlProperty, DefaultValue((float) 1.5f)]
        public float AnimationTime
        {
            get => 
                this.GetPropertyValue<float>("AnimationTime", 1.5f);
            set
            {
                this.SetPropertyValue<float>("AnimationTime", value);
            }
        }

        [DefaultValue(true), ClientPropertyName("autoHideControls"), ExtenderControlProperty]
        public bool AutoHideControls
        {
            get => 
                this.GetPropertyValue<bool>("AutoHideControls", true);
            set
            {
                this.SetPropertyValue<bool>("AutoHideControls", value);
            }
        }

        [ClientPropertyName("blendTime"), DefaultValue((float) 0.5f), ExtenderControlProperty]
        public float BlendTime
        {
            get => 
                this.GetPropertyValue<float>("BlendTime", 0.5f);
            set
            {
                this.SetPropertyValue<float>("BlendTime", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("clickDistThreshold"), DefaultValue(2)]
        public int clickDistThreshold
        {
            get => 
                this.GetPropertyValue<int>("clickDistThreshold", 2);
            set
            {
                this.SetPropertyValue<int>("clickDistThreshold", value);
            }
        }

        [ClientPropertyName("clickTimeThreshold"), ExtenderControlProperty, DefaultValue(200)]
        public int ClickTimeThreshold
        {
            get => 
                this.GetPropertyValue<int>("ClickTimeThreshold", 200);
            set
            {
                this.SetPropertyValue<int>("ClickTimeThreshold", value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SeadragonControl> ControlsCollection
        {
            get
            {
                if (this._controls == null)
                {
                    this._controls = new List<SeadragonControl>();
                }
                return this._controls;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("imageLoaderLimit"), DefaultValue(2)]
        public int ImageLoaderLimit
        {
            get => 
                this.GetPropertyValue<int>("SpringStiffness", 2);
            set
            {
                this.SetPropertyValue<int>("SpringStiffness", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true), ClientPropertyName("immediateRender")]
        public bool ImmediateRender
        {
            get => 
                this.GetPropertyValue<bool>("ImmediateRender", true);
            set
            {
                this.SetPropertyValue<bool>("ImmediateRender", value);
            }
        }

        [DefaultValue(100), ExtenderControlProperty, ClientPropertyName("maxImageCacheCount")]
        public int MaxImageCacheCount
        {
            get => 
                this.GetPropertyValue<int>("maxImageCacheCount", 100);
            set
            {
                this.SetPropertyValue<int>("maxImageCacheCount", value);
            }
        }

        [DefaultValue((float) 2f), ClientPropertyName("maxZoomPixelRatio"), ExtenderControlProperty]
        public float MaxZoomPixelRatio
        {
            get => 
                this.GetPropertyValue<float>("MaxZoomPixelRatio", 2f);
            set
            {
                this.SetPropertyValue<float>("MaxZoomPixelRatio", value);
            }
        }

        [ClientPropertyName("minPixelRatio"), ExtenderControlProperty, DefaultValue((float) 0.5f)]
        public float MinPixelRatio
        {
            get => 
                this.GetPropertyValue<float>("MinPixelRatio", 0.5f);
            set
            {
                this.SetPropertyValue<float>("MinPixelRatio", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("minZoomDimension"), DefaultValue((float) 0.8f)]
        public float MinZoomDimension
        {
            get => 
                this.GetPropertyValue<float>("MinZoomDimension", 0.8f);
            set
            {
                this.SetPropertyValue<float>("MinZoomDimension", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true), ClientPropertyName("mouseNavEnabled")]
        public bool MouseNavEnabled
        {
            get => 
                this.GetPropertyValue<bool>("MouseNavEnabled", true);
            set
            {
                this.SetPropertyValue<bool>("MouseNavEnabled", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("animation"), DefaultValue("")]
        public string OnClientAnimation
        {
            get => 
                this.GetPropertyValue<string>("OnClientAnimation", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientAnimation", value);
            }
        }

        [ClientPropertyName("animationend"), ExtenderControlEvent, DefaultValue("")]
        public string OnClientAnimationEnd
        {
            get => 
                this.GetPropertyValue<string>("OnClientAnimationEnd", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientAnimationEnd", value);
            }
        }

        [ClientPropertyName("animationstart"), ExtenderControlEvent, DefaultValue("")]
        public string OnClientAnimationStart
        {
            get => 
                this.GetPropertyValue<string>("OnClientAnimationStart", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientAnimationStart", value);
            }
        }

        [DefaultValue(""), ExtenderControlEvent, ClientPropertyName("error")]
        public string OnClientError
        {
            get => 
                this.GetPropertyValue<string>("OnClientError", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientError", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("ignore"), DefaultValue("")]
        public string OnClientIgnore
        {
            get => 
                this.GetPropertyValue<string>("OnClientIgnore", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientIgnore", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("0pen"), DefaultValue("")]
        public string OnClientOpen
        {
            get => 
                this.GetPropertyValue<string>("OnClientOpen", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientOpen", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("resize"), ExtenderControlEvent]
        public string OnClientResize
        {
            get => 
                this.GetPropertyValue<string>("OnClientResize", string.Empty);
            set
            {
                this.SetPropertyValue<string>("OnClientResize", value);
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Editor(typeof(OverlayCollectionEditor), typeof(UITypeEditor))]
        public List<SeadragonOverlay> OverlaysCollection
        {
            get
            {
                if (this._overlays == null)
                {
                    this._overlays = new List<SeadragonOverlay>();
                }
                return this._overlays;
            }
        }

        [DefaultValue(true), ClientPropertyName("showNavigationControl"), ExtenderControlProperty]
        public bool ShowNavigationControl
        {
            get => 
                this.GetPropertyValue<bool>("ShowNavigationControl", true);
            set
            {
                this.SetPropertyValue<bool>("ShowNavigationControl", value);
            }
        }

        [Editor(typeof(SeadragonUrlEditor), typeof(UITypeEditor))]
        public string SourceUrl
        {
            get => 
                this.GetPropertyValue<string>("SourceUrl", string.Empty);
            set
            {
                this.SetPropertyValue<string>("SourceUrl", value);
            }
        }

        [ClientPropertyName("springStiffness"), ExtenderControlProperty, DefaultValue((float) 5f)]
        public float SpringStiffness
        {
            get => 
                this.GetPropertyValue<float>("SpringStiffness", 5f);
            set
            {
                this.SetPropertyValue<float>("SpringStiffness", value);
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Div;

        [ExtenderControlProperty, DefaultValue((float) 0.5f), ClientPropertyName("visibilityRatio")]
        public float VisibilityRatio
        {
            get => 
                this.GetPropertyValue<float>("VisibilityRatio", 0.5f);
            set
            {
                this.SetPropertyValue<float>("VisibilityRatio", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty, ClientPropertyName("wrapHorizontal")]
        public bool WrapHorizontal
        {
            get => 
                this.GetPropertyValue<bool>("WrapHorizontal", false);
            set
            {
                this.SetPropertyValue<bool>("WrapHorizontal", value);
            }
        }

        [ClientPropertyName("wrapVertical"), ExtenderControlProperty, DefaultValue(false)]
        public bool WrapVertical
        {
            get => 
                this.GetPropertyValue<bool>("WrapVertical", false);
            set
            {
                this.SetPropertyValue<bool>("WrapVertical", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("zoomPerClick"), DefaultValue((float) 2f)]
        public float ZoomPerClick
        {
            get => 
                this.GetPropertyValue<float>("ZoomPerClick", 2f);
            set
            {
                this.SetPropertyValue<float>("ZoomPerClick", value);
            }
        }

        [ClientPropertyName("zoomPerSecond"), ExtenderControlProperty, DefaultValue((float) 2f)]
        public float ZoomPerSecond
        {
            get => 
                this.GetPropertyValue<float>("ZoomPerSecond", 2f);
            set
            {
                this.SetPropertyValue<float>("ZoomPerSecond", value);
            }
        }
    }
}

