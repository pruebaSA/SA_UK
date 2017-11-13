namespace System.Xml.XmlConfiguration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Xml;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class XmlTextReaderSection : ConfigurationSection
    {
        private bool _LimitCharactersFromEntities
        {
            get
            {
                string limitCharactersFromEntitiesString = this.LimitCharactersFromEntitiesString;
                bool result = true;
                XmlConvert.TryToBoolean(limitCharactersFromEntitiesString, out result);
                return result;
            }
        }

        internal static bool LimitCharactersFromEntities
        {
            get
            {
                XmlTextReaderSection section = ConfigurationManager.GetSection(XmlConfigurationString.XmlTextReaderSectionPath) as XmlTextReaderSection;
                return ((section == null) || section._LimitCharactersFromEntities);
            }
        }

        [ConfigurationProperty("limitCharactersFromEntities", DefaultValue="true")]
        internal string LimitCharactersFromEntitiesString
        {
            get => 
                ((string) base["limitCharactersFromEntities"]);
            set
            {
                base["limitCharactersFromEntities"] = value;
            }
        }
    }
}

