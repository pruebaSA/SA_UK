namespace System.Web.UI
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ExpressionBinding
    {
        private string _expression;
        private string _expressionPrefix;
        private bool _generated;
        private object _parsedExpressionData;
        private string _propertyName;
        private Type _propertyType;

        public ExpressionBinding(string propertyName, Type propertyType, string expressionPrefix, string expression) : this(propertyName, propertyType, expressionPrefix, expression, false, null)
        {
        }

        internal ExpressionBinding(string propertyName, Type propertyType, string expressionPrefix, string expression, bool generated, object parsedExpressionData)
        {
            this._propertyName = propertyName;
            this._propertyType = propertyType;
            this._expression = expression;
            this._expressionPrefix = expressionPrefix;
            this._generated = generated;
            this._parsedExpressionData = parsedExpressionData;
        }

        public override bool Equals(object obj)
        {
            if ((obj != null) && (obj is ExpressionBinding))
            {
                ExpressionBinding binding = (ExpressionBinding) obj;
                return StringUtil.EqualsIgnoreCase(this._propertyName, binding.PropertyName);
            }
            return false;
        }

        public override int GetHashCode() => 
            this._propertyName.ToLower(CultureInfo.InvariantCulture).GetHashCode();

        public string Expression
        {
            get => 
                this._expression;
            set
            {
                this._expression = value;
            }
        }

        public string ExpressionPrefix
        {
            get => 
                this._expressionPrefix;
            set
            {
                this._expressionPrefix = value;
            }
        }

        public bool Generated =>
            this._generated;

        public object ParsedExpressionData =>
            this._parsedExpressionData;

        public string PropertyName =>
            this._propertyName;

        public Type PropertyType =>
            this._propertyType;
    }
}

