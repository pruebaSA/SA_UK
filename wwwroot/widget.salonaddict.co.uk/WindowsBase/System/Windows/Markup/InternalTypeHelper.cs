namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class InternalTypeHelper
    {
        protected InternalTypeHelper()
        {
        }

        protected internal abstract void AddEventHandler(EventInfo eventInfo, object target, Delegate handler);
        protected internal abstract Delegate CreateDelegate(Type delegateType, object target, string handler);
        protected internal abstract object CreateInstance(Type type, CultureInfo culture);
        protected internal abstract object GetPropertyValue(PropertyInfo propertyInfo, object target, CultureInfo culture);
        protected internal abstract void SetPropertyValue(PropertyInfo propertyInfo, object target, object value, CultureInfo culture);
    }
}

