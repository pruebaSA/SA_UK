namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class VerticalAlignConverter : EnumConverter
    {
        private static string[] stringValues = new string[] { "NotSet", "Top", "Middle", "Bottom" };

        public VerticalAlignConverter() : base(typeof(VerticalAlign))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertTo(context, sourceType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string)
            {
                string str = ((string) value).Trim();
                if (str.Length == 0)
                {
                    return VerticalAlign.NotSet;
                }
                switch (str)
                {
                    case "NotSet":
                        return VerticalAlign.NotSet;

                    case "Top":
                        return VerticalAlign.Top;

                    case "Middle":
                        return VerticalAlign.Middle;

                    case "Bottom":
                        return VerticalAlign.Bottom;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (((int) value) <= 3))
            {
                return stringValues[(int) value];
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

