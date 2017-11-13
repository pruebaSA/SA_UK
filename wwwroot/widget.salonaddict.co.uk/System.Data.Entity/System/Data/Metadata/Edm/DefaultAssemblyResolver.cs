namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class DefaultAssemblyResolver : MetadataArtifactAssemblyResolver
    {
        private static IEnumerable<Assembly> GetAllDiscoverableAssemblies()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            HashSet<Assembly> set = new HashSet<Assembly>(new AssemblyComparer<Assembly>());
            foreach (Assembly assembly2 in GetAlreadyLoadedNonSystemAssemblies())
            {
                set.Add(assembly2);
            }
            AspProxy proxy = new AspProxy();
            if (!proxy.IsAspNetEnvironment())
            {
                if (entryAssembly != null)
                {
                    AssemblyName[] referencedAssemblies = entryAssembly.GetReferencedAssemblies();
                    if ((referencedAssemblies != null) && (referencedAssemblies.Length > 0))
                    {
                        foreach (AssemblyName name in referencedAssemblies)
                        {
                            if (!ObjectItemCollection.ShouldFilterAssembly(name.FullName))
                            {
                                set.Add(Assembly.ReflectionOnlyLoad(name.FullName));
                            }
                        }
                    }
                    if (!ObjectItemCollection.ShouldFilterAssembly(entryAssembly.FullName))
                    {
                        set.Add(entryAssembly);
                    }
                }
                return set;
            }
            if (proxy.HasBuildManagerType())
            {
                IEnumerable<Assembly> buildManagerReferencedAssemblies = proxy.GetBuildManagerReferencedAssemblies();
                if (buildManagerReferencedAssemblies != null)
                {
                    foreach (object obj2 in buildManagerReferencedAssemblies)
                    {
                        Assembly item = obj2 as Assembly;
                        if (!ObjectItemCollection.ShouldFilterAssembly(item.FullName))
                        {
                            set.Add(item);
                        }
                    }
                }
            }
            if ((entryAssembly != null) && !ObjectItemCollection.ShouldFilterAssembly(entryAssembly.FullName))
            {
                set.Add(entryAssembly);
            }
            return (from a in set
                where a != null
                select a);
        }

        private static IEnumerable<Assembly> GetAlreadyLoadedNonSystemAssemblies() => 
            (from a in AppDomain.CurrentDomain.GetAssemblies()
                where (a != null) && !ObjectItemCollection.ShouldFilterAssembly(a.FullName)
                select a);

        internal override IEnumerable<Assembly> GetWildcardAssemblies() => 
            GetAllDiscoverableAssemblies();

        internal Assembly ResolveAssembly(AssemblyName referenceName)
        {
            Assembly assembly = null;
            foreach (Assembly assembly2 in GetAlreadyLoadedNonSystemAssemblies())
            {
                if (AssemblyName.ReferenceMatchesDefinition(referenceName, assembly2.GetName()))
                {
                    return assembly2;
                }
            }
            if (assembly == null)
            {
                try
                {
                    assembly = Assembly.ReflectionOnlyLoad(referenceName.FullName);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }
                catch (FileNotFoundException)
                {
                }
            }
            this.TryFindWildcardAssemblyMatch(referenceName, out assembly);
            return assembly;
        }

        private bool TryFindWildcardAssemblyMatch(AssemblyName referenceName, out Assembly assembly)
        {
            foreach (Assembly assembly2 in GetAllDiscoverableAssemblies())
            {
                if (AssemblyName.ReferenceMatchesDefinition(referenceName, assembly2.GetName()))
                {
                    assembly = assembly2;
                    return true;
                }
            }
            assembly = null;
            return false;
        }

        internal override bool TryResolveAssemblyReference(AssemblyName refernceName, out Assembly assembly)
        {
            assembly = this.ResolveAssembly(refernceName);
            return (assembly != null);
        }

        internal sealed class AssemblyComparer<T> : IEqualityComparer<T> where T: Assembly
        {
            public bool Equals(T x, T y) => 
                (object.ReferenceEquals(x, y) || (AssemblyName.ReferenceMatchesDefinition(x.GetName(), y.GetName()) && AssemblyName.ReferenceMatchesDefinition(y.GetName(), x.GetName())));

            public int GetHashCode(T assembly) => 
                assembly.FullName.GetHashCode();
        }
    }
}

