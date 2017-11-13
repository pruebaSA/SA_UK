namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SessionPageStateSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propHistorySize = new ConfigurationProperty("historySize", typeof(int), 9, null, StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.None);
        public const int DefaultHistorySize = 9;

        static SessionPageStateSection()
        {
            _properties.Add(_propHistorySize);
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("historySize", DefaultValue=9)]
        public int HistorySize
        {
            get => 
                ((int) base[_propHistorySize]);
            set
            {
                base[_propHistorySize] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

