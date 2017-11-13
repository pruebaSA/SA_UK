namespace System.Runtime.Serialization.Formatters.Binary
{
    using System;
    using System.Reflection;

    internal sealed class SerObjectInfoCache
    {
        internal string assemblyString;
        internal string fullTypeName;
        internal MemberInfo[] memberInfos;
        internal string[] memberNames;
        internal Type[] memberTypes;
    }
}

