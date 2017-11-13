namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ExpressionBuilder : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propExpressionPrefix = new ConfigurationProperty("expressionPrefix", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);

        static ExpressionBuilder()
        {
            _properties.Add(_propExpressionPrefix);
            _properties.Add(_propType);
        }

        internal ExpressionBuilder()
        {
        }

        public ExpressionBuilder(string expressionPrefix, string theType)
        {
            this.ExpressionPrefix = expressionPrefix;
            this.Type = theType;
        }

        [ConfigurationProperty("expressionPrefix", IsRequired=true, IsKey=true, DefaultValue=""), StringValidator(MinLength=1)]
        public string ExpressionPrefix
        {
            get => 
                ((string) base[_propExpressionPrefix]);
            set
            {
                base[_propExpressionPrefix] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [StringValidator(MinLength=1), ConfigurationProperty("type", IsRequired=true, DefaultValue="")]
        public string Type
        {
            get => 
                ((string) base[_propType]);
            set
            {
                base[_propType] = value;
            }
        }

        internal System.Type TypeInternal =>
            CompilationUtil.LoadTypeWithChecks(this.Type, typeof(System.Web.Compilation.ExpressionBuilder), null, this, "type");
    }
}

