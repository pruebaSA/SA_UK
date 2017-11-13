namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;
    using System.Web.ApplicationServices;
    using System.Web.ClientServices;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Security;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class ClientSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private string _ConnectionString;
        private string _ConnectionStringProvider = "";
        private bool _firstTime = true;
        private bool _HonorCookieExpiry;
        private static Type[] _KnownTypesArray = null;
        private static Hashtable _KnownTypesHashtable = null;
        private static object _lock = new object();
        private bool _NeedToDoReset;
        private SettingsPropertyCollection _Properties;
        private SettingsPropertyValueCollection _PropertyValues = new SettingsPropertyValueCollection();
        private static string _ServiceUri = "";
        private static ApplicationSettingsBase _SettingsBaseClass = null;
        private string _UserName = "";
        private static bool _UsingFileSystemStore = false;
        private static bool _UsingIsolatedStore = true;
        private static bool _UsingWFCService = false;

        public event EventHandler<SettingsSavedEventArgs> SettingsSaved;

        private void AddProperty(string name, string storedAs, string propVal)
        {
            if (((storedAs == "S") || (storedAs == "B")) || (storedAs == "N"))
            {
                SettingsProperty property = this._Properties[name];
                if (property != null)
                {
                    SettingsPropertyValue value2 = this._PropertyValues[name];
                    bool flag = false;
                    if (value2 == null)
                    {
                        value2 = new SettingsPropertyValue(property);
                        flag = true;
                    }
                    string str = storedAs;
                    if (str != null)
                    {
                        if (str == "S")
                        {
                            value2.SerializedValue = propVal;
                        }
                        else if (str == "B")
                        {
                            value2.SerializedValue = Convert.FromBase64String(propVal);
                        }
                        else if (str == "N")
                        {
                            value2.SerializedValue = null;
                        }
                    }
                    value2.Deserialized = false;
                    value2.IsDirty = false;
                    if (flag)
                    {
                        this._PropertyValues.Add(value2);
                    }
                }
            }
        }

        private static void AddToColl(ProfilePropertyMetadata p, SettingsPropertyCollection retColl, bool isAuthenticated)
        {
            string propertyName = p.PropertyName;
            Type propertyType = Type.GetType(p.TypeName, false, true);
            bool allowAnonymousAccess = p.AllowAnonymousAccess;
            bool isReadOnly = p.IsReadOnly;
            if (allowAnonymousAccess || isAuthenticated)
            {
                SettingsSerializeAs serializeAs = (SettingsSerializeAs) p.SerializeAs;
                SettingsAttributeDictionary attributes = new SettingsAttributeDictionary();
                attributes.Add("AllowAnonymous", allowAnonymousAccess);
                retColl.Add(new SettingsProperty(propertyName, propertyType, null, isReadOnly, p.DefaultValue, serializeAs, attributes, true, true));
            }
        }

        private string GetConnectionString()
        {
            if (this._ConnectionString == null)
            {
                this._ConnectionString = SqlHelper.GetDefaultConnectionString();
            }
            return this._ConnectionString;
        }

        private bool GetIsCacheMoreFresh()
        {
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                return ClientDataManager.GetUserClientData(Thread.CurrentPrincipal.Identity.Name, _UsingIsolatedStore).SettingsCacheIsMoreFresh;
            }
            string tagValue = this.GetTagValue("IsCacheMoreFresh");
            return ((tagValue != null) && (tagValue == "1"));
        }

        internal static Type[] GetKnownTypes(ICustomAttributeProvider knownTypeAttributeTarget)
        {
            if (_KnownTypesArray == null)
            {
                InitKnownTypes();
            }
            return _KnownTypesArray;
        }

        private bool GetNeedToReset()
        {
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                return ClientDataManager.GetUserClientData(Thread.CurrentPrincipal.Identity.Name, _UsingIsolatedStore).SettingsNeedReset;
            }
            string tagValue = this.GetTagValue("NeeedToDoReset");
            return ((tagValue != null) && (tagValue == "1"));
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            if (this._Properties == null)
            {
                this._Properties = new SettingsPropertyCollection();
            }
            if (this._Properties[property.Name] == null)
            {
                this._Properties.Add(property);
            }
            this.GetPropertyValuesCore();
            return this._PropertyValues[property.Name];
        }

        public static SettingsPropertyCollection GetPropertyMetadata(string serviceUri)
        {
            CookieContainer cookies = null;
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            SettingsPropertyCollection retColl = new SettingsPropertyCollection();
            if (identity is ClientFormsIdentity)
            {
                cookies = ((ClientFormsIdentity) identity).AuthenticationCookies;
            }
            if (serviceUri.EndsWith(".svc", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }
            Collection<ProfilePropertyMetadata> collection = (Collection<ProfilePropertyMetadata>) ProxyHelper.CreateWebRequestAndGetResponse(serviceUri + "/GetPropertiesMetadata", ref cookies, identity.Name, null, null, null, null, typeof(Collection<ProfilePropertyMetadata>));
            if (collection != null)
            {
                foreach (ProfilePropertyMetadata metadata in collection)
                {
                    AddToColl(metadata, retColl, identity.IsAuthenticated);
                }
            }
            return retColl;
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection propertyCollection)
        {
            if ((propertyCollection == null) || (propertyCollection.Count < 1))
            {
                return new SettingsPropertyValueCollection();
            }
            lock (_lock)
            {
                if ((_SettingsBaseClass == null) && (context != null))
                {
                    Type type = context["SettingsClassType"] as Type;
                    if (type != null)
                    {
                        _SettingsBaseClass = type.InvokeMember("Default", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static, null, null, null, CultureInfo.InvariantCulture) as ApplicationSettingsBase;
                    }
                }
                this._PropertyValues = new SettingsPropertyValueCollection();
                this._Properties = propertyCollection;
                StoreKnownTypes(propertyCollection);
                this.GetPropertyValuesCore();
                return this._PropertyValues;
            }
        }

        private void GetPropertyValuesCore()
        {
            this._UserName = Thread.CurrentPrincipal.Identity.Name;
            if (this._firstTime)
            {
                this._firstTime = false;
                this._NeedToDoReset = this.GetNeedToReset();
                this.RegisterForValidateUserEvent();
            }
            if (this._NeedToDoReset)
            {
                this._NeedToDoReset = false;
                this.SetNeedToReset(false);
                this._PropertyValues = new SettingsPropertyValueCollection();
                this.SetRemainingValuesToDefault();
                this.SetPropertyValuesCore(this._PropertyValues, false);
            }
            bool isCacheMoreFresh = this.GetIsCacheMoreFresh();
            this.GetPropertyValuesFromSQL();
            if (!ConnectivityStatus.IsOffline)
            {
                if (isCacheMoreFresh)
                {
                    this.SetPropertyValuesWeb(this._PropertyValues, isCacheMoreFresh);
                }
                else
                {
                    this.GetPropertyValuesFromWeb();
                    this.SetPropertyValuesSQL(this._PropertyValues, false);
                }
            }
            if (this._PropertyValues.Count < this._Properties.Count)
            {
                this.SetRemainingValuesToDefault();
            }
        }

        private void GetPropertyValuesFromSQL()
        {
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(Thread.CurrentPrincipal.Identity.Name, _UsingIsolatedStore);
                if ((userClientData.SettingsNames != null) && (userClientData.SettingsValues != null))
                {
                    int length = userClientData.SettingsNames.Length;
                    if ((userClientData.SettingsNames.Length == userClientData.SettingsStoredAs.Length) && (userClientData.SettingsValues.Length == userClientData.SettingsStoredAs.Length))
                    {
                        for (int i = 0; i < length; i++)
                        {
                            this.AddProperty(userClientData.SettingsNames[i], userClientData.SettingsStoredAs[i], userClientData.SettingsValues[i]);
                        }
                    }
                }
            }
            else
            {
                using (DbConnection connection = SqlHelper.GetConnection(Thread.CurrentPrincipal.Identity.Name, this.GetConnectionString(), this._ConnectionStringProvider))
                {
                    DbTransaction transaction = null;
                    try
                    {
                        transaction = connection.BeginTransaction();
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT PropertyName, PropertyStoredAs, PropertyValue FROM Settings";
                        command.Transaction = transaction;
                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader.GetString(0);
                                string storedAs = reader.GetString(1);
                                string propVal = reader.IsDBNull(2) ? null : reader.GetString(2);
                                this.AddProperty(name, storedAs, propVal);
                            }
                        }
                    }
                    catch
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback();
                            transaction = null;
                        }
                        throw;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        private void GetPropertyValuesFromWeb()
        {
            this.GetPropertyValuesFromWebCore(this._HonorCookieExpiry);
            bool flag = this._PropertyValues.Count < this._Properties.Count;
            if (!this._HonorCookieExpiry && flag)
            {
                ClientFormsIdentity identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
                if (identity != null)
                {
                    identity.RevalidateUser();
                    this.GetPropertyValuesFromWebCore(true);
                }
            }
        }

        private void GetPropertyValuesFromWebCore(bool bubbleExceptionFromSvc)
        {
            string[] strArray = new string[this._Properties.Count];
            int num = 0;
            CookieContainer cookies = null;
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            foreach (SettingsProperty property in this._Properties)
            {
                strArray[num++] = property.Name;
            }
            if (identity is ClientFormsIdentity)
            {
                cookies = ((ClientFormsIdentity) identity).AuthenticationCookies;
            }
            if (_UsingWFCService)
            {
                throw new NotImplementedException();
            }
            string[] paramNames = new string[] { "properties", "authenticatedUserOnly" };
            object[] paramValues = new object[] { strArray, !identity.IsAuthenticated ? ((object) 0) : ((object) (identity is ClientFormsIdentity)) };
            object obj2 = null;
            try
            {
                obj2 = ProxyHelper.CreateWebRequestAndGetResponse(this.GetServiceUri() + "/GetPropertiesForCurrentUser", ref cookies, identity.Name, this._ConnectionString, this._ConnectionStringProvider, paramNames, paramValues, typeof(Dictionary<string, object>));
            }
            catch
            {
                if (bubbleExceptionFromSvc)
                {
                    throw;
                }
            }
            if (obj2 != null)
            {
                Dictionary<string, object> dictionary = (Dictionary<string, object>) obj2;
                foreach (KeyValuePair<string, object> pair in dictionary)
                {
                    SettingsProperty property2 = this._Properties[pair.Key];
                    if (property2 != null)
                    {
                        bool flag = false;
                        SettingsPropertyValue value2 = this._PropertyValues[property2.Name];
                        if (value2 == null)
                        {
                            value2 = new SettingsPropertyValue(property2);
                            flag = true;
                        }
                        if ((pair.Value != null) && !property2.PropertyType.IsAssignableFrom(pair.Value.GetType()))
                        {
                            object convertedObject = null;
                            if (!ObjectConverter.TryConvertObjectToType(pair.Value, property2.PropertyType, new JavaScriptSerializer(), out convertedObject))
                            {
                                continue;
                            }
                            value2.PropertyValue = convertedObject;
                        }
                        else
                        {
                            value2.PropertyValue = pair.Value;
                        }
                        value2.Deserialized = true;
                        value2.IsDirty = false;
                        if (flag)
                        {
                            this._PropertyValues.Add(value2);
                        }
                    }
                }
            }
        }

        private string GetServiceUri()
        {
            if (string.IsNullOrEmpty(_ServiceUri))
            {
                throw new ArgumentException(AtlasWeb.ServiceUriNotFound);
            }
            return _ServiceUri;
        }

        private string GetTagValue(string tagName)
        {
            using (DbConnection connection = SqlHelper.GetConnection(Thread.CurrentPrincipal.Identity.Name, this.GetConnectionString(), this._ConnectionStringProvider))
            {
                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT PropertyValue FROM Settings WHERE PropertyName = @PropName AND PropertyStoredAs='I'";
                SqlHelper.AddParameter(connection, cmd, "@PropName", tagName);
                return (cmd.ExecuteScalar() as string);
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _UsingIsolatedStore = false;
            string str = ConfigurationManager.AppSettings["ClientSettingsProvider.ServiceUri"];
            if (!string.IsNullOrEmpty(str))
            {
                ServiceUri = str;
            }
            str = ConfigurationManager.AppSettings["ClientSettingsProvider.ConnectionStringName"];
            if (!string.IsNullOrEmpty(str))
            {
                if (ConfigurationManager.ConnectionStrings[str] != null)
                {
                    this._ConnectionStringProvider = ConfigurationManager.ConnectionStrings[str].ProviderName;
                    this._ConnectionString = ConfigurationManager.ConnectionStrings[str].ConnectionString;
                }
                else
                {
                    this._ConnectionString = str;
                }
            }
            else
            {
                this._ConnectionString = SqlHelper.GetDefaultConnectionString();
            }
            str = ConfigurationManager.AppSettings["ClientSettingsProvider.HonorCookieExpiry"];
            if (!string.IsNullOrEmpty(str))
            {
                this._HonorCookieExpiry = string.Compare(str, "true", StringComparison.OrdinalIgnoreCase) == 0;
            }
            if (name == null)
            {
                name = base.GetType().ToString();
            }
            base.Initialize(name, config);
            if (config != null)
            {
                str = config["serviceUri"];
                if (!string.IsNullOrEmpty(str))
                {
                    ServiceUri = str;
                }
                str = config["connectionStringName"];
                if (!string.IsNullOrEmpty(str))
                {
                    if (ConfigurationManager.ConnectionStrings[str] != null)
                    {
                        this._ConnectionStringProvider = ConfigurationManager.ConnectionStrings[str].ProviderName;
                        this._ConnectionString = ConfigurationManager.ConnectionStrings[str].ConnectionString;
                    }
                    else
                    {
                        this._ConnectionString = str;
                    }
                }
                config.Remove("name");
                config.Remove("description");
                config.Remove("connectionStringName");
                config.Remove("serviceUri");
                foreach (string str2 in config.Keys)
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.AttributeNotRecognized, new object[] { str2 }));
                    }
                }
            }
            switch (SqlHelper.IsSpecialConnectionString(this._ConnectionString))
            {
                case 1:
                    _UsingFileSystemStore = true;
                    return;

                case 2:
                    _UsingIsolatedStore = true;
                    return;
            }
        }

        private static void InitKnownTypes()
        {
            _KnownTypesHashtable = new Hashtable();
            _KnownTypesArray = new Type[] { typeof(bool), typeof(string), typeof(ArrayList), typeof(ProfilePropertyMetadata), typeof(IDictionary<string, object>), typeof(Collection<string>) };
            for (int i = 0; i < _KnownTypesArray.Length; i++)
            {
                _KnownTypesHashtable.Add(_KnownTypesArray[i], string.Empty);
            }
        }

        private void OnUserValidated(object src, UserValidatedEventArgs e)
        {
            this._NeedToDoReset = this.GetNeedToReset();
            if (((this._Properties != null) && (this._Properties.Count > 0)) && (string.Compare(e.UserName, this._UserName, StringComparison.OrdinalIgnoreCase) != 0))
            {
                try
                {
                    if (_SettingsBaseClass != null)
                    {
                        _SettingsBaseClass.Reload();
                    }
                }
                catch
                {
                }
            }
        }

        private void RegisterForValidateUserEvent()
        {
            foreach (MembershipProvider provider in Membership.Providers)
            {
                EventInfo info = provider.GetType().GetEvent("UserValidated");
                if (info != null)
                {
                    MethodInfo addMethod = info.GetAddMethod();
                    if (addMethod != null)
                    {
                        Delegate delegate2 = Delegate.CreateDelegate(addMethod.GetParameters()[0].ParameterType, this, "OnUserValidated");
                        addMethod.Invoke(provider, new object[] { delegate2 });
                    }
                }
            }
        }

        public void Reset(SettingsContext context)
        {
            lock (_lock)
            {
                if (this._Properties == null)
                {
                    this.SetNeedToReset(true);
                }
                else
                {
                    this._PropertyValues = new SettingsPropertyValueCollection();
                    this.SetRemainingValuesToDefault();
                    this.SetPropertyValues(context, this._PropertyValues);
                    this._NeedToDoReset = false;
                    this.SetNeedToReset(false);
                }
            }
        }

        private void SetIsCacheMoreFresh(bool fSet)
        {
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(Thread.CurrentPrincipal.Identity.Name, _UsingIsolatedStore);
                userClientData.SettingsCacheIsMoreFresh = fSet;
                userClientData.Save();
            }
            else
            {
                this.SetTagValue("IsCacheMoreFresh", fSet ? "1" : "0");
            }
        }

        private void SetNeedToReset(bool fSet)
        {
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(Thread.CurrentPrincipal.Identity.Name, _UsingIsolatedStore);
                userClientData.SettingsNeedReset = fSet;
                userClientData.Save();
            }
            else
            {
                this.SetTagValue("NeeedToDoReset", fSet ? "1" : "0");
            }
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propertyValueCollection)
        {
            if ((propertyValueCollection != null) && (propertyValueCollection.Count >= 1))
            {
                lock (_lock)
                {
                    StoreKnownTypes(propertyValueCollection);
                    this.SetPropertyValuesCore(propertyValueCollection, true);
                }
            }
        }

        private void SetPropertyValuesCore(SettingsPropertyValueCollection values, bool raiseEvent)
        {
            lock (_lock)
            {
                bool isCacheMoreFresh = this.GetIsCacheMoreFresh();
                this.SetPropertyValuesSQL(values, true);
                Collection<string> failedSettingsList = null;
                if (!ConnectivityStatus.IsOffline)
                {
                    failedSettingsList = this.SetPropertyValuesWeb(values, isCacheMoreFresh);
                }
                if (raiseEvent && (this.SettingsSaved != null))
                {
                    if (failedSettingsList == null)
                    {
                        failedSettingsList = new Collection<string>();
                    }
                    this.SettingsSaved(this, new SettingsSavedEventArgs(failedSettingsList));
                }
            }
        }

        private void SetPropertyValuesSQL(SettingsPropertyValueCollection values, bool updateSaveTime)
        {
            string name = Thread.CurrentPrincipal.Identity.Name;
            if (_UsingFileSystemStore || _UsingIsolatedStore)
            {
                ClientData userClientData = ClientDataManager.GetUserClientData(name, _UsingIsolatedStore);
                userClientData.SettingsNames = new string[values.Count];
                userClientData.SettingsStoredAs = new string[values.Count];
                userClientData.SettingsValues = new string[values.Count];
                int index = 0;
                foreach (SettingsPropertyValue value2 in values)
                {
                    userClientData.SettingsNames[index] = value2.Property.Name;
                    object serializedValue = value2.SerializedValue;
                    if (serializedValue == null)
                    {
                        userClientData.SettingsStoredAs[index] = "N";
                    }
                    else if (serializedValue is string)
                    {
                        userClientData.SettingsStoredAs[index] = "S";
                        userClientData.SettingsValues[index] = (string) serializedValue;
                    }
                    else
                    {
                        userClientData.SettingsStoredAs[index] = "B";
                        userClientData.SettingsValues[index] = Convert.ToBase64String((byte[]) serializedValue);
                    }
                    index++;
                }
                if (updateSaveTime)
                {
                    userClientData.SettingsCacheIsMoreFresh = true;
                }
                userClientData.Save();
            }
            else
            {
                using (DbConnection connection = SqlHelper.GetConnection(name, this.GetConnectionString(), this._ConnectionStringProvider))
                {
                    DbTransaction transaction = null;
                    try
                    {
                        transaction = connection.BeginTransaction();
                        foreach (SettingsPropertyValue value3 in values)
                        {
                            DbCommand cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;
                            cmd.CommandText = "DELETE FROM Settings WHERE PropertyName = @PropName";
                            SqlHelper.AddParameter(connection, cmd, "@PropName", value3.Property.Name);
                            cmd.ExecuteNonQuery();
                            cmd = connection.CreateCommand();
                            cmd.Transaction = transaction;
                            object obj3 = value3.SerializedValue;
                            if (obj3 == null)
                            {
                                cmd.CommandText = "INSERT INTO Settings (PropertyName, PropertyStoredAs, PropertyValue) VALUES (@PropName, 'N', '')";
                                SqlHelper.AddParameter(connection, cmd, "@PropName", value3.Property.Name);
                            }
                            else if (obj3 is string)
                            {
                                cmd.CommandText = "INSERT INTO Settings (PropertyName, PropertyStoredAs, PropertyValue) VALUES (@PropName, 'S', @PropVal)";
                                SqlHelper.AddParameter(connection, cmd, "@PropName", value3.Property.Name);
                                SqlHelper.AddParameter(connection, cmd, "@PropVal", (string) obj3);
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO Settings (PropertyName, PropertyStoredAs, PropertyValue) VALUES (@PropName, 'B', @PropVal)";
                                SqlHelper.AddParameter(connection, cmd, "@PropName", value3.Property.Name);
                                SqlHelper.AddParameter(connection, cmd, "@PropVal", Convert.ToBase64String((byte[]) obj3));
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch
                    {
                        if (transaction != null)
                        {
                            transaction.Rollback();
                            transaction = null;
                        }
                        throw;
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
                if (updateSaveTime)
                {
                    this.SetIsCacheMoreFresh(true);
                }
            }
        }

        private Collection<string> SetPropertyValuesWeb(SettingsPropertyValueCollection values, bool cacheIsMoreFresh)
        {
            bool flag = false;
            Collection<string> collection = null;
            ClientFormsIdentity identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
            try
            {
                collection = this.SetPropertyValuesWebCore(values, cacheIsMoreFresh);
                flag = (collection != null) && (collection.Count > 0);
            }
            catch (WebException)
            {
                if ((identity == null) || this._HonorCookieExpiry)
                {
                    throw;
                }
                flag = true;
            }
            if ((!this._HonorCookieExpiry && flag) && (identity != null))
            {
                identity.RevalidateUser();
                collection = this.SetPropertyValuesWebCore(values, cacheIsMoreFresh);
            }
            return collection;
        }

        private Collection<string> SetPropertyValuesWebCore(SettingsPropertyValueCollection values, bool cacheIsMoreFresh)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Collection<string> collection = null;
            foreach (SettingsPropertyValue value2 in values)
            {
                if (cacheIsMoreFresh || value2.IsDirty)
                {
                    dictionary.Add(value2.Property.Name, value2.PropertyValue);
                }
            }
            CookieContainer cookies = null;
            IIdentity identity = Thread.CurrentPrincipal.Identity;
            if (identity is ClientFormsIdentity)
            {
                cookies = ((ClientFormsIdentity) identity).AuthenticationCookies;
            }
            if (_UsingWFCService)
            {
                throw new NotImplementedException();
            }
            string[] paramNames = new string[] { "values", "authenticatedUserOnly" };
            object[] paramValues = new object[] { dictionary, !identity.IsAuthenticated ? ((object) 0) : ((object) (identity is ClientFormsIdentity)) };
            collection = (Collection<string>) ProxyHelper.CreateWebRequestAndGetResponse(this.GetServiceUri() + "/SetPropertiesForCurrentUser", ref cookies, identity.Name, this._ConnectionString, this._ConnectionStringProvider, paramNames, paramValues, typeof(Collection<string>));
            this.SetIsCacheMoreFresh(false);
            return collection;
        }

        private void SetRemainingValuesToDefault()
        {
            foreach (SettingsProperty property in this._Properties)
            {
                if (this._PropertyValues[property.Name] == null)
                {
                    SettingsPropertyValue value2 = new SettingsPropertyValue(property) {
                        SerializedValue = property.DefaultValue,
                        Deserialized = false
                    };
                    object propertyValue = value2.PropertyValue;
                    value2.PropertyValue = propertyValue;
                    this._PropertyValues.Add(value2);
                }
            }
        }

        private void SetTagValue(string tagName, string tagValue)
        {
            using (DbConnection connection = SqlHelper.GetConnection(Thread.CurrentPrincipal.Identity.Name, this.GetConnectionString(), this._ConnectionStringProvider))
            {
                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = "DELETE FROM Settings WHERE PropertyName = @PropName AND PropertyStoredAs='I'";
                SqlHelper.AddParameter(connection, cmd, "@PropName", tagName);
                cmd.ExecuteNonQuery();
                if (tagValue != null)
                {
                    cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO Settings (PropertyName, PropertyStoredAs, PropertyValue) VALUES  (@PropName, 'I', @PropValue)";
                    SqlHelper.AddParameter(connection, cmd, "@PropName", tagName);
                    SqlHelper.AddParameter(connection, cmd, "@PropValue", tagValue);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void StoreKnownTypes(SettingsPropertyCollection propertyCollection)
        {
            if (_KnownTypesHashtable == null)
            {
                InitKnownTypes();
            }
            ArrayList list = null;
            foreach (SettingsProperty property in propertyCollection)
            {
                if (!_KnownTypesHashtable.Contains(property.PropertyType))
                {
                    _KnownTypesHashtable.Add(property.PropertyType, string.Empty);
                    if (list == null)
                    {
                        list = new ArrayList();
                    }
                    list.Add(property.PropertyType);
                }
            }
            if (list != null)
            {
                Type[] array = new Type[_KnownTypesArray.Length + list.Count];
                _KnownTypesArray.CopyTo(array, 0);
                list.CopyTo(array, _KnownTypesArray.Length);
                _KnownTypesArray = array;
            }
        }

        private static void StoreKnownTypes(SettingsPropertyValueCollection propertyValueCollection)
        {
            if (_KnownTypesHashtable == null)
            {
                InitKnownTypes();
            }
            ArrayList list = null;
            foreach (SettingsPropertyValue value2 in propertyValueCollection)
            {
                if (!_KnownTypesHashtable.Contains(value2.Property.PropertyType))
                {
                    _KnownTypesHashtable.Add(value2.Property.PropertyType, string.Empty);
                    if (list == null)
                    {
                        list = new ArrayList();
                    }
                    list.Add(value2.Property.PropertyType);
                }
            }
            if (list != null)
            {
                Type[] array = new Type[_KnownTypesArray.Length + list.Count];
                _KnownTypesArray.CopyTo(array, 0);
                list.CopyTo(array, _KnownTypesArray.Length);
                _KnownTypesArray = array;
            }
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
        }

        public override string ApplicationName
        {
            get => 
                "";
            set
            {
            }
        }

        public static string ServiceUri
        {
            get => 
                _ServiceUri;
            set
            {
                _ServiceUri = value;
                if (string.IsNullOrEmpty(_ServiceUri))
                {
                    _UsingWFCService = false;
                }
                else
                {
                    _UsingWFCService = _ServiceUri.EndsWith(".svc", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}

