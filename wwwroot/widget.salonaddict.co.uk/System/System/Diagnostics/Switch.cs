namespace System.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.Xml.Serialization;

    public abstract class Switch
    {
        private StringDictionary attributes;
        private string defaultValue;
        private string description;
        private string displayName;
        private bool initialized;
        private bool initializing;
        private static List<WeakReference> switches = new List<WeakReference>();
        private int switchSetting;
        private SwitchElementsCollection switchSettings;
        private string switchValueString;

        protected Switch(string displayName, string description) : this(displayName, description, "0")
        {
        }

        protected Switch(string displayName, string description, string defaultSwitchValue)
        {
            this.switchValueString = string.Empty;
            if (displayName == null)
            {
                displayName = string.Empty;
            }
            this.displayName = displayName;
            this.description = description;
            lock (switches)
            {
                switches.Add(new WeakReference(this));
            }
            this.defaultValue = defaultSwitchValue;
        }

        protected internal virtual string[] GetSupportedAttributes() => 
            null;

        private void Initialize()
        {
            this.InitializeWithStatus();
        }

        private bool InitializeConfigSettings()
        {
            if (this.switchSettings == null)
            {
                if (!DiagnosticsConfiguration.CanInitialize())
                {
                    return false;
                }
                this.switchSettings = DiagnosticsConfiguration.SwitchSettings;
            }
            return true;
        }

        private bool InitializeWithStatus()
        {
            if (!this.initialized)
            {
                if (this.initializing)
                {
                    return false;
                }
                this.initializing = true;
                if ((this.switchSettings == null) && !this.InitializeConfigSettings())
                {
                    return false;
                }
                if (this.switchSettings != null)
                {
                    SwitchElement element = this.switchSettings[this.displayName];
                    if (element != null)
                    {
                        string str = element.Value;
                        if (str != null)
                        {
                            this.Value = str;
                        }
                        else
                        {
                            this.Value = this.defaultValue;
                        }
                        try
                        {
                            TraceUtils.VerifyAttributes(element.Attributes, this.GetSupportedAttributes(), this);
                        }
                        catch (ConfigurationException)
                        {
                            this.initialized = false;
                            this.initializing = false;
                            throw;
                        }
                        this.attributes = new StringDictionary();
                        this.attributes.contents = element.Attributes;
                    }
                    else
                    {
                        this.switchValueString = this.defaultValue;
                        this.OnValueChanged();
                    }
                }
                else
                {
                    this.switchValueString = this.defaultValue;
                    this.OnValueChanged();
                }
                this.initialized = true;
                this.initializing = false;
            }
            return true;
        }

        protected virtual void OnSwitchSettingChanged()
        {
        }

        protected virtual void OnValueChanged()
        {
            this.SwitchSetting = int.Parse(this.Value, CultureInfo.InvariantCulture);
        }

        internal void Refresh()
        {
            this.initialized = false;
            this.switchSettings = null;
            this.Initialize();
        }

        internal static void RefreshAll()
        {
            lock (switches)
            {
                for (int i = 0; i < switches.Count; i++)
                {
                    Switch target = (Switch) switches[i].Target;
                    if (target != null)
                    {
                        target.Refresh();
                    }
                }
            }
        }

        [XmlIgnore]
        public StringDictionary Attributes
        {
            get
            {
                this.Initialize();
                if (this.attributes == null)
                {
                    this.attributes = new StringDictionary();
                }
                return this.attributes;
            }
        }

        public string Description
        {
            get
            {
                if (this.description != null)
                {
                    return this.description;
                }
                return string.Empty;
            }
        }

        public string DisplayName =>
            this.displayName;

        protected int SwitchSetting
        {
            get
            {
                if (!this.initialized)
                {
                    if (!this.InitializeWithStatus())
                    {
                        return 0;
                    }
                    this.OnSwitchSettingChanged();
                }
                return this.switchSetting;
            }
            set
            {
                this.initialized = true;
                if (this.switchSetting != value)
                {
                    this.switchSetting = value;
                    this.OnSwitchSettingChanged();
                }
            }
        }

        protected string Value
        {
            get
            {
                this.Initialize();
                return this.switchValueString;
            }
            set
            {
                this.Initialize();
                this.switchValueString = value;
                try
                {
                    this.OnValueChanged();
                }
                catch (ArgumentException exception)
                {
                    throw new ConfigurationErrorsException(System.SR.GetString("BadConfigSwitchValue", new object[] { this.DisplayName }), exception);
                }
                catch (FormatException exception2)
                {
                    throw new ConfigurationErrorsException(System.SR.GetString("BadConfigSwitchValue", new object[] { this.DisplayName }), exception2);
                }
                catch (OverflowException exception3)
                {
                    throw new ConfigurationErrorsException(System.SR.GetString("BadConfigSwitchValue", new object[] { this.DisplayName }), exception3);
                }
            }
        }
    }
}

