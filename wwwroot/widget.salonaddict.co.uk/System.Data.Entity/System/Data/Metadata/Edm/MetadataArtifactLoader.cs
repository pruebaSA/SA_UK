namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.IO;
    using System.Xml;

    internal abstract class MetadataArtifactLoader
    {
        protected static readonly string altPathSeparator = @"\";
        protected static readonly string resPathPrefix = "res://";
        protected static readonly string resPathSeparator = "/";
        protected static readonly string wildcard = "*";

        protected MetadataArtifactLoader()
        {
        }

        internal static void CheckArtifactExtension(string path, string validExtension)
        {
            string extension = System.IO.Path.GetExtension(path);
            if (!extension.Equals(validExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw EntityUtil.Metadata(Strings.InvalidFileExtension(path, extension, validExtension));
            }
        }

        public abstract void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet);
        public static MetadataArtifactLoader Create(List<MetadataArtifactLoader> allCollections) => 
            new MetadataArtifactLoaderComposite(allCollections);

        public static MetadataArtifactLoader Create(string path, ExtensionCheck extensionCheck, string validExtension, ICollection<string> uriRegistry) => 
            Create(path, extensionCheck, validExtension, uriRegistry, new DefaultAssemblyResolver());

        internal static MetadataArtifactLoader Create(string path, ExtensionCheck extensionCheck, string validExtension, ICollection<string> uriRegistry, MetadataArtifactAssemblyResolver resolver)
        {
            if (PathStartsWithResPrefix(path))
            {
                return MetadataArtifactLoaderCompositeResource.CreateResourceLoader(path, extensionCheck, validExtension, uriRegistry, resolver);
            }
            string str = NormalizeFilePaths(path);
            if (Directory.Exists(str))
            {
                return new MetadataArtifactLoaderCompositeFile(str, uriRegistry);
            }
            if (!File.Exists(str))
            {
                throw EntityUtil.Metadata(Strings.InvalidMetadataPath);
            }
            switch (extensionCheck)
            {
                case ExtensionCheck.Specific:
                    CheckArtifactExtension(str, validExtension);
                    break;

                case ExtensionCheck.All:
                    if (!IsValidArtifact(str))
                    {
                        throw EntityUtil.Metadata(Strings.InvalidMetadataPath);
                    }
                    break;
            }
            return new MetadataArtifactLoaderFile(str, uriRegistry);
        }

        public static MetadataArtifactLoader CreateCompositeFromFilePaths(IEnumerable<string> filePaths, string validExtension) => 
            CreateCompositeFromFilePaths(filePaths, validExtension, new DefaultAssemblyResolver());

        internal static MetadataArtifactLoader CreateCompositeFromFilePaths(IEnumerable<string> filePaths, string validExtension, MetadataArtifactAssemblyResolver resolver)
        {
            ExtensionCheck all;
            if (string.IsNullOrEmpty(validExtension))
            {
                all = ExtensionCheck.All;
            }
            else
            {
                all = ExtensionCheck.Specific;
            }
            List<MetadataArtifactLoader> allCollections = new List<MetadataArtifactLoader>();
            HashSet<string> uriRegistry = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string str in filePaths)
            {
                if (string.IsNullOrEmpty(str))
                {
                    throw EntityUtil.Metadata(Strings.NotValidInputPath, EntityUtil.CollectionParameterElementIsNullOrEmpty("filePaths"));
                }
                string path = str.Trim();
                if (path.Length > 0)
                {
                    allCollections.Add(Create(path, all, validExtension, uriRegistry, resolver));
                }
            }
            return Create(allCollections);
        }

        public static MetadataArtifactLoader CreateCompositeFromXmlReaders(IEnumerable<XmlReader> xmlReaders)
        {
            List<MetadataArtifactLoader> allCollections = new List<MetadataArtifactLoader>();
            foreach (XmlReader reader in xmlReaders)
            {
                if (reader == null)
                {
                    throw EntityUtil.CollectionParameterElementIsNull("xmlReaders");
                }
                allCollections.Add(new MetadataArtifactLoaderXmlReaderWrapper(reader));
            }
            return Create(allCollections);
        }

        public abstract List<XmlReader> CreateReaders(DataSpace spaceToGet);
        public virtual List<string> GetOriginalPaths() => 
            new List<string>(new string[] { this.Path });

        public virtual List<string> GetOriginalPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            if (IsArtifactOfDataSpace(this.Path, spaceToGet))
            {
                list.Add(this.Path);
            }
            return list;
        }

        public abstract List<string> GetPaths();
        public abstract List<string> GetPaths(DataSpace spaceToGet);
        public List<XmlReader> GetReaders() => 
            this.GetReaders(null);

        public abstract List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary);
        protected static bool IsArtifactOfDataSpace(string resource, DataSpace dataSpace)
        {
            if (dataSpace == DataSpace.CSpace)
            {
                return IsCSpaceArtifact(resource);
            }
            if (dataSpace == DataSpace.SSpace)
            {
                return IsSSpaceArtifact(resource);
            }
            return ((dataSpace == DataSpace.CSSpace) && IsCSSpaceArtifact(resource));
        }

        protected static bool IsCSpaceArtifact(string resource)
        {
            string extension = System.IO.Path.GetExtension(resource);
            return (!string.IsNullOrEmpty(extension) && (string.Compare(extension, ".csdl", StringComparison.OrdinalIgnoreCase) == 0));
        }

        protected static bool IsCSSpaceArtifact(string resource)
        {
            string extension = System.IO.Path.GetExtension(resource);
            return (!string.IsNullOrEmpty(extension) && (string.Compare(extension, ".msl", StringComparison.OrdinalIgnoreCase) == 0));
        }

        protected static bool IsSSpaceArtifact(string resource)
        {
            string extension = System.IO.Path.GetExtension(resource);
            return (!string.IsNullOrEmpty(extension) && (string.Compare(extension, ".ssdl", StringComparison.OrdinalIgnoreCase) == 0));
        }

        internal static bool IsValidArtifact(string resource)
        {
            string extension = System.IO.Path.GetExtension(resource);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }
            if ((string.Compare(extension, ".csdl", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(extension, ".ssdl", StringComparison.OrdinalIgnoreCase) != 0))
            {
                return (string.Compare(extension, ".msl", StringComparison.OrdinalIgnoreCase) == 0);
            }
            return true;
        }

        internal static string NormalizeFilePaths(string path)
        {
            bool flag = true;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim();
                if (path.StartsWith("~", StringComparison.Ordinal))
                {
                    path = new AspProxy().MapWebPath(path);
                    flag = false;
                }
                if ((path.Length == 2) && (path[1] == System.IO.Path.VolumeSeparatorChar))
                {
                    path = path + System.IO.Path.DirectorySeparatorChar;
                }
                else
                {
                    string datadir = null;
                    string str2 = DbConnectionOptions.ExpandDataDirectory("metadata", path, ref datadir);
                    if (str2 != null)
                    {
                        path = str2;
                        flag = false;
                    }
                }
            }
            try
            {
                if (flag)
                {
                    path = System.IO.Path.GetFullPath(path);
                }
            }
            catch (ArgumentException exception)
            {
                throw EntityUtil.Metadata(Strings.NotValidInputPath, exception);
            }
            catch (NotSupportedException exception2)
            {
                throw EntityUtil.Metadata(Strings.NotValidInputPath, exception2);
            }
            catch (PathTooLongException)
            {
                throw EntityUtil.Metadata(Strings.NotValidInputPath);
            }
            return path;
        }

        internal static bool PathStartsWithResPrefix(string path) => 
            path.StartsWith(resPathPrefix, StringComparison.OrdinalIgnoreCase);

        public virtual bool IsComposite =>
            false;

        public abstract string Path { get; }

        public enum ExtensionCheck
        {
            None,
            Specific,
            All
        }
    }
}

