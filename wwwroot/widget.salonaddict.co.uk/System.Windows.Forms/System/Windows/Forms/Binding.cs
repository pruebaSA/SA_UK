﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;

    [TypeConverter(typeof(ListBindingConverter))]
    public class Binding
    {
        private System.Windows.Forms.BindingManagerBase bindingManagerBase;
        private System.Windows.Forms.BindToObject bindToObject;
        private bool bound;
        private IBindableComponent control;
        private System.Windows.Forms.ControlUpdateMode controlUpdateMode;
        private System.Windows.Forms.DataSourceUpdateMode dataSourceUpdateMode;
        private object dsNullValue;
        private bool dsNullValueSet;
        private IFormatProvider formatInfo;
        private string formatString;
        private bool formattingEnabled;
        private bool inOnBindingComplete;
        private bool inPushOrPull;
        private bool inSetPropValue;
        private bool modified;
        private object nullValue;
        private string propertyName;
        private PropertyDescriptor propInfo;
        private TypeConverter propInfoConverter;
        private PropertyDescriptor propIsNullInfo;
        private EventDescriptor validateInfo;

        public event BindingCompleteEventHandler BindingComplete;

        public event ConvertEventHandler Format;

        public event ConvertEventHandler Parse;

        private Binding()
        {
            this.propertyName = "";
            this.formatString = string.Empty;
            this.dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
        }

        public Binding(string propertyName, object dataSource, string dataMember) : this(propertyName, dataSource, dataMember, false, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled) : this(propertyName, dataSource, dataMember, formattingEnabled, System.Windows.Forms.DataSourceUpdateMode.OnValidation, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, System.Windows.Forms.DataSourceUpdateMode dataSourceUpdateMode) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, System.Windows.Forms.DataSourceUpdateMode dataSourceUpdateMode, object nullValue) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, System.Windows.Forms.DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, System.Windows.Forms.DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
        {
            this.propertyName = "";
            this.formatString = string.Empty;
            this.dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
            this.bindToObject = new System.Windows.Forms.BindToObject(this, dataSource, dataMember);
            this.propertyName = propertyName;
            this.formattingEnabled = formattingEnabled;
            this.formatString = formatString;
            this.nullValue = nullValue;
            this.formatInfo = formatInfo;
            this.formattingEnabled = formattingEnabled;
            this.dataSourceUpdateMode = dataSourceUpdateMode;
            this.CheckBinding();
        }

        private void binding_MetaDataChanged(object sender, EventArgs e)
        {
            this.CheckBinding();
        }

        private void BindTarget(bool bind)
        {
            if (bind)
            {
                if (this.IsBinding)
                {
                    if ((this.propInfo != null) && (this.control != null))
                    {
                        EventHandler handler = new EventHandler(this.Target_PropertyChanged);
                        this.propInfo.AddValueChanged(this.control, handler);
                    }
                    if (this.validateInfo != null)
                    {
                        CancelEventHandler handler2 = new CancelEventHandler(this.Target_Validate);
                        this.validateInfo.AddEventHandler(this.control, handler2);
                    }
                }
            }
            else
            {
                if ((this.propInfo != null) && (this.control != null))
                {
                    EventHandler handler3 = new EventHandler(this.Target_PropertyChanged);
                    this.propInfo.RemoveValueChanged(this.control, handler3);
                }
                if (this.validateInfo != null)
                {
                    CancelEventHandler handler4 = new CancelEventHandler(this.Target_Validate);
                    this.validateInfo.RemoveEventHandler(this.control, handler4);
                }
            }
        }

        private void CheckBinding()
        {
            this.bindToObject.CheckBinding();
            if ((this.control == null) || (this.propertyName.Length <= 0))
            {
                this.propInfo = null;
                this.validateInfo = null;
            }
            else
            {
                PropertyDescriptorCollection properties;
                this.control.DataBindings.CheckDuplicates(this);
                System.Type componentType = this.control.GetType();
                string b = this.propertyName + "IsNull";
                PropertyDescriptor descriptor = null;
                PropertyDescriptor descriptor2 = null;
                InheritanceAttribute attribute = (InheritanceAttribute) TypeDescriptor.GetAttributes(this.control)[typeof(InheritanceAttribute)];
                if ((attribute != null) && (attribute.InheritanceLevel != InheritanceLevel.NotInherited))
                {
                    properties = TypeDescriptor.GetProperties(componentType);
                }
                else
                {
                    properties = TypeDescriptor.GetProperties(this.control);
                }
                for (int i = 0; i < properties.Count; i++)
                {
                    if ((descriptor == null) && string.Equals(properties[i].Name, this.propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        descriptor = properties[i];
                        if (descriptor2 != null)
                        {
                            break;
                        }
                    }
                    if ((descriptor2 == null) && string.Equals(properties[i].Name, b, StringComparison.OrdinalIgnoreCase))
                    {
                        descriptor2 = properties[i];
                        if (descriptor != null)
                        {
                            break;
                        }
                    }
                }
                if (descriptor == null)
                {
                    throw new ArgumentException(System.Windows.Forms.SR.GetString("ListBindingBindProperty", new object[] { this.propertyName }), "PropertyName");
                }
                if (descriptor.IsReadOnly && (this.controlUpdateMode != System.Windows.Forms.ControlUpdateMode.Never))
                {
                    throw new ArgumentException(System.Windows.Forms.SR.GetString("ListBindingBindPropertyReadOnly", new object[] { this.propertyName }), "PropertyName");
                }
                this.propInfo = descriptor;
                System.Type propertyType = this.propInfo.PropertyType;
                this.propInfoConverter = this.propInfo.Converter;
                if (((descriptor2 != null) && (descriptor2.PropertyType == typeof(bool))) && !descriptor2.IsReadOnly)
                {
                    this.propIsNullInfo = descriptor2;
                }
                EventDescriptor descriptor3 = null;
                string str2 = "Validating";
                EventDescriptorCollection events = TypeDescriptor.GetEvents(this.control);
                for (int j = 0; j < events.Count; j++)
                {
                    if ((descriptor3 == null) && string.Equals(events[j].Name, str2, StringComparison.OrdinalIgnoreCase))
                    {
                        descriptor3 = events[j];
                        break;
                    }
                }
                this.validateInfo = descriptor3;
            }
            this.UpdateIsBinding();
        }

        internal bool ControlAtDesignTime()
        {
            IComponent control = this.control;
            if (control == null)
            {
                return false;
            }
            ISite site = control.Site;
            return site?.DesignMode;
        }

        private BindingCompleteEventArgs CreateBindingCompleteEventArgs(BindingCompleteContext context, Exception ex)
        {
            bool cancel = false;
            string message = string.Empty;
            BindingCompleteState success = BindingCompleteState.Success;
            if (ex != null)
            {
                message = ex.Message;
                success = BindingCompleteState.Exception;
                cancel = true;
            }
            else
            {
                message = this.BindToObject.DataErrorText;
                if (!string.IsNullOrEmpty(message))
                {
                    success = BindingCompleteState.DataError;
                }
            }
            return new BindingCompleteEventArgs(this, success, context, message, ex, cancel);
        }

        private object FormatObject(object value)
        {
            if (this.ControlAtDesignTime())
            {
                return value;
            }
            System.Type propertyType = this.propInfo.PropertyType;
            if (this.formattingEnabled)
            {
                ConvertEventArgs args = new ConvertEventArgs(value, propertyType);
                this.OnFormat(args);
                if (args.Value != value)
                {
                    return args.Value;
                }
                TypeConverter sourceConverter = null;
                if (this.bindToObject.FieldInfo != null)
                {
                    sourceConverter = this.bindToObject.FieldInfo.Converter;
                }
                return Formatter.FormatObject(value, propertyType, sourceConverter, this.propInfoConverter, this.formatString, this.formatInfo, this.nullValue, this.dsNullValue);
            }
            ConvertEventArgs cevent = new ConvertEventArgs(value, propertyType);
            this.OnFormat(cevent);
            object obj2 = cevent.Value;
            if (propertyType == typeof(object))
            {
                return value;
            }
            if ((obj2 != null) && (obj2.GetType().IsSubclassOf(propertyType) || (obj2.GetType() == propertyType)))
            {
                return obj2;
            }
            TypeConverter converter2 = TypeDescriptor.GetConverter((value != null) ? value.GetType() : typeof(object));
            if ((converter2 != null) && converter2.CanConvertTo(propertyType))
            {
                return converter2.ConvertTo(value, propertyType);
            }
            if (value is IConvertible)
            {
                obj2 = Convert.ChangeType(value, propertyType, CultureInfo.CurrentCulture);
                if ((obj2 != null) && (obj2.GetType().IsSubclassOf(propertyType) || (obj2.GetType() == propertyType)))
                {
                    return obj2;
                }
            }
            throw new FormatException(System.Windows.Forms.SR.GetString("ListBindingFormatFailed"));
        }

        private void FormLoaded(object sender, EventArgs e)
        {
            this.CheckBinding();
        }

        private object GetDataSourceNullValue(System.Type type)
        {
            if (!this.dsNullValueSet)
            {
                return Formatter.GetDefaultDataSourceNullValue(type);
            }
            return this.dsNullValue;
        }

        private object GetPropValue()
        {
            bool flag = false;
            if (this.propIsNullInfo != null)
            {
                flag = (bool) this.propIsNullInfo.GetValue(this.control);
            }
            if (flag)
            {
                return this.DataSourceNullValue;
            }
            object dataSourceNullValue = this.propInfo.GetValue(this.control);
            if (dataSourceNullValue == null)
            {
                dataSourceNullValue = this.DataSourceNullValue;
            }
            return dataSourceNullValue;
        }

        internal static bool IsComponentCreated(IBindableComponent component)
        {
            System.Windows.Forms.Control control = component as System.Windows.Forms.Control;
            if (control != null)
            {
                return control.Created;
            }
            return true;
        }

        protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
        {
            if (!this.inOnBindingComplete)
            {
                try
                {
                    this.inOnBindingComplete = true;
                    if (this.onComplete != null)
                    {
                        this.onComplete(this, e);
                    }
                }
                catch (Exception exception)
                {
                    if (System.Windows.Forms.ClientUtils.IsSecurityOrCriticalException(exception))
                    {
                        throw;
                    }
                    e.Cancel = true;
                }
                catch
                {
                    e.Cancel = true;
                }
                finally
                {
                    this.inOnBindingComplete = false;
                }
            }
        }

        protected virtual void OnFormat(ConvertEventArgs cevent)
        {
            if (this.onFormat != null)
            {
                this.onFormat(this, cevent);
            }
            if (((!this.formattingEnabled && !(cevent.Value is DBNull)) && ((cevent.DesiredType != null) && !cevent.DesiredType.IsInstanceOfType(cevent.Value))) && (cevent.Value is IConvertible))
            {
                cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
            }
        }

        protected virtual void OnParse(ConvertEventArgs cevent)
        {
            if (this.onParse != null)
            {
                this.onParse(this, cevent);
            }
            if (((!this.formattingEnabled && !(cevent.Value is DBNull)) && ((cevent.Value != null) && (cevent.DesiredType != null))) && (!cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible)))
            {
                cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
            }
        }

        private object ParseObject(object value)
        {
            System.Type bindToType = this.bindToObject.BindToType;
            if (this.formattingEnabled)
            {
                ConvertEventArgs args = new ConvertEventArgs(value, bindToType);
                this.OnParse(args);
                object objB = args.Value;
                if (!object.Equals(value, objB))
                {
                    return objB;
                }
                TypeConverter targetConverter = null;
                if (this.bindToObject.FieldInfo != null)
                {
                    targetConverter = this.bindToObject.FieldInfo.Converter;
                }
                return Formatter.ParseObject(value, bindToType, (value == null) ? this.propInfo.PropertyType : value.GetType(), targetConverter, this.propInfoConverter, this.formatInfo, this.nullValue, this.GetDataSourceNullValue(bindToType));
            }
            ConvertEventArgs cevent = new ConvertEventArgs(value, bindToType);
            this.OnParse(cevent);
            if ((cevent.Value != null) && ((cevent.Value.GetType().IsSubclassOf(bindToType) || (cevent.Value.GetType() == bindToType)) || (cevent.Value is DBNull)))
            {
                return cevent.Value;
            }
            TypeConverter converter2 = TypeDescriptor.GetConverter((value != null) ? value.GetType() : typeof(object));
            if ((converter2 != null) && converter2.CanConvertTo(bindToType))
            {
                return converter2.ConvertTo(value, bindToType);
            }
            if (value is IConvertible)
            {
                object obj3 = Convert.ChangeType(value, bindToType, CultureInfo.CurrentCulture);
                if ((obj3 != null) && (obj3.GetType().IsSubclassOf(bindToType) || (obj3.GetType() == bindToType)))
                {
                    return obj3;
                }
            }
            return null;
        }

        internal bool PullData() => 
            this.PullData(true, false);

        internal bool PullData(bool reformat) => 
            this.PullData(reformat, false);

        internal bool PullData(bool reformat, bool force)
        {
            if (this.ControlUpdateMode == System.Windows.Forms.ControlUpdateMode.Never)
            {
                reformat = false;
            }
            bool flag = false;
            object obj2 = null;
            Exception ex = null;
            if (this.IsBinding)
            {
                if (!force)
                {
                    if (this.propInfo.SupportsChangeEvents && !this.modified)
                    {
                        return false;
                    }
                    if (this.DataSourceUpdateMode == System.Windows.Forms.DataSourceUpdateMode.Never)
                    {
                        return false;
                    }
                }
                if (this.inPushOrPull && this.formattingEnabled)
                {
                    return false;
                }
                this.inPushOrPull = true;
                object propValue = this.GetPropValue();
                try
                {
                    obj2 = this.ParseObject(propValue);
                }
                catch (Exception exception2)
                {
                    ex = exception2;
                }
                try
                {
                    if ((ex != null) || (!this.FormattingEnabled && (obj2 == null)))
                    {
                        flag = true;
                        obj2 = this.bindToObject.GetValue();
                    }
                    if (reformat && (!this.FormattingEnabled || !flag))
                    {
                        object objA = this.FormatObject(obj2);
                        if ((force || !this.FormattingEnabled) || !object.Equals(objA, propValue))
                        {
                            this.SetPropValue(objA);
                        }
                    }
                    if (!flag)
                    {
                        this.bindToObject.SetValue(obj2);
                    }
                }
                catch (Exception exception3)
                {
                    ex = exception3;
                    if (!this.FormattingEnabled)
                    {
                        throw;
                    }
                }
                finally
                {
                    this.inPushOrPull = false;
                }
                if (this.FormattingEnabled)
                {
                    BindingCompleteEventArgs e = this.CreateBindingCompleteEventArgs(BindingCompleteContext.DataSourceUpdate, ex);
                    this.OnBindingComplete(e);
                    if ((e.BindingCompleteState == BindingCompleteState.Success) && !e.Cancel)
                    {
                        this.modified = false;
                    }
                    return e.Cancel;
                }
                this.modified = false;
            }
            return false;
        }

        internal bool PushData() => 
            this.PushData(false);

        internal bool PushData(bool force)
        {
            object obj2 = null;
            Exception ex = null;
            if (force || (this.ControlUpdateMode != System.Windows.Forms.ControlUpdateMode.Never))
            {
                if (this.inPushOrPull && this.formattingEnabled)
                {
                    return false;
                }
                this.inPushOrPull = true;
                try
                {
                    if (this.IsBinding)
                    {
                        obj2 = this.bindToObject.GetValue();
                        object obj3 = this.FormatObject(obj2);
                        this.SetPropValue(obj3);
                        this.modified = false;
                    }
                    else
                    {
                        this.SetPropValue(null);
                    }
                }
                catch (Exception exception2)
                {
                    ex = exception2;
                    if (!this.FormattingEnabled)
                    {
                        throw;
                    }
                }
                finally
                {
                    this.inPushOrPull = false;
                }
                if (this.FormattingEnabled)
                {
                    BindingCompleteEventArgs e = this.CreateBindingCompleteEventArgs(BindingCompleteContext.ControlUpdate, ex);
                    this.OnBindingComplete(e);
                    return e.Cancel;
                }
            }
            return false;
        }

        public void ReadValue()
        {
            this.PushData(true);
        }

        internal void SetBindableComponent(IBindableComponent value)
        {
            if (this.control != value)
            {
                IBindableComponent control = this.control;
                this.BindTarget(false);
                this.control = value;
                this.BindTarget(true);
                try
                {
                    this.CheckBinding();
                }
                catch
                {
                    this.BindTarget(false);
                    this.control = control;
                    this.BindTarget(true);
                    throw;
                }
                BindingContext.UpdateBinding(((this.control != null) && IsComponentCreated(this.control)) ? this.control.BindingContext : null, this);
                Form form = value as Form;
                if (form != null)
                {
                    form.Load += new EventHandler(this.FormLoaded);
                }
            }
        }

        internal void SetListManager(System.Windows.Forms.BindingManagerBase bindingManagerBase)
        {
            if (this.bindingManagerBase is CurrencyManager)
            {
                ((CurrencyManager) this.bindingManagerBase).MetaDataChanged -= new EventHandler(this.binding_MetaDataChanged);
            }
            this.bindingManagerBase = bindingManagerBase;
            if (this.bindingManagerBase is CurrencyManager)
            {
                ((CurrencyManager) this.bindingManagerBase).MetaDataChanged += new EventHandler(this.binding_MetaDataChanged);
            }
            this.BindToObject.SetBindingManagerBase(bindingManagerBase);
            this.CheckBinding();
        }

        private void SetPropValue(object value)
        {
            if (!this.ControlAtDesignTime())
            {
                this.inSetPropValue = true;
                try
                {
                    if ((value == null) || Formatter.IsNullData(value, this.DataSourceNullValue))
                    {
                        if (this.propIsNullInfo != null)
                        {
                            this.propIsNullInfo.SetValue(this.control, true);
                        }
                        else if (this.propInfo.PropertyType == typeof(object))
                        {
                            this.propInfo.SetValue(this.control, this.DataSourceNullValue);
                        }
                        else
                        {
                            this.propInfo.SetValue(this.control, null);
                        }
                    }
                    else
                    {
                        this.propInfo.SetValue(this.control, value);
                    }
                }
                finally
                {
                    this.inSetPropValue = false;
                }
            }
        }

        private bool ShouldSerializeDataSourceNullValue() => 
            (this.dsNullValueSet && (this.dsNullValue != Formatter.GetDefaultDataSourceNullValue(null)));

        private bool ShouldSerializeFormatString() => 
            ((this.formatString != null) && (this.formatString.Length > 0));

        private bool ShouldSerializeNullValue() => 
            (this.nullValue != null);

        private void Target_PropertyChanged(object sender, EventArgs e)
        {
            if (!this.inSetPropValue && this.IsBinding)
            {
                this.modified = true;
                if (this.DataSourceUpdateMode == System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged)
                {
                    this.PullData(false);
                    this.modified = true;
                }
            }
        }

        private void Target_Validate(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.PullData(true))
                {
                    e.Cancel = true;
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }

        internal void UpdateIsBinding()
        {
            bool bind = (this.IsBindable && this.ComponentCreated) && this.bindingManagerBase.IsBinding;
            if (this.bound != bind)
            {
                this.bound = bind;
                this.BindTarget(bind);
                if (this.bound)
                {
                    if (this.controlUpdateMode == System.Windows.Forms.ControlUpdateMode.Never)
                    {
                        this.PullData(false, true);
                    }
                    else
                    {
                        this.PushData();
                    }
                }
            }
        }

        public void WriteValue()
        {
            this.PullData(true, true);
        }

        [DefaultValue((string) null)]
        public IBindableComponent BindableComponent =>
            this.control;

        public System.Windows.Forms.BindingManagerBase BindingManagerBase =>
            this.bindingManagerBase;

        public System.Windows.Forms.BindingMemberInfo BindingMemberInfo =>
            this.bindToObject.BindingMemberInfo;

        internal System.Windows.Forms.BindToObject BindToObject =>
            this.bindToObject;

        internal bool ComponentCreated =>
            IsComponentCreated(this.control);

        [DefaultValue((string) null)]
        public System.Windows.Forms.Control Control =>
            (this.control as System.Windows.Forms.Control);

        [DefaultValue(0)]
        public System.Windows.Forms.ControlUpdateMode ControlUpdateMode
        {
            get => 
                this.controlUpdateMode;
            set
            {
                if (this.controlUpdateMode != value)
                {
                    this.controlUpdateMode = value;
                    if (this.IsBinding)
                    {
                        this.PushData();
                    }
                }
            }
        }

        public object DataSource =>
            this.bindToObject.DataSource;

        public object DataSourceNullValue
        {
            get => 
                this.dsNullValue;
            set
            {
                if (!object.Equals(this.dsNullValue, value))
                {
                    object dsNullValue = this.dsNullValue;
                    this.dsNullValue = value;
                    this.dsNullValueSet = true;
                    if (this.IsBinding)
                    {
                        object obj3 = this.bindToObject.GetValue();
                        if (Formatter.IsNullData(obj3, dsNullValue))
                        {
                            this.WriteValue();
                        }
                        if (Formatter.IsNullData(obj3, value))
                        {
                            this.ReadValue();
                        }
                    }
                }
            }
        }

        [DefaultValue(0)]
        public System.Windows.Forms.DataSourceUpdateMode DataSourceUpdateMode
        {
            get => 
                this.dataSourceUpdateMode;
            set
            {
                if (this.dataSourceUpdateMode != value)
                {
                    this.dataSourceUpdateMode = value;
                }
            }
        }

        [DefaultValue((string) null)]
        public IFormatProvider FormatInfo
        {
            get => 
                this.formatInfo;
            set
            {
                if (this.formatInfo != value)
                {
                    this.formatInfo = value;
                    if (this.IsBinding)
                    {
                        this.PushData();
                    }
                }
            }
        }

        public string FormatString
        {
            get => 
                this.formatString;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                if (!value.Equals(this.formatString))
                {
                    this.formatString = value;
                    if (this.IsBinding)
                    {
                        this.PushData();
                    }
                }
            }
        }

        [DefaultValue(false)]
        public bool FormattingEnabled
        {
            get => 
                this.formattingEnabled;
            set
            {
                if (this.formattingEnabled != value)
                {
                    this.formattingEnabled = value;
                    if (this.IsBinding)
                    {
                        this.PushData();
                    }
                }
            }
        }

        internal bool IsBindable =>
            ((((this.control != null) && (this.propertyName.Length > 0)) && (this.bindToObject.DataSource != null)) && (this.bindingManagerBase != null));

        public bool IsBinding =>
            this.bound;

        public object NullValue
        {
            get => 
                this.nullValue;
            set
            {
                if (!object.Equals(this.nullValue, value))
                {
                    this.nullValue = value;
                    if (this.IsBinding && Formatter.IsNullData(this.bindToObject.GetValue(), this.dsNullValue))
                    {
                        this.PushData();
                    }
                }
            }
        }

        [DefaultValue("")]
        public string PropertyName =>
            this.propertyName;
    }
}

