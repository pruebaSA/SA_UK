namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class MetadataArtifactLoaderComposite : MetadataArtifactLoader, IEnumerable<MetadataArtifactLoader>, IEnumerable
    {
        private readonly ReadOnlyCollection<MetadataArtifactLoader> _children;

        public MetadataArtifactLoaderComposite(List<MetadataArtifactLoader> children)
        {
            this._children = new List<MetadataArtifactLoader>(children).AsReadOnly();
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
            foreach (MetadataArtifactLoader loader in this._children)
            {
                loader.CollectFilePermissionPaths(paths, spaceToGet);
            }
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            List<XmlReader> list = new List<XmlReader>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.CreateReaders(spaceToGet));
            }
            return list;
        }

        public IEnumerator<MetadataArtifactLoader> GetEnumerator() => 
            this._children.GetEnumerator();

        public override List<string> GetOriginalPaths()
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.GetOriginalPaths());
            }
            return list;
        }

        public override List<string> GetOriginalPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.GetOriginalPaths(spaceToGet));
            }
            return list;
        }

        public override List<string> GetPaths()
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.GetPaths());
            }
            return list;
        }

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.GetPaths(spaceToGet));
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader>();
            foreach (MetadataArtifactLoader loader in this._children)
            {
                list.AddRange(loader.GetReaders(sourceDictionary));
            }
            return list;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this._children.GetEnumerator();

        public override bool IsComposite =>
            true;

        public override string Path =>
            string.Empty;
    }
}

