namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.IO;
    using System.Web.Compilation.XmlSerializerDataSvc;
    using System.Xml.Serialization;

    internal class DataSvcMapFileLoader : AbstractDataSvcMapFileLoader
    {
        private string mapFilePath;

        public DataSvcMapFileLoader(string mapFilePath)
        {
            this.mapFilePath = mapFilePath;
        }

        protected override TextReader GetMapFileReader() => 
            File.OpenText(this.mapFilePath);

        private string GetMetadataFileFullPath(string name) => 
            Path.Combine(Path.GetDirectoryName(this.mapFilePath), name);

        protected override byte[] ReadExtensionFile(string name) => 
            File.ReadAllBytes(this.GetMetadataFileFullPath(name));

        protected override byte[] ReadMetadataFile(string name) => 
            File.ReadAllBytes(this.GetMetadataFileFullPath(name));

        protected override XmlSerializer Serializer
        {
            get
            {
                if (base.serializer == null)
                {
                    base.serializer = new DataSvcMapFileSerializer();
                }
                return base.serializer;
            }
        }
    }
}

