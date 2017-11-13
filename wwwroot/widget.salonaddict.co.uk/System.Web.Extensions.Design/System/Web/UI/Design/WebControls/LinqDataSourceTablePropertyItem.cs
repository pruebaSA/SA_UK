namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Web.UI.WebControls;

    internal class LinqDataSourceTablePropertyItem : LinqDataSourcePropertyItem, ILinqDataSourcePropertyItem, IComparable<ILinqDataSourcePropertyItem>
    {
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;
        private string _toString;

        public LinqDataSourceTablePropertyItem(FieldInfo fieldInfo)
        {
            this._fieldInfo = fieldInfo;
            base._name = fieldInfo.Name;
            base._isNullable = LinqDataSourcePropertyItem.IsNullable(fieldInfo.FieldType);
            base._isReadOnly = false;
            base.SetType(FindDataObjectType(fieldInfo.FieldType));
            base.CheckAttributes(fieldInfo);
            this._toString = GetSignature(fieldInfo);
        }

        public LinqDataSourceTablePropertyItem(PropertyInfo propertyInfo)
        {
            this._propertyInfo = propertyInfo;
            base._name = propertyInfo.Name;
            base._isNullable = LinqDataSourcePropertyItem.IsNullable(propertyInfo.PropertyType);
            base._isReadOnly = !propertyInfo.CanWrite;
            base.SetType(FindDataObjectType(propertyInfo.PropertyType));
            base.CheckAttributes(propertyInfo);
            this._toString = GetSignature(propertyInfo);
        }

        public LinqDataSourceTablePropertyItem(string name)
        {
            base._name = name;
            base._type = null;
        }

        internal static Type FindDataObjectType(Type propertyType)
        {
            Type type = LinqDataSourceHelper.FindGenericEnumerableType(propertyType);
            if (type == null)
            {
                return propertyType;
            }
            return type.GetGenericArguments()[0];
        }

        internal static string GetSignature(FieldInfo fi) => 
            GetSignature(fi.Name, fi.FieldType);

        internal static string GetSignature(PropertyInfo pi) => 
            GetSignature(pi.Name, pi.PropertyType);

        internal static string GetSignature(string name, Type type) => 
            string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[] { name, GetTypeDisplay(type) });

        internal static string GetTypeDisplay(Type type)
        {
            StringBuilder builder = new StringBuilder();
            typeof(ObjectDataSourceDesigner).Assembly.GetType("System.Web.UI.Design.WebControls.ObjectDataSourceMethodEditor").GetMethod("AppendTypeName", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { type, false, builder });
            return builder.ToString();
        }

        public override string ToString() => 
            this._toString;

        bool ILinqDataSourcePropertyItem.IsProperty =>
            (this._propertyInfo != null);
    }
}

