namespace System.Xml.Serialization.Configuration
{
    using System.Configuration;

    public sealed class SerializationSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("dateTimeSerialization")]
        public DateTimeSerializationSection DateTimeSerialization =>
            ((DateTimeSerializationSection) base.Sections["dateTimeSerialization"]);

        [ConfigurationProperty("schemaImporterExtensions")]
        public SchemaImporterExtensionsSection SchemaImporterExtensions =>
            ((SchemaImporterExtensionsSection) base.Sections["schemaImporterExtensions"]);

        public XmlSerializerSection XmlSerializer =>
            ((XmlSerializerSection) base.Sections["xmlSerializer"]);
    }
}

