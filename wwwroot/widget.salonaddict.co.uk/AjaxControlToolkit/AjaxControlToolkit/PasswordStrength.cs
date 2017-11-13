namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [TargetControlType(typeof(TextBox)), Designer("AjaxControlToolkit.PasswordStrengthExtenderDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.PasswordStrengthExtenderBehavior", "PasswordStrength.PasswordStrengthExtenderBehavior.js"), RequiredScript(typeof(CommonToolkitScripts)), ToolboxBitmap(typeof(PasswordStrength), "PasswordStrength.PasswordStrength.ico")]
    public class PasswordStrength : ExtenderControlBase
    {
        private const string _barBorderCssClass = "BarBorderCssClass";
        private const string _barIndicatorCssClass = "BarIndicatorCssClass";
        private const string _calcWeightings = "CalculationWeightings";
        private const string _displayPosition = "DisplayPosition";
        private const string _helpHandleCssClass = "HelpHandleCssClass";
        private const string _helphandlePosition = "HelpHandlePosition";
        private const string _helpStatusLabelID = "HelpStatusLabelID";
        private const string _minLowerCaseChars = "MinimumLowerCaseCharacters";
        private const string _minPasswordNumerics = "MinimumNumericCharacters";
        private const string _minPasswordSymbols = "MinimumSymbolCharacters";
        private const string _minUpperCaseChars = "MinimumUpperCaseCharacters";
        private const string _preferredPasswordLength = "PreferredPasswordLength";
        private const string _prefixText = "PrefixText";
        private const string _prefixTextDefault = "Strength: ";
        private const string _requiresUpperLowerCase = "RequiresUpperAndLowerCaseCharacters";
        private const string _strengthIndicatorType = "StrengthIndicatorType";
        private const string _strengthStyles = "StrengthStyles";
        private const string _txtDisplayIndicators = "TextStrengthDescriptions";
        private const string _txtPasswordCssClass = "TextCssClass";
        private const char TXT_INDICATOR_DELIMITER = ';';
        private const int TXT_INDICATORS_MAX_COUNT = 10;
        private const int TXT_INDICATORS_MIN_COUNT = 2;

        [DefaultValue((string) null), ExtenderControlProperty]
        public string BarBorderCssClass
        {
            get => 
                base.GetPropertyValue<string>("BarBorderCssClass", null);
            set
            {
                base.SetPropertyValue<string>("BarBorderCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue((string) null)]
        public string BarIndicatorCssClass
        {
            get => 
                base.GetPropertyValue<string>("BarIndicatorCssClass", null);
            set
            {
                base.SetPropertyValue<string>("BarIndicatorCssClass", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string CalculationWeightings
        {
            get => 
                base.GetPropertyValue<string>("CalculationWeightings", "");
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.SetPropertyValue<string>("CalculationWeightings", value);
                }
                else
                {
                    int num = 0;
                    if (value != null)
                    {
                        foreach (string str in value.Split(new char[] { ';' }))
                        {
                            int num2;
                            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
                            {
                                num += num2;
                            }
                        }
                    }
                    if (num != 100)
                    {
                        throw new ArgumentException("There must be 4 Calculation Weighting items which must total 100");
                    }
                    base.SetPropertyValue<string>("CalculationWeightings", value);
                }
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public AjaxControlToolkit.DisplayPosition DisplayPosition
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.DisplayPosition>("DisplayPosition", AjaxControlToolkit.DisplayPosition.RightSide);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.DisplayPosition>("DisplayPosition", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string HelpHandleCssClass
        {
            get => 
                base.GetPropertyValue<string>("HelpHandleCssClass", "");
            set
            {
                base.SetPropertyValue<string>("HelpHandleCssClass", value);
            }
        }

        [DefaultValue(1), ExtenderControlProperty]
        public AjaxControlToolkit.DisplayPosition HelpHandlePosition
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.DisplayPosition>("HelpHandlePosition", AjaxControlToolkit.DisplayPosition.AboveRight);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.DisplayPosition>("HelpHandlePosition", value);
            }
        }

        [DefaultValue(""), IDReferenceProperty(typeof(Label)), ExtenderControlProperty]
        public string HelpStatusLabelID
        {
            get => 
                base.GetPropertyValue<string>("HelpStatusLabelID", "");
            set
            {
                base.SetPropertyValue<string>("HelpStatusLabelID", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int MinimumLowerCaseCharacters
        {
            get => 
                base.GetPropertyValue<int>("MinimumLowerCaseCharacters", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumLowerCaseCharacters", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int MinimumNumericCharacters
        {
            get => 
                base.GetPropertyValue<int>("MinimumNumericCharacters", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumNumericCharacters", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int MinimumSymbolCharacters
        {
            get => 
                base.GetPropertyValue<int>("MinimumSymbolCharacters", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumSymbolCharacters", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int MinimumUpperCaseCharacters
        {
            get => 
                base.GetPropertyValue<int>("MinimumUpperCaseCharacters", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumUpperCaseCharacters", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int PreferredPasswordLength
        {
            get => 
                base.GetPropertyValue<int>("PreferredPasswordLength", 0);
            set
            {
                base.SetPropertyValue<int>("PreferredPasswordLength", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("Strength: ")]
        public string PrefixText
        {
            get => 
                base.GetPropertyValue<string>("PrefixText", "Strength: ");
            set
            {
                base.SetPropertyValue<string>("PrefixText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool RequiresUpperAndLowerCaseCharacters
        {
            get => 
                base.GetPropertyValue<bool>("RequiresUpperAndLowerCaseCharacters", false);
            set
            {
                base.SetPropertyValue<bool>("RequiresUpperAndLowerCaseCharacters", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public StrengthIndicatorTypes StrengthIndicatorType
        {
            get => 
                base.GetPropertyValue<StrengthIndicatorTypes>("StrengthIndicatorType", StrengthIndicatorTypes.Text);
            set
            {
                base.SetPropertyValue<StrengthIndicatorTypes>("StrengthIndicatorType", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string StrengthStyles
        {
            get => 
                base.GetPropertyValue<string>("StrengthStyles", "");
            set
            {
                bool flag = false;
                if (!string.IsNullOrEmpty(value) && (value.Split(new char[] { ';' }).Length <= 10))
                {
                    flag = true;
                }
                if (!flag)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid property specification for TextStrengthDescriptionStyles property. Must match the number of entries for the TextStrengthDescriptions property.", new object[0]));
                }
                base.SetPropertyValue<string>("StrengthStyles", value);
            }
        }

        [DefaultValue((string) null), ExtenderControlProperty]
        public string TextCssClass
        {
            get => 
                base.GetPropertyValue<string>("TextCssClass", null);
            set
            {
                base.SetPropertyValue<string>("TextCssClass", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string TextStrengthDescriptions
        {
            get => 
                base.GetPropertyValue<string>("TextStrengthDescriptions", "");
            set
            {
                bool flag = false;
                if (!string.IsNullOrEmpty(value))
                {
                    string[] strArray = value.Split(new char[] { ';' });
                    if ((strArray.Length >= 2) && (strArray.Length <= 10))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid property specification for TextStrengthDescriptions property. Must be a string delimited with '{0}', contain a minimum of {1} entries, and a maximum of {2}.", new object[] { ';', 2, 10 }));
                }
                base.SetPropertyValue<string>("TextStrengthDescriptions", value);
            }
        }

        [DefaultValue(""), Obsolete("This property has been deprecated. Please use the StrengthStyles property instead."), ExtenderControlProperty]
        public string TextStrengthDescriptionStyles
        {
            get => 
                this.StrengthStyles;
            set
            {
                this.StrengthStyles = value;
            }
        }
    }
}

