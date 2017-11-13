namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Script.Serialization;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingJsonSerializationSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propConverters = new ConfigurationProperty("converters", typeof(ConvertersCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        private static ConfigurationPropertyCollection _properties = BuildProperties();
        private static readonly ConfigurationProperty _propMaxJsonLength = new ConfigurationProperty("maxJsonLength", typeof(int), 0x19000, null, new IntegerValidator(1, 0x7fffffff), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRecursionLimitLimit = new ConfigurationProperty("recursionLimit", typeof(int), 100, null, new IntegerValidator(1, 0x7fffffff), ConfigurationPropertyOptions.None);

        private static ConfigurationPropertyCollection BuildProperties() => 
            new ConfigurationPropertyCollection { 
                _propConverters,
                _propRecursionLimitLimit,
                _propMaxJsonLength
            };

        [ConfigurationProperty("converters", IsKey=true, DefaultValue="")]
        public ConvertersCollection Converters =>
            ((ConvertersCollection) base[_propConverters]);

        [ConfigurationProperty("maxJsonLength", DefaultValue=0x19000)]
        public int MaxJsonLength
        {
            get => 
                ((int) base[_propMaxJsonLength]);
            set
            {
                base[_propMaxJsonLength] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("recursionLimit", DefaultValue=100)]
        public int RecursionLimit
        {
            get => 
                ((int) base[_propRecursionLimitLimit]);
            set
            {
                base[_propRecursionLimitLimit] = value;
            }
        }

        internal class ApplicationSettings
        {
            private JavaScriptConverter[] _converters;
            private int _maxJsonLimit;
            private int _recusionLimit;

            internal ApplicationSettings()
            {
                ScriptingJsonSerializationSection section = (ScriptingJsonSerializationSection) WebConfigurationManager.GetSection("system.web.extensions/scripting/webServices/jsonSerialization");
                if (section != null)
                {
                    this._recusionLimit = section.RecursionLimit;
                    this._maxJsonLimit = section.MaxJsonLength;
                    this._converters = section.Converters.CreateConverters();
                }
                else
                {
                    this._recusionLimit = (int) ScriptingJsonSerializationSection._propRecursionLimitLimit.DefaultValue;
                    this._maxJsonLimit = (int) ScriptingJsonSerializationSection._propMaxJsonLength.DefaultValue;
                    this._converters = new JavaScriptConverter[0];
                }
            }

            internal JavaScriptConverter[] Converters =>
                this._converters;

            internal int MaxJsonLimit =>
                this._maxJsonLimit;

            internal int RecursionLimit =>
                this._recusionLimit;
        }
    }
}

