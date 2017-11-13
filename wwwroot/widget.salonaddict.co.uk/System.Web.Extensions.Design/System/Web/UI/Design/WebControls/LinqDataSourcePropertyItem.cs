namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Reflection;

    internal class LinqDataSourcePropertyItem : ILinqDataSourcePropertyItem, IComparable<ILinqDataSourcePropertyItem>
    {
        protected string _displayName;
        protected bool _isIdentity;
        protected bool _isNullable;
        protected bool _isPrimaryKey;
        protected bool _isProperty;
        protected bool _isReadOnly;
        protected string _name;
        protected Type _type;

        protected LinqDataSourcePropertyItem()
        {
        }

        public LinqDataSourcePropertyItem(FieldInfo fieldInfo) : this(fieldInfo.Name, fieldInfo.Name, fieldInfo.FieldType)
        {
            this._isProperty = false;
            this._isReadOnly = false;
            this.CheckAttributes(fieldInfo);
        }

        public LinqDataSourcePropertyItem(PropertyInfo propertyInfo) : this(propertyInfo.Name, propertyInfo.Name, propertyInfo.PropertyType)
        {
            this._isProperty = true;
            this._isReadOnly = !propertyInfo.CanWrite;
            this.CheckAttributes(propertyInfo);
        }

        public LinqDataSourcePropertyItem(string name, string displayName) : this(name, displayName, null)
        {
        }

        private LinqDataSourcePropertyItem(string name, string displayName, Type type)
        {
            this._name = name;
            this._displayName = displayName;
            if (type != null)
            {
                this.SetType(type);
            }
        }

        protected void CheckAttributes(MemberInfo memberInfo)
        {
            ColumnAttribute customAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(ColumnAttribute)) as ColumnAttribute;
            if (customAttribute != null)
            {
                this._isNullable = customAttribute.CanBeNull && !customAttribute.IsPrimaryKey;
                this._isIdentity = customAttribute.IsDbGenerated;
                this._isPrimaryKey = customAttribute.IsPrimaryKey;
            }
            else
            {
                this._isIdentity = false;
                this._isPrimaryKey = false;
            }
        }

        public static bool IsNullable(Type t) => 
            (!t.IsValueType || (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(Nullable<>))));

        protected void SetType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                this._type = underlyingType;
                this._isNullable = true;
            }
            else
            {
                this._type = type;
                this._isNullable = !type.IsValueType;
            }
        }

        int IComparable<ILinqDataSourcePropertyItem>.CompareTo(ILinqDataSourcePropertyItem other) => 
            string.Compare(this.ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase);

        public override string ToString() => 
            this._displayName;

        string ILinqDataSourcePropertyItem.DisplayName
        {
            get => 
                this._displayName;
            set
            {
                this._displayName = value;
            }
        }

        bool ILinqDataSourcePropertyItem.IsIdentity =>
            this._isIdentity;

        bool ILinqDataSourcePropertyItem.IsNullable =>
            this._isNullable;

        bool ILinqDataSourcePropertyItem.IsPrimaryKey =>
            this._isPrimaryKey;

        bool ILinqDataSourcePropertyItem.IsProperty =>
            this._isProperty;

        bool ILinqDataSourcePropertyItem.IsReadOnly =>
            this._isReadOnly;

        bool ILinqDataSourcePropertyItem.IsUnique =>
            this._isPrimaryKey;

        int ILinqDataSourcePropertyItem.Length =>
            -1;

        string ILinqDataSourcePropertyItem.Name =>
            this._name;

        Type ILinqDataSourcePropertyItem.Type =>
            this._type;
    }
}

