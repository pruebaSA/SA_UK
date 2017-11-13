namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    internal class MetadataArtifactLoaderXmlReaderWrapper : MetadataArtifactLoader, IComparable
    {
        private readonly XmlReader _reader;
        private readonly string _resourceUri;

        public MetadataArtifactLoaderXmlReaderWrapper(XmlReader xmlReader)
        {
            this._reader = xmlReader;
            this._resourceUri = xmlReader.BaseURI;
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
        }

        public int CompareTo(object obj)
        {
            MetadataArtifactLoaderXmlReaderWrapper wrapper = obj as MetadataArtifactLoaderXmlReaderWrapper;
            if ((wrapper != null) && object.ReferenceEquals(this._reader, wrapper._reader))
            {
                return 0;
            }
            return -1;
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            List<XmlReader> list = new List<XmlReader>();
            if (MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
            {
                list.Add(this._reader);
            }
            return list;
        }

        public override bool Equals(object obj) => 
            (this.CompareTo(obj) == 0);

        public override int GetHashCode() => 
            this._reader.GetHashCode();

        public override List<string> GetPaths() => 
            new List<string>(new string[] { this.Path });

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            if (MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
            {
                list.Add(this.Path);
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader> {
                this._reader
            };
            if (sourceDictionary != null)
            {
                sourceDictionary.Add(this, this._reader);
            }
            return list;
        }

        public override string Path
        {
            get
            {
                if (string.IsNullOrEmpty(this._resourceUri))
                {
                    return string.Empty;
                }
                return this._resourceUri;
            }
        }
    }
}

