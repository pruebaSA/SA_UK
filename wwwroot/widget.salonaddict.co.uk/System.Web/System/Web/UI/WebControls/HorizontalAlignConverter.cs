namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class HorizontalAlignConverter : EnumConverter
    {
        private static string[] stringValues = new string[] { "NotSet", "Left", "Center", "Right", "Justify" };

        public HorizontalAlignConverter() : base(typeof(HorizontalAlign))
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
                    return HorizontalAlign.NotSet;
                }
                switch (str)
                {
                    case "NotSet":
                        return HorizontalAlign.NotSet;

                    case "Left":
                        return HorizontalAlign.Left;

                    case "Center":
                        return HorizontalAlign.Center;

                    case "Right":
                        return HorizontalAlign.Right;

                    case "Justify":
                        return HorizontalAlign.Justify;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((destinationType == typeof(string)) && (((int) value) <= 4))
            {
                return stringValues[(int) value];
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

