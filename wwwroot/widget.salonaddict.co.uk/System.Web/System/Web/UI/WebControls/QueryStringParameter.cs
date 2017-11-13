﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("QueryStringField"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class QueryStringParameter : Parameter
    {
        public QueryStringParameter()
        {
        }

        protected QueryStringParameter(QueryStringParameter original) : base(original)
        {
            this.QueryStringField = original.QueryStringField;
        }

        public QueryStringParameter(string name, string queryStringField) : base(name)
        {
            this.QueryStringField = queryStringField;
        }

        public QueryStringParameter(string name, DbType dbType, string queryStringField) : base(name, dbType)
        {
            this.QueryStringField = queryStringField;
        }

        public QueryStringParameter(string name, TypeCode type, string queryStringField) : base(name, type)
        {
            this.QueryStringField = queryStringField;
        }

        protected override Parameter Clone() => 
            new QueryStringParameter(this);

        protected override object Evaluate(HttpContext context, Control control)
        {
            if ((context != null) && (context.Request != null))
            {
                return context.Request.QueryString[this.QueryStringField];
            }
            return null;
        }

        [DefaultValue(""), WebCategory("Parameter"), WebSysDescription("QueryStringParameter_QueryStringField")]
        public string QueryStringField
        {
            get
            {
                object obj2 = base.ViewState["QueryStringField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                if (this.QueryStringField != value)
                {
                    base.ViewState["QueryStringField"] = value;
                    base.OnParameterChanged();
                }
            }
        }
    }
}

