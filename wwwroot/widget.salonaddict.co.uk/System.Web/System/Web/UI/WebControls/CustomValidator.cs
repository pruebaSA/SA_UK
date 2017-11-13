namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultEvent("ServerValidate"), ToolboxData("<{0}:CustomValidator runat=\"server\" ErrorMessage=\"CustomValidator\"></{0}:CustomValidator>"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CustomValidator : BaseValidator
    {
        private static readonly object EventServerValidate = new object();

        [WebSysDescription("CustomValidator_ServerValidate")]
        public event ServerValidateEventHandler ServerValidate
        {
            add
            {
                base.Events.AddHandler(EventServerValidate, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventServerValidate, value);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);
            if (base.RenderUplevel)
            {
                string clientID = this.ClientID;
                HtmlTextWriter writer2 = base.EnableLegacyRendering ? writer : null;
                base.AddExpandoAttribute(writer2, clientID, "evaluationfunction", "CustomValidatorEvaluateIsValid", false);
                if (this.ClientValidationFunction.Length > 0)
                {
                    base.AddExpandoAttribute(writer2, clientID, "clientvalidationfunction", this.ClientValidationFunction);
                    if (this.ValidateEmptyText)
                    {
                        base.AddExpandoAttribute(writer2, clientID, "validateemptytext", "true", false);
                    }
                }
            }
        }

        protected override bool ControlPropertiesValid()
        {
            string controlToValidate = base.ControlToValidate;
            if (controlToValidate.Length > 0)
            {
                base.CheckControlValidationProperty(controlToValidate, "ControlToValidate");
            }
            return true;
        }

        protected override bool EvaluateIsValid()
        {
            string controlValidationValue = string.Empty;
            string controlToValidate = base.ControlToValidate;
            if (controlToValidate.Length > 0)
            {
                controlValidationValue = base.GetControlValidationValue(controlToValidate);
                if (((controlValidationValue == null) || (controlValidationValue.Trim().Length == 0)) && !this.ValidateEmptyText)
                {
                    return true;
                }
            }
            return this.OnServerValidate(controlValidationValue);
        }

        protected virtual bool OnServerValidate(string value)
        {
            ServerValidateEventHandler handler = (ServerValidateEventHandler) base.Events[EventServerValidate];
            ServerValidateEventArgs args = new ServerValidateEventArgs(value, true);
            if (handler != null)
            {
                handler(this, args);
                return args.IsValid;
            }
            return true;
        }

        [DefaultValue(""), WebCategory("Behavior"), WebSysDescription("CustomValidator_ClientValidationFunction"), Themeable(false)]
        public string ClientValidationFunction
        {
            get
            {
                object obj2 = this.ViewState["ClientValidationFunction"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ClientValidationFunction"] = value;
            }
        }

        [DefaultValue(false), WebSysDescription("CustomValidator_ValidateEmptyText"), WebCategory("Behavior"), Themeable(false)]
        public bool ValidateEmptyText
        {
            get
            {
                object obj2 = this.ViewState["ValidateEmptyText"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                this.ViewState["ValidateEmptyText"] = value;
            }
        }
    }
}

