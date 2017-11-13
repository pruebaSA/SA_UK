namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.UI;

    [ToolboxItem(false), TargetControlType(typeof(AjaxControlToolkit.Rating)), ClientScriptResource("Sys.Extended.UI.RatingBehavior", "Rating.RatingBehavior.js")]
    public class RatingExtender : ExtenderControlBase
    {
        public RatingExtender()
        {
            base.EnableClientState = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never), ExtenderControlProperty, Browsable(false)]
        public bool AutoPostBack
        {
            get => 
                base.GetPropertyValue<bool>("AutoPostback", false);
            set
            {
                base.SetPropertyValue<bool>("AutoPostback", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string CallbackID
        {
            get => 
                base.GetPropertyValue<string>("CallbackID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("CallbackID", value);
            }
        }

        [DefaultValue(""), RequiredProperty, ExtenderControlProperty]
        public string EmptyStarCssClass
        {
            get => 
                base.GetPropertyValue<string>("EmptyStarCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("EmptyStarCssClass", value);
            }
        }

        [DefaultValue(""), RequiredProperty, ExtenderControlProperty]
        public string FilledStarCssClass
        {
            get => 
                base.GetPropertyValue<string>("FilledStarCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("FilledStarCssClass", value);
            }
        }

        [ExtenderControlProperty(true, true), ClientPropertyName("_isServerControl")]
        public bool IsServerControl =>
            true;

        [DefaultValue(5), ExtenderControlProperty]
        public int MaxRating
        {
            get => 
                base.GetPropertyValue<int>("MaxRating", 5);
            set
            {
                base.SetPropertyValue<int>("MaxRating", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int Rating
        {
            get
            {
                string clientState = base.ClientState;
                if (clientState == null)
                {
                    clientState = "0";
                }
                return int.Parse(clientState, CultureInfo.InvariantCulture);
            }
            set
            {
                base.ClientState = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int RatingDirection
        {
            get => 
                base.GetPropertyValue<int>("RatingDirection", 0);
            set
            {
                base.SetPropertyValue<int>("RatingDirection", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool ReadOnly
        {
            get => 
                base.GetPropertyValue<bool>("ReadOnly", false);
            set
            {
                base.SetPropertyValue<bool>("ReadOnly", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty, RequiredProperty]
        public string StarCssClass
        {
            get => 
                base.GetPropertyValue<string>("StarCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("StarCssClass", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string Tag
        {
            get => 
                base.GetPropertyValue<string>("Tag", string.Empty);
            set
            {
                base.SetPropertyValue<string>("Tag", value);
            }
        }

        [RequiredProperty, DefaultValue(""), ExtenderControlProperty]
        public string WaitingStarCssClass
        {
            get => 
                base.GetPropertyValue<string>("WaitingStarCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("WaitingStarCssClass", value);
            }
        }
    }
}

