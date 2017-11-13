namespace System.Web.Script.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Compilation;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Profile;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Security;
    using System.Web.Services;
    using System.Web.UI;
    using System.Xml;

    internal class WebServiceData : JavaScriptTypeResolver
    {
        internal const string _authenticationServiceFileName = "Authentication_JSON_AppService.axd";
        private Dictionary<Type, string> _clientTypeNameDictionary;
        private Dictionary<string, WebServiceTypeData> _clientTypesDictionary;
        private bool _clientTypesProcessed;
        private Dictionary<string, WebServiceEnumData> _enumTypesDictionary;
        private Dictionary<string, WebServiceMethodData> _methods;
        private bool _pageMethods;
        private Hashtable _processedTypes;
        internal const string _profileServiceFileName = "Profile_JSON_AppService.axd";
        internal const string _roleServiceFileName = "Role_JSON_AppService.axd";
        private JavaScriptSerializer _serializer;
        private WebServiceTypeData _typeData;
        private Dictionary<string, string> _typeResolverSpecials;

        private WebServiceData()
        {
            this._typeResolverSpecials = new Dictionary<string, string>();
        }

        private WebServiceData(WebServiceTypeData typeData)
        {
            this._typeResolverSpecials = new Dictionary<string, string>();
            this._typeData = typeData;
            this._serializer = new JavaScriptSerializer(this);
            ScriptingJsonSerializationSection.ApplicationSettings settings = new ScriptingJsonSerializationSection.ApplicationSettings();
            this._serializer.MaxJsonLength = settings.MaxJsonLimit;
            this._serializer.RecursionLimit = settings.RecursionLimit;
            this._serializer.RegisterConverters(settings.Converters);
        }

        internal WebServiceData(Type type, bool pageMethods) : this(new WebServiceTypeData(type.Name, type.Namespace, type))
        {
            this._pageMethods = pageMethods;
            if (!this._pageMethods && (type.GetCustomAttributes(typeof(ScriptServiceAttribute), true).Length == 0))
            {
                throw new InvalidOperationException(AtlasWeb.WebService_NoScriptServiceAttribute);
            }
        }

        internal WebServiceData(WebServiceTypeData typeData, Dictionary<string, WebServiceMethodData> methods) : this(typeData)
        {
            this._methods = methods;
        }

        private void AddMethod(Dictionary<string, WebServiceMethodData> methods, MethodInfo method)
        {
            object[] customAttributes = method.GetCustomAttributes(typeof(WebMethodAttribute), true);
            if (customAttributes.Length != 0)
            {
                ScriptMethodAttribute scriptMethodAttribute = null;
                object[] objArray2 = method.GetCustomAttributes(typeof(ScriptMethodAttribute), true);
                if (objArray2.Length > 0)
                {
                    scriptMethodAttribute = (ScriptMethodAttribute) objArray2[0];
                }
                WebServiceMethodData data = new WebServiceMethodData(this, method, (WebMethodAttribute) customAttributes[0], scriptMethodAttribute);
                methods[data.MethodName] = data;
            }
        }

        private void EnsureClientTypesProcessed()
        {
            if (!this._clientTypesProcessed)
            {
                lock (this)
                {
                    if (!this._clientTypesProcessed)
                    {
                        this.ProcessClientTypes();
                    }
                }
            }
        }

        private void EnsureMethods()
        {
            if ((this._methods == null) && (this._typeData.Type != null))
            {
                lock (this)
                {
                    List<Type> list = new List<Type>();
                    Type item = this._typeData.Type;
                    list.Add(item);
                    while (item.BaseType != null)
                    {
                        item = item.BaseType;
                        list.Add(item);
                    }
                    Dictionary<string, WebServiceMethodData> methods = new Dictionary<string, WebServiceMethodData>(StringComparer.OrdinalIgnoreCase);
                    BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.DeclaredOnly;
                    if (this._pageMethods)
                    {
                        bindingAttr |= BindingFlags.Static;
                    }
                    else
                    {
                        bindingAttr |= BindingFlags.Instance;
                    }
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        foreach (MethodInfo info in list[i].GetMethods(bindingAttr))
                        {
                            this.AddMethod(methods, info);
                        }
                    }
                    this._methods = methods;
                }
            }
        }

        private static WebServiceData GetApplicationService(string appRelativePath)
        {
            if (appRelativePath.LastIndexOf('/') == 1)
            {
                string fileName = Path.GetFileName(appRelativePath);
                if (fileName.Equals("Profile_JSON_AppService.axd", StringComparison.OrdinalIgnoreCase))
                {
                    return new WebServiceData(typeof(ProfileService), false);
                }
                if (fileName.Equals("Authentication_JSON_AppService.axd", StringComparison.OrdinalIgnoreCase))
                {
                    return new WebServiceData(typeof(AuthenticationService), false);
                }
                if (fileName.Equals("Role_JSON_AppService.axd", StringComparison.OrdinalIgnoreCase))
                {
                    return new WebServiceData(typeof(RoleService), false);
                }
            }
            return null;
        }

        private static string GetCacheKey(string virtualPath) => 
            ("System.Web.Script.Services.WebServiceData:" + virtualPath);

        internal WebServiceMethodData GetMethodData(string methodName)
        {
            this.EnsureMethods();
            WebServiceMethodData data = null;
            if (!this._methods.TryGetValue(methodName, out data))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.WebService_UnknownWebMethod, new object[] { methodName }), "methodName");
            }
            this.EnsureClientTypesProcessed();
            return data;
        }

        internal string GetTypeStringRepresentation(string typeName) => 
            this.GetTypeStringRepresentation(typeName, true);

        internal string GetTypeStringRepresentation(WebServiceTypeData typeData)
        {
            string stringRepresentation = typeData.StringRepresentation;
            if (stringRepresentation == null)
            {
                stringRepresentation = this.GetTypeStringRepresentation(typeData.TypeName, true);
            }
            return stringRepresentation;
        }

        internal string GetTypeStringRepresentation(string typeName, bool ensure)
        {
            string str;
            if (ensure)
            {
                this.EnsureClientTypesProcessed();
            }
            if (this._typeResolverSpecials.TryGetValue(typeName, out str))
            {
                return str;
            }
            return typeName;
        }

        internal static WebServiceData GetWebServiceData(ContractDescription contract)
        {
            WebServiceData owner = new WebServiceData {
                _typeData = new WebServiceTypeData(XmlConvert.DecodeName(contract.Name), XmlConvert.DecodeName(contract.Namespace), contract.ContractType)
            };
            Dictionary<string, WebServiceTypeData> dictionary = new Dictionary<string, WebServiceTypeData>();
            owner._clientTypesDictionary = dictionary;
            Dictionary<string, WebServiceEnumData> dictionary2 = new Dictionary<string, WebServiceEnumData>();
            owner._enumTypesDictionary = dictionary2;
            owner._processedTypes = new Hashtable();
            owner._clientTypesProcessed = true;
            owner._clientTypeNameDictionary = new Dictionary<Type, string>();
            Dictionary<string, WebServiceMethodData> dictionary3 = new Dictionary<string, WebServiceMethodData>();
            owner._methods = dictionary3;
            foreach (OperationDescription description in contract.Operations)
            {
                Dictionary<string, WebServiceParameterData> parameterData = new Dictionary<string, WebServiceParameterData>();
                bool useHttpGet = description.Behaviors.Find<WebGetAttribute>() != null;
                WebServiceMethodData data2 = new WebServiceMethodData(owner, XmlConvert.DecodeName(description.Name), parameterData, useHttpGet);
                MessageDescription description2 = description.Messages[0];
                if (description2 != null)
                {
                    int count = description2.Body.Parts.Count;
                    for (int j = 0; j < count; j++)
                    {
                        MessagePartDescription description3 = description2.Body.Parts[j];
                        Type paramType = ReplaceMessageWithObject(description3.Type);
                        WebServiceParameterData data3 = new WebServiceParameterData(XmlConvert.DecodeName(description3.Name), paramType, j);
                        parameterData[data3.ParameterName] = data3;
                        owner.ProcessClientType(paramType, false, true);
                    }
                }
                if (description.Messages.Count > 1)
                {
                    MessageDescription description4 = description.Messages[1];
                    if (((description4 != null) && (description4.Body.ReturnValue != null)) && (description4.Body.ReturnValue.Type != null))
                    {
                        owner.ProcessClientType(ReplaceMessageWithObject(description4.Body.ReturnValue.Type), false, true);
                    }
                }
                for (int i = 0; i < description.KnownTypes.Count; i++)
                {
                    owner.ProcessClientType(description.KnownTypes[i], false, true);
                }
                dictionary3[data2.MethodName] = data2;
            }
            owner._processedTypes = null;
            return owner;
        }

        internal static WebServiceData GetWebServiceData(HttpContext context, string virtualPath) => 
            GetWebServiceData(context, virtualPath, true, false, false);

        internal static WebServiceData GetWebServiceData(HttpContext context, string virtualPath, bool failIfNoData, bool pageMethods) => 
            GetWebServiceData(context, virtualPath, failIfNoData, pageMethods, false);

        internal static WebServiceData GetWebServiceData(HttpContext context, string virtualPath, bool failIfNoData, bool pageMethods, bool inlineScript)
        {
            virtualPath = VirtualPathUtility.ToAbsolute(virtualPath);
            string cacheKey = GetCacheKey(virtualPath);
            WebServiceData applicationService = context.Cache[cacheKey] as WebServiceData;
            if (applicationService == null)
            {
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    Type compiledType = null;
                    try
                    {
                        compiledType = BuildManager.GetCompiledType(virtualPath);
                        if (compiledType == null)
                        {
                            object obj2 = BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(Page));
                            if (obj2 != null)
                            {
                                compiledType = obj2.GetType();
                            }
                        }
                    }
                    catch (SecurityException)
                    {
                    }
                    if (compiledType != null)
                    {
                        applicationService = new WebServiceData(compiledType, pageMethods);
                        BuildDependencySet cachedBuildDependencySet = BuildManager.GetCachedBuildDependencySet(context, virtualPath);
                        CacheDependency dependencies = HostingEnvironment.VirtualPathProvider.GetCacheDependency(virtualPath, cachedBuildDependencySet.VirtualPaths, DateTime.Now);
                        context.Cache.Insert(cacheKey, applicationService, dependencies);
                    }
                }
                else if (virtualPath.EndsWith("_AppService.axd", StringComparison.OrdinalIgnoreCase))
                {
                    applicationService = GetApplicationService(context.Request.AppRelativeCurrentExecutionFilePath);
                    if (applicationService != null)
                    {
                        context.Cache.Insert(cacheKey, applicationService);
                    }
                }
            }
            if (applicationService != null)
            {
                return applicationService;
            }
            if (!failIfNoData)
            {
                return null;
            }
            if (inlineScript)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.WebService_NoWebServiceDataInlineScript, new object[] { virtualPath }));
            }
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.WebService_NoWebServiceData, new object[] { virtualPath }));
        }

        private void ProcessClientType(Type t)
        {
            this.ProcessClientType(t, false, false);
        }

        private void ProcessClientType(Type t, bool force)
        {
            this.ProcessClientType(t, force, false);
        }

        private void ProcessClientType(Type t, bool force, bool isWCF)
        {
            if (force || !this._processedTypes.Contains(t))
            {
                this._processedTypes[t] = null;
                if (t.IsEnum)
                {
                    WebServiceEnumData webServiceTypeData = null;
                    if (isWCF)
                    {
                        webServiceTypeData = (WebServiceEnumData) WebServiceTypeData.GetWebServiceTypeData(t);
                    }
                    else
                    {
                        webServiceTypeData = new WebServiceEnumData(t.Name, t.Namespace, t, Enum.GetNames(t), Enum.GetValues(t), Enum.GetUnderlyingType(t) == typeof(ulong));
                    }
                    this._enumTypesDictionary[this.GetTypeStringRepresentation(webServiceTypeData.TypeName, false)] = webServiceTypeData;
                }
                else if (t.IsGenericType)
                {
                    if (isWCF)
                    {
                        this.ProcessKnownTypes(t);
                    }
                    else
                    {
                        Type[] genericArguments = t.GetGenericArguments();
                        if (genericArguments.Length <= 1)
                        {
                            this.ProcessClientType(genericArguments[0], false, isWCF);
                        }
                    }
                }
                else if (t.IsArray)
                {
                    this.ProcessClientType(t.GetElementType(), false, isWCF);
                }
                else if (((!t.IsPrimitive && (t != typeof(object))) && ((t != typeof(string)) && (t != typeof(DateTime)))) && ((((t != typeof(void)) && (t != typeof(decimal))) && ((t != typeof(Guid)) && !typeof(IEnumerable).IsAssignableFrom(t))) && (!typeof(IDictionary).IsAssignableFrom(t) && (isWCF || System.Web.Script.Serialization.ObjectConverter.IsClientInstantiatableType(t, this._serializer)))))
                {
                    if (isWCF)
                    {
                        this.ProcessKnownTypes(t);
                    }
                    else
                    {
                        string typeStringRepresentation = this.GetTypeStringRepresentation(t.FullName, false);
                        this._clientTypesDictionary[typeStringRepresentation] = new WebServiceTypeData(t.Name, t.Namespace, t);
                        this._clientTypeNameDictionary[t] = typeStringRepresentation;
                    }
                }
            }
        }

        private void ProcessClientTypes()
        {
            this._clientTypesDictionary = new Dictionary<string, WebServiceTypeData>();
            this._enumTypesDictionary = new Dictionary<string, WebServiceEnumData>();
            this._clientTypeNameDictionary = new Dictionary<Type, string>();
            try
            {
                this._processedTypes = new Hashtable();
                this.ProcessIncludeAttributes((GenerateScriptTypeAttribute[]) this._typeData.Type.GetCustomAttributes(typeof(GenerateScriptTypeAttribute), true));
                foreach (WebServiceMethodData data in this.MethodDatas)
                {
                    this.ProcessIncludeAttributes((GenerateScriptTypeAttribute[]) data.MethodInfo.GetCustomAttributes(typeof(GenerateScriptTypeAttribute), true));
                    foreach (WebServiceParameterData data2 in data.ParameterDatas)
                    {
                        this.ProcessClientType(data2.ParameterInfo.ParameterType);
                    }
                    if (!data.UseXmlResponse)
                    {
                        this.ProcessClientType(data.ReturnType);
                    }
                }
                this._clientTypesProcessed = true;
            }
            catch
            {
                this._clientTypesDictionary = null;
                this._enumTypesDictionary = null;
                this._clientTypeNameDictionary = null;
                throw;
            }
            finally
            {
                this._processedTypes = null;
            }
        }

        private void ProcessIncludeAttributes(GenerateScriptTypeAttribute[] attributes)
        {
            foreach (GenerateScriptTypeAttribute attribute in attributes)
            {
                if (!string.IsNullOrEmpty(attribute.ScriptTypeId))
                {
                    this._typeResolverSpecials[attribute.Type.FullName] = attribute.ScriptTypeId;
                }
                Type c = attribute.Type;
                if (((((c.IsPrimitive || (c == typeof(object))) || ((c == typeof(string)) || (c == typeof(DateTime)))) || (((c == typeof(Guid)) || typeof(IEnumerable).IsAssignableFrom(c)) || typeof(IDictionary).IsAssignableFrom(c))) || (c.IsGenericType && (c.GetGenericArguments().Length > 1))) || !System.Web.Script.Serialization.ObjectConverter.IsClientInstantiatableType(c, this._serializer))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.WebService_InvalidGenerateScriptType, new object[] { c.FullName }));
                }
                this.ProcessClientType(c, true);
            }
        }

        private void ProcessKnownTypes(Type t)
        {
            WebServiceTypeData webServiceTypeData = WebServiceTypeData.GetWebServiceTypeData(t);
            bool flag = false;
            if (webServiceTypeData != null)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(t) && !typeof(IDictionary).IsAssignableFrom(t))
                {
                    this._clientTypeNameDictionary[t] = this.GetTypeStringRepresentation(webServiceTypeData.TypeName);
                    flag = this.ProcessTypeData(webServiceTypeData);
                }
                if (!flag)
                {
                    foreach (WebServiceTypeData data2 in WebServiceTypeData.GetKnownTypes(t, webServiceTypeData))
                    {
                        this.ProcessTypeData(data2);
                    }
                }
            }
        }

        private bool ProcessTypeData(WebServiceTypeData typeData)
        {
            string typeStringRepresentation = this.GetTypeStringRepresentation(typeData.TypeName);
            bool flag = true;
            if (typeData is WebServiceEnumData)
            {
                if (!this._enumTypesDictionary.ContainsKey(typeStringRepresentation))
                {
                    this._enumTypesDictionary[typeStringRepresentation] = (WebServiceEnumData) typeData;
                    flag = false;
                }
                return flag;
            }
            if (!this._clientTypesDictionary.ContainsKey(typeStringRepresentation))
            {
                this._clientTypesDictionary[typeStringRepresentation] = typeData;
                flag = false;
            }
            return flag;
        }

        private static Type ReplaceMessageWithObject(Type t)
        {
            if (!typeof(Message).IsAssignableFrom(t))
            {
                return t;
            }
            return typeof(object);
        }

        public override Type ResolveType(string id)
        {
            WebServiceTypeData data = null;
            if (this.ClientTypeDictionary.TryGetValue(id, out data) && (data != null))
            {
                return data.Type;
            }
            return null;
        }

        public override string ResolveTypeId(Type type)
        {
            string typeStringRepresentation = this.GetTypeStringRepresentation(type.FullName);
            if (!this.ClientTypeDictionary.ContainsKey(typeStringRepresentation))
            {
                return null;
            }
            return typeStringRepresentation;
        }

        internal Dictionary<string, WebServiceTypeData> ClientTypeDictionary
        {
            get
            {
                this.EnsureClientTypesProcessed();
                return this._clientTypesDictionary;
            }
            set
            {
                this._clientTypesDictionary = value;
            }
        }

        internal Dictionary<Type, string> ClientTypeNameDictionary
        {
            get
            {
                this.EnsureClientTypesProcessed();
                return this._clientTypeNameDictionary;
            }
        }

        internal IEnumerable<WebServiceTypeData> ClientTypes =>
            this.ClientTypeDictionary.Values;

        internal Dictionary<string, WebServiceEnumData> EnumTypeDictionary
        {
            get
            {
                this.EnsureClientTypesProcessed();
                return this._enumTypesDictionary;
            }
            set
            {
                this._enumTypesDictionary = value;
            }
        }

        internal IEnumerable<WebServiceEnumData> EnumTypes
        {
            get
            {
                this.EnsureClientTypesProcessed();
                return this._enumTypesDictionary.Values;
            }
        }

        internal ICollection<WebServiceMethodData> MethodDatas
        {
            get
            {
                this.EnsureMethods();
                return this._methods.Values;
            }
        }

        internal JavaScriptSerializer Serializer =>
            this._serializer;

        internal WebServiceTypeData TypeData =>
            this._typeData;
    }
}

