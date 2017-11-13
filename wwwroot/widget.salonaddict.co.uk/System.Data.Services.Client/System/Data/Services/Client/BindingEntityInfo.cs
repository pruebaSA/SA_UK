namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Services.Common;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class BindingEntityInfo
    {
        private static readonly Dictionary<Type, BindingEntityInfoPerType> bindingEntityInfos = new Dictionary<Type, BindingEntityInfoPerType>(EqualityComparer<Type>.Default);
        private static readonly object FalseObject = new object();
        private static readonly HashSet<Type> knownNonEntityTypes = new HashSet<Type>(EqualityComparer<Type>.Default);
        private static readonly Dictionary<Type, object> knownObservableCollectionTypes = new Dictionary<Type, object>(EqualityComparer<Type>.Default);
        private static readonly ReaderWriterLockSlim metadataCacheLock = new ReaderWriterLockSlim();
        private static readonly object TrueObject = new object();

        private static bool CanBeComplexProperty(ClientType.ClientProperty property) => 
            typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType);

        private static BindingEntityInfoPerType GetBindingEntityInfoFor(Type entityType)
        {
            BindingEntityInfoPerType type;
            metadataCacheLock.EnterReadLock();
            try
            {
                if (bindingEntityInfos.TryGetValue(entityType, out type))
                {
                    return type;
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }
            type = new BindingEntityInfoPerType();
            object[] customAttributes = entityType.GetCustomAttributes(typeof(EntitySetAttribute), true);
            type.EntitySet = ((customAttributes != null) && (customAttributes.Length == 1)) ? ((EntitySetAttribute) customAttributes[0]).EntitySet : null;
            type.ClientType = ClientType.Create(entityType);
            foreach (ClientType.ClientProperty property in type.ClientType.Properties)
            {
                BindingPropertyInfo item = null;
                Type propertyType = property.PropertyType;
                if (property.CollectionType != null)
                {
                    if (IsDataServiceCollection(propertyType))
                    {
                        item = new BindingPropertyInfo {
                            PropertyKind = BindingPropertyKind.BindingPropertyKindCollection
                        };
                    }
                }
                else if (IsEntityType(propertyType))
                {
                    item = new BindingPropertyInfo {
                        PropertyKind = BindingPropertyKind.BindingPropertyKindEntity
                    };
                }
                else if (CanBeComplexProperty(property))
                {
                    item = new BindingPropertyInfo {
                        PropertyKind = BindingPropertyKind.BindingPropertyKindComplex
                    };
                }
                if (item != null)
                {
                    item.PropertyInfo = property;
                    if (type.ClientType.IsEntityType || (item.PropertyKind == BindingPropertyKind.BindingPropertyKindComplex))
                    {
                        type.ObservableProperties.Add(item);
                    }
                }
            }
            metadataCacheLock.EnterWriteLock();
            try
            {
                if (!bindingEntityInfos.ContainsKey(entityType))
                {
                    bindingEntityInfos[entityType] = type;
                }
            }
            finally
            {
                metadataCacheLock.ExitWriteLock();
            }
            return type;
        }

        internal static ClientType GetClientType(Type entityType) => 
            GetBindingEntityInfoFor(entityType).ClientType;

        internal static string GetEntitySet(object target, string targetEntitySet)
        {
            if (!string.IsNullOrEmpty(targetEntitySet))
            {
                return targetEntitySet;
            }
            return GetEntitySetAttribute(target.GetType());
        }

        private static string GetEntitySetAttribute(Type entityType) => 
            GetBindingEntityInfoFor(entityType).EntitySet;

        internal static IList<BindingPropertyInfo> GetObservableProperties(Type entityType) => 
            GetBindingEntityInfoFor(entityType).ObservableProperties;

        internal static object GetPropertyValue(object source, string sourceProperty, out BindingPropertyInfo bindingPropertyInfo)
        {
            Type entityType = source.GetType();
            bindingPropertyInfo = GetObservableProperties(entityType).SingleOrDefault<BindingPropertyInfo>(x => x.PropertyInfo.PropertyName == sourceProperty);
            return bindingPropertyInfo?.PropertyInfo.GetValue(source);
        }

        internal static bool IsDataServiceCollection(Type collectionType)
        {
            metadataCacheLock.EnterReadLock();
            try
            {
                object obj2;
                if (knownObservableCollectionTypes.TryGetValue(collectionType, out obj2))
                {
                    return (obj2 == TrueObject);
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }
            Type c = collectionType;
            bool flag = false;
            while (c != null)
            {
                if (c.IsGenericType)
                {
                    Type[] genericArguments = c.GetGenericArguments();
                    if (((genericArguments != null) && (genericArguments.Length == 1)) && IsEntityType(genericArguments[0]))
                    {
                        Type dataServiceCollectionOfT = WebUtil.GetDataServiceCollectionOfT(genericArguments);
                        if ((dataServiceCollectionOfT != null) && dataServiceCollectionOfT.IsAssignableFrom(c))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                c = c.BaseType;
            }
            metadataCacheLock.EnterWriteLock();
            try
            {
                if (!knownObservableCollectionTypes.ContainsKey(collectionType))
                {
                    knownObservableCollectionTypes[collectionType] = flag ? TrueObject : FalseObject;
                }
            }
            finally
            {
                metadataCacheLock.ExitWriteLock();
            }
            return flag;
        }

        internal static bool IsEntityType(Type type)
        {
            metadataCacheLock.EnterReadLock();
            try
            {
                if (knownNonEntityTypes.Contains(type))
                {
                    return false;
                }
            }
            finally
            {
                metadataCacheLock.ExitReadLock();
            }
            try
            {
                if (IsDataServiceCollection(type))
                {
                    return false;
                }
                return ClientType.Create(type).IsEntityType;
            }
            catch (InvalidOperationException)
            {
                metadataCacheLock.EnterWriteLock();
                try
                {
                    if (!knownNonEntityTypes.Contains(type))
                    {
                        knownNonEntityTypes.Add(type);
                    }
                }
                finally
                {
                    metadataCacheLock.ExitWriteLock();
                }
                return false;
            }
        }

        private sealed class BindingEntityInfoPerType
        {
            private List<BindingEntityInfo.BindingPropertyInfo> observableProperties = new List<BindingEntityInfo.BindingPropertyInfo>();

            public System.Data.Services.Client.ClientType ClientType { get; set; }

            public string EntitySet { get; set; }

            public List<BindingEntityInfo.BindingPropertyInfo> ObservableProperties =>
                this.observableProperties;
        }

        internal class BindingPropertyInfo
        {
            public ClientType.ClientProperty PropertyInfo { get; set; }

            public BindingPropertyKind PropertyKind { get; set; }
        }
    }
}

