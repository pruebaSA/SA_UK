namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;

    public class ServicePathConverter : StringConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string str = (string) value;
                if (string.IsNullOrEmpty(str))
                {
                    HttpContext current = HttpContext.Current;
                    if (current != null)
                    {
                        return current.Request.FilePath;
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

