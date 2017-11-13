namespace System.Runtime.InteropServices
{
    using System;

    [Serializable, Flags, ComVisible(true)]
    public enum TypeLibImporterFlags
    {
        ImportAsAgnostic = 0x800,
        ImportAsItanium = 0x400,
        ImportAsX64 = 0x200,
        ImportAsX86 = 0x100,
        None = 0,
        PreventClassMembers = 0x10,
        PrimaryInteropAssembly = 1,
        ReflectionOnlyLoading = 0x1000,
        SafeArrayAsSystemArray = 4,
        SerializableValueClasses = 0x20,
        TransformDispRetVals = 8,
        UnsafeInterfaces = 2
    }
}

