namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("CookieName"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CookieParameter : Parameter
    {
        public CookieParameter()
        {
        }

        protected CookieParameter(CookieParameter original) : base(original)
        {
            this.CookieName = original.CookieName;
        }

        public CookieParameter(string name, string cookieName) : base(name)
        {
            this.CookieName = cookieName;
        }

        public CookieParameter(string name, DbType dbType, string cookieName) : base(name, dbType)
        {
            this.CookieName = cookieName;
        }

        public CookieParameter(string name, TypeCode type, string cookieName) : base(name, type)
        {
            this.CookieName = cookieName;
        }

        protected override Parameter Clone() => 
            new CookieParameter(this);

        protected override object Evaluate(HttpContext context, Control control)
        {
            if ((context == null) || (context.Request == null))
            {
                return null;
            }
            HttpCookie cookie = context.Request.Cookies[this.CookieName];
            return cookie?.Value;
        }

        [DefaultValue(""), WebSysDescription("CookieParameter_CookieName"), WebCategory("Parameter")]
        public string CookieName
        {
            get
            {
                object obj2 = base.ViewState["CookieName"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                if (this.CookieName != value)
                {
                    base.ViewState["CookieName"] = value;
                    base.OnParameterChanged();
                }
            }
        }
    }
}

