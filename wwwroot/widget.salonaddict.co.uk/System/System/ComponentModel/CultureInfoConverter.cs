namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Threading;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class CultureInfoConverter : TypeConverter
    {
        private TypeConverter.StandardValuesCollection values;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => 
            ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => 
            ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            string strA = (string) value;
            CultureInfo invariantCulture = null;
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            if ((culture != null) && culture.Equals(CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            try
            {
                if (((strA == null) || (strA.Length == 0)) || (string.Compare(strA, this.DefaultCultureString, StringComparison.Ordinal) == 0))
                {
                    invariantCulture = CultureInfo.InvariantCulture;
                }
                if (invariantCulture == null)
                {
                    IEnumerator enumerator = ((IEnumerable) this.GetStandardValues(context)).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        CultureInfo current = (CultureInfo) enumerator.Current;
                        if ((current != null) && (string.Compare(current.DisplayName, strA, StringComparison.Ordinal) == 0))
                        {
                            invariantCulture = current;
                            break;
                        }
                    }
                }
                if (invariantCulture == null)
                {
                    try
                    {
                        invariantCulture = new CultureInfo(strA);
                    }
                    catch
                    {
                    }
                }
                if (invariantCulture == null)
                {
                    strA = strA.ToLower(CultureInfo.CurrentCulture);
                    IEnumerator enumerator2 = this.values.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        CultureInfo info4 = (CultureInfo) enumerator2.Current;
                        if ((info4 != null) && info4.DisplayName.ToLower(CultureInfo.CurrentCulture).StartsWith(strA))
                        {
                            invariantCulture = info4;
                            goto Label_0113;
                        }
                    }
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
            }
        Label_0113:
            if (invariantCulture == null)
            {
                throw new ArgumentException(SR.GetString("CultureInfoConverterInvalidCulture", new object[] { (string) value }));
            }
            return invariantCulture;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType == typeof(string))
            {
                string displayName;
                CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
                if ((culture != null) && culture.Equals(CultureInfo.InvariantCulture))
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                try
                {
                    if ((value == null) || (value == CultureInfo.InvariantCulture))
                    {
                        return this.DefaultCultureString;
                    }
                    displayName = ((CultureInfo) value).DisplayName;
                }
                finally
                {
                    Thread.CurrentThread.CurrentUICulture = currentUICulture;
                }
                return displayName;
            }
            if ((destinationType == typeof(InstanceDescriptor)) && (value is CultureInfo))
            {
                CultureInfo info2 = (CultureInfo) value;
                ConstructorInfo constructor = typeof(CultureInfo).GetConstructor(new Type[] { typeof(string) });
                if (constructor != null)
                {
                    return new InstanceDescriptor(constructor, new object[] { info2.Name });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (this.values == null)
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures);
                CultureInfo[] destinationArray = new CultureInfo[cultures.Length + 1];
                Array.Copy(cultures, destinationArray, cultures.Length);
                Array.Sort(destinationArray, new CultureComparer());
                if (destinationArray[0] == null)
                {
                    destinationArray[0] = CultureInfo.InvariantCulture;
                }
                this.values = new TypeConverter.StandardValuesCollection(destinationArray);
            }
            return this.values;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
            false;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
            true;

        private string DefaultCultureString =>
            SR.GetString("CultureInfoConverterDefaultCultureString");

        private class CultureComparer : IComparer
        {
            public int Compare(object item1, object item2)
            {
                if (item1 == null)
                {
                    if (item2 == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (item2 == null)
                {
                    return 1;
                }
                string displayName = ((CultureInfo) item1).DisplayName;
                string str2 = ((CultureInfo) item2).DisplayName;
                return CultureInfo.CurrentCulture.CompareInfo.Compare(displayName, str2, CompareOptions.StringSort);
            }
        }
    }
}

