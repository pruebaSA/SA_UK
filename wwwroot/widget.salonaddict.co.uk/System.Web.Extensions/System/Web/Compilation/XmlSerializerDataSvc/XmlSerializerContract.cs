namespace System.Web.Compilation.XmlSerializerDataSvc
{
    using System;
    using System.Collections;
    using System.Web.Compilation.WCFModel;
    using System.Xml.Serialization;

    internal class XmlSerializerContract : XmlSerializerImplementation
    {
        private Hashtable readMethods;
        private Hashtable typedSerializers;
        private Hashtable writeMethods;

        public override bool CanSerialize(Type type) => 
            (type == typeof(DataSvcMapFile));

        public override XmlSerializer GetSerializer(Type type)
        {
            if (type == typeof(DataSvcMapFile))
            {
                return new DataSvcMapFileSerializer();
            }
            return null;
        }

        public override XmlSerializationReader Reader =>
            new XmlSerializationReaderDataSvcMapFile();

        public override Hashtable ReadMethods
        {
            get
            {
                if (this.readMethods == null)
                {
                    Hashtable hashtable = new Hashtable {
                        ["System.Web.Compilation.WCFModel.DataSvcMapFile:urn:schemas-microsoft-com:xml-dataservicemap:ReferenceGroup:True:"] = "Read8_ReferenceGroup"
                    };
                    if (this.readMethods == null)
                    {
                        this.readMethods = hashtable;
                    }
                }
                return this.readMethods;
            }
        }

        public override Hashtable TypedSerializers
        {
            get
            {
                if (this.typedSerializers == null)
                {
                    Hashtable hashtable = new Hashtable {
                        { 
                            "System.Web.Compilation.WCFModel.DataSvcMapFile:urn:schemas-microsoft-com:xml-dataservicemap:ReferenceGroup:True:",
                            new DataSvcMapFileSerializer()
                        }
                    };
                    if (this.typedSerializers == null)
                    {
                        this.typedSerializers = hashtable;
                    }
                }
                return this.typedSerializers;
            }
        }

        public override Hashtable WriteMethods
        {
            get
            {
                if (this.writeMethods == null)
                {
                    Hashtable hashtable = new Hashtable {
                        ["System.Web.Compilation.WCFModel.DataSvcMapFile:urn:schemas-microsoft-com:xml-dataservicemap:ReferenceGroup:True:"] = "Write8_ReferenceGroup"
                    };
                    if (this.writeMethods == null)
                    {
                        this.writeMethods = hashtable;
                    }
                }
                return this.writeMethods;
            }
        }

        public override XmlSerializationWriter Writer =>
            new XmlSerializationWriterDataSvcMapFile();
    }
}

