namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(MaskedEditValidator), "MaskedEdit.MaskedEdit.ico")]
    public class MaskedEditValidator : BaseValidator
    {
        private string _ClientValidationFunction = "";
        private string _ControlExtender = "";
        private CultureInfo _Culture;
        private string _InitialValue = "";
        private bool _IsValidEmpty = true;
        private string _MaximumValue = "";
        private string _MessageEmpty = "";
        private string _MessageInvalid = "";
        private string _MessageMax = "";
        private string _MessageMin = "";
        private string _MessageTip = "";
        private string _MinimumValue = "";
        private string _TextEmpty = "";
        private string _TextInvalid = "";
        private string _TextMax = "";
        private string _TextMin = "";
        private string _ValidationExpression = "";

        public event EventHandler<ServerValidateEventArgs> MaskedEditServerValidator;

        protected override bool ControlPropertiesValid() => 
            (this.FindControl(base.ControlToValidate) is TextBox);

        protected override bool EvaluateIsValid()
        {
            MaskedEditExtender extender = (MaskedEditExtender) this.FindControl(this.ControlExtender);
            TextBox sender = (TextBox) extender.FindControl(base.ControlToValidate);
            base.ErrorMessage = "";
            base.Text = "";
            string onInvalidCssClass = "";
            bool isValid = true;
            if (!this.IsValidEmpty && (sender.Text.Trim() == this.InitialValue))
            {
                base.ErrorMessage = this.EmptyValueMessage;
                if (string.IsNullOrEmpty(this.EmptyValueBlurredText))
                {
                    base.Text = base.ErrorMessage;
                }
                else
                {
                    base.Text = this.EmptyValueBlurredText;
                }
                onInvalidCssClass = extender.OnInvalidCssClass;
                isValid = false;
            }
            if ((isValid && (sender.Text.Length != 0)) && (this.ValidationExpression.Length != 0))
            {
                try
                {
                    isValid = new Regex(this.ValidationExpression).IsMatch(sender.Text);
                }
                catch
                {
                    isValid = false;
                }
            }
            if (isValid && (sender.Text.Length != 0))
            {
                if (string.IsNullOrEmpty(extender.CultureName))
                {
                    string name = CultureInfo.CurrentCulture.Name;
                }
                string str3 = "";
                if (!string.IsNullOrEmpty(this.ControlCulture.DateTimeFormat.AMDesignator) && !string.IsNullOrEmpty(this.ControlCulture.DateTimeFormat.PMDesignator))
                {
                    str3 = this.ControlCulture.DateTimeFormat.AMDesignator + ";" + this.ControlCulture.DateTimeFormat.PMDesignator;
                }
                switch (extender.MaskType)
                {
                    case MaskedEditType.Date:
                    case MaskedEditType.Time:
                    case MaskedEditType.DateTime:
                    {
                        int length = sender.Text.Length;
                        if ((extender.AcceptAMPM && !string.IsNullOrEmpty(str3)) && ((extender.MaskType == MaskedEditType.Time) || (extender.MaskType == MaskedEditType.DateTime)))
                        {
                            char[] separator = new char[] { ';' };
                            string[] strArray = str3.Split(separator);
                            if (strArray[0].Length != 0)
                            {
                                length -= strArray[0].Length + 1;
                            }
                        }
                        int num2 = MaskedEditCommon.GetValidMask(extender.Mask).Length;
                        if (extender.MaskType != MaskedEditType.Time)
                        {
                            int num3 = (string.IsNullOrEmpty(extender.CultureName) ? ((int) CultureInfo.CurrentCulture) : ((int) CultureInfo.GetCultureInfo(extender.CultureName))).DateTimeFormat.DateSeparator.Length;
                            num2 += (num3 - 1) * 2;
                        }
                        if (num2 != length)
                        {
                            isValid = false;
                        }
                        if (isValid)
                        {
                            try
                            {
                                DateTime.Parse(sender.Text, this.ControlCulture);
                            }
                            catch
                            {
                                isValid = false;
                            }
                        }
                        break;
                    }
                    case MaskedEditType.Number:
                        try
                        {
                            decimal.Parse(sender.Text, this.ControlCulture);
                        }
                        catch
                        {
                            isValid = false;
                        }
                        break;
                }
                if (!isValid)
                {
                    base.ErrorMessage = this.InvalidValueMessage;
                    if (string.IsNullOrEmpty(this.InvalidValueBlurredMessage))
                    {
                        base.Text = base.ErrorMessage;
                    }
                    else
                    {
                        base.Text = this.InvalidValueBlurredMessage;
                    }
                    onInvalidCssClass = extender.OnInvalidCssClass;
                }
                if (isValid && (!string.IsNullOrEmpty(this.MaximumValue) || !string.IsNullOrEmpty(this.MinimumValue)))
                {
                    switch (extender.MaskType)
                    {
                        case MaskedEditType.None:
                            if (!string.IsNullOrEmpty(this.MaximumValue))
                            {
                                try
                                {
                                    isValid = int.Parse(this.MaximumValue, this.ControlCulture) >= sender.Text.Length;
                                }
                                catch
                                {
                                    base.ErrorMessage = this.InvalidValueMessage;
                                    if (string.IsNullOrEmpty(this.InvalidValueBlurredMessage))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.InvalidValueBlurredMessage;
                                    }
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MaximumValueMessage;
                                    if (string.IsNullOrEmpty(this.MaximumValueBlurredMessage))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MaximumValueBlurredMessage;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            if (isValid && !string.IsNullOrEmpty(this.MinimumValue))
                            {
                                try
                                {
                                    isValid = int.Parse(this.MinimumValue, this.ControlCulture) <= sender.Text.Length;
                                }
                                catch
                                {
                                    base.ErrorMessage = this.InvalidValueMessage;
                                    if (string.IsNullOrEmpty(this.InvalidValueBlurredMessage))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.InvalidValueBlurredMessage;
                                    }
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MinimumValueMessage;
                                    if (string.IsNullOrEmpty(this.MinimumValueBlurredText))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MinimumValueBlurredText;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            break;

                        case MaskedEditType.Date:
                        case MaskedEditType.Time:
                        case MaskedEditType.DateTime:
                        {
                            DateTime time = DateTime.Parse(sender.Text, this.ControlCulture);
                            if (!string.IsNullOrEmpty(this.MaximumValue))
                            {
                                try
                                {
                                    isValid = DateTime.Parse(this.MaximumValue, this.ControlCulture) >= time;
                                }
                                catch
                                {
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MaximumValueMessage;
                                    if (string.IsNullOrEmpty(this.MaximumValueBlurredMessage))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MaximumValueBlurredMessage;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            if (isValid && !string.IsNullOrEmpty(this.MinimumValue))
                            {
                                try
                                {
                                    isValid = DateTime.Parse(this.MinimumValue, this.ControlCulture) <= time;
                                }
                                catch
                                {
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MinimumValueMessage;
                                    if (string.IsNullOrEmpty(this.MinimumValueBlurredText))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MinimumValueBlurredText;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            break;
                        }
                        case MaskedEditType.Number:
                        {
                            decimal num5 = decimal.Parse(sender.Text, this.ControlCulture);
                            if (!string.IsNullOrEmpty(this.MaximumValue))
                            {
                                try
                                {
                                    isValid = decimal.Parse(this.MaximumValue, this.ControlCulture) >= num5;
                                }
                                catch
                                {
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MaximumValueMessage;
                                    if (string.IsNullOrEmpty(this.MaximumValueBlurredMessage))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MaximumValueBlurredMessage;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            if (isValid && !string.IsNullOrEmpty(this.MinimumValue))
                            {
                                try
                                {
                                    isValid = decimal.Parse(this.MinimumValue, this.ControlCulture) <= num5;
                                }
                                catch
                                {
                                    isValid = false;
                                }
                                if (!isValid)
                                {
                                    base.ErrorMessage = this.MinimumValueMessage;
                                    if (string.IsNullOrEmpty(this.MinimumValueBlurredText))
                                    {
                                        base.Text = base.ErrorMessage;
                                    }
                                    else
                                    {
                                        base.Text = this.MinimumValueBlurredText;
                                    }
                                    onInvalidCssClass = extender.OnInvalidCssClass;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            if (isValid && (this.MaskedEditServerValidator != null))
            {
                ServerValidateEventArgs e = new ServerValidateEventArgs(sender.Text, isValid);
                this.MaskedEditServerValidator(sender, e);
                isValid = e.IsValid;
                if (!isValid)
                {
                    onInvalidCssClass = extender.OnInvalidCssClass;
                    base.ErrorMessage = this.InvalidValueMessage;
                    if (string.IsNullOrEmpty(this.InvalidValueBlurredMessage))
                    {
                        base.Text = base.ErrorMessage;
                    }
                    else
                    {
                        base.Text = this.InvalidValueBlurredMessage;
                    }
                }
            }
            if (!isValid)
            {
                string script = "MaskedEditSetCssClass(" + this.ClientID + ",'" + onInvalidCssClass + "');";
                ScriptManager.RegisterStartupScript(this, typeof(MaskedEditValidator), "MaskedEditServerValidator_" + this.ID, script, true);
            }
            return isValid;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (base.EnableClientScript)
            {
                string str4;
                string str6;
                ScriptManager.RegisterClientScriptResource(this, typeof(MaskedEditValidator), "MaskedEdit.MaskedEditValidator.js");
                MaskedEditExtender extender = (MaskedEditExtender) this.FindControl(this.ControlExtender);
                TextBox box = (TextBox) extender.FindControl(base.ControlToValidate);
                int firstMaskPosition = -1;
                int num2 = -1;
                if (extender.ClearMaskOnLostFocus)
                {
                    firstMaskPosition = 0;
                    num2 = MaskedEditCommon.GetValidMask(extender.Mask).Length + 1;
                }
                else
                {
                    firstMaskPosition = MaskedEditCommon.GetFirstMaskPosition(extender.Mask);
                    num2 = MaskedEditCommon.GetLastMaskPosition(extender.Mask) + 1;
                }
                bool flag = true;
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "IsMaskedEdit", flag.ToString().ToLower(CultureInfo.InvariantCulture), true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "ValidEmpty", this.IsValidEmpty.ToString().ToLower(CultureInfo.InvariantCulture), true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MaximumValue", this.MaximumValue, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MinimumValue", this.MinimumValue, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "InitialValue", this.InitialValue, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "ValidationExpression", this.ValidationExpression, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "ClientValidationFunction", this.ClientValidationFunction, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "TargetValidator", box.ClientID, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "EmptyValueMessage", this.EmptyValueMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "EmptyValueText", this.EmptyValueBlurredText, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MaximumValueMessage", this.MaximumValueMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MaximumValueText", this.MaximumValueBlurredMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MinimumValueMessage", this.MinimumValueMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "MinimumValueText", this.MinimumValueBlurredText, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "InvalidValueMessage", this.InvalidValueMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "InvalidValueText", this.InvalidValueBlurredMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "InvalidValueCssClass", extender.OnInvalidCssClass, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "CssBlurNegative", extender.OnBlurCssNegative, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "CssFocus", extender.OnFocusCssClass, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "CssFocusNegative", extender.OnFocusCssNegative, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "TooltipMessage", this.TooltipMessage, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "FirstMaskPosition", firstMaskPosition.ToString(CultureInfo.InvariantCulture), true);
                if (!string.IsNullOrEmpty(extender.CultureName) && extender.OverridePageCulture)
                {
                    this.ControlCulture = CultureInfo.GetCultureInfo(extender.CultureName);
                }
                else
                {
                    this.ControlCulture = CultureInfo.CurrentCulture;
                }
                switch (extender.MaskType)
                {
                    case MaskedEditType.None:
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", "MaskedEditValidatorNone", true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "LastMaskPosition", num2.ToString(CultureInfo.InvariantCulture), true);
                        return;

                    case MaskedEditType.Date:
                    {
                        string dateSeparator = this.ControlCulture.DateTimeFormat.DateSeparator;
                        string[] strArray3 = this.ControlCulture.DateTimeFormat.ShortDatePattern.Split(new string[] { this.ControlCulture.DateTimeFormat.DateSeparator }, StringSplitOptions.None);
                        string attributeValue = strArray3[0].Substring(0, 1).ToUpper(this.ControlCulture) + strArray3[1].Substring(0, 1).ToUpper(this.ControlCulture) + strArray3[2].Substring(0, 1).ToUpper(this.ControlCulture);
                        attributeValue = (extender.UserDateFormat == MaskedEditUserDateFormat.None) ? attributeValue : extender.UserDateFormat.ToString();
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "DateSeparator", dateSeparator, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "DateFormat", attributeValue, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "Century", extender.Century.ToString(CultureInfo.InvariantCulture), true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", "MaskedEditValidatorDate", true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "LastMaskPosition", num2.ToString(CultureInfo.InvariantCulture), true);
                        return;
                    }
                    case MaskedEditType.Number:
                    {
                        string currencySymbol = this.ControlCulture.NumberFormat.CurrencySymbol;
                        string currencyDecimalSeparator = this.ControlCulture.NumberFormat.CurrencyDecimalSeparator;
                        string currencyGroupSeparator = this.ControlCulture.NumberFormat.CurrencyGroupSeparator;
                        if (extender.DisplayMoney != MaskedEditShowSymbol.None)
                        {
                            num2 += extender.CultureCurrencySymbolPlaceholder.Length + 1;
                        }
                        if (extender.AcceptNegative != MaskedEditShowSymbol.None)
                        {
                            if (extender.DisplayMoney != MaskedEditShowSymbol.None)
                            {
                                num2++;
                            }
                            else
                            {
                                num2 += 2;
                            }
                        }
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "Money", currencySymbol, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "Decimal", currencyDecimalSeparator, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "Thousands", currencyGroupSeparator, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", "MaskedEditValidatorNumber", true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "LastMaskPosition", num2.ToString(CultureInfo.InvariantCulture), true);
                        return;
                    }
                    case MaskedEditType.Time:
                    {
                        string timeSeparator = this.ControlCulture.DateTimeFormat.TimeSeparator;
                        string str10 = "";
                        if (!string.IsNullOrEmpty(this.ControlCulture.DateTimeFormat.AMDesignator + this.ControlCulture.DateTimeFormat.PMDesignator))
                        {
                            str10 = this.ControlCulture.DateTimeFormat.AMDesignator + ";" + this.ControlCulture.DateTimeFormat.PMDesignator;
                        }
                        else
                        {
                            str10 = "";
                        }
                        str10 = (extender.UserTimeFormat == MaskedEditUserTimeFormat.None) ? str10 : "";
                        if (extender.AcceptAMPM && !string.IsNullOrEmpty(str10))
                        {
                            char ch2 = char.Parse(timeSeparator);
                            string[] strArray4 = str10.Split(new char[] { ch2 });
                            num2 += strArray4[0].Length + 1;
                        }
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "TimeSeparator", timeSeparator, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "AmPmSymbol", str10, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", "MaskedEditValidatorTime", true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "LastMaskPosition", num2.ToString(CultureInfo.InvariantCulture), true);
                        return;
                    }
                    case MaskedEditType.DateTime:
                    {
                        str4 = this.ControlCulture.DateTimeFormat.DateSeparator;
                        string[] strArray = this.ControlCulture.DateTimeFormat.ShortDatePattern.Split(new string[] { this.ControlCulture.DateTimeFormat.DateSeparator }, StringSplitOptions.None);
                        string str5 = strArray[0].Substring(0, 1).ToUpper(this.ControlCulture) + strArray[1].Substring(0, 1).ToUpper(this.ControlCulture) + strArray[2].Substring(0, 1).ToUpper(this.ControlCulture);
                        str5 = (extender.UserDateFormat == MaskedEditUserDateFormat.None) ? str5 : extender.UserDateFormat.ToString();
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "DateSeparator", str4, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "DateFormat", str5, true);
                        ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "Century", extender.Century.ToString(CultureInfo.InvariantCulture), true);
                        str4 = this.ControlCulture.DateTimeFormat.TimeSeparator;
                        str6 = "";
                        if (!string.IsNullOrEmpty(this.ControlCulture.DateTimeFormat.AMDesignator + this.ControlCulture.DateTimeFormat.PMDesignator))
                        {
                            str6 = this.ControlCulture.DateTimeFormat.AMDesignator + ";" + this.ControlCulture.DateTimeFormat.PMDesignator;
                            break;
                        }
                        str6 = "";
                        break;
                    }
                    default:
                        return;
                }
                str6 = (extender.UserTimeFormat == MaskedEditUserTimeFormat.None) ? str6 : "";
                if (extender.AcceptAMPM && !string.IsNullOrEmpty(str6))
                {
                    char ch = char.Parse(str4);
                    string[] strArray2 = str6.Split(new char[] { ch });
                    num2 += strArray2[0].Length + 1;
                }
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "TimeSeparator", str4, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "AmPmSymbol", str6, true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "evaluationfunction", "MaskedEditValidatorDateTime", true);
                ScriptManager.RegisterExpandoAttribute(this, this.ClientID, "LastMaskPosition", num2.ToString(CultureInfo.InvariantCulture), true);
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string ClientValidationFunction
        {
            get
            {
                if (this._ClientValidationFunction == null)
                {
                    return string.Empty;
                }
                return this._ClientValidationFunction;
            }
            set
            {
                this._ClientValidationFunction = value;
            }
        }

        protected CultureInfo ControlCulture
        {
            get
            {
                if (this._Culture == null)
                {
                    this._Culture = CultureInfo.CurrentCulture;
                }
                return this._Culture;
            }
            set
            {
                this._Culture = value;
            }
        }

        [TypeConverter(typeof(MaskedEditTypeConvert)), DefaultValue(""), RequiredProperty, Category("MaskedEdit")]
        public string ControlExtender
        {
            get
            {
                if (this._ControlExtender == null)
                {
                    return string.Empty;
                }
                return this._ControlExtender;
            }
            set
            {
                this._ControlExtender = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string EmptyValueBlurredText
        {
            get
            {
                if (this._TextEmpty == null)
                {
                    return string.Empty;
                }
                return this._TextEmpty;
            }
            set
            {
                this._TextEmpty = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string EmptyValueMessage
        {
            get
            {
                if (this._MessageEmpty == null)
                {
                    return string.Empty;
                }
                return this._MessageEmpty;
            }
            set
            {
                this._MessageEmpty = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (string.IsNullOrEmpty(base.ErrorMessage))
                {
                    base.ErrorMessage = base.ID;
                }
                return base.ErrorMessage;
            }
            set
            {
                base.ErrorMessage = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string InitialValue
        {
            get
            {
                if (this._InitialValue == null)
                {
                    return string.Empty;
                }
                return this._InitialValue;
            }
            set
            {
                this._InitialValue = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string InvalidValueBlurredMessage
        {
            get
            {
                if (this._TextInvalid == null)
                {
                    return string.Empty;
                }
                return this._TextInvalid;
            }
            set
            {
                this._TextInvalid = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string InvalidValueMessage
        {
            get
            {
                if (this._MessageInvalid == null)
                {
                    return string.Empty;
                }
                return this._MessageInvalid;
            }
            set
            {
                this._MessageInvalid = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue(true)]
        public bool IsValidEmpty
        {
            get => 
                this._IsValidEmpty;
            set
            {
                this._IsValidEmpty = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string MaximumValue
        {
            get
            {
                if (this._MaximumValue == null)
                {
                    return string.Empty;
                }
                return this._MaximumValue;
            }
            set
            {
                this._MaximumValue = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string MaximumValueBlurredMessage
        {
            get
            {
                if (this._TextMax == null)
                {
                    return string.Empty;
                }
                return this._TextMax;
            }
            set
            {
                this._TextMax = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string MaximumValueMessage
        {
            get
            {
                if (this._MessageMax == null)
                {
                    return string.Empty;
                }
                return this._MessageMax;
            }
            set
            {
                this._MessageMax = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string MinimumValue
        {
            get
            {
                if (this._MinimumValue == null)
                {
                    return string.Empty;
                }
                return this._MinimumValue;
            }
            set
            {
                this._MinimumValue = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string MinimumValueBlurredText
        {
            get
            {
                if (this._TextMin == null)
                {
                    return string.Empty;
                }
                return this._TextMin;
            }
            set
            {
                this._TextMin = value;
            }
        }

        [Category("MaskedEdit"), DefaultValue("")]
        public string MinimumValueMessage
        {
            get
            {
                if (this._MessageMin == null)
                {
                    return string.Empty;
                }
                return this._MessageMin;
            }
            set
            {
                this._MessageMin = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string TooltipMessage
        {
            get
            {
                if (this._MessageTip == null)
                {
                    return string.Empty;
                }
                return this._MessageTip;
            }
            set
            {
                this._MessageTip = value;
            }
        }

        [DefaultValue(""), Category("MaskedEdit")]
        public string ValidationExpression
        {
            get
            {
                if (this._ValidationExpression == null)
                {
                    return string.Empty;
                }
                return this._ValidationExpression;
            }
            set
            {
                this._ValidationExpression = value;
            }
        }
    }
}

