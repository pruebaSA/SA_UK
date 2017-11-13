namespace AjaxControlToolkit.Design
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class ExtenderPropertiesTypeDescriptor : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return "";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

