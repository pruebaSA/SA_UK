namespace Microsoft.Practices.Unity
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true)]
    public sealed class InjectionMethodAttribute : Attribute
    {
    }
}

