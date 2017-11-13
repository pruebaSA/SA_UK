namespace System.Data.Objects
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class ObjectParameter
    {
        private System.Data.Metadata.Edm.TypeUsage _effectiveType;
        private Type _mappableType;
        private string _name;
        private Type _type;
        private object _value;

        private ObjectParameter(ObjectParameter template)
        {
            this._name = template._name;
            this._type = template._type;
            this._mappableType = template._mappableType;
            this._effectiveType = template._effectiveType;
            this._value = template._value;
        }

        public ObjectParameter(string name, object value)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            EntityUtil.CheckArgumentNull<object>(value, "value");
            if (!ValidateParameterName(name))
            {
                throw EntityUtil.Argument(Strings.ObjectParameter_InvalidParameterName(name), "name");
            }
            this._name = name;
            this._type = value.GetType();
            this._value = value;
            this.FindMappableType(this._type);
        }

        public ObjectParameter(string name, Type type)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            EntityUtil.CheckArgumentNull<Type>(type, "type");
            if (!ValidateParameterName(name))
            {
                throw EntityUtil.Argument(Strings.ObjectParameter_InvalidParameterName(name), "name");
            }
            this._name = name;
            this._type = type;
            this.FindMappableType(this._type);
        }

        private void FindMappableType(Type type)
        {
            this._mappableType = type;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                this._mappableType = type.GetGenericArguments()[0];
            }
        }

        internal ObjectParameter ShallowCopy() => 
            new ObjectParameter(this);

        internal static bool ValidateParameterName(string name) => 
            DbCommandTree.IsValidParameterName(name);

        internal bool ValidateParameterType(ClrPerspective perspective)
        {
            System.Data.Metadata.Edm.TypeUsage outTypeUsage = null;
            if (!perspective.TryGetType(this._mappableType, out outTypeUsage) || (!TypeSemantics.IsPrimitiveType(outTypeUsage) && !TypeSemantics.IsEnumerationType(outTypeUsage)))
            {
                return false;
            }
            return true;
        }

        internal Type MappableType =>
            this._mappableType;

        public string Name =>
            this._name;

        public Type ParameterType =>
            this._type;

        internal System.Data.Metadata.Edm.TypeUsage TypeUsage
        {
            get => 
                this._effectiveType;
            set
            {
                this._effectiveType = value;
            }
        }

        public object Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }
    }
}

