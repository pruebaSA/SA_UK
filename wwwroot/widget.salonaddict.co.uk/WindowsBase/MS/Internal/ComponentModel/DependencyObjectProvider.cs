namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    internal sealed class DependencyObjectProvider : TypeDescriptionProvider
    {
        private static Hashtable _attachInfoMap = new Hashtable();
        private static readonly UncommonField<IDictionary> _cacheSlot = new UncommonField<IDictionary>(null);
        private static Dictionary<PropertyKey, DependencyPropertyKind> _propertyKindMap = new Dictionary<PropertyKey, DependencyPropertyKind>();
        private static Dictionary<PropertyKey, DependencyObjectPropertyDescriptor> _propertyMap = new Dictionary<PropertyKey, DependencyObjectPropertyDescriptor>();

        public DependencyObjectProvider() : base(TypeDescriptor.GetProvider(typeof(DependencyObject)))
        {
            TypeDescriptor.Refreshed += delegate (RefreshEventArgs args) {
                if ((args.TypeChanged != null) && typeof(DependencyObject).IsAssignableFrom(args.TypeChanged))
                {
                    ClearCache();
                    DependencyObjectPropertyDescriptor.ClearCache();
                    DPCustomTypeDescriptor.ClearCache();
                    DependencyPropertyDescriptor.ClearCache();
                }
            };
        }

        private static void ClearCache()
        {
            lock (_propertyMap)
            {
                _propertyMap.Clear();
            }
            lock (_propertyKindMap)
            {
                _propertyKindMap.Clear();
            }
            lock (_attachInfoMap)
            {
                _attachInfoMap.Clear();
            }
        }

        internal static DependencyObjectPropertyDescriptor GetAttachedPropertyDescriptor(DependencyProperty dp, Type targetType)
        {
            DependencyObjectPropertyDescriptor descriptor;
            PropertyKey key = new PropertyKey(targetType, dp);
            lock (_propertyMap)
            {
                if (!_propertyMap.TryGetValue(key, out descriptor))
                {
                    descriptor = new DependencyObjectPropertyDescriptor(dp, targetType);
                    _propertyMap[key] = descriptor;
                }
            }
            return descriptor;
        }

        internal static AttachInfo GetAttachInfo(DependencyProperty dp)
        {
            AttachInfo info = (AttachInfo) _attachInfoMap[dp];
            if (info == null)
            {
                info = new AttachInfo(dp);
                lock (_attachInfoMap)
                {
                    _attachInfoMap[dp] = info;
                }
            }
            return info;
        }

        public override IDictionary GetCache(object instance)
        {
            DependencyObject obj2 = instance as DependencyObject;
            if (obj2 == null)
            {
                return base.GetCache(instance);
            }
            IDictionary dictionary = _cacheSlot.GetValue(obj2);
            if ((dictionary == null) && !obj2.IsSealed)
            {
                dictionary = new Hashtable();
                _cacheSlot.SetValue(obj2, dictionary);
            }
            return dictionary;
        }

        internal static DependencyPropertyKind GetDependencyPropertyKind(DependencyProperty dp, Type targetType)
        {
            DependencyPropertyKind kind;
            PropertyKey key = new PropertyKey(targetType, dp);
            lock (_propertyKindMap)
            {
                if (!_propertyKindMap.TryGetValue(key, out kind))
                {
                    kind = new DependencyPropertyKind(dp, targetType);
                    _propertyKindMap[key] = kind;
                }
            }
            return kind;
        }

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
        {
            ICustomTypeDescriptor extendedTypeDescriptor = base.GetExtendedTypeDescriptor(instance);
            if ((instance != null) && !(instance is Type))
            {
                extendedTypeDescriptor = new APCustomTypeDescriptor(extendedTypeDescriptor, instance);
            }
            return extendedTypeDescriptor;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) => 
            new DPCustomTypeDescriptor(base.GetTypeDescriptor(objectType, instance), objectType, instance);
    }
}

