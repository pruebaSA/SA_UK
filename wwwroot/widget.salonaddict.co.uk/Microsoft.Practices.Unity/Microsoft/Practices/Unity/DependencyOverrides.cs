namespace Microsoft.Practices.Unity
{
    using System;

    public class DependencyOverrides : OverrideCollection<DependencyOverride, Type, object>
    {
        protected override DependencyOverride MakeOverride(Type key, object value) => 
            new DependencyOverride(key, value);
    }
}

