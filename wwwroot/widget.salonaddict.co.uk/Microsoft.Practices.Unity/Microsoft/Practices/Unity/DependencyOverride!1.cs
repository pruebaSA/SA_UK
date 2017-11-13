namespace Microsoft.Practices.Unity
{
    using System;

    public class DependencyOverride<T> : DependencyOverride
    {
        public DependencyOverride(object dependencyValue) : base(typeof(T), dependencyValue)
        {
        }
    }
}

