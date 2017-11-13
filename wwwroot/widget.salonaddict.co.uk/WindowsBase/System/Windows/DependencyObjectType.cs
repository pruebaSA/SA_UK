namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;

    public class DependencyObjectType
    {
        private DependencyObjectType _baseDType;
        private int _id;
        private static object _lock = new object();
        private Type _systemType;
        private static int DTypeCount = 0;
        private static Hashtable DTypeFromCLRType = new Hashtable();

        private DependencyObjectType()
        {
        }

        public static DependencyObjectType FromSystemType(Type systemType)
        {
            if (systemType == null)
            {
                throw new ArgumentNullException("systemType");
            }
            if (!typeof(DependencyObject).IsAssignableFrom(systemType))
            {
                throw new ArgumentException(System.Windows.SR.Get("DTypeNotSupportForSystemType", new object[] { systemType.Name }));
            }
            return FromSystemTypeInternal(systemType);
        }

        [FriendAccessAllowed]
        internal static DependencyObjectType FromSystemTypeInternal(Type systemType)
        {
            lock (_lock)
            {
                return FromSystemTypeRecursive(systemType);
            }
        }

        private static DependencyObjectType FromSystemTypeRecursive(Type systemType)
        {
            DependencyObjectType type = (DependencyObjectType) DTypeFromCLRType[systemType];
            if (type == null)
            {
                type = new DependencyObjectType {
                    _systemType = systemType
                };
                DTypeFromCLRType[systemType] = type;
                if (systemType != typeof(DependencyObject))
                {
                    type._baseDType = FromSystemTypeRecursive(systemType.BaseType);
                }
                type._id = DTypeCount++;
            }
            return type;
        }

        public override int GetHashCode() => 
            this._id;

        public bool IsInstanceOfType(DependencyObject dependencyObject)
        {
            if (dependencyObject != null)
            {
                DependencyObjectType dependencyObjectType = dependencyObject.DependencyObjectType;
                do
                {
                    if (dependencyObjectType.Id == this.Id)
                    {
                        return true;
                    }
                    dependencyObjectType = dependencyObjectType._baseDType;
                }
                while (dependencyObjectType != null);
            }
            return false;
        }

        public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
        {
            if (dependencyObjectType != null)
            {
                for (DependencyObjectType type = this._baseDType; type != null; type = type._baseDType)
                {
                    if (type.Id == dependencyObjectType.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public DependencyObjectType BaseType =>
            this._baseDType;

        public int Id =>
            this._id;

        public string Name =>
            this.SystemType.Name;

        public Type SystemType =>
            this._systemType;
    }
}

