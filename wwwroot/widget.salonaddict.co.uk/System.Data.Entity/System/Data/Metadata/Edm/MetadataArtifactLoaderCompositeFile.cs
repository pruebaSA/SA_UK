namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class MetadataArtifactLoaderCompositeFile : MetadataArtifactLoader
    {
        private ReadOnlyCollection<MetadataArtifactLoaderFile> _csdlChildren;
        private ReadOnlyCollection<MetadataArtifactLoaderFile> _mslChildren;
        private readonly string _path;
        private ReadOnlyCollection<MetadataArtifactLoaderFile> _ssdlChildren;
        private readonly ICollection<string> _uriRegistry;

        public MetadataArtifactLoaderCompositeFile(string path, ICollection<string> uriRegistry)
        {
            this._path = path;
            this._uriRegistry = uriRegistry;
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
            IList<MetadataArtifactLoaderFile> list;
            if (this.TryGetListForSpace(spaceToGet, out list))
            {
                foreach (MetadataArtifactLoaderFile file in list)
                {
                    file.CollectFilePermissionPaths(paths, spaceToGet);
                }
            }
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            IList<MetadataArtifactLoaderFile> list2;
            List<XmlReader> list = new List<XmlReader>();
            if (this.TryGetListForSpace(spaceToGet, out list2))
            {
                foreach (MetadataArtifactLoaderFile file in list2)
                {
                    list.AddRange(file.CreateReaders(spaceToGet));
                }
            }
            return list;
        }

        private static List<MetadataArtifactLoaderFile> GetArtifactsInDirectory(string directory, string extension, ICollection<string> uriRegistry)
        {
            List<MetadataArtifactLoaderFile> list = new List<MetadataArtifactLoaderFile>();
            foreach (string str in Directory.GetFiles(directory, MetadataArtifactLoader.wildcard + extension, SearchOption.TopDirectoryOnly))
            {
                string item = System.IO.Path.Combine(directory, str);
                if (!uriRegistry.Contains(item) && str.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(new MetadataArtifactLoaderFile(item, uriRegistry));
                }
            }
            return list;
        }

        public override List<string> GetOriginalPaths(DataSpace spaceToGet) => 
            this.GetOriginalPaths();

        public override List<string> GetPaths()
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoaderFile file in this.CsdlChildren)
            {
                list.AddRange(file.GetPaths());
            }
            foreach (MetadataArtifactLoaderFile file2 in this.SsdlChildren)
            {
                list.AddRange(file2.GetPaths());
            }
            foreach (MetadataArtifactLoaderFile file3 in this.MslChildren)
            {
                list.AddRange(file3.GetPaths());
            }
            return list;
        }

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            IList<MetadataArtifactLoaderFile> list2;
            List<string> list = new List<string>();
            if (this.TryGetListForSpace(spaceToGet, out list2))
            {
                foreach (MetadataArtifactLoaderFile file in list2)
                {
                    list.AddRange(file.GetPaths(spaceToGet));
                }
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader>();
            foreach (MetadataArtifactLoaderFile file in this.CsdlChildren)
            {
                list.AddRange(file.GetReaders(sourceDictionary));
            }
            foreach (MetadataArtifactLoaderFile file2 in this.SsdlChildren)
            {
                list.AddRange(file2.GetReaders(sourceDictionary));
            }
            foreach (MetadataArtifactLoaderFile file3 in this.MslChildren)
            {
                list.AddRange(file3.GetReaders(sourceDictionary));
            }
            return list;
        }

        private void LoadCollections()
        {
            if (this._csdlChildren == null)
            {
                ReadOnlyCollection<MetadataArtifactLoaderFile> onlys = GetArtifactsInDirectory(this._path, ".csdl", this._uriRegistry).AsReadOnly();
                Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._csdlChildren, onlys, null);
            }
            if (this._ssdlChildren == null)
            {
                ReadOnlyCollection<MetadataArtifactLoaderFile> onlys2 = GetArtifactsInDirectory(this._path, ".ssdl", this._uriRegistry).AsReadOnly();
                Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._ssdlChildren, onlys2, null);
            }
            if (this._mslChildren == null)
            {
                ReadOnlyCollection<MetadataArtifactLoaderFile> onlys3 = GetArtifactsInDirectory(this._path, ".msl", this._uriRegistry).AsReadOnly();
                Interlocked.CompareExchange<ReadOnlyCollection<MetadataArtifactLoaderFile>>(ref this._mslChildren, onlys3, null);
            }
        }

        private bool TryGetListForSpace(DataSpace spaceToGet, out IList<MetadataArtifactLoaderFile> files)
        {
            switch (spaceToGet)
            {
                case DataSpace.CSpace:
                    files = this.CsdlChildren;
                    return true;

                case DataSpace.SSpace:
                    files = this.SsdlChildren;
                    return true;

                case DataSpace.CSSpace:
                    files = this.MslChildren;
                    return true;
            }
            files = null;
            return false;
        }

        internal ReadOnlyCollection<MetadataArtifactLoaderFile> CsdlChildren
        {
            get
            {
                this.LoadCollections();
                return this._csdlChildren;
            }
        }

        public override bool IsComposite =>
            true;

        internal ReadOnlyCollection<MetadataArtifactLoaderFile> MslChildren
        {
            get
            {
                this.LoadCollections();
                return this._mslChildren;
            }
        }

        public override string Path =>
            this._path;

        internal ReadOnlyCollection<MetadataArtifactLoaderFile> SsdlChildren
        {
            get
            {
                this.LoadCollections();
                return this._ssdlChildren;
            }
        }
    }
}

