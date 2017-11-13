namespace System.Web.Compilation.XmlSerializer
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
            (type == typeof(SvcMapFile));

        public override XmlSerializer GetSerializer(Type type)
        {
            if (type == typeof(SvcMapFile))
            {
                return new SvcMapFileSerializer();
            }
            return null;
        }

        public override XmlSerializationReader Reader =>
            new XmlSerializationReaderSvcMapFile();

        public override Hashtable ReadMethods
        {
            get
            {
                if (this.readMethods == null)
                {
                    Hashtable hashtable = new Hashtable {
                        ["System.Web.Compilation.WCFModel.SvcMapFile:urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceGroup:True:"] = "Read16_ReferenceGroup"
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
                            "System.Web.Compilation.WCFModel.SvcMapFile:urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceGroup:True:",
                            new SvcMapFileSerializer()
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
                        ["System.Web.Compilation.WCFModel.SvcMapFile:urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceGroup:True:"] = "Write16_ReferenceGroup"
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
            new XmlSerializationWriterSvcMapFile();
    }
}

