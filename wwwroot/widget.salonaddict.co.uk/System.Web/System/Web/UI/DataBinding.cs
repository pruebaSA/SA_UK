namespace System.Web.UI
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DataBinding
    {
        private string expression;
        private string propertyName;
        private Type propertyType;

        public DataBinding(string propertyName, Type propertyType, string expression)
        {
            this.propertyName = propertyName;
            this.propertyType = propertyType;
            this.expression = expression;
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is DataBinding))
            {
                DataBinding binding = (DataBinding) obj;
                return StringUtil.EqualsIgnoreCase(this.propertyName, binding.PropertyName);
            }
            return false;
        }

        public override int GetHashCode() => 
            this.propertyName.ToLower(CultureInfo.InvariantCulture).GetHashCode();

        public string Expression
        {
            get => 
                this.expression;
            set
            {
                this.expression = value;
            }
        }

        public string PropertyName =>
            this.propertyName;

        public Type PropertyType =>
            this.propertyType;
    }
}

