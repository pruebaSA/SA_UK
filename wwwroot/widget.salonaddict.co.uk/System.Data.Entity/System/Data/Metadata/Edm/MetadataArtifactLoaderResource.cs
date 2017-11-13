namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class MetadataArtifactLoaderResource : MetadataArtifactLoader, IComparable
    {
        private readonly bool _alreadyLoaded;
        private readonly Assembly _assembly;
        private readonly string _resourceName;

        internal MetadataArtifactLoaderResource(Assembly assembly, string resourceName, ICollection<string> uriRegistry)
        {
            this._assembly = assembly;
            this._resourceName = resourceName;
            string item = MetadataArtifactLoaderCompositeResource.CreateResPath(this._assembly, this._resourceName);
            this._alreadyLoaded = uriRegistry.Contains(item);
            if (!this._alreadyLoaded)
            {
                uriRegistry.Add(item);
            }
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
        }

        public int CompareTo(object obj)
        {
            MetadataArtifactLoaderResource resource = obj as MetadataArtifactLoaderResource;
            if (resource != null)
            {
                return string.Compare(this.Path, resource.Path, StringComparison.OrdinalIgnoreCase);
            }
            return -1;
        }

        private XmlReader CreateReader()
        {
            Stream input = this.LoadResource();
            XmlReaderSettings settings = Schema.CreateEdmStandardXmlReaderSettings();
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            return XmlReader.Create(input, settings);
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            List<XmlReader> list = new List<XmlReader>();
            if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
            {
                XmlReader item = this.CreateReader();
                list.Add(item);
            }
            return list;
        }

        public override bool Equals(object obj) => 
            (this.CompareTo(obj) == 0);

        public override int GetHashCode() => 
            this.Path.GetHashCode();

        public override List<string> GetPaths()
        {
            List<string> list = new List<string>();
            if (!this._alreadyLoaded)
            {
                list.Add(this.Path);
            }
            return list;
        }

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            if (!this._alreadyLoaded && MetadataArtifactLoader.IsArtifactOfDataSpace(this.Path, spaceToGet))
            {
                list.Add(this.Path);
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader>();
            if (!this._alreadyLoaded)
            {
                XmlReader item = this.CreateReader();
                list.Add(item);
                if (sourceDictionary != null)
                {
                    sourceDictionary.Add(this, item);
                }
            }
            return list;
        }

        private Stream LoadResource()
        {
            Stream stream;
            if (!this.TryCreateResourceStream(out stream))
            {
                throw EntityUtil.Metadata(Strings.UnableToLoadResource);
            }
            return stream;
        }

        private bool TryCreateResourceStream(out Stream resourceStream)
        {
            resourceStream = this._assembly.GetManifestResourceStream(this._resourceName);
            return (resourceStream != null);
        }

        public override string Path =>
            MetadataArtifactLoaderCompositeResource.CreateResPath(this._assembly, this._resourceName);
    }
}

