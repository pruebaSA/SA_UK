namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;

    internal class DataPagerStylesTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            (sourceType == typeof(string));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            DataPagerActionList instance = (DataPagerActionList) context.Instance;
            string a = (string) value;
            DesignerPagerStyle style = new NextPreviousPagerStyle(instance.Component.Site);
            if (string.Equals(a, style.ToString()))
            {
                return style;
            }
            style = new NumericPagerStyle(instance.Component.Site);
            if (string.Equals(a, style.ToString()))
            {
                return style;
            }
            return null;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            DataPagerActionList instance = (DataPagerActionList) context.Instance;
            ArrayList values = new ArrayList(3);
            if (instance.IsCustomStyle)
            {
                values.Add(new CustomPagerStyle(instance.Component.Site));
            }
            else if (instance.IsNullStyle)
            {
                values.Add(new NullPagerStyle(instance.Component.Site));
            }
            values.Add(new NextPreviousPagerStyle(instance.Component.Site));
            values.Add(new NumericPagerStyle(instance.Component.Site));
            return new TypeConverter.StandardValuesCollection(values);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            true;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;
    }
}

