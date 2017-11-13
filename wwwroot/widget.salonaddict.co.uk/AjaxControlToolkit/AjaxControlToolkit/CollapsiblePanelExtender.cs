namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(CollapsiblePanelExtender), "CollapsiblePanel.CollapsiblePanel.ico"), RequiredScript(typeof(AnimationScripts)), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.CollapsiblePanelBehavior", "CollapsiblePanel.CollapsiblePanelBehavior.js"), TargetControlType(typeof(Panel)), DefaultProperty("CollapseControlID"), Designer("AjaxControlToolkit.CollapsiblePanelDesigner, AjaxControlToolkit")]
    public class CollapsiblePanelExtender : ExtenderControlBase
    {
        public CollapsiblePanelExtender()
        {
            base.ClientStateValuesLoaded += new EventHandler(this.CollapsiblePanelExtender_ClientStateValuesLoaded);
            base.EnableClientState = true;
        }

        private void CollapsiblePanelExtender_ClientStateValuesLoaded(object sender, EventArgs e)
        {
            WebControl control = this.FindControl(base.TargetControlID) as WebControl;
            if ((control != null) && !string.IsNullOrEmpty(base.ClientState))
            {
                if (bool.Parse(base.ClientState))
                {
                    control.Style["display"] = "none";
                }
                else
                {
                    control.Style["display"] = "";
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void EnsureValid()
        {
            base.EnsureValid();
            if (((this.ExpandedText != null) || (this.CollapsedText != null)) && (this.TextLabelID == null))
            {
                throw new ArgumentException("If CollapsedText or ExpandedText is set, TextLabelID must also be set.");
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool AutoCollapse
        {
            get => 
                base.GetPropertyValue<bool>("AutoCollapse", false);
            set
            {
                base.SetPropertyValue<bool>("AutoCollapse", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool AutoExpand
        {
            get => 
                base.GetPropertyValue<bool>("AutoExpand", false);
            set
            {
                base.SetPropertyValue<bool>("AutoExpand", value);
            }
        }

        [IDReferenceProperty(typeof(WebControl)), ExtenderControlProperty, DefaultValue("")]
        public string CollapseControlID
        {
            get => 
                base.GetPropertyValue<string>("CollapseControlID", "");
            set
            {
                base.SetPropertyValue<string>("CollapseControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool Collapsed
        {
            get => 
                base.GetPropertyValue<bool>("Collapsed", false);
            set
            {
                base.SetPropertyValue<bool>("Collapsed", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), UrlProperty]
        public string CollapsedImage
        {
            get => 
                base.GetPropertyValue<string>("CollapsedImage", "");
            set
            {
                base.SetPropertyValue<string>("CollapsedImage", value);
            }
        }

        [DefaultValue(-1), ExtenderControlProperty]
        public int CollapsedSize
        {
            get => 
                base.GetPropertyValue<int>("CollapseHeight", -1);
            set
            {
                base.SetPropertyValue<int>("CollapseHeight", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string CollapsedText
        {
            get => 
                base.GetPropertyValue<string>("CollapsedText", "");
            set
            {
                base.SetPropertyValue<string>("CollapsedText", value);
            }
        }

        [IDReferenceProperty(typeof(WebControl)), ExtenderControlProperty, DefaultValue("")]
        public string ExpandControlID
        {
            get => 
                base.GetPropertyValue<string>("ExpandControlID", "");
            set
            {
                base.SetPropertyValue<string>("ExpandControlID", value);
            }
        }

        [DefaultValue(1), ExtenderControlProperty]
        public CollapsiblePanelExpandDirection ExpandDirection
        {
            get => 
                base.GetPropertyValue<CollapsiblePanelExpandDirection>("ExpandDirection", CollapsiblePanelExpandDirection.Vertical);
            set
            {
                base.SetPropertyValue<CollapsiblePanelExpandDirection>("ExpandDirection", value);
            }
        }

        [UrlProperty, ExtenderControlProperty, DefaultValue("")]
        public string ExpandedImage
        {
            get => 
                base.GetPropertyValue<string>("ExpandedImage", "");
            set
            {
                base.SetPropertyValue<string>("ExpandedImage", value);
            }
        }

        [DefaultValue(-1), ExtenderControlProperty]
        public int ExpandedSize
        {
            get => 
                base.GetPropertyValue<int>("ExpandedSize", -1);
            set
            {
                base.SetPropertyValue<int>("ExpandedSize", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string ExpandedText
        {
            get => 
                base.GetPropertyValue<string>("ExpandedText", "");
            set
            {
                base.SetPropertyValue<string>("ExpandedText", value);
            }
        }

        [IDReferenceProperty(typeof(System.Web.UI.WebControls.Image)), DefaultValue(""), ExtenderControlProperty]
        public string ImageControlID
        {
            get => 
                base.GetPropertyValue<string>("ImageControlID", "");
            set
            {
                base.SetPropertyValue<string>("ImageControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool ScrollContents
        {
            get => 
                base.GetPropertyValue<bool>("ScrollContents", false);
            set
            {
                base.SetPropertyValue<bool>("ScrollContents", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool SuppressPostBack
        {
            get => 
                base.GetPropertyValue<bool>("SuppressPostBack", false);
            set
            {
                base.SetPropertyValue<bool>("SuppressPostBack", value);
            }
        }

        [IDReferenceProperty(typeof(Label)), ExtenderControlProperty, DefaultValue("")]
        public string TextLabelID
        {
            get => 
                base.GetPropertyValue<string>("TextLabelID", "");
            set
            {
                base.SetPropertyValue<string>("TextLabelID", value);
            }
        }
    }
}

