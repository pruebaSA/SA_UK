namespace System.Data.OracleClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Data.Common;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [DefaultProperty("DataSource"), TypeConverter(typeof(OracleConnectionStringBuilder.OracleConnectionStringBuilderConverter))]
    public sealed class OracleConnectionStringBuilder : DbConnectionStringBuilder
    {
        private string _dataSource;
        private bool _enlist;
        private bool _integratedSecurity;
        private static readonly Dictionary<string, Keywords> _keywords;
        private int _loadBalanceTimeout;
        private int _maxPoolSize;
        private int _minPoolSize;
        private bool _omitOracleConnectionName;
        private string _password;
        private bool _persistSecurityInfo;
        private bool _pooling;
        private bool _unicode;
        private string _userID;
        private static readonly string[] _validKeywords;

        static OracleConnectionStringBuilder()
        {
            string[] strArray = new string[12];
            strArray[0] = "Data Source";
            strArray[5] = "Enlist";
            strArray[2] = "Integrated Security";
            strArray[10] = "Load Balance Timeout";
            strArray[8] = "Max Pool Size";
            strArray[7] = "Min Pool Size";
            strArray[4] = "Password";
            strArray[1] = "Persist Security Info";
            strArray[6] = "Pooling";
            strArray[9] = "Unicode";
            strArray[3] = "User ID";
            strArray[11] = "Omit Oracle Connection Name";
            _validKeywords = strArray;
            Dictionary<string, Keywords> dictionary = new Dictionary<string, Keywords>(0x13, StringComparer.OrdinalIgnoreCase) {
                { 
                    "Data Source",
                    Keywords.DataSource
                },
                { 
                    "Enlist",
                    Keywords.Enlist
                },
                { 
                    "Integrated Security",
                    Keywords.IntegratedSecurity
                },
                { 
                    "Load Balance Timeout",
                    Keywords.LoadBalanceTimeout
                },
                { 
                    "Max Pool Size",
                    Keywords.MaxPoolSize
                },
                { 
                    "Min Pool Size",
                    Keywords.MinPoolSize
                },
                { 
                    "Omit Oracle Connection Name",
                    Keywords.OmitOracleConnectionName
                },
                { 
                    "Password",
                    Keywords.Password
                },
                { 
                    "Persist Security Info",
                    Keywords.PersistSecurityInfo
                },
                { 
                    "Pooling",
                    Keywords.Pooling
                },
                { 
                    "Unicode",
                    Keywords.Unicode
                },
                { 
                    "User ID",
                    Keywords.UserID
                },
                { 
                    "server",
                    Keywords.DataSource
                },
                { 
                    "connection lifetime",
                    Keywords.LoadBalanceTimeout
                },
                { 
                    "pwd",
                    Keywords.Password
                },
                { 
                    "persistsecurityinfo",
                    Keywords.PersistSecurityInfo
                },
                { 
                    "uid",
                    Keywords.UserID
                },
                { 
                    "user",
                    Keywords.UserID
                },
                { 
                    "Workaround Oracle Bug 914652",
                    Keywords.OmitOracleConnectionName
                }
            };
            _keywords = dictionary;
        }

        public OracleConnectionStringBuilder() : this(null)
        {
        }

        public OracleConnectionStringBuilder(string connectionString)
        {
            this._dataSource = "";
            this._password = "";
            this._userID = "";
            this._maxPoolSize = 100;
            this._enlist = true;
            this._pooling = true;
            if (!System.Data.Common.ADP.IsEmpty(connectionString))
            {
                base.ConnectionString = connectionString;
            }
        }

        public override void Clear()
        {
            base.Clear();
            for (int i = 0; i < _validKeywords.Length; i++)
            {
                this.Reset((Keywords) i);
            }
        }

        internal void ClearPropertyDescriptors()
        {
            base.ClearPropertyDescriptors();
        }

        public override bool ContainsKey(string keyword)
        {
            System.Data.Common.ADP.CheckArgumentNull(keyword, "keyword");
            return _keywords.ContainsKey(keyword);
        }

        private static bool ConvertToBoolean(object value) => 
            System.Data.Common.DbConnectionStringBuilderUtil.ConvertToBoolean(value);

        private static int ConvertToInt32(object value) => 
            System.Data.Common.DbConnectionStringBuilderUtil.ConvertToInt32(value);

        private static bool ConvertToIntegratedSecurity(object value) => 
            System.Data.Common.DbConnectionStringBuilderUtil.ConvertToIntegratedSecurity(value);

        private static string ConvertToString(object value) => 
            System.Data.Common.DbConnectionStringBuilderUtil.ConvertToString(value);

        private object GetAt(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    return this.DataSource;

                case Keywords.PersistSecurityInfo:
                    return this.PersistSecurityInfo;

                case Keywords.IntegratedSecurity:
                    return this.IntegratedSecurity;

                case Keywords.UserID:
                    return this.UserID;

                case Keywords.Password:
                    return this.Password;

                case Keywords.Enlist:
                    return this.Enlist;

                case Keywords.Pooling:
                    return this.Pooling;

                case Keywords.MinPoolSize:
                    return this.MinPoolSize;

                case Keywords.MaxPoolSize:
                    return this.MaxPoolSize;

                case Keywords.Unicode:
                    return this.Unicode;

                case Keywords.LoadBalanceTimeout:
                    return this.LoadBalanceTimeout;

                case Keywords.OmitOracleConnectionName:
                    return this.OmitOracleConnectionName;
            }
            throw System.Data.Common.ADP.KeywordNotSupported(_validKeywords[(int) index]);
        }

        private Attribute[] GetAttributesFromCollection(AttributeCollection collection)
        {
            Attribute[] array = new Attribute[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

        private Keywords GetIndex(string keyword)
        {
            Keywords keywords;
            System.Data.Common.ADP.CheckArgumentNull(keyword, "keyword");
            if (!_keywords.TryGetValue(keyword, out keywords))
            {
                throw System.Data.Common.ADP.KeywordNotSupported(keyword);
            }
            return keywords;
        }

        protected override void GetProperties(Hashtable propertyDescriptors)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this, true))
            {
                bool flag2 = false;
                bool isReadOnly = false;
                string displayName = descriptor.DisplayName;
                if ("Integrated Security" == displayName)
                {
                    flag2 = true;
                    isReadOnly = descriptor.IsReadOnly;
                }
                else
                {
                    if (("Password" != displayName) && ("User ID" != displayName))
                    {
                        continue;
                    }
                    isReadOnly = this.IntegratedSecurity;
                }
                Attribute[] attributesFromCollection = this.GetAttributesFromCollection(descriptor.Attributes);
                System.Data.Common.DbConnectionStringBuilderDescriptor descriptor2 = new System.Data.Common.DbConnectionStringBuilderDescriptor(descriptor.Name, descriptor.ComponentType, descriptor.PropertyType, isReadOnly, attributesFromCollection) {
                    RefreshOnChange = flag2
                };
                propertyDescriptors[displayName] = descriptor2;
            }
            base.GetProperties(propertyDescriptors);
        }

        public override bool Remove(string keyword)
        {
            Keywords keywords;
            System.Data.Common.ADP.CheckArgumentNull(keyword, "keyword");
            if (_keywords.TryGetValue(keyword, out keywords))
            {
                base.Remove(_validKeywords[(int) keywords]);
                this.Reset(keywords);
                return true;
            }
            return false;
        }

        private void Reset(Keywords index)
        {
            switch (index)
            {
                case Keywords.DataSource:
                    this._dataSource = "";
                    return;

                case Keywords.PersistSecurityInfo:
                    this._persistSecurityInfo = false;
                    return;

                case Keywords.IntegratedSecurity:
                    this._integratedSecurity = false;
                    return;

                case Keywords.UserID:
                    this._userID = "";
                    return;

                case Keywords.Password:
                    this._password = "";
                    return;

                case Keywords.Enlist:
                    this._enlist = true;
                    return;

                case Keywords.Pooling:
                    this._pooling = true;
                    return;

                case Keywords.MinPoolSize:
                    this._minPoolSize = 0;
                    return;

                case Keywords.MaxPoolSize:
                    this._maxPoolSize = 100;
                    return;

                case Keywords.Unicode:
                    this._unicode = false;
                    return;

                case Keywords.LoadBalanceTimeout:
                    this._loadBalanceTimeout = 0;
                    return;

                case Keywords.OmitOracleConnectionName:
                    this._omitOracleConnectionName = false;
                    return;
            }
            throw System.Data.Common.ADP.KeywordNotSupported(_validKeywords[(int) index]);
        }

        private void SetValue(string keyword, bool value)
        {
            base[keyword] = value.ToString(null);
        }

        private void SetValue(string keyword, int value)
        {
            base[keyword] = value.ToString((IFormatProvider) null);
        }

        private void SetValue(string keyword, string value)
        {
            System.Data.Common.ADP.CheckArgumentNull(value, keyword);
            base[keyword] = value;
        }

        public override bool ShouldSerialize(string keyword)
        {
            Keywords keywords;
            System.Data.Common.ADP.CheckArgumentNull(keyword, "keyword");
            return (_keywords.TryGetValue(keyword, out keywords) && base.ShouldSerialize(_validKeywords[(int) keywords]));
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            Keywords keywords;
            if (_keywords.TryGetValue(keyword, out keywords))
            {
                value = this.GetAt(keywords);
                return true;
            }
            value = null;
            return false;
        }

        [RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Source"), ResDescription("DbConnectionString_DataSource"), DisplayName("Data Source")]
        public string DataSource
        {
            get => 
                this._dataSource;
            set
            {
                if ((value != null) && (0x80 < value.Length))
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionLength("Data Source", 0x80);
                }
                this.SetValue("Data Source", value);
                this._dataSource = value;
            }
        }

        [ResCategory("DataCategory_Pooling"), DisplayName("Enlist"), RefreshProperties(RefreshProperties.All), ResDescription("DbConnectionString_Enlist")]
        public bool Enlist
        {
            get => 
                this._enlist;
            set
            {
                this.SetValue("Enlist", value);
                this._enlist = value;
            }
        }

        [ResCategory("DataCategory_Security"), ResDescription("DbConnectionString_IntegratedSecurity"), RefreshProperties(RefreshProperties.All), DisplayName("Integrated Security")]
        public bool IntegratedSecurity
        {
            get => 
                this._integratedSecurity;
            set
            {
                this.SetValue("Integrated Security", value);
                this._integratedSecurity = value;
            }
        }

        public override bool IsFixedSize =>
            true;

        public override object this[string keyword]
        {
            get
            {
                Keywords index = this.GetIndex(keyword);
                return this.GetAt(index);
            }
            set
            {
                Bid.Trace("<comm.OracleConnectionStringBuilder.set_Item|API> keyword='%ls'\n", keyword);
                if (value != null)
                {
                    switch (this.GetIndex(keyword))
                    {
                        case Keywords.DataSource:
                            this.DataSource = ConvertToString(value);
                            return;

                        case Keywords.PersistSecurityInfo:
                            this.PersistSecurityInfo = ConvertToBoolean(value);
                            return;

                        case Keywords.IntegratedSecurity:
                            this.IntegratedSecurity = ConvertToIntegratedSecurity(value);
                            return;

                        case Keywords.UserID:
                            this.UserID = ConvertToString(value);
                            return;

                        case Keywords.Password:
                            this.Password = ConvertToString(value);
                            return;

                        case Keywords.Enlist:
                            this.Enlist = ConvertToBoolean(value);
                            return;

                        case Keywords.Pooling:
                            this.Pooling = ConvertToBoolean(value);
                            return;

                        case Keywords.MinPoolSize:
                            this.MinPoolSize = ConvertToInt32(value);
                            return;

                        case Keywords.MaxPoolSize:
                            this.MaxPoolSize = ConvertToInt32(value);
                            return;

                        case Keywords.Unicode:
                            this.Unicode = ConvertToBoolean(value);
                            return;

                        case Keywords.LoadBalanceTimeout:
                            this.LoadBalanceTimeout = ConvertToInt32(value);
                            return;

                        case Keywords.OmitOracleConnectionName:
                            this.OmitOracleConnectionName = ConvertToBoolean(value);
                            return;
                    }
                    throw System.Data.Common.ADP.KeywordNotSupported(keyword);
                }
                this.Remove(keyword);
            }
        }

        public override ICollection Keys =>
            new System.Data.Common.ReadOnlyCollection<string>(_validKeywords);

        [DisplayName("Load Balance Timeout"), ResDescription("DbConnectionString_LoadBalanceTimeout"), RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Pooling")]
        public int LoadBalanceTimeout
        {
            get => 
                this._loadBalanceTimeout;
            set
            {
                if (value < 0)
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionValue("Load Balance Timeout");
                }
                this.SetValue("Load Balance Timeout", value);
                this._loadBalanceTimeout = value;
            }
        }

        [DisplayName("Max Pool Size"), RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Pooling"), ResDescription("DbConnectionString_MaxPoolSize")]
        public int MaxPoolSize
        {
            get => 
                this._maxPoolSize;
            set
            {
                if (value < 1)
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionValue("Max Pool Size");
                }
                this.SetValue("Max Pool Size", value);
                this._maxPoolSize = value;
            }
        }

        [DisplayName("Min Pool Size"), ResCategory("DataCategory_Pooling"), RefreshProperties(RefreshProperties.All), ResDescription("DbConnectionString_MinPoolSize")]
        public int MinPoolSize
        {
            get => 
                this._minPoolSize;
            set
            {
                if (value < 0)
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionValue("Min Pool Size");
                }
                this.SetValue("Min Pool Size", value);
                this._minPoolSize = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), ResDescription("DbConnectionString_OmitOracleConnectionName"), DisplayName("Omit Oracle Connection Name"), ResCategory("DataCategory_Initialization")]
        public bool OmitOracleConnectionName
        {
            get => 
                this._omitOracleConnectionName;
            set
            {
                this.SetValue("Omit Oracle Connection Name", value);
                this._omitOracleConnectionName = value;
            }
        }

        [PasswordPropertyText(true), DisplayName("Password"), ResCategory("DataCategory_Security"), ResDescription("DbConnectionString_Password"), RefreshProperties(RefreshProperties.All)]
        public string Password
        {
            get => 
                this._password;
            set
            {
                if ((value != null) && (30 < value.Length))
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionLength("Password", 30);
                }
                this.SetValue("Password", value);
                this._password = value;
            }
        }

        [ResDescription("DbConnectionString_PersistSecurityInfo"), RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Security"), DisplayName("Persist Security Info")]
        public bool PersistSecurityInfo
        {
            get => 
                this._persistSecurityInfo;
            set
            {
                this.SetValue("Persist Security Info", value);
                this._persistSecurityInfo = value;
            }
        }

        [RefreshProperties(RefreshProperties.All), DisplayName("Pooling"), ResCategory("DataCategory_Pooling"), ResDescription("DbConnectionString_Pooling")]
        public bool Pooling
        {
            get => 
                this._pooling;
            set
            {
                this.SetValue("Pooling", value);
                this._pooling = value;
            }
        }

        [DisplayName("Unicode"), ResDescription("DbConnectionString_Unicode"), ResCategory("DataCategory_Initialization"), RefreshProperties(RefreshProperties.All)]
        public bool Unicode
        {
            get => 
                this._unicode;
            set
            {
                this.SetValue("Unicode", value);
                this._unicode = value;
            }
        }

        [DisplayName("User ID"), ResDescription("DbConnectionString_UserID"), RefreshProperties(RefreshProperties.All), ResCategory("DataCategory_Security")]
        public string UserID
        {
            get => 
                this._userID;
            set
            {
                if ((value != null) && (30 < value.Length))
                {
                    throw System.Data.Common.ADP.InvalidConnectionOptionLength("User ID", 30);
                }
                this.SetValue("User ID", value);
                this._userID = value;
            }
        }

        public override ICollection Values
        {
            get
            {
                object[] items = new object[_validKeywords.Length];
                for (int i = 0; i < _validKeywords.Length; i++)
                {
                    items[i] = this.GetAt((Keywords) i);
                }
                return new System.Data.Common.ReadOnlyCollection<object>(items);
            }
        }

        private enum Keywords
        {
            DataSource,
            PersistSecurityInfo,
            IntegratedSecurity,
            UserID,
            Password,
            Enlist,
            Pooling,
            MinPoolSize,
            MaxPoolSize,
            Unicode,
            LoadBalanceTimeout,
            OmitOracleConnectionName
        }

        internal sealed class OracleConnectionStringBuilderConverter : ExpandableObjectConverter
        {
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
                ((typeof(InstanceDescriptor) == destinationType) || base.CanConvertTo(context, destinationType));

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                {
                    throw System.Data.Common.ADP.ArgumentNull("destinationType");
                }
                if (typeof(InstanceDescriptor) == destinationType)
                {
                    OracleConnectionStringBuilder options = value as OracleConnectionStringBuilder;
                    if (options != null)
                    {
                        return this.ConvertToInstanceDescriptor(options);
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }

            private InstanceDescriptor ConvertToInstanceDescriptor(OracleConnectionStringBuilder options)
            {
                Type[] types = new Type[] { typeof(string) };
                return new InstanceDescriptor(typeof(OracleConnectionStringBuilder).GetConstructor(types), new object[] { options.ConnectionString });
            }
        }
    }
}

