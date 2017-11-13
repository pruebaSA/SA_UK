namespace System.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration.Internal;
    using System.Diagnostics;

    [DebuggerDisplay("FactoryRecord {ConfigKey}")]
    internal class FactoryRecord : IConfigErrorInfo
    {
        private ConfigurationAllowDefinition _allowDefinition;
        private ConfigurationAllowExeDefinition _allowExeDefinition;
        private string _configKey;
        private List<ConfigurationException> _errors;
        private object _factory;
        private string _factoryTypeName;
        private string _filename;
        private SimpleBitVector32 _flags;
        private string _group;
        private int _lineNumber;
        private string _name;
        private OverrideModeSetting _overrideModeDefault;
        private const int Flag_AllowLocation = 1;
        private const int Flag_IsFactoryTrustedWithoutAptca = 0x20;
        private const int Flag_IsFromTrustedConfigRecord = 0x10;
        private const int Flag_IsGroup = 8;
        private const int Flag_IsUndeclared = 0x40;
        private const int Flag_RequirePermission = 4;
        private const int Flag_RestartOnExternalChanges = 2;

        internal FactoryRecord(string configKey, string group, string name, string factoryTypeName, string filename, int lineNumber)
        {
            this._configKey = configKey;
            this._group = group;
            this._name = name;
            this._factoryTypeName = factoryTypeName;
            this.IsGroup = true;
            this._filename = filename;
            this._lineNumber = lineNumber;
        }

        private FactoryRecord(string configKey, string group, string name, object factory, string factoryTypeName, SimpleBitVector32 flags, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, OverrideModeSetting overrideModeDefault, string filename, int lineNumber, ICollection<ConfigurationException> errors)
        {
            this._configKey = configKey;
            this._group = group;
            this._name = name;
            this._factory = factory;
            this._factoryTypeName = factoryTypeName;
            this._flags = flags;
            this._allowDefinition = allowDefinition;
            this._allowExeDefinition = allowExeDefinition;
            this._overrideModeDefault = overrideModeDefault;
            this._filename = filename;
            this._lineNumber = lineNumber;
            this.AddErrors(errors);
        }

        internal FactoryRecord(string configKey, string group, string name, string factoryTypeName, bool allowLocation, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, OverrideModeSetting overrideModeDefault, bool restartOnExternalChanges, bool requirePermission, bool isFromTrustedConfigRecord, bool isUndeclared, string filename, int lineNumber)
        {
            this._configKey = configKey;
            this._group = group;
            this._name = name;
            this._factoryTypeName = factoryTypeName;
            this._allowDefinition = allowDefinition;
            this._allowExeDefinition = allowExeDefinition;
            this._overrideModeDefault = overrideModeDefault;
            this.AllowLocation = allowLocation;
            this.RestartOnExternalChanges = restartOnExternalChanges;
            this.RequirePermission = requirePermission;
            this.IsFromTrustedConfigRecord = isFromTrustedConfigRecord;
            this.IsUndeclared = isUndeclared;
            this._filename = filename;
            this._lineNumber = lineNumber;
        }

        internal void AddErrors(ICollection<ConfigurationException> coll)
        {
            ErrorsHelper.AddErrors(ref this._errors, coll);
        }

        internal FactoryRecord CloneSection(string filename, int lineNumber) => 
            new FactoryRecord(this._configKey, this._group, this._name, this._factory, this._factoryTypeName, this._flags, this._allowDefinition, this._allowExeDefinition, this._overrideModeDefault, filename, lineNumber, this.Errors);

        internal FactoryRecord CloneSectionGroup(string factoryTypeName, string filename, int lineNumber)
        {
            if (this._factoryTypeName != null)
            {
                factoryTypeName = this._factoryTypeName;
            }
            return new FactoryRecord(this._configKey, this._group, this._name, this._factory, factoryTypeName, this._flags, this._allowDefinition, this._allowExeDefinition, this._overrideModeDefault, filename, lineNumber, this.Errors);
        }

        internal bool IsEquivalentSectionFactory(IInternalConfigHost host, string typeName, bool allowLocation, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, bool restartOnExternalChanges, bool requirePermission) => 
            ((((allowLocation == this.AllowLocation) && (allowDefinition == this.AllowDefinition)) && (((allowExeDefinition == this.AllowExeDefinition) && (restartOnExternalChanges == this.RestartOnExternalChanges)) && (requirePermission == this.RequirePermission))) && this.IsEquivalentType(host, typeName));

        internal bool IsEquivalentSectionGroupFactory(IInternalConfigHost host, string typeName)
        {
            if ((typeName != null) && (this._factoryTypeName != null))
            {
                return this.IsEquivalentType(host, typeName);
            }
            return true;
        }

        internal bool IsEquivalentType(IInternalConfigHost host, string typeName)
        {
            try
            {
                Type typeWithReflectionPermission;
                Type type2;
                if (this._factoryTypeName == typeName)
                {
                    return true;
                }
                if (host != null)
                {
                    typeWithReflectionPermission = System.Configuration.TypeUtil.GetTypeWithReflectionPermission(host, typeName, false);
                    type2 = System.Configuration.TypeUtil.GetTypeWithReflectionPermission(host, this._factoryTypeName, false);
                }
                else
                {
                    typeWithReflectionPermission = System.Configuration.TypeUtil.GetTypeWithReflectionPermission(typeName, false);
                    type2 = System.Configuration.TypeUtil.GetTypeWithReflectionPermission(this._factoryTypeName, false);
                }
                return ((typeWithReflectionPermission != null) && (typeWithReflectionPermission == type2));
            }
            catch
            {
            }
            return false;
        }

        internal bool IsIgnorable()
        {
            if (this._factory != null)
            {
                return (this._factory is IgnoreSectionHandler);
            }
            return ((this._factoryTypeName != null) && this._factoryTypeName.Contains("System.Configuration.IgnoreSection"));
        }

        internal void ThrowOnErrors()
        {
            ErrorsHelper.ThrowOnErrors(this._errors);
        }

        internal ConfigurationAllowDefinition AllowDefinition
        {
            get => 
                this._allowDefinition;
            set
            {
                this._allowDefinition = value;
            }
        }

        internal ConfigurationAllowExeDefinition AllowExeDefinition
        {
            get => 
                this._allowExeDefinition;
            set
            {
                this._allowExeDefinition = value;
            }
        }

        internal bool AllowLocation
        {
            get => 
                this._flags[1];
            set
            {
                this._flags[1] = value;
            }
        }

        internal string ConfigKey =>
            this._configKey;

        internal List<ConfigurationException> Errors =>
            this._errors;

        internal object Factory
        {
            get => 
                this._factory;
            set
            {
                this._factory = value;
            }
        }

        internal string FactoryTypeName
        {
            get => 
                this._factoryTypeName;
            set
            {
                this._factoryTypeName = value;
            }
        }

        public string Filename
        {
            get => 
                this._filename;
            set
            {
                this._filename = value;
            }
        }

        internal string Group =>
            this._group;

        internal bool HasErrors =>
            ErrorsHelper.GetHasErrors(this._errors);

        internal bool HasFile =>
            (this._lineNumber >= 0);

        internal bool IsFactoryTrustedWithoutAptca
        {
            get => 
                this._flags[0x20];
            set
            {
                this._flags[0x20] = value;
            }
        }

        internal bool IsFromTrustedConfigRecord
        {
            get => 
                this._flags[0x10];
            set
            {
                this._flags[0x10] = value;
            }
        }

        internal bool IsGroup
        {
            get => 
                this._flags[8];
            set
            {
                this._flags[8] = value;
            }
        }

        internal bool IsUndeclared
        {
            get => 
                this._flags[0x40];
            set
            {
                this._flags[0x40] = value;
            }
        }

        public int LineNumber
        {
            get => 
                this._lineNumber;
            set
            {
                this._lineNumber = value;
            }
        }

        internal string Name =>
            this._name;

        internal OverrideModeSetting OverrideModeDefault =>
            this._overrideModeDefault;

        internal bool RequirePermission
        {
            get => 
                this._flags[4];
            set
            {
                this._flags[4] = value;
            }
        }

        internal bool RestartOnExternalChanges
        {
            get => 
                this._flags[2];
            set
            {
                this._flags[2] = value;
            }
        }
    }
}

