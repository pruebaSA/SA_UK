namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Xml;

    internal class MetadataArtifactLoaderFile : MetadataArtifactLoader, IComparable
    {
        private readonly bool _alreadyLoaded;
        private readonly string _path;

        public MetadataArtifactLoaderFile(string path, ICollection<string> uriRegistry)
        {
            this._path = path;
            this._alreadyLoaded = uriRegistry.Contains(this._path);
            if (!this._alreadyLoaded)
            {
                uriRegistry.Add(this._path);
            }
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
            if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this._path, spaceToGet))
            {
                paths.Add(this._path);
            }
        }

        public int CompareTo(object obj)
        {
            MetadataArtifactLoaderFile file = obj as MetadataArtifactLoaderFile;
            if (file != null)
            {
                return string.Compare(this._path, file._path, StringComparison.OrdinalIgnoreCase);
            }
            return -1;
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            List<XmlReader> list = new List<XmlReader>();
            if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this._path, spaceToGet))
            {
                XmlReader item = this.CreateXmlReader();
                list.Add(item);
            }
            return list;
        }

        private XmlReader CreateXmlReader()
        {
            XmlReaderSettings settings = Schema.CreateEdmStandardXmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            return XmlReader.Create(this._path, settings);
        }

        public override bool Equals(object obj) => 
            (this.CompareTo(obj) == 0);

        public override int GetHashCode() => 
            this._path.GetHashCode();

        public override List<string> GetPaths()
        {
            List<string> list = new List<string>();
            if (!this._alreadyLoaded)
            {
                list.Add(this._path);
            }
            return list;
        }

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this._path, spaceToGet))
            {
                list.Add(this._path);
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader>();
            if (!this._alreadyLoaded)
            {
                XmlReader item = this.CreateXmlReader();
                list.Add(item);
                if (sourceDictionary != null)
                {
                    sourceDictionary.Add(this, item);
                }
            }
            return list;
        }

        public override string Path =>
            this._path;
    }
}

