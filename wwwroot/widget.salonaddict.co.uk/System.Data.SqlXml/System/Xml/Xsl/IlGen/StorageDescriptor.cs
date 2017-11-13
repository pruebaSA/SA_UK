namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct StorageDescriptor
    {
        private ItemLocation location;
        private object locationObject;
        private Type itemStorageType;
        private bool isCached;
        public static StorageDescriptor None() => 
            new StorageDescriptor();

        public static StorageDescriptor Stack(Type itemStorageType, bool isCached) => 
            new StorageDescriptor { 
                location = ItemLocation.Stack,
                itemStorageType = itemStorageType,
                isCached = isCached
            };

        public static StorageDescriptor Parameter(int paramIndex, Type itemStorageType, bool isCached) => 
            new StorageDescriptor { 
                location = ItemLocation.Parameter,
                locationObject = paramIndex,
                itemStorageType = itemStorageType,
                isCached = isCached
            };

        public static StorageDescriptor Local(LocalBuilder loc, Type itemStorageType, bool isCached) => 
            new StorageDescriptor { 
                location = ItemLocation.Local,
                locationObject = loc,
                itemStorageType = itemStorageType,
                isCached = isCached
            };

        public static StorageDescriptor Current(LocalBuilder locIter, Type itemStorageType) => 
            new StorageDescriptor { 
                location = ItemLocation.Current,
                locationObject = locIter,
                itemStorageType = itemStorageType
            };

        public static StorageDescriptor Global(MethodInfo methGlobal, Type itemStorageType, bool isCached) => 
            new StorageDescriptor { 
                location = ItemLocation.Global,
                locationObject = methGlobal,
                itemStorageType = itemStorageType,
                isCached = isCached
            };

        public StorageDescriptor ToStack() => 
            Stack(this.itemStorageType, this.isCached);

        public StorageDescriptor ToLocal(LocalBuilder loc) => 
            Local(loc, this.itemStorageType, this.isCached);

        public StorageDescriptor ToStorageType(Type itemStorageType)
        {
            StorageDescriptor descriptor = this;
            descriptor.itemStorageType = itemStorageType;
            return descriptor;
        }

        public ItemLocation Location =>
            this.location;
        public int ParameterLocation =>
            ((int) this.locationObject);
        public LocalBuilder LocalLocation =>
            (this.locationObject as LocalBuilder);
        public LocalBuilder CurrentLocation =>
            (this.locationObject as LocalBuilder);
        public MethodInfo GlobalLocation =>
            (this.locationObject as MethodInfo);
        public bool IsCached =>
            this.isCached;
        public Type ItemStorageType =>
            this.itemStorageType;
    }
}

