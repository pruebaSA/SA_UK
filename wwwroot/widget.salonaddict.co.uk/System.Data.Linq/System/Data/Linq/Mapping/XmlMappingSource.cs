namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.IO;
    using System.Xml;

    public sealed class XmlMappingSource : MappingSource
    {
        private DatabaseMapping map;

        private XmlMappingSource(DatabaseMapping map)
        {
            this.map = map;
        }

        protected override MetaModel CreateModel(Type dataContextType)
        {
            if (dataContextType == null)
            {
                throw Error.ArgumentNull("dataContextType");
            }
            return new MappedMetaModel(this, dataContextType, this.map);
        }

        public static XmlMappingSource FromReader(XmlReader reader)
        {
            if (reader == null)
            {
                throw Error.ArgumentNull("reader");
            }
            reader.MoveToContent();
            DatabaseMapping map = XmlMappingReader.ReadDatabaseMapping(reader);
            if (map == null)
            {
                throw Error.DatabaseNodeNotFound("http://schemas.microsoft.com/linqtosql/mapping/2007");
            }
            return new XmlMappingSource(map);
        }

        public static XmlMappingSource FromStream(Stream stream)
        {
            if (stream == null)
            {
                throw Error.ArgumentNull("stream");
            }
            XmlReader reader = new XmlTextReader(stream);
            return FromReader(reader);
        }

        public static XmlMappingSource FromUrl(string url)
        {
            XmlMappingSource source;
            if (url == null)
            {
                throw Error.ArgumentNull("url");
            }
            XmlReader reader = new XmlTextReader(url);
            try
            {
                source = FromReader(reader);
            }
            finally
            {
                reader.Close();
            }
            return source;
        }

        public static XmlMappingSource FromXml(string xml)
        {
            if (xml == null)
            {
                throw Error.ArgumentNull("xml");
            }
            XmlReader reader = new XmlTextReader(new StringReader(xml));
            return FromReader(reader);
        }
    }
}

