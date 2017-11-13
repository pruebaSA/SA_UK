namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.Util;

    [ToolboxData("<{0}:CompareValidator runat=\"server\" ErrorMessage=\"CompareValidator\"></{0}:CompareValidator>"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CompareValidator : BaseCompareValidator
    {
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            if (base.RenderUplevel)
            {
                string clientID = this.ClientID;
                HtmlTextWriter writer2 = base.EnableLegacyRendering ? writer : null;
                base.AddExpandoAttribute(writer2, clientID, "evaluationfunction", "CompareValidatorEvaluateIsValid", false);
                if (this.ControlToCompare.Length > 0)
                {
                    string controlRenderID = base.GetControlRenderID(this.ControlToCompare);
                    base.AddExpandoAttribute(writer2, clientID, "controltocompare", controlRenderID);
                    base.AddExpandoAttribute(writer2, clientID, "controlhookup", controlRenderID);
                }
                if (this.ValueToCompare.Length > 0)
                {
                    string valueToCompare = this.ValueToCompare;
                    if (base.CultureInvariantValues)
                    {
                        valueToCompare = base.ConvertCultureInvariantToCurrentCultureFormat(valueToCompare, base.Type);
                    }
                    base.AddExpandoAttribute(writer2, clientID, "valuetocompare", valueToCompare);
                }
                if (this.Operator != ValidationCompareOperator.Equal)
                {
                    base.AddExpandoAttribute(writer2, clientID, "operator", PropertyConverter.EnumToString(typeof(ValidationCompareOperator), this.Operator), false);
                }
            }
        }

        protected override bool ControlPropertiesValid()
        {
            if (this.ControlToCompare.Length > 0)
            {
                base.CheckControlValidationProperty(this.ControlToCompare, "ControlToCompare");
                if (StringUtil.EqualsIgnoreCase(base.ControlToValidate, this.ControlToCompare))
                {
                    throw new HttpException(System.Web.SR.GetString("Validator_bad_compare_control", new object[] { this.ID, this.ControlToCompare }));
                }
            }
            else if ((this.Operator != ValidationCompareOperator.DataTypeCheck) && !BaseCompareValidator.CanConvert(this.ValueToCompare, base.Type, base.CultureInvariantValues))
            {
                throw new HttpException(System.Web.SR.GetString("Validator_value_bad_type", new string[] { this.ValueToCompare, "ValueToCompare", this.ID, PropertyConverter.EnumToString(typeof(ValidationDataType), base.Type) }));
            }
            return base.ControlPropertiesValid();
        }

        protected override bool EvaluateIsValid()
        {
            string controlValidationValue = base.GetControlValidationValue(base.ControlToValidate);
            if (controlValidationValue.Trim().Length == 0)
            {
                return true;
            }
            bool flag = (base.Type == ValidationDataType.Date) && !this.DetermineRenderUplevel();
            if (flag && !base.IsInStandardDateFormat(controlValidationValue))
            {
                controlValidationValue = base.ConvertToShortDateString(controlValidationValue);
            }
            bool cultureInvariantRightText = false;
            string date = string.Empty;
            if (this.ControlToCompare.Length > 0)
            {
                date = base.GetControlValidationValue(this.ControlToCompare);
                if (flag && !base.IsInStandardDateFormat(date))
                {
                    date = base.ConvertToShortDateString(date);
                }
            }
            else
            {
                date = this.ValueToCompare;
                cultureInvariantRightText = base.CultureInvariantValues;
            }
            return BaseCompareValidator.Compare(controlValidationValue, false, date, cultureInvariantRightText, this.Operator, base.Type);
        }

        [TypeConverter(typeof(ValidatedControlConverter)), WebCategory("Behavior"), Themeable(false), DefaultValue(""), WebSysDescription("CompareValidator_ControlToCompare")]
        public string ControlToCompare
        {
            get
            {
                object obj2 = this.ViewState["ControlToCompare"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ControlToCompare"] = value;
            }
        }

        [WebCategory("Behavior"), Themeable(false), DefaultValue(0), WebSysDescription("CompareValidator_Operator")]
        public ValidationCompareOperator Operator
        {
            get
            {
                object obj2 = this.ViewState["Operator"];
                if (obj2 != null)
                {
                    return (ValidationCompareOperator) obj2;
                }
                return ValidationCompareOperator.Equal;
            }
            set
            {
                if ((value < ValidationCompareOperator.Equal) || (value > ValidationCompareOperator.DataTypeCheck))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["Operator"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("CompareValidator_ValueToCompare"), WebCategory("Behavior"), Themeable(false)]
        public string ValueToCompare
        {
            get
            {
                object obj2 = this.ViewState["ValueToCompare"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ValueToCompare"] = value;
            }
        }
    }
}

