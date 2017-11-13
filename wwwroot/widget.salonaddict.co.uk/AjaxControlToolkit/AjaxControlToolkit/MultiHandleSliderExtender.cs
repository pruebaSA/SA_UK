namespace AjaxControlToolkit
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientCssResource("MultiHandleSlider.MultiHandleSlider_resource.css"), TargetControlType(typeof(TextBox)), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.MultiHandleSliderBehavior", "MultiHandleSlider.MultiHandleSliderBehavior.js"), ToolboxBitmap(typeof(MultiHandleSliderExtender), "MultiHandleSlider.MultiHandleSlider.ico"), RequiredScript(typeof(TimerScript)), Description("A multi-handled slider allowing selection of multiple point values on a graphical rail."), Designer(typeof(MultiHandleSliderDesigner)), RequiredScript(typeof(DragDropScripts)), RequiredScript(typeof(AnimationScripts))]
    public class MultiHandleSliderExtender : ExtenderControlBase
    {
        public MultiHandleSliderExtender()
        {
            base.EnableClientState = true;
        }

        [IDReferenceProperty(typeof(WebControl)), ClientPropertyName("boundControlID"), ExtenderControlProperty, DefaultValue("")]
        public string BoundControlID
        {
            get => 
                base.GetPropertyValue<string>("BoundControlID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("BoundControlID", value);
            }
        }

        [ClientPropertyName("cssClass"), Description("The master style to apply to slider graphical elements."), ExtenderControlProperty, DefaultValue("")]
        public string CssClass
        {
            get => 
                base.GetPropertyValue<string>("CssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("CssClass", value);
            }
        }

        [ClientPropertyName("decimals"), DefaultValue(0), ExtenderControlProperty]
        public int Decimals
        {
            get => 
                base.GetPropertyValue<int>("Decimals", 0);
            set
            {
                base.SetPropertyValue<int>("Decimals", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("enableHandleAnimation"), Description("Determines if the slider handles display an animation effect when changing position."), DefaultValue(false)]
        public bool EnableHandleAnimation
        {
            get => 
                base.GetPropertyValue<bool>("EnableHandleAnimation", false);
            set
            {
                base.SetPropertyValue<bool>("EnableHandleAnimation", value);
            }
        }

        [Description("Determines if the inner rail range can be dragged as a whole, moving both handles defining it."), DefaultValue(false), ClientPropertyName("enableInnerRangeDrag"), ExtenderControlProperty]
        public bool EnableInnerRangeDrag
        {
            get => 
                base.GetPropertyValue<bool>("EnableInnerRangeDrag", false);
            set
            {
                base.SetPropertyValue<bool>("EnableInnerRangeDrag", value);
            }
        }

        [ClientPropertyName("enableKeyboard"), DefaultValue(true), Description("Determines if the slider will respond to arrow keys when it has focus."), ExtenderControlProperty]
        public bool EnableKeyboard
        {
            get => 
                base.GetPropertyValue<bool>("EnableKeyboard", true);
            set
            {
                base.SetPropertyValue<bool>("EnableKeyboard", value);
            }
        }

        [DefaultValue(true), ClientPropertyName("enableMouseWheel"), Description("Determines if the slider will respond to the mouse wheel when it has focus."), ExtenderControlProperty]
        public bool EnableMouseWheel
        {
            get => 
                base.GetPropertyValue<bool>("EnableMouseWheel", true);
            set
            {
                base.SetPropertyValue<bool>("EnableMouseWheel", value);
            }
        }

        [Description("Determines if clicking on the rail will detect and move the closest handle."), ExtenderControlProperty, DefaultValue(true), ClientPropertyName("enableRailClick")]
        public bool EnableRailClick
        {
            get => 
                base.GetPropertyValue<bool>("EnableRailClick", true);
            set
            {
                base.SetPropertyValue<bool>("EnableRailClick", value);
            }
        }

        [ExtenderControlProperty, DefaultValue((float) 0.02f), ClientPropertyName("handleAnimationDuration"), Description("Determines the total duration of the animation effect, in seconds.")]
        public float HandleAnimationDuration
        {
            get => 
                base.GetPropertyValue<float>("HandleAnimationDuration", 0.1f);
            set
            {
                base.SetPropertyValue<float>("HandleAnimationDuration", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("handleCssClass")]
        public string HandleCssClass
        {
            get => 
                base.GetPropertyValue<string>("HandleCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("HandleCssClass", value);
            }
        }

        [ExtenderControlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, ClientPropertyName("handleImageUrl"), DefaultValue("")]
        public string HandleImageUrl
        {
            get => 
                base.GetPropertyValue<string>("HandleImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("HandleImageUrl", value);
            }
        }

        [ClientPropertyName("increment"), Description("Determines the number of points to increment or decrement the slider using the keyboard or mousewheel; ignored if steps is used."), DefaultValue(1), ExtenderControlProperty]
        public int Increment
        {
            get => 
                base.GetPropertyValue<int>("Increment", 1);
            set
            {
                base.SetPropertyValue<int>("Increment", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty, Description("Determines how the inner rail style is handled."), ClientPropertyName("innerRailStyle")]
        public MultiHandleInnerRailStyle InnerRailStyle
        {
            get => 
                base.GetPropertyValue<MultiHandleInnerRailStyle>("InnerRailStyle", MultiHandleInnerRailStyle.AsIs);
            set
            {
                base.SetPropertyValue<MultiHandleInnerRailStyle>("InnerRailStyle", value);
            }
        }

        [ExtenderControlProperty, Description("Determines if the slider and its values can be manipulated."), ClientPropertyName("isReadOnly"), DefaultValue(false)]
        public bool IsReadOnly
        {
            get => 
                base.GetPropertyValue<bool>("IsReadOnly", false);
            set
            {
                base.SetPropertyValue<bool>("IsReadOnly", value);
            }
        }

        [ClientPropertyName("_isServerControl"), ExtenderControlProperty(true, true)]
        public bool IsServerControl =>
            true;

        [Description("The length of the slider rail in pixels."), DefaultValue(150), ClientPropertyName("length"), ExtenderControlProperty]
        public int Length
        {
            get => 
                base.GetPropertyValue<int>("Length", 150);
            set
            {
                base.SetPropertyValue<int>("Length", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("100"), ClientPropertyName("maximum"), Description("The highest value on the slider.")]
        public int Maximum
        {
            get => 
                base.GetPropertyValue<int>("Maximum", 100);
            set
            {
                base.SetPropertyValue<int>("Maximum", value);
            }
        }

        [ExtenderControlProperty, Description("The lowest value on the slider."), DefaultValue("0"), ClientPropertyName("minimum")]
        public int Minimum
        {
            get => 
                base.GetPropertyValue<int>("Minimum", 0);
            set
            {
                base.SetPropertyValue<int>("Minimum", value);
            }
        }

        [NotifyParentProperty(true), ExtenderControlProperty(true, true), ClientPropertyName("multiHandleSliderTargets"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("The list of controls used to bind slider handle values. These should be Label or TextBox controls.")]
        public Collection<MultiHandleSliderTarget> MultiHandleSliderTargets
        {
            get => 
                base.GetPropertyValue<Collection<MultiHandleSliderTarget>>("MultiHandleSliderTargets", null);
            set
            {
                base.SetPropertyValue<Collection<MultiHandleSliderTarget>>("MultiHandleSliderTargets", value);
            }
        }

        [ClientPropertyName("drag"), Description("The event raised when the user drags the slider."), ExtenderControlEvent, DefaultValue("")]
        public string OnClientDrag
        {
            get => 
                base.GetPropertyValue<string>("OnClientDrag", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientDrag", value);
            }
        }

        [ClientPropertyName("dragEnd"), DefaultValue(""), ExtenderControlEvent, Description("The event raised when the user drops the slider.")]
        public string OnClientDragEnd
        {
            get => 
                base.GetPropertyValue<string>("OnClientDragEnd", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientDragEnd", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("dragStart"), Description("The event raised when the user initiates a drag operation on the slider."), DefaultValue("")]
        public string OnClientDragStart
        {
            get => 
                base.GetPropertyValue<string>("OnClientDragStart", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientDragStart", value);
            }
        }

        [Description("The event raised when the slider is completely loaded on the page."), DefaultValue(""), ClientPropertyName("load"), ExtenderControlEvent]
        public string OnClientLoad
        {
            get => 
                base.GetPropertyValue<string>("OnClientLoad", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientLoad", value);
            }
        }

        [DefaultValue(""), ExtenderControlEvent, Description("The event raised when the slider changes its state."), ClientPropertyName("valueChanged")]
        public string OnClientValueChanged
        {
            get => 
                base.GetPropertyValue<string>("OnClientValueChanged", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientValueChanged", value);
            }
        }

        [ClientPropertyName("orientation"), Description("Determines if the slider's orientation is horizontal or vertical."), DefaultValue(0), ExtenderControlProperty]
        public SliderOrientation Orientation
        {
            get => 
                base.GetPropertyValue<SliderOrientation>("Orientation", SliderOrientation.Horizontal);
            set
            {
                base.SetPropertyValue<SliderOrientation>("Orientation", value);
            }
        }

        [ClientPropertyName("railCssClass"), ExtenderControlProperty, DefaultValue("")]
        public string RailCssClass
        {
            get => 
                base.GetPropertyValue<string>("RailCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("RailCssClass", value);
            }
        }

        [ClientPropertyName("raiseChangeOnlyOnMouseUp"), Description("Determines if changes to the slider's values are raised as an event when dragging; otherwise, they are raised on drag end."), DefaultValue(true), ExtenderControlProperty]
        public bool RaiseChangeOnlyOnMouseUp
        {
            get => 
                base.GetPropertyValue<bool>("RaiseChangeOnlyOnMouseUp", true);
            set
            {
                base.SetPropertyValue<bool>("RaiseChangeOnlyOnMouseUp", value);
            }
        }

        [ClientPropertyName("showHandleDragStyle"), DefaultValue(false), Description("Determines if the slider handles will show a style effect when they are being dragged."), ExtenderControlProperty]
        public bool ShowHandleDragStyle
        {
            get => 
                base.GetPropertyValue<bool>("ShowHandleDragStyle", false);
            set
            {
                base.SetPropertyValue<bool>("ShowHandleDragStyle", value);
            }
        }

        [ClientPropertyName("showHandleHoverStyle"), ExtenderControlProperty, Description("Determines if the slider handles will show a style effect when they are hovered over."), DefaultValue(false)]
        public bool ShowHandleHoverStyle
        {
            get => 
                base.GetPropertyValue<bool>("ShowHandleHoverStyle", false);
            set
            {
                base.SetPropertyValue<bool>("ShowHandleHoverStyle", value);
            }
        }

        [DefaultValue(false), ClientPropertyName("showInnerRail"), Description("Determines if the slider will show an inner selected range rail; otherwise, it will display as a uniform rail."), ExtenderControlProperty]
        public bool ShowInnerRail
        {
            get => 
                base.GetPropertyValue<bool>("ShowInnerRail", false);
            set
            {
                base.SetPropertyValue<bool>("ShowInnerRail", value);
            }
        }

        [Description("Determines number of discrete locations on the slider; otherwise, the slider is continous."), DefaultValue(0), ClientPropertyName("steps"), ExtenderControlProperty]
        public int Steps
        {
            get => 
                base.GetPropertyValue<int>("Steps", 0);
            set
            {
                base.SetPropertyValue<int>("Steps", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("tooltipText"), Description("Determines the text to display as the tooltip; {0} denotes the current handle's value in the format string.")]
        public string TooltipText
        {
            get => 
                base.GetPropertyValue<string>("TooltipText", string.Empty);
            set
            {
                base.SetPropertyValue<string>("TooltipText", value);
            }
        }
    }
}

