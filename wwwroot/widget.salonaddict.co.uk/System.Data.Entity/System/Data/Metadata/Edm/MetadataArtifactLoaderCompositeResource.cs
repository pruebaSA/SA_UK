namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class MetadataArtifactLoaderCompositeResource : MetadataArtifactLoader
    {
        private readonly ReadOnlyCollection<MetadataArtifactLoaderResource> _children;
        private readonly string _originalPath;

        internal MetadataArtifactLoaderCompositeResource(string originalPath, string assemblyName, string resourceName, ICollection<string> uriRegistry, MetadataArtifactAssemblyResolver resolver)
        {
            this._originalPath = originalPath;
            this._children = LoadResources(assemblyName, resourceName, uriRegistry, resolver).AsReadOnly();
        }

        private static bool AssemblyContainsResource(Assembly assembly, ref string resourceName)
        {
            if (resourceName == null)
            {
                return true;
            }
            foreach (string str in GetManifestResourceNamesForAssembly(assembly))
            {
                if (string.Equals(resourceName, str, StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = str;
                    return true;
                }
            }
            return false;
        }

        public override void CollectFilePermissionPaths(List<string> paths, DataSpace spaceToGet)
        {
            foreach (MetadataArtifactLoaderResource resource in this._children)
            {
                resource.CollectFilePermissionPaths(paths, spaceToGet);
            }
        }

        private static void CreateAndAddSingleResourceLoader(Assembly assembly, string resourceName, ICollection<string> uriRegistry, List<MetadataArtifactLoaderResource> loaders)
        {
            string item = CreateResPath(assembly, resourceName);
            if (!uriRegistry.Contains(item))
            {
                loaders.Add(new MetadataArtifactLoaderResource(assembly, resourceName, uriRegistry));
            }
        }

        public override List<XmlReader> CreateReaders(DataSpace spaceToGet)
        {
            List<XmlReader> list = new List<XmlReader>();
            foreach (MetadataArtifactLoaderResource resource in this._children)
            {
                list.AddRange(resource.CreateReaders(spaceToGet));
            }
            return list;
        }

        internal static MetadataArtifactLoader CreateResourceLoader(string path, MetadataArtifactLoader.ExtensionCheck extensionCheck, string validExtension, ICollection<string> uriRegistry, MetadataArtifactAssemblyResolver resolver)
        {
            bool flag = false;
            string assemblyName = null;
            string resourceName = null;
            ParseResourcePath(path, out assemblyName, out resourceName);
            flag = (assemblyName != null) && ((resourceName == null) || (assemblyName.Trim() == MetadataArtifactLoader.wildcard));
            ValidateExtension(extensionCheck, validExtension, resourceName);
            if (flag)
            {
                return new MetadataArtifactLoaderCompositeResource(path, assemblyName, resourceName, uriRegistry, resolver);
            }
            return new MetadataArtifactLoaderResource(ResolveAssemblyName(assemblyName, resolver), resourceName, uriRegistry);
        }

        internal static string CreateResPath(Assembly assembly, string resourceName) => 
            string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", new object[] { MetadataArtifactLoader.resPathPrefix, assembly.FullName, MetadataArtifactLoader.resPathSeparator, resourceName });

        internal static string[] GetManifestResourceNamesForAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch (NotSupportedException)
            {
                return new string[0];
            }
        }

        public override List<string> GetOriginalPaths(DataSpace spaceToGet) => 
            this.GetOriginalPaths();

        public override List<string> GetPaths()
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoaderResource resource in this._children)
            {
                list.AddRange(resource.GetPaths());
            }
            return list;
        }

        public override List<string> GetPaths(DataSpace spaceToGet)
        {
            List<string> list = new List<string>();
            foreach (MetadataArtifactLoaderResource resource in this._children)
            {
                list.AddRange(resource.GetPaths(spaceToGet));
            }
            return list;
        }

        public override List<XmlReader> GetReaders(Dictionary<MetadataArtifactLoader, XmlReader> sourceDictionary)
        {
            List<XmlReader> list = new List<XmlReader>();
            foreach (MetadataArtifactLoaderResource resource in this._children)
            {
                list.AddRange(resource.GetReaders(sourceDictionary));
            }
            return list;
        }

        private static void LoadAllResourcesFromAssembly(Assembly assembly, ICollection<string> uriRegistry, List<MetadataArtifactLoaderResource> loaders, MetadataArtifactAssemblyResolver resolver)
        {
            foreach (string str in GetManifestResourceNamesForAssembly(assembly))
            {
                CreateAndAddSingleResourceLoader(assembly, str, uriRegistry, loaders);
            }
        }

        private static List<MetadataArtifactLoaderResource> LoadResources(string assemblyName, string resourceName, ICollection<string> uriRegistry, MetadataArtifactAssemblyResolver resolver)
        {
            List<MetadataArtifactLoaderResource> loaders = new List<MetadataArtifactLoaderResource>();
            if (assemblyName == MetadataArtifactLoader.wildcard)
            {
                foreach (Assembly assembly in resolver.GetWildcardAssemblies())
                {
                    if (AssemblyContainsResource(assembly, ref resourceName))
                    {
                        LoadResourcesFromAssembly(assembly, resourceName, uriRegistry, loaders, resolver);
                    }
                }
            }
            else
            {
                LoadResourcesFromAssembly(ResolveAssemblyName(assemblyName, resolver), resourceName, uriRegistry, loaders, resolver);
            }
            if ((resourceName != null) && (loaders.Count == 0))
            {
                throw EntityUtil.Metadata(Strings.UnableToLoadResource);
            }
            return loaders;
        }

        private static void LoadResourcesFromAssembly(Assembly assembly, string resourceName, ICollection<string> uriRegistry, List<MetadataArtifactLoaderResource> loaders, MetadataArtifactAssemblyResolver resolver)
        {
            if (resourceName == null)
            {
                LoadAllResourcesFromAssembly(assembly, uriRegistry, loaders, resolver);
            }
            else
            {
                if (!AssemblyContainsResource(assembly, ref resourceName))
                {
                    throw EntityUtil.Metadata(Strings.UnableToLoadResource);
                }
                CreateAndAddSingleResourceLoader(assembly, resourceName, uriRegistry, loaders);
            }
        }

        private static void ParseResourcePath(string path, out string assemblyName, out string resourceName)
        {
            int length = MetadataArtifactLoader.resPathPrefix.Length;
            string[] strArray = path.Substring(length).Split(new string[] { MetadataArtifactLoader.resPathSeparator, MetadataArtifactLoader.altPathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if ((strArray.Length == 0) || (strArray.Length > 2))
            {
                throw EntityUtil.Metadata(Strings.InvalidMetadataPath);
            }
            if (strArray.Length >= 1)
            {
                assemblyName = strArray[0];
            }
            else
            {
                assemblyName = null;
            }
            if (strArray.Length == 2)
            {
                resourceName = strArray[1];
            }
            else
            {
                resourceName = null;
            }
        }

        private static Assembly ResolveAssemblyName(string assemblyName, MetadataArtifactAssemblyResolver resolver)
        {
            Assembly assembly;
            AssemblyName refernceName = new AssemblyName(assemblyName);
            if (!resolver.TryResolveAssemblyReference(refernceName, out assembly))
            {
                throw new FileNotFoundException(Strings.UnableToResolveAssembly(assemblyName));
            }
            return assembly;
        }

        private static void ValidateExtension(MetadataArtifactLoader.ExtensionCheck extensionCheck, string validExtension, string resourceName)
        {
            if (resourceName != null)
            {
                switch (extensionCheck)
                {
                    case MetadataArtifactLoader.ExtensionCheck.Specific:
                        MetadataArtifactLoader.CheckArtifactExtension(resourceName, validExtension);
                        return;

                    case MetadataArtifactLoader.ExtensionCheck.All:
                        if (!MetadataArtifactLoader.IsValidArtifact(resourceName))
                        {
                            throw EntityUtil.Metadata(Strings.InvalidMetadataPath);
                        }
                        return;
                }
            }
        }

        public override bool IsComposite =>
            true;

        public override string Path =>
            this._originalPath;
    }
}

