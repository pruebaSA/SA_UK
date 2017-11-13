namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public static class ScriptObjectBuilder
    {
        private static readonly Dictionary<Type, List<ResourceEntry>> _cache = new Dictionary<Type, List<ResourceEntry>>();
        private static readonly Dictionary<Type, IList<string>> _cssCache = new Dictionary<Type, IList<string>>();
        private static Dictionary<Type, Converter<object, string>> _customConverters = new Dictionary<Type, Converter<object, string>>();
        private static readonly object _sync = new object();

        static ScriptObjectBuilder()
        {
            CustomConverters.Add(typeof(Color), value => ColorTranslator.ToHtml((Color) value));
            Converter<object, string> converter = delegate (object value) {
                DateTime? nullable = (DateTime?) value;
                if (!nullable.HasValue)
                {
                    return null;
                }
                return nullable.Value.ToUniversalTime().ToString("r");
            };
            CustomConverters.Add(typeof(DateTime), converter);
            CustomConverters.Add(typeof(DateTime?), converter);
        }

        public static void DescribeComponent(object instance, ScriptComponentDescriptor descriptor, IUrlResolutionService urlResolver, IControlResolver controlResolver)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }
            if (urlResolver == null)
            {
                urlResolver = instance as IUrlResolutionService;
            }
            if (controlResolver == null)
            {
                controlResolver = instance as IControlResolver;
            }
            foreach (PropertyDescriptor descriptor2 in TypeDescriptor.GetProperties(instance))
            {
                ExtenderControlPropertyAttribute attribute = null;
                ExtenderControlEventAttribute attribute2 = null;
                ClientPropertyNameAttribute attribute3 = null;
                IDReferencePropertyAttribute attribute4 = null;
                UrlPropertyAttribute attribute5 = null;
                ElementReferenceAttribute attribute6 = null;
                ComponentReferenceAttribute attribute7 = null;
                foreach (Attribute attribute8 in descriptor2.Attributes)
                {
                    Type type = attribute8.GetType();
                    if (type == typeof(ExtenderControlPropertyAttribute))
                    {
                        attribute = attribute8 as ExtenderControlPropertyAttribute;
                    }
                    else if (type == typeof(ExtenderControlEventAttribute))
                    {
                        attribute2 = attribute8 as ExtenderControlEventAttribute;
                    }
                    else if (type == typeof(ClientPropertyNameAttribute))
                    {
                        attribute3 = attribute8 as ClientPropertyNameAttribute;
                    }
                    else if (type == typeof(IDReferencePropertyAttribute))
                    {
                        attribute4 = attribute8 as IDReferencePropertyAttribute;
                    }
                    else if (type == typeof(UrlPropertyAttribute))
                    {
                        attribute5 = attribute8 as UrlPropertyAttribute;
                    }
                    else if (type == typeof(ElementReferenceAttribute))
                    {
                        attribute6 = attribute8 as ElementReferenceAttribute;
                    }
                    else if (type == typeof(ComponentReferenceAttribute))
                    {
                        attribute7 = attribute8 as ComponentReferenceAttribute;
                    }
                }
                string name = descriptor2.Name;
                if (((attribute != null) && attribute.IsScriptProperty) || ((attribute2 != null) && attribute2.IsScriptEvent))
                {
                    if ((attribute3 != null) && !string.IsNullOrEmpty(attribute3.PropertyName))
                    {
                        name = attribute3.PropertyName;
                    }
                    if (descriptor2.ShouldSerializeValue(instance) || descriptor2.IsReadOnly)
                    {
                        Control control = null;
                        object input = descriptor2.GetValue(instance);
                        if (input != null)
                        {
                            if ((attribute2 != null) && (descriptor2.PropertyType != typeof(string)))
                            {
                                throw new InvalidOperationException("ExtenderControlEventAttribute can only be applied to a property with a PropertyType of System.String.");
                            }
                            if (!descriptor2.PropertyType.IsPrimitive && !descriptor2.PropertyType.IsEnum)
                            {
                                Converter<object, string> converter = null;
                                if (!_customConverters.TryGetValue(descriptor2.PropertyType, out converter))
                                {
                                    foreach (KeyValuePair<Type, Converter<object, string>> pair in _customConverters)
                                    {
                                        if (descriptor2.PropertyType.IsSubclassOf(pair.Key))
                                        {
                                            converter = pair.Value;
                                            break;
                                        }
                                    }
                                }
                                if (converter != null)
                                {
                                    input = converter(input);
                                }
                                else if ((attribute == null) || !attribute.UseJsonSerialization)
                                {
                                    input = descriptor2.Converter.ConvertToString(null, CultureInfo.InvariantCulture, input);
                                }
                            }
                            if ((attribute4 != null) && (controlResolver != null))
                            {
                                control = controlResolver.ResolveControl((string) input);
                            }
                            if ((attribute5 != null) && (urlResolver != null))
                            {
                                input = urlResolver.ResolveClientUrl((string) input);
                            }
                            if (attribute2 != null)
                            {
                                descriptor.AddEvent(name, (string) input);
                            }
                            else if (attribute6 != null)
                            {
                                if ((control == null) && (controlResolver != null))
                                {
                                    control = controlResolver.ResolveControl((string) input);
                                }
                                if (control != null)
                                {
                                    input = control.ClientID;
                                }
                                descriptor.AddElementProperty(name, (string) input);
                            }
                            else if (attribute7 != null)
                            {
                                if ((control == null) && (controlResolver != null))
                                {
                                    control = controlResolver.ResolveControl((string) input);
                                }
                                if (control != null)
                                {
                                    ExtenderControlBase base2 = control as ExtenderControlBase;
                                    if ((base2 != null) && (base2.BehaviorID.Length > 0))
                                    {
                                        input = base2.BehaviorID;
                                    }
                                    else
                                    {
                                        input = control.ClientID;
                                    }
                                }
                                descriptor.AddComponentProperty(name, (string) input);
                            }
                            else
                            {
                                if (control != null)
                                {
                                    input = control.ClientID;
                                }
                                descriptor.AddProperty(name, input);
                            }
                        }
                    }
                }
            }
            foreach (MethodInfo info in instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                ExtenderControlMethodAttribute customAttribute = (ExtenderControlMethodAttribute) Attribute.GetCustomAttribute(info, typeof(ExtenderControlMethodAttribute));
                if ((customAttribute != null) && customAttribute.IsScriptMethod)
                {
                    Control control2 = instance as Control;
                    if (control2 != null)
                    {
                        control2.Page.ClientScript.GetCallbackEventReference(control2, null, null, null);
                        descriptor.AddProperty("_callbackTarget", control2.UniqueID);
                        return;
                    }
                    break;
                }
            }
        }

        public static string ExecuteCallbackMethod(Control control, string callbackArgument)
        {
            Type type = control.GetType();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> dictionary = serializer.DeserializeObject(callbackArgument) as Dictionary<string, object>;
            string name = (string) dictionary["name"];
            object[] objArray = (object[]) dictionary["args"];
            string clientState = (string) dictionary["state"];
            IClientStateManager manager = control as IClientStateManager;
            if ((manager != null) && manager.SupportsClientState)
            {
                manager.LoadClientState(clientState);
            }
            object obj2 = null;
            string str3 = null;
            try
            {
                MethodInfo method = type.GetMethod(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method == null)
                {
                    throw new MissingMethodException(type.FullName, name);
                }
                ParameterInfo[] parameters = method.GetParameters();
                ExtenderControlMethodAttribute customAttribute = (ExtenderControlMethodAttribute) Attribute.GetCustomAttribute(method, typeof(ExtenderControlMethodAttribute));
                if (((customAttribute == null) || !customAttribute.IsScriptMethod) || (objArray.Length != parameters.Length))
                {
                    throw new MissingMethodException(type.FullName, name);
                }
                object[] objArray2 = new object[objArray.Length];
                for (int i = 0; i < objArray2.Length; i++)
                {
                    if (objArray[i] != null)
                    {
                        objArray2[i] = Convert.ChangeType(objArray[i], parameters[i].ParameterType, CultureInfo.InvariantCulture);
                    }
                }
                obj2 = method.Invoke(control, objArray2);
            }
            catch (Exception innerException)
            {
                if (innerException is TargetInvocationException)
                {
                    innerException = innerException.InnerException;
                }
                str3 = innerException.GetType().FullName + ":" + innerException.Message;
            }
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            if (str3 == null)
            {
                dictionary2["result"] = obj2;
                if ((manager != null) && manager.SupportsClientState)
                {
                    dictionary2["state"] = manager.SaveClientState();
                }
            }
            else
            {
                dictionary2["error"] = str3;
            }
            return serializer.Serialize(dictionary2);
        }

        public static IEnumerable<string> GetCssReferences(Control control) => 
            GetCssReferences(control, control.GetType(), new Stack<Type>());

        private static IEnumerable<string> GetCssReferences(Control control, Type type, Stack<Type> typeReferenceStack)
        {
            IList<string> list;
            IEnumerable<string> enumerable;
            if (typeReferenceStack.Contains(type))
            {
                throw new InvalidOperationException("Circular reference detected.");
            }
            if (_cssCache.TryGetValue(type, out list))
            {
                return list;
            }
            typeReferenceStack.Push(type);
            try
            {
                lock (_sync)
                {
                    if (_cssCache.TryGetValue(type, out list))
                    {
                        return list;
                    }
                    List<string> list2 = new List<string>();
                    List<RequiredScriptAttribute> list3 = new List<RequiredScriptAttribute>();
                    foreach (RequiredScriptAttribute attribute in type.GetCustomAttributes(typeof(RequiredScriptAttribute), true))
                    {
                        list3.Add(attribute);
                    }
                    list3.Sort((left, right) => left.LoadOrder.CompareTo(right.LoadOrder));
                    foreach (RequiredScriptAttribute attribute2 in list3)
                    {
                        if (attribute2.ExtenderType != null)
                        {
                            list2.AddRange(GetCssReferences(control, attribute2.ExtenderType, typeReferenceStack));
                        }
                    }
                    List<ResourceEntry> list4 = new List<ResourceEntry>();
                    int num = 0;
                    for (Type type2 = type; (type2 != null) && (type2 != typeof(object)); type2 = type2.BaseType)
                    {
                        object[] objArray = Attribute.GetCustomAttributes(type2, typeof(ClientCssResourceAttribute), false);
                        num -= objArray.Length;
                        foreach (ClientCssResourceAttribute attribute3 in objArray)
                        {
                            list4.Add(new ResourceEntry(attribute3.ResourcePath, type2, num + attribute3.LoadOrder));
                        }
                    }
                    list4.Sort((l, r) => l.Order.CompareTo(r.Order));
                    foreach (ResourceEntry entry in list4)
                    {
                        list2.Add(control.Page.ClientScript.GetWebResourceUrl(entry.ComponentType, entry.ResourcePath));
                    }
                    Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    List<string> list5 = new List<string>();
                    foreach (string str in list2)
                    {
                        if (!dictionary.ContainsKey(str))
                        {
                            dictionary.Add(str, null);
                            list5.Add(str);
                        }
                    }
                    list = new ReadOnlyCollection<string>(list5);
                    _cssCache.Add(type, list);
                    enumerable = list;
                }
            }
            finally
            {
                typeReferenceStack.Pop();
            }
            return enumerable;
        }

        public static IEnumerable<ScriptReference> GetScriptReferences(Type type) => 
            GetScriptReferences(type, false);

        public static IEnumerable<ScriptReference> GetScriptReferences(Type type, bool ignoreStartingTypeReferences) => 
            ScriptReferencesFromResourceEntries(GetScriptReferencesInternal(type, new Stack<Type>(), ignoreStartingTypeReferences));

        private static List<ResourceEntry> GetScriptReferencesInternal(Type type, Stack<Type> typeReferenceStack, bool ignoreStartingTypeReferences)
        {
            List<ResourceEntry> list;
            List<ResourceEntry> list4;
            if (typeReferenceStack.Contains(type))
            {
                throw new InvalidOperationException("Circular reference detected.");
            }
            if (!ignoreStartingTypeReferences && _cache.TryGetValue(type, out list))
            {
                return list;
            }
            typeReferenceStack.Push(type);
            try
            {
                lock (_sync)
                {
                    if (ignoreStartingTypeReferences || !_cache.TryGetValue(type, out list))
                    {
                        list = new List<ResourceEntry>();
                        List<RequiredScriptAttribute> list2 = new List<RequiredScriptAttribute>();
                        foreach (RequiredScriptAttribute attribute in type.GetCustomAttributes(typeof(RequiredScriptAttribute), true))
                        {
                            list2.Add(attribute);
                        }
                        list2.Sort((left, right) => left.LoadOrder.CompareTo(right.LoadOrder));
                        foreach (RequiredScriptAttribute attribute2 in list2)
                        {
                            if (attribute2.ExtenderType != null)
                            {
                                list.AddRange(GetScriptReferencesInternal(attribute2.ExtenderType, typeReferenceStack, false));
                            }
                        }
                        int num = 0;
                        List<ResourceEntry> collection = new List<ResourceEntry>();
                        for (Type type2 = type; (type2 != null) && (type2 != typeof(object)); type2 = type2.BaseType)
                        {
                            if (!ignoreStartingTypeReferences || (type2 != type))
                            {
                                object[] objArray = Attribute.GetCustomAttributes(type2, typeof(ClientScriptResourceAttribute), false);
                                num -= objArray.Length;
                                foreach (ClientScriptResourceAttribute attribute3 in objArray)
                                {
                                    ResourceEntry item = new ResourceEntry(attribute3.ResourcePath, type2, num + attribute3.LoadOrder);
                                    if (!list.Contains(item) && !collection.Contains(item))
                                    {
                                        collection.Add(item);
                                    }
                                }
                            }
                        }
                        collection.Sort((l, r) => l.Order.CompareTo(r.Order));
                        list.AddRange(collection);
                        if (!ignoreStartingTypeReferences)
                        {
                            _cache.Add(type, list);
                        }
                    }
                    list4 = list;
                }
            }
            finally
            {
                typeReferenceStack.Pop();
            }
            return list4;
        }

        public static void RegisterCssReferences(Control control)
        {
            HtmlHead header = control.Page.Header;
            foreach (string str in GetCssReferences(control))
            {
                if (header == null)
                {
                    throw new NotSupportedException("This page is missing a HtmlHead control which is required for the CSS stylesheet link that is being added. Please add <head runat=\"server\" />.");
                }
                bool flag = true;
                foreach (Control control2 in header.Controls)
                {
                    HtmlLink link = control2 as HtmlLink;
                    if ((link != null) && str.Equals(link.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    HtmlLink child = new HtmlLink {
                        Href = str
                    };
                    child.Attributes.Add("type", "text/css");
                    child.Attributes.Add("rel", "stylesheet");
                    header.Controls.Add(child);
                    ScriptManager current = ScriptManager.GetCurrent(control.Page);
                    if (current == null)
                    {
                        throw new InvalidOperationException(Resources.E_NoScriptManager);
                    }
                    if (current.IsInAsyncPostBack)
                    {
                        ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "RegisterCssReferences", "if (window.__ExtendedControlCssLoaded == null || typeof window.__ExtendedControlCssLoaded == 'undefined') {    window.__ExtendedControlCssLoaded = new Array();}var controlCssLoaded = window.__ExtendedControlCssLoaded; var head = document.getElementsByTagName('HEAD')[0];if (head && !Array.contains(controlCssLoaded,'" + str + "')) {var linkElement = document.createElement('link');linkElement.type = 'text/css';linkElement.rel = 'stylesheet';linkElement.href = '" + str + "';head.appendChild(linkElement);controlCssLoaded.push('" + str + "');}", true);
                    }
                }
            }
        }

        private static IEnumerable<ScriptReference> ScriptReferencesFromResourceEntries(IList<ResourceEntry> entries)
        {
            IList<ScriptReference> list = new List<ScriptReference>(entries.Count);
            foreach (ResourceEntry entry in entries)
            {
                list.Add(entry.ToScriptReference());
            }
            return list;
        }

        public static IDictionary<Type, Converter<object, string>> CustomConverters =>
            _customConverters;

        [StructLayout(LayoutKind.Sequential)]
        private struct ResourceEntry
        {
            public string ResourcePath;
            public Type ComponentType;
            public int Order;
            private string AssemblyName
            {
                get
                {
                    if (this.ComponentType != null)
                    {
                        return this.ComponentType.Assembly.FullName;
                    }
                    return "";
                }
            }
            public ResourceEntry(string path, Type componentType, int order)
            {
                this.ResourcePath = path;
                this.ComponentType = componentType;
                this.Order = order;
            }

            public ScriptReference ToScriptReference() => 
                new ScriptReference { 
                    Assembly = this.AssemblyName,
                    Name = this.ResourcePath
                };

            public override bool Equals(object obj)
            {
                ScriptObjectBuilder.ResourceEntry entry = (ScriptObjectBuilder.ResourceEntry) obj;
                return (this.ResourcePath.Equals(entry.ResourcePath, StringComparison.OrdinalIgnoreCase) && this.AssemblyName.Equals(entry.AssemblyName, StringComparison.OrdinalIgnoreCase));
            }

            public static bool operator ==(ScriptObjectBuilder.ResourceEntry obj1, ScriptObjectBuilder.ResourceEntry obj2) => 
                obj1.Equals(obj2);

            public static bool operator !=(ScriptObjectBuilder.ResourceEntry obj1, ScriptObjectBuilder.ResourceEntry obj2) => 
                !obj1.Equals(obj2);

            public override int GetHashCode() => 
                (this.AssemblyName.GetHashCode() ^ this.ResourcePath.GetHashCode());
        }
    }
}

