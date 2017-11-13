namespace AjaxControlToolkit.MaskedEditValidatorCompatibility
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    internal static class ValidatorHelper
    {
        private const string ValidatorFileName = "WebUIValidation.js";
        private const string ValidatorIncludeScriptKey = "ValidatorIncludeScript";
        private const string ValidatorStartupScript = "\r\nvar Page_ValidationActive = false;\r\nif (typeof(ValidatorOnLoad) == \"function\") {\r\n    ValidatorOnLoad();\r\n}\r\n\r\nfunction ValidatorOnSubmit() {\r\n    if (Page_ValidationActive) {\r\n        return ValidatorCommonOnSubmit();\r\n    }\r\n    else {\r\n        return true;\r\n    }\r\n}\r\n";

        public static void AddExpandoAttribute(WebControl webControl, string controlId, string attributeName, string attributeValue)
        {
            AddExpandoAttribute(webControl, controlId, attributeName, attributeValue, true);
        }

        public static void AddExpandoAttribute(WebControl webControl, string controlId, string attributeName, string attributeValue, bool encode)
        {
            ScriptManager.RegisterExpandoAttribute(webControl, controlId, attributeName, attributeValue, encode);
        }

        public static void DoBaseValidatorAddAttributes(System.Web.UI.WebControls.BaseValidator validator, IBaseValidatorAccessor validatorAccessor, HtmlTextWriter writer)
        {
            bool flag = !validator.Enabled;
            if (flag)
            {
                validator.Enabled = true;
            }
            try
            {
                if (validatorAccessor.RenderUpLevel)
                {
                    validatorAccessor.EnsureID();
                    string clientID = validator.ClientID;
                    if (validator.ControlToValidate.Length > 0)
                    {
                        AddExpandoAttribute(validator, clientID, "controltovalidate", validatorAccessor.GetControlRenderID(validator.ControlToValidate));
                    }
                    if (validator.SetFocusOnError)
                    {
                        AddExpandoAttribute(validator, clientID, "focusOnError", "t", false);
                    }
                    if (validator.ErrorMessage.Length > 0)
                    {
                        AddExpandoAttribute(validator, clientID, "errormessage", validator.ErrorMessage);
                    }
                    ValidatorDisplay enumValue = validator.Display;
                    if (enumValue != ValidatorDisplay.Static)
                    {
                        AddExpandoAttribute(validator, clientID, "display", PropertyConverter.EnumToString(typeof(ValidatorDisplay), enumValue), false);
                    }
                    if (!validator.IsValid)
                    {
                        AddExpandoAttribute(validator, clientID, "isvalid", "False", false);
                    }
                    if (flag)
                    {
                        AddExpandoAttribute(validator, clientID, "enabled", "False", false);
                    }
                    if (validator.ValidationGroup.Length > 0)
                    {
                        AddExpandoAttribute(validator, clientID, "validationGroup", validator.ValidationGroup);
                    }
                }
                DoWebControlAddAttributes(validator, validatorAccessor, writer);
            }
            finally
            {
                if (flag)
                {
                    validator.Enabled = false;
                }
            }
        }

        public static void DoInitRegistration(Page page)
        {
            page.ClientScript.RegisterClientScriptBlock(typeof(System.Web.UI.WebControls.BaseValidator), "ValidatorIncludeScript", string.Empty);
        }

        public static void DoPreRenderRegistration(System.Web.UI.WebControls.BaseValidator validator, IBaseValidatorAccessor validatorAccessor)
        {
            if (validatorAccessor.RenderUpLevel)
            {
                ScriptManager.RegisterClientScriptResource(validator, typeof(System.Web.UI.WebControls.BaseValidator), "WebUIValidation.js");
                ScriptManager.RegisterStartupScript(validator, typeof(System.Web.UI.WebControls.BaseValidator), "ValidatorIncludeScript", "\r\nvar Page_ValidationActive = false;\r\nif (typeof(ValidatorOnLoad) == \"function\") {\r\n    ValidatorOnLoad();\r\n}\r\n\r\nfunction ValidatorOnSubmit() {\r\n    if (Page_ValidationActive) {\r\n        return ValidatorCommonOnSubmit();\r\n    }\r\n    else {\r\n        return true;\r\n    }\r\n}\r\n", true);
                ScriptManager.RegisterOnSubmitStatement(validator, typeof(System.Web.UI.WebControls.BaseValidator), "ValidatorOnSubmit", "if (typeof(ValidatorOnSubmit) == \"function\" && ValidatorOnSubmit() == false) return false;");
            }
        }

        public static void DoValidatorArrayDeclaration(System.Web.UI.WebControls.BaseValidator validator, Type validatorType)
        {
            string arrayValue = "document.getElementById(\"" + validator.ClientID + "\")";
            ScriptManager.RegisterArrayDeclaration(validator, "Page_Validators", arrayValue);
            ScriptManager.RegisterStartupScript(validator, validatorType, validator.ClientID + "_DisposeScript", string.Format(CultureInfo.InvariantCulture, "\r\ndocument.getElementById('{0}').dispose = function() {{\r\n    Array.remove(Page_Validators, document.getElementById('{0}'));\r\n}}\r\n", new object[] { validator.ClientID }), true);
        }

        public static void DoWebControlAddAttributes(WebControl webControl, IWebControlAccessor webControlAccessor, HtmlTextWriter writer)
        {
            if (webControl.ID != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, webControl.ClientID);
            }
            string accessKey = webControl.AccessKey;
            if (!string.IsNullOrEmpty(accessKey))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey);
            }
            if (!webControl.Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            int tabIndex = webControl.TabIndex;
            if (tabIndex != 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, tabIndex.ToString(NumberFormatInfo.InvariantInfo));
            }
            accessKey = webControl.ToolTip;
            if (!string.IsNullOrEmpty(accessKey))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Title, accessKey);
            }
            if (((webControlAccessor.TagKey == HtmlTextWriterTag.Span) || (webControlAccessor.TagKey == HtmlTextWriterTag.A)) && (((webControl.BorderStyle != BorderStyle.NotSet) || !webControl.BorderWidth.IsEmpty) || (!webControl.Height.IsEmpty || !webControl.Width.IsEmpty)))
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "inline-block");
            }
            if (webControl.ControlStyleCreated && !webControl.ControlStyle.IsEmpty)
            {
                webControl.ControlStyle.AddAttributesToRender(writer, webControl);
            }
            AttributeCollection attributes = webControl.Attributes;
            IEnumerator enumerator = attributes.Keys.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = (string) enumerator.Current;
                writer.AddAttribute(current, attributes[current]);
            }
        }
    }
}

