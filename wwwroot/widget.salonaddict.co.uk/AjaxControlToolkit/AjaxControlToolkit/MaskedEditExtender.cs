namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(MaskedEditExtender), "MaskedEdit.MaskedEdit.ico"), Designer("AjaxControlToolkit.MaskedEditDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.MaskedEditBehavior", "MaskedEdit.MaskedEditValidator.js"), ClientScriptResource("Sys.Extended.UI.MaskedEditBehavior", "MaskedEdit.MaskedEditBehavior.js"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(TimerScript)), TargetControlType(typeof(TextBox))]
    public class MaskedEditExtender : ExtenderControlBase
    {
        public MaskedEditExtender()
        {
            base.EnableClientState = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ((TextBox) this.FindControl(base.TargetControlID)).MaxLength = 0;
            base.ClientState = (string.Compare(this.Page.Form.DefaultFocus, base.TargetControlID, StringComparison.OrdinalIgnoreCase) == 0) ? "Focused" : null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            switch (this.MaskType)
            {
                case MaskedEditType.None:
                    this.AcceptAMPM = false;
                    this.AcceptNegative = MaskedEditShowSymbol.None;
                    this.DisplayMoney = MaskedEditShowSymbol.None;
                    this.InputDirection = MaskedEditInputDirection.LeftToRight;
                    break;

                case MaskedEditType.Date:
                    this.AcceptAMPM = false;
                    this.AcceptNegative = MaskedEditShowSymbol.None;
                    this.DisplayMoney = MaskedEditShowSymbol.None;
                    this.InputDirection = MaskedEditInputDirection.LeftToRight;
                    break;

                case MaskedEditType.Number:
                    this.AcceptAMPM = false;
                    break;

                case MaskedEditType.Time:
                    this.AcceptNegative = MaskedEditShowSymbol.None;
                    this.DisplayMoney = MaskedEditShowSymbol.None;
                    this.InputDirection = MaskedEditInputDirection.LeftToRight;
                    break;

                case MaskedEditType.DateTime:
                    this.AcceptNegative = MaskedEditShowSymbol.None;
                    this.DisplayMoney = MaskedEditShowSymbol.None;
                    this.InputDirection = MaskedEditInputDirection.LeftToRight;
                    break;
            }
            if (string.IsNullOrEmpty(this.CultureName))
            {
                this.CultureName = "";
            }
        }

        private bool validateMaskType()
        {
            string mask = this.Mask;
            MaskedEditType maskType = this.MaskType;
            if (!string.IsNullOrEmpty(mask) && ((maskType == MaskedEditType.Date) || (maskType == MaskedEditType.Time)))
            {
                string validMask = MaskedEditCommon.GetValidMask(mask);
                switch (maskType)
                {
                    case MaskedEditType.Date:
                        return (Array.IndexOf<string>(new string[] { "99/99/9999", "99/9999/99", "9999/99/99", "99/99/99" }, validMask) >= 0);

                    case MaskedEditType.Number:
                        foreach (char ch in validMask)
                        {
                            if (((ch != '9') && (ch != '.')) && (ch != ','))
                            {
                                return false;
                            }
                        }
                        break;

                    case MaskedEditType.Time:
                        return (Array.IndexOf<string>(new string[] { "99:99:99", "99:99" }, validMask) >= 0);

                    case MaskedEditType.DateTime:
                        return (Array.IndexOf<string>(new string[] { "99/99/9999 99:99:99", "99/99/9999 99:99", "99/9999/99 99:99:99", "99/9999/99 99:99", "9999/99/99 99:99:99", "9999/99/99 99:99", "99/99/99 99:99:99", "99/99/99 99:99" }, validMask) >= 0);
                }
            }
            return true;
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool AcceptAMPM
        {
            get => 
                base.GetPropertyValue<bool>("AcceptAmPm", false);
            set
            {
                base.SetPropertyValue<bool>("AcceptAmPm", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public MaskedEditShowSymbol AcceptNegative
        {
            get => 
                base.GetPropertyValue<MaskedEditShowSymbol>("AcceptNegative", MaskedEditShowSymbol.None);
            set
            {
                base.SetPropertyValue<MaskedEditShowSymbol>("AcceptNegative", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true)]
        public bool AutoComplete
        {
            get => 
                base.GetPropertyValue<bool>("AutoComplete", true);
            set
            {
                base.SetPropertyValue<bool>("AutoComplete", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string AutoCompleteValue
        {
            get => 
                base.GetPropertyValue<string>("AutoCompleteValue", "");
            set
            {
                base.SetPropertyValue<string>("AutoCompleteValue", value);
            }
        }

        [DefaultValue(0x76c), ExtenderControlProperty]
        public int Century
        {
            get => 
                base.GetPropertyValue<int>("Century", 0x76c);
            set
            {
                if (value.ToString(CultureInfo.InvariantCulture).Length != 4)
                {
                    throw new ArgumentException("The Century must have 4 digits.");
                }
                base.SetPropertyValue<int>("Century", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true)]
        public bool ClearMaskOnLostFocus
        {
            get => 
                base.GetPropertyValue<bool>("ClearMaskOnLostfocus", true);
            set
            {
                base.SetPropertyValue<bool>("ClearMaskOnLostfocus", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool ClearTextOnInvalid
        {
            get => 
                base.GetPropertyValue<bool>("ClearTextOnInvalid", false);
            set
            {
                base.SetPropertyValue<bool>("ClearTextOnInvalid", value);
            }
        }

        [DefaultValue(true), ExtenderControlProperty]
        public bool ClipboardEnabled
        {
            get => 
                base.GetPropertyValue<bool>("ClipboardEnabled", true);
            set
            {
                base.SetPropertyValue<bool>("ClipboardEnabled", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("Your browser security settings don't permit the automatic execution of paste operations. Please use the keyboard shortcut Ctrl+V instead.")]
        public string ClipboardText
        {
            get => 
                base.GetPropertyValue<string>("ClipboardText", "Your browser security settings don't permit the automatic execution of paste operations. Please use the keyboard shortcut Ctrl+V instead.");
            set
            {
                base.SetPropertyValue<string>("ClipboardText", value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), ExtenderControlProperty, Browsable(false)]
        public string CultureAMPMPlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureAMPMPlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureAMPMPlaceholder", value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), ExtenderControlProperty]
        public string CultureCurrencySymbolPlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureCurrencySymbolPlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureCurrencySymbolPlaceholder", value);
            }
        }

        [ExtenderControlProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CultureDateFormat
        {
            get => 
                base.GetPropertyValue<string>("CultureDateFormat", "");
            set
            {
                base.SetPropertyValue<string>("CultureDateFormat", value);
            }
        }

        [ExtenderControlProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CultureDatePlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureDatePlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureDatePlaceholder", value);
            }
        }

        [ExtenderControlProperty, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string CultureDecimalPlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureDecimalPlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureDecimalPlaceholder", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), RefreshProperties(RefreshProperties.All)]
        public string CultureName
        {
            get => 
                base.GetPropertyValue<string>("CultureName", "");
            set
            {
                CultureInfo culture = null;
                if (string.IsNullOrEmpty(value))
                {
                    culture = CultureInfo.CurrentCulture;
                    this.OverridePageCulture = false;
                }
                else
                {
                    culture = CultureInfo.GetCultureInfo(value);
                    this.OverridePageCulture = true;
                }
                base.SetPropertyValue<string>("CultureName", culture.Name);
                this.CultureDatePlaceholder = culture.DateTimeFormat.DateSeparator;
                this.CultureTimePlaceholder = culture.DateTimeFormat.TimeSeparator;
                this.CultureDecimalPlaceholder = culture.NumberFormat.NumberDecimalSeparator;
                this.CultureThousandsPlaceholder = culture.NumberFormat.NumberGroupSeparator;
                string[] strArray = culture.DateTimeFormat.ShortDatePattern.Split(new string[] { culture.DateTimeFormat.DateSeparator }, StringSplitOptions.None);
                string str = strArray[0].Substring(0, 1).ToUpper(culture) + strArray[1].Substring(0, 1).ToUpper(culture) + strArray[2].Substring(0, 1).ToUpper(culture);
                this.CultureDateFormat = str;
                this.CultureCurrencySymbolPlaceholder = culture.NumberFormat.CurrencySymbol;
                if (string.IsNullOrEmpty(culture.DateTimeFormat.AMDesignator + culture.DateTimeFormat.PMDesignator))
                {
                    this.CultureAMPMPlaceholder = "";
                }
                else
                {
                    this.CultureAMPMPlaceholder = culture.DateTimeFormat.AMDesignator + ";" + culture.DateTimeFormat.PMDesignator;
                }
            }
        }

        [Browsable(false), ExtenderControlProperty, EditorBrowsable(EditorBrowsableState.Never)]
        public string CultureThousandsPlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureThousandsPlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureThousandsPlaceholder", value);
            }
        }

        [ExtenderControlProperty, EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string CultureTimePlaceholder
        {
            get => 
                base.GetPropertyValue<string>("CultureTimePlaceholder", "");
            set
            {
                base.SetPropertyValue<string>("CultureTimePlaceholder", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public MaskedEditShowSymbol DisplayMoney
        {
            get => 
                base.GetPropertyValue<MaskedEditShowSymbol>("DisplayMoney", MaskedEditShowSymbol.None);
            set
            {
                base.SetPropertyValue<MaskedEditShowSymbol>("DisplayMoney", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string ErrorTooltipCssClass
        {
            get => 
                base.GetPropertyValue<string>("ErrorTooltipCssClass", "");
            set
            {
                base.SetPropertyValue<string>("ErrorTooltipCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool ErrorTooltipEnabled
        {
            get => 
                base.GetPropertyValue<bool>("ErrorTooltipEnabled", false);
            set
            {
                base.SetPropertyValue<bool>("ErrorTooltipEnabled", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string Filtered
        {
            get => 
                base.GetPropertyValue<string>("Filtered", "");
            set
            {
                base.SetPropertyValue<string>("Filtered", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public MaskedEditInputDirection InputDirection
        {
            get => 
                base.GetPropertyValue<MaskedEditInputDirection>("InputDirection", MaskedEditInputDirection.LeftToRight);
            set
            {
                base.SetPropertyValue<MaskedEditInputDirection>("InputDirection", value);
            }
        }

        [RequiredProperty, DefaultValue(""), ExtenderControlProperty]
        public string Mask
        {
            get => 
                base.GetPropertyValue<string>("Mask", "");
            set
            {
                if (!this.validateMaskType())
                {
                    throw new ArgumentException("Validate Type and/or Mask is invalid!");
                }
                base.SetPropertyValue<string>("Mask", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty, RefreshProperties(RefreshProperties.All)]
        public MaskedEditType MaskType
        {
            get => 
                base.GetPropertyValue<MaskedEditType>("MaskType", MaskedEditType.None);
            set
            {
                base.SetPropertyValue<MaskedEditType>("MaskType", value);
                switch (value)
                {
                    case MaskedEditType.None:
                        this.AcceptAMPM = false;
                        this.AcceptNegative = MaskedEditShowSymbol.None;
                        this.DisplayMoney = MaskedEditShowSymbol.None;
                        this.InputDirection = MaskedEditInputDirection.LeftToRight;
                        return;

                    case MaskedEditType.Date:
                        this.AcceptAMPM = false;
                        this.AcceptNegative = MaskedEditShowSymbol.None;
                        this.DisplayMoney = MaskedEditShowSymbol.None;
                        this.InputDirection = MaskedEditInputDirection.LeftToRight;
                        return;

                    case MaskedEditType.Number:
                        this.AcceptAMPM = false;
                        return;

                    case MaskedEditType.Time:
                        this.AcceptNegative = MaskedEditShowSymbol.None;
                        this.DisplayMoney = MaskedEditShowSymbol.None;
                        this.InputDirection = MaskedEditInputDirection.LeftToRight;
                        return;

                    case MaskedEditType.DateTime:
                        this.AcceptNegative = MaskedEditShowSymbol.None;
                        this.DisplayMoney = MaskedEditShowSymbol.None;
                        this.InputDirection = MaskedEditInputDirection.LeftToRight;
                        return;
                }
            }
        }

        [DefaultValue(true), ExtenderControlProperty]
        public bool MessageValidatorTip
        {
            get => 
                base.GetPropertyValue<bool>("MessageValidatorTip", true);
            set
            {
                base.SetPropertyValue<bool>("MessageValidatorTip", value);
            }
        }

        [DefaultValue("MaskedEditBlurNegative"), ExtenderControlProperty]
        public string OnBlurCssNegative
        {
            get => 
                base.GetPropertyValue<string>("OnBlurCssNegative", "MaskedEditBlurNegative");
            set
            {
                base.SetPropertyValue<string>("OnBlurCssNegative", value);
            }
        }

        [DefaultValue("MaskedEditFocus"), ExtenderControlProperty]
        public string OnFocusCssClass
        {
            get => 
                base.GetPropertyValue<string>("OnFocusCssClass", "MaskedEditFocus");
            set
            {
                base.SetPropertyValue<string>("OnFocusCssClass", value);
            }
        }

        [DefaultValue("MaskedEditFocusNegative"), ExtenderControlProperty]
        public string OnFocusCssNegative
        {
            get => 
                base.GetPropertyValue<string>("OnFocusCssNegative", "MaskedEditFocusNegative");
            set
            {
                base.SetPropertyValue<string>("OnFocusCssNegative", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("MaskedEditError")]
        public string OnInvalidCssClass
        {
            get => 
                base.GetPropertyValue<string>("OnInvalidCssClass", "MaskedEditError");
            set
            {
                base.SetPropertyValue<string>("OnInvalidCssClass", value);
            }
        }

        internal bool OverridePageCulture
        {
            get => 
                base.GetPropertyValue<bool>("OverridePageCulture", false);
            set
            {
                base.SetPropertyValue<bool>("OverridePageCulture", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("_")]
        public string PromptCharacter
        {
            get => 
                base.GetPropertyValue<string>("PromptChar", "_");
            set
            {
                base.SetPropertyValue<string>("PromptChar", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public MaskedEditUserDateFormat UserDateFormat
        {
            get => 
                base.GetPropertyValue<MaskedEditUserDateFormat>("UserDateFormat", MaskedEditUserDateFormat.None);
            set
            {
                base.SetPropertyValue<MaskedEditUserDateFormat>("UserDateFormat", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public MaskedEditUserTimeFormat UserTimeFormat
        {
            get => 
                base.GetPropertyValue<MaskedEditUserTimeFormat>("UserTimeFormat", MaskedEditUserTimeFormat.None);
            set
            {
                base.SetPropertyValue<MaskedEditUserTimeFormat>("UserTimeFormat", value);
            }
        }
    }
}

