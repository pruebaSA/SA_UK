namespace System.Web.Query.Dynamic
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class DynamicClass
    {
        protected DynamicClass()
        {
        }

        public override string ToString()
        {
            PropertyInfo[] properties = base.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            for (int i = 0; i < properties.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(properties[i].Name);
                builder.Append("=");
                builder.Append(properties[i].GetValue(this, null));
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}

