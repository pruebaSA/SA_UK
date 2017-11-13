namespace System.Web.Script.Services
{
    using System;
    using System.Globalization;

    internal class WebServiceEnumData : WebServiceTypeData
    {
        private bool isULong;
        private string[] names;
        private long[] values;

        internal WebServiceEnumData(string typeName, string typeNamespace, string[] names, long[] values, bool isULong) : base(typeName, typeNamespace)
        {
            this.InitWebServiceEnumData(names, values, isULong);
        }

        internal WebServiceEnumData(string typeName, string typeNamespace, string[] names, Array values, bool isULong) : base(typeName, typeNamespace)
        {
            this.InitWebServiceEnumData(names, values, isULong);
        }

        internal WebServiceEnumData(string typeName, string typeNamespace, Type t, string[] names, long[] values, bool isULong) : base(typeName, typeNamespace, t)
        {
            this.InitWebServiceEnumData(names, values, isULong);
        }

        internal WebServiceEnumData(string typeName, string typeNamespace, Type t, string[] names, Array values, bool isULong) : base(typeName, typeNamespace)
        {
            this.InitWebServiceEnumData(names, values, isULong);
        }

        private void InitWebServiceEnumData(string[] names, long[] values, bool isULong)
        {
            this.names = names;
            this.values = values;
            this.isULong = isULong;
        }

        private void InitWebServiceEnumData(string[] names, Array values, bool isULong)
        {
            this.names = names;
            this.values = new long[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                object obj2 = values.GetValue(i);
                if (isULong)
                {
                    this.values[i] = (long) ((IConvertible) obj2).ToUInt64(CultureInfo.InvariantCulture);
                }
                else
                {
                    this.values[i] = ((IConvertible) obj2).ToInt64(CultureInfo.InvariantCulture);
                }
            }
            this.isULong = isULong;
        }

        internal bool IsULong =>
            this.isULong;

        internal string[] Names =>
            this.names;

        internal long[] Values =>
            this.values;
    }
}

