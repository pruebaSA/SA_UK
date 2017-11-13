namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class LinqDataSourceInsertUpdateDeleteStatementConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) => 
            null;

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => 
            false;
    }
}

