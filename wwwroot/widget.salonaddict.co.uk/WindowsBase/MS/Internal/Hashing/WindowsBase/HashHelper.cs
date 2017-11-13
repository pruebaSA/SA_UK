namespace MS.Internal.Hashing.WindowsBase
{
    using MS.Internal;
    using System;

    internal static class HashHelper
    {
        static HashHelper()
        {
            Initialize();
            Type[] types = new Type[0];
            BaseHashHelper.RegisterTypes(typeof(HashHelper).Assembly, types);
        }

        internal static bool HasReliableHashCode(object item) => 
            BaseHashHelper.HasReliableHashCode(item);

        internal static void Initialize()
        {
        }
    }
}

