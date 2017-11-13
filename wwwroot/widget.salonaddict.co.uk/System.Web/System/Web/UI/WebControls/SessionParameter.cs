namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("SessionField"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SessionParameter : Parameter
    {
        public SessionParameter()
        {
        }

        protected SessionParameter(SessionParameter original) : base(original)
        {
            this.SessionField = original.SessionField;
        }

        public SessionParameter(string name, string sessionField) : base(name)
        {
            this.SessionField = sessionField;
        }

        public SessionParameter(string name, DbType dbType, string sessionField) : base(name, dbType)
        {
            this.SessionField = sessionField;
        }

        public SessionParameter(string name, TypeCode type, string sessionField) : base(name, type)
        {
            this.SessionField = sessionField;
        }

        protected override Parameter Clone() => 
            new SessionParameter(this);

        protected override object Evaluate(HttpContext context, Control control)
        {
            if ((context != null) && (context.Session != null))
            {
                return context.Session[this.SessionField];
            }
            return null;
        }

        [DefaultValue(""), WebCategory("Parameter"), WebSysDescription("SessionParameter_SessionField")]
        public string SessionField
        {
            get
            {
                object obj2 = base.ViewState["SessionField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                if (this.SessionField != value)
                {
                    base.ViewState["SessionField"] = value;
                    base.OnParameterChanged();
                }
            }
        }
    }
}

