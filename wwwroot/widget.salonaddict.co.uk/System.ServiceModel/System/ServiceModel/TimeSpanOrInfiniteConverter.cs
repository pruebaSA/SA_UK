namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    internal class TimeSpanOrInfiniteConverter : TimeSpanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            if (string.Equals((string) data, "infinite", StringComparison.OrdinalIgnoreCase))
            {
                return TimeSpan.MaxValue;
            }
            return base.ConvertFrom(ctx, ci, data);
        }

        public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
        {
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }
            if (!(value is TimeSpan))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("value", System.ServiceModel.SR.GetString("SFxWrongType2", new object[] { typeof(TimeSpan), value.GetType() }));
            }
            if (((TimeSpan) value) == TimeSpan.MaxValue)
            {
                return "Infinite";
            }
            return base.ConvertTo(ctx, ci, value, type);
        }
    }
}

