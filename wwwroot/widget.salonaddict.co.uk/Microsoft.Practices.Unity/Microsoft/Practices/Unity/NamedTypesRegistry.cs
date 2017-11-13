namespace Microsoft.Practices.Unity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class NamedTypesRegistry
    {
        private readonly NamedTypesRegistry parent;
        private readonly Dictionary<Type, List<string>> registeredKeys;

        public NamedTypesRegistry() : this(null)
        {
        }

        public NamedTypesRegistry(NamedTypesRegistry parent)
        {
            this.parent = parent;
            this.registeredKeys = new Dictionary<Type, List<string>>();
        }

        public void Clear()
        {
            this.registeredKeys.Clear();
        }

        public IEnumerable<string> GetKeys(Type t)
        {
            IEnumerable<string> first = Enumerable.Empty<string>();
            if (this.parent != null)
            {
                first = first.Concat<string>(this.parent.GetKeys(t));
            }
            if (this.registeredKeys.ContainsKey(t))
            {
                first = first.Concat<string>(this.registeredKeys[t]);
            }
            return first;
        }

        public void RegisterType(Type t, string name)
        {
            if (!this.registeredKeys.ContainsKey(t))
            {
                this.registeredKeys[t] = new List<string>();
            }
            this.RemoveMatchingKeys(t, name);
            this.registeredKeys[t].Add(name);
        }

        private void RemoveMatchingKeys(Type t, string name)
        {
            IEnumerable<string> source = from registeredName in this.registeredKeys[t]
                where registeredName != name
                select registeredName;
            this.registeredKeys[t] = source.ToList<string>();
        }

        public IEnumerable<Type> RegisteredTypes =>
            this.registeredKeys.Keys;
    }
}

