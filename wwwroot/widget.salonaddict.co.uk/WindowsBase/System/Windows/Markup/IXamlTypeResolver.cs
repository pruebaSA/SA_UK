namespace System.Windows.Markup
{
    using System;

    public interface IXamlTypeResolver
    {
        Type Resolve(string qualifiedTypeName);
    }
}

