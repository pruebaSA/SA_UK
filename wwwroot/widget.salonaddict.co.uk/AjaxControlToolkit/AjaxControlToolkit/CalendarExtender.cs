namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(CalendarExtender), "Calendar.Calendar.ico"), RequiredScript(typeof(DateTimeScripts), 1), RequiredScript(typeof(PopupExtender), 2), ClientScriptResource("Sys.Extended.UI.CalendarBehavior", "Calendar.CalendarBehavior.js"), Designer("AjaxControlToolkit.CalendarDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts), 0), RequiredScript(typeof(AnimationScripts), 3), RequiredScript(typeof(ThreadingScripts), 4), TargetControlType(typeof(TextBox)), ClientCssResource("Calendar.Calendar_resource.css")]
    public class CalendarExtender : ExtenderControlBase
    {
        [DefaultValue(true), ExtenderControlProperty, ClientPropertyName("animated")]
        public virtual bool Animated
        {
            get => 
                base.GetPropertyValue<bool>("Animated", true);
            set
            {
                base.SetPropertyValue<bool>("Animated", value);
            }
        }

        [DefaultValue(false), ClientPropertyName("clearTime"), ExtenderControlProperty]
        public virtual bool ClearTime
        {
            get => 
                base.GetPropertyValue<bool>("ClearTime", false);
            set
            {
                base.SetPropertyValue<bool>("ClearTime", value);
            }
        }

        [ClientPropertyName("cssClass"), ExtenderControlProperty, DefaultValue("")]
        public virtual string CssClass
        {
            get => 
                base.GetPropertyValue<string>("CssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("CssClass", value);
            }
        }

        [ClientPropertyName("daysModeTitleFormat"), ExtenderControlProperty, DefaultValue("MMMM, yyyy")]
        public virtual string DaysModeTitleFormat
        {
            get => 
                base.GetPropertyValue<string>("DaysModeTitleFormat", "MMMM, yyyy");
            set
            {
                base.SetPropertyValue<string>("DaysModeTitleFormat", value);
            }
        }

        [DefaultValue(0), ClientPropertyName("defaultView"), Description("Default view of the calendar when it first pops up."), ExtenderControlProperty]
        public virtual CalendarDefaultView DefaultView
        {
            get => 
                base.GetPropertyValue<CalendarDefaultView>("DefaultView", CalendarDefaultView.Days);
            set
            {
                base.SetPropertyValue<CalendarDefaultView>("DefaultView", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true), ClientPropertyName("enabled")]
        public virtual bool EnabledOnClient
        {
            get => 
                base.GetPropertyValue<bool>("EnabledOnClient", true);
            set
            {
                base.SetPropertyValue<bool>("EnabledOnClient", value);
            }
        }

        [ClientPropertyName("endDate"), ExtenderControlProperty, DefaultValue((string) null)]
        public DateTime? EndDate
        {
            get
            {
                DateTime? propertyValue = base.GetPropertyValue<DateTime?>("EndDate", null);
                if (!propertyValue.HasValue)
                {
                    return null;
                }
                return new DateTime?(DateTime.SpecifyKind(propertyValue.Value, DateTimeKind.Utc));
            }
            set
            {
                base.SetPropertyValue<DateTime?>("EndDate", value.HasValue ? new DateTime?(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null);
            }
        }

        [ClientPropertyName("firstDayOfWeek"), DefaultValue(7), ExtenderControlProperty]
        public virtual System.Web.UI.WebControls.FirstDayOfWeek FirstDayOfWeek
        {
            get => 
                base.GetPropertyValue<System.Web.UI.WebControls.FirstDayOfWeek>("FirstDayOfWeek", System.Web.UI.WebControls.FirstDayOfWeek.Default);
            set
            {
                base.SetPropertyValue<System.Web.UI.WebControls.FirstDayOfWeek>("FirstDayOfWeek", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("format"), DefaultValue("d")]
        public virtual string Format
        {
            get => 
                base.GetPropertyValue<string>("Format", "d");
            set
            {
                base.SetPropertyValue<string>("Format", value);
            }
        }

        [DefaultValue(""), ExtenderControlEvent, ClientPropertyName("dateSelectionChanged")]
        public virtual string OnClientDateSelectionChanged
        {
            get => 
                base.GetPropertyValue<string>("OnClientDateSelectionChanged", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientDateSelectionChanged", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("hidden"), ExtenderControlEvent]
        public virtual string OnClientHidden
        {
            get => 
                base.GetPropertyValue<string>("OnClientHidden", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHidden", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("hiding"), ExtenderControlEvent]
        public virtual string OnClientHiding
        {
            get => 
                base.GetPropertyValue<string>("OnClientHiding", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHiding", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("showing"), ExtenderControlEvent]
        public virtual string OnClientShowing
        {
            get => 
                base.GetPropertyValue<string>("OnClientShowing", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShowing", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("shown"), ExtenderControlEvent]
        public virtual string OnClientShown
        {
            get => 
                base.GetPropertyValue<string>("OnClientShown", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShown", value);
            }
        }

        [IDReferenceProperty, ExtenderControlProperty, ElementReference, DefaultValue(""), ClientPropertyName("button")]
        public virtual string PopupButtonID
        {
            get => 
                base.GetPropertyValue<string>("PopupButtonID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("PopupButtonID", value);
            }
        }

        [ClientPropertyName("popupPosition"), ExtenderControlProperty, DefaultValue(0), Description("Indicates where you want the calendar displayed, bottom or top of the textbox.")]
        public virtual CalendarPosition PopupPosition
        {
            get => 
                base.GetPropertyValue<CalendarPosition>("PopupPosition", CalendarPosition.BottomLeft);
            set
            {
                base.SetPropertyValue<CalendarPosition>("PopupPosition", value);
            }
        }

        [DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("selectedDate")]
        public DateTime? SelectedDate
        {
            get
            {
                DateTime? propertyValue = base.GetPropertyValue<DateTime?>("SelectedDate", null);
                if (!propertyValue.HasValue)
                {
                    return null;
                }
                return new DateTime?(DateTime.SpecifyKind(propertyValue.Value, DateTimeKind.Utc));
            }
            set
            {
                DateTime? nullable = value.HasValue ? new DateTime?(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null;
                base.SetPropertyValue<DateTime?>("SelectedDate", nullable);
            }
        }

        [DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("startDate")]
        public DateTime? StartDate
        {
            get
            {
                DateTime? propertyValue = base.GetPropertyValue<DateTime?>("StartDate", null);
                if (!propertyValue.HasValue)
                {
                    return null;
                }
                return new DateTime?(DateTime.SpecifyKind(propertyValue.Value, DateTimeKind.Utc));
            }
            set
            {
                base.SetPropertyValue<DateTime?>("StartDate", value.HasValue ? new DateTime?(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)) : null);
            }
        }

        [DefaultValue("MMMM d, yyyy"), ExtenderControlProperty, ClientPropertyName("todaysDateFormat")]
        public virtual string TodaysDateFormat
        {
            get => 
                base.GetPropertyValue<string>("TodaysDateFormat", "MMMM d, yyyy");
            set
            {
                base.SetPropertyValue<string>("TodaysDateFormat", value);
            }
        }
    }
}

