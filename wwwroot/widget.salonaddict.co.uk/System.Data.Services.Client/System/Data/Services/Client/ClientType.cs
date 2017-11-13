namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [DebuggerDisplay("{ElementTypeName}")]
    internal sealed class ClientType
    {
        internal readonly Type ElementType;
        internal readonly string ElementTypeName;
        private System.Data.Services.Common.EpmSourceTree epmSourceTree;
        private System.Data.Services.Common.EpmTargetTree epmTargetTree;
        internal readonly bool IsEntityType;
        internal readonly int KeyCount;
        private ClientProperty mediaDataMember;
        private bool mediaLinkEntry;
        private static readonly Dictionary<TypeName, Type> namedTypes = new Dictionary<TypeName, Type>(new TypeNameEqualityComparer());
        private ArraySet<ClientProperty> properties;
        private static readonly Dictionary<Type, ClientType> types = new Dictionary<Type, ClientType>(EqualityComparer<Type>.Default);

        private ClientType(Type type, string typeName, bool skipSettableCheck)
        {
            Func<string, bool> predicate = null;
            this.ElementTypeName = typeName;
            this.ElementType = Nullable.GetUnderlyingType(type) ?? type;
            if (!ClientConvert.IsKnownType(this.ElementType))
            {
                Type c = null;
                bool flag = type.GetCustomAttributes(true).OfType<DataServiceEntityAttribute>().Any<DataServiceEntityAttribute>();
                DataServiceKeyAttribute attribute = type.GetCustomAttributes(true).OfType<DataServiceKeyAttribute>().FirstOrDefault<DataServiceKeyAttribute>();
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Type propertyType = info.PropertyType;
                    propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    if (((!propertyType.IsPointer && ((!propertyType.IsArray || (typeof(byte[]) == propertyType)) || (typeof(char[]) == propertyType))) && (((typeof(IntPtr) != propertyType) && (typeof(UIntPtr) != propertyType)) && info.CanRead)) && ((!propertyType.IsValueType || info.CanWrite) && (!propertyType.ContainsGenericParameters && (info.GetIndexParameters().Length == 0))))
                    {
                        bool keyProperty = (attribute != null) ? attribute.KeyNames.Contains(info.Name) : false;
                        if (keyProperty)
                        {
                            if (c == null)
                            {
                                c = info.DeclaringType;
                            }
                            else if (c != info.DeclaringType)
                            {
                                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_KeysOnDifferentDeclaredType(this.ElementTypeName));
                            }
                            if (!ClientConvert.IsKnownType(propertyType))
                            {
                                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_KeysMustBeSimpleTypes(this.ElementTypeName));
                            }
                            this.KeyCount++;
                        }
                        ClientProperty item = new ClientProperty(info, propertyType, keyProperty);
                        if (!this.properties.Add(item, new Func<ClientProperty, ClientProperty, bool>(ClientProperty.NameEquality)))
                        {
                            int index = this.IndexOfProperty(item.PropertyName);
                            if (!item.DeclaringType.IsAssignableFrom(this.properties[index].DeclaringType))
                            {
                                this.properties.RemoveAt(index);
                                this.properties.Add(item, null);
                            }
                        }
                    }
                }
                if (c == null)
                {
                    ClientProperty property2 = null;
                    for (int i = this.properties.Count - 1; 0 <= i; i--)
                    {
                        string propertyName = this.properties[i].PropertyName;
                        if (propertyName.EndsWith("ID", StringComparison.Ordinal))
                        {
                            string name = this.properties[i].DeclaringType.Name;
                            if ((propertyName.Length == (name.Length + 2)) && propertyName.StartsWith(name, StringComparison.Ordinal))
                            {
                                if ((c == null) || this.properties[i].DeclaringType.IsAssignableFrom(c))
                                {
                                    c = this.properties[i].DeclaringType;
                                    property2 = this.properties[i];
                                }
                            }
                            else if ((c == null) && (2 == propertyName.Length))
                            {
                                c = this.properties[i].DeclaringType;
                                property2 = this.properties[i];
                            }
                        }
                    }
                    if (property2 != null)
                    {
                        property2.KeyProperty = true;
                        this.KeyCount++;
                    }
                }
                else if (this.KeyCount != attribute.KeyNames.Count)
                {
                    if (predicate == null)
                    {
                        predicate = a => null == (from b in this.properties
                            where b.PropertyName == a
                            select b).FirstOrDefault<ClientProperty>();
                    }
                    string str3 = attribute.KeyNames.Cast<string>().Where<string>(predicate).First<string>();
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingProperty(this.ElementTypeName, str3));
                }
                this.IsEntityType = (c != null) || flag;
                this.WireUpMimeTypeProperties();
                this.CheckMediaLinkEntry();
                if (!skipSettableCheck && (this.properties.Count == 0))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_NoSettableFields(this.ElementTypeName));
                }
            }
            this.properties.TrimToSize();
            this.properties.Sort<string>(new Func<ClientProperty, string>(ClientProperty.GetPropertyName), new Func<string, string, int>(string.CompareOrdinal));
            this.BuildEpmInfo(type);
        }

        private void BuildEpmInfo(Type type)
        {
            if ((type.BaseType != null) && (type.BaseType != typeof(object)))
            {
                this.BuildEpmInfo(type.BaseType);
            }
            foreach (EntityPropertyMappingAttribute attribute in type.GetCustomAttributes(typeof(EntityPropertyMappingAttribute), false))
            {
                this.BuildEpmInfo(attribute, type);
            }
        }

        private void BuildEpmInfo(EntityPropertyMappingAttribute epmAttr, Type definingType)
        {
            EntityPropertyMappingInfo epmInfo = new EntityPropertyMappingInfo {
                Attribute = epmAttr,
                DefiningType = definingType,
                ActualType = this
            };
            this.EpmSourceTree.Add(epmInfo);
        }

        internal static bool CanAssignNull(Type type) => 
            (!type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))));

        internal static bool CheckElementTypeIsEntity(Type t)
        {
            t = TypeSystem.GetElementType(t);
            t = Nullable.GetUnderlyingType(t) ?? t;
            return Create(t, false).IsEntityType;
        }

        private void CheckMediaLinkEntry()
        {
            object[] customAttributes = this.ElementType.GetCustomAttributes(typeof(MediaEntryAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                MediaEntryAttribute attribute = (MediaEntryAttribute) customAttributes[0];
                this.mediaLinkEntry = true;
                int num = this.IndexOfProperty(attribute.MediaMemberName);
                if (num < 0)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingMediaEntryProperty(attribute.MediaMemberName));
                }
                this.mediaDataMember = this.properties[num];
            }
            customAttributes = this.ElementType.GetCustomAttributes(typeof(HasStreamAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                this.mediaLinkEntry = true;
            }
        }

        internal static ClientType Create(Type type) => 
            Create(type, true);

        internal static ClientType Create(Type type, bool expectModelType)
        {
            ClientType type2;
            lock (types)
            {
                types.TryGetValue(type, out type2);
            }
            if (type2 == null)
            {
                bool skipSettableCheck = !expectModelType;
                type2 = new ClientType(type, type.ToString(), skipSettableCheck);
                if (!expectModelType)
                {
                    return type2;
                }
                lock (types)
                {
                    ClientType type3;
                    if (types.TryGetValue(type, out type3))
                    {
                        return type3;
                    }
                    types.Add(type, type2);
                }
            }
            return type2;
        }

        internal object CreateInstance() => 
            Activator.CreateInstance(this.ElementType);

        internal static MethodInfo GetAddToCollectionMethod(Type collectionType, out Type type) => 
            GetCollectionMethod(collectionType, typeof(ICollection<>), "Add", out type);

        internal static MethodInfo GetCollectionMethod(Type propertyType, Type genericTypeDefinition, string methodName, out Type type)
        {
            type = null;
            Type implementationType = GetImplementationType(propertyType, genericTypeDefinition);
            if (implementationType != null)
            {
                Type[] genericArguments = implementationType.GetGenericArguments();
                MethodInfo method = implementationType.GetMethod(methodName);
                type = genericArguments[genericArguments.Length - 1];
                return method;
            }
            return null;
        }

        internal static Type GetImplementationType(Type propertyType, Type genericTypeDefinition)
        {
            if (IsConstructedGeneric(propertyType, genericTypeDefinition))
            {
                return propertyType;
            }
            Type type = null;
            foreach (Type type2 in propertyType.GetInterfaces())
            {
                if (IsConstructedGeneric(type2, genericTypeDefinition))
                {
                    if (type != null)
                    {
                        throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.ClientType_MultipleImplementationNotSupported);
                    }
                    type = type2;
                }
            }
            return type;
        }

        internal ClientProperty GetProperty(string propertyName, bool ignoreMissingProperties)
        {
            int num = this.IndexOfProperty(propertyName);
            if (0 <= num)
            {
                return this.properties[num];
            }
            if (!ignoreMissingProperties)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingProperty(this.ElementTypeName, propertyName));
            }
            return null;
        }

        internal static MethodInfo GetRemoveFromCollectionMethod(Type collectionType, out Type type) => 
            GetCollectionMethod(collectionType, typeof(ICollection<>), "Remove", out type);

        private int IndexOfProperty(string propertyName) => 
            this.properties.IndexOf<string>(propertyName, new Func<ClientProperty, string>(ClientProperty.GetPropertyName), new Func<string, string, bool>(string.Equals));

        private static bool IsConstructedGeneric(Type type, Type genericTypeDefinition) => 
            ((type.IsGenericType && (type.GetGenericTypeDefinition() == genericTypeDefinition)) && !type.ContainsGenericParameters);

        internal static object ReadPropertyValue(object element, ClientType resourceType, string srcPath)
        {
            ClientProperty resourceProperty = null;
            return ReadPropertyValue(element, resourceType, srcPath.Split(new char[] { '/' }), 0, ref resourceProperty);
        }

        private static object ReadPropertyValue(object element, ClientType resourceType, string[] srcPathSegments, int currentSegment, ref ClientProperty resourceProperty)
        {
            if ((element == null) || (currentSegment == srcPathSegments.Length))
            {
                return element;
            }
            string propertyName = srcPathSegments[currentSegment];
            resourceProperty = resourceType.GetProperty(propertyName, true);
            if (resourceProperty == null)
            {
                throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.EpmSourceTree_InaccessiblePropertyOnType(propertyName, resourceType.ElementTypeName));
            }
            if (resourceProperty.IsKnownType ^ (currentSegment == (srcPathSegments.Length - 1)))
            {
                throw System.Data.Services.Client.Error.InvalidOperation(!resourceProperty.IsKnownType ? System.Data.Services.Client.Strings.EpmClientType_PropertyIsComplex(resourceProperty.PropertyName) : System.Data.Services.Client.Strings.EpmClientType_PropertyIsPrimitive(resourceProperty.PropertyName));
            }
            return ReadPropertyValue(element.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(element, null), resourceProperty.IsKnownType ? null : Create(resourceProperty.PropertyType), srcPathSegments, ++currentSegment, ref resourceProperty);
        }

        internal static Type ResolveFromName(string wireName, Type userType)
        {
            Type type;
            TypeName name;
            bool flag;
            name.Type = userType;
            name.Name = wireName;
            lock (namedTypes)
            {
                flag = namedTypes.TryGetValue(name, out type);
            }
            if (!flag)
            {
                string wireClassName = wireName;
                int num = wireName.LastIndexOf('.');
                if ((0 <= num) && (num < (wireName.Length - 1)))
                {
                    wireClassName = wireName.Substring(num + 1);
                }
                if (userType.Name == wireClassName)
                {
                    type = userType;
                }
                else
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Type type2 = assembly.GetType(wireName, false);
                        ResolveSubclass(wireClassName, userType, type2, ref type);
                        if (type2 == null)
                        {
                            Type[] types = null;
                            try
                            {
                                types = assembly.GetTypes();
                            }
                            catch (ReflectionTypeLoadException)
                            {
                            }
                            if (types != null)
                            {
                                foreach (Type type3 in types)
                                {
                                    ResolveSubclass(wireClassName, userType, type3, ref type);
                                }
                            }
                        }
                    }
                }
                lock (namedTypes)
                {
                    namedTypes[name] = type;
                }
            }
            return type;
        }

        private static void ResolveSubclass(string wireClassName, Type userType, Type type, ref Type existing)
        {
            if (((type != null) && type.IsVisible) && ((wireClassName == type.Name) && userType.IsAssignableFrom(type)))
            {
                if (existing != null)
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_Ambiguous(wireClassName, userType));
                }
                existing = type;
            }
        }

        private void WireUpMimeTypeProperties()
        {
            MimeTypePropertyAttribute attribute = (MimeTypePropertyAttribute) this.ElementType.GetCustomAttributes(typeof(MimeTypePropertyAttribute), true).SingleOrDefault<object>();
            if (attribute != null)
            {
                int num;
                int num2;
                if ((0 > (num = this.IndexOfProperty(attribute.DataPropertyName))) || (0 > (num2 = this.IndexOfProperty(attribute.MimeTypePropertyName))))
                {
                    throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingMimeTypeProperty(attribute.DataPropertyName, attribute.MimeTypePropertyName));
                }
                this.Properties[num].MimeTypeProperty = this.Properties[num2];
            }
        }

        internal bool EpmIsV1Compatible
        {
            get
            {
                if (this.HasEntityPropertyMappings)
                {
                    return this.EpmTargetTree.IsV1Compatible;
                }
                return true;
            }
        }

        internal System.Data.Services.Common.EpmSourceTree EpmSourceTree
        {
            get
            {
                if (this.epmSourceTree == null)
                {
                    this.epmTargetTree = new System.Data.Services.Common.EpmTargetTree();
                    this.epmSourceTree = new System.Data.Services.Common.EpmSourceTree(this.epmTargetTree);
                }
                return this.epmSourceTree;
            }
        }

        internal System.Data.Services.Common.EpmTargetTree EpmTargetTree =>
            this.epmTargetTree;

        internal bool HasEntityPropertyMappings =>
            (this.epmSourceTree != null);

        internal bool IsMediaLinkEntry =>
            this.mediaLinkEntry;

        internal ClientProperty MediaDataMember =>
            this.mediaDataMember;

        internal ArraySet<ClientProperty> Properties =>
            this.properties;

        [DebuggerDisplay("{PropertyName}")]
        internal sealed class ClientProperty
        {
            private readonly MethodInfo addMethod;
            internal readonly Type CollectionType;
            private readonly MethodInfo containsMethod;
            internal readonly bool IsKnownType;
            private bool keyProperty;
            private ClientType.ClientProperty mimeTypeProperty;
            internal readonly Type NullablePropertyType;
            private readonly MethodInfo propertyGetter;
            internal readonly string PropertyName;
            private readonly MethodInfo propertySetter;
            internal readonly Type PropertyType;
            private readonly MethodInfo removeMethod;
            private readonly MethodInfo setMethod;

            internal ClientProperty(PropertyInfo property, Type propertyType, bool keyProperty)
            {
                this.PropertyName = property.Name;
                this.NullablePropertyType = property.PropertyType;
                this.PropertyType = propertyType;
                this.propertyGetter = property.GetGetMethod();
                this.propertySetter = property.GetSetMethod();
                this.keyProperty = keyProperty;
                this.IsKnownType = ClientConvert.IsKnownType(propertyType);
                if (!this.IsKnownType)
                {
                    this.setMethod = ClientType.GetCollectionMethod(this.PropertyType, typeof(IDictionary<,>), "set_Item", out this.CollectionType);
                    if (this.setMethod == null)
                    {
                        this.containsMethod = ClientType.GetCollectionMethod(this.PropertyType, typeof(ICollection<>), "Contains", out this.CollectionType);
                        this.addMethod = ClientType.GetAddToCollectionMethod(this.PropertyType, out this.CollectionType);
                        this.removeMethod = ClientType.GetRemoveFromCollectionMethod(this.PropertyType, out this.CollectionType);
                    }
                }
            }

            internal static bool GetKeyProperty(ClientType.ClientProperty x) => 
                x.KeyProperty;

            internal static string GetPropertyName(ClientType.ClientProperty x) => 
                x.PropertyName;

            internal object GetValue(object instance) => 
                this.propertyGetter.Invoke(instance, null);

            internal static bool NameEquality(ClientType.ClientProperty x, ClientType.ClientProperty y) => 
                string.Equals(x.PropertyName, y.PropertyName);

            internal void RemoveValue(object instance, object value)
            {
                this.removeMethod.Invoke(instance, new object[] { value });
            }

            internal void SetValue(object instance, object value, string propertyName, bool allowAdd)
            {
                if (this.setMethod != null)
                {
                    this.setMethod.Invoke(instance, new object[] { propertyName, value });
                }
                else if (allowAdd && (this.addMethod != null))
                {
                    if (!((bool) this.containsMethod.Invoke(instance, new object[] { value })))
                    {
                        this.addMethod.Invoke(instance, new object[] { value });
                    }
                }
                else
                {
                    if (this.propertySetter == null)
                    {
                        throw System.Data.Services.Client.Error.InvalidOperation(System.Data.Services.Client.Strings.ClientType_MissingProperty(value.GetType().ToString(), propertyName));
                    }
                    this.propertySetter.Invoke(instance, new object[] { value });
                }
            }

            internal Type DeclaringType =>
                this.propertyGetter.DeclaringType;

            internal bool KeyProperty
            {
                get => 
                    this.keyProperty;
                set
                {
                    this.keyProperty = value;
                }
            }

            internal ClientType.ClientProperty MimeTypeProperty
            {
                get => 
                    this.mimeTypeProperty;
                set
                {
                    this.mimeTypeProperty = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TypeName
        {
            internal System.Type Type;
            internal string Name;
        }

        private sealed class TypeNameEqualityComparer : IEqualityComparer<ClientType.TypeName>
        {
            public bool Equals(ClientType.TypeName x, ClientType.TypeName y) => 
                ((x.Type == y.Type) && (x.Name == y.Name));

            public int GetHashCode(ClientType.TypeName obj) => 
                (obj.Type.GetHashCode() ^ obj.Name.GetHashCode());
        }
    }
}

