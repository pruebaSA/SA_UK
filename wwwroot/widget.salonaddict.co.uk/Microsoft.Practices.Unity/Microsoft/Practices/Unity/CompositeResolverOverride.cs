namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeResolverOverride : ResolverOverride, IEnumerable<ResolverOverride>, IEnumerable
    {
        private readonly List<ResolverOverride> overrides = new List<ResolverOverride>();

        public void Add(ResolverOverride newOverride)
        {
            this.overrides.Add(newOverride);
        }

        public void AddRange(IEnumerable<ResolverOverride> newOverrides)
        {
            this.overrides.AddRange(newOverrides);
        }

        public IEnumerator<ResolverOverride> GetEnumerator() => 
            this.overrides.GetEnumerator();

        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            for (int i = this.overrides.Count<ResolverOverride>() - 1; i >= 0; i--)
            {
                IDependencyResolverPolicy resolver = this.overrides[i].GetResolver(context, dependencyType);
                if (resolver != null)
                {
                    return resolver;
                }
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();
    }
}

