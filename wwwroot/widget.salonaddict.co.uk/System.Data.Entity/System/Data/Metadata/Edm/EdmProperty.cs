namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Threading;

    public sealed class EdmProperty : EdmMember
    {
        private Func<object, object> _memberGetter;
        private Action<object, object> _memberSetter;
        internal readonly RuntimeMethodHandle PropertyGetterHandle;
        internal readonly RuntimeMethodHandle PropertySetterHandle;

        internal EdmProperty(string name, TypeUsage typeUsage) : base(name, typeUsage)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.GenericCheckArgumentNull<TypeUsage>(typeUsage, "typeUsage");
        }

        internal EdmProperty(string name, TypeUsage typeUsage, PropertyInfo propertyInfo) : this(name, typeUsage)
        {
            if (propertyInfo != null)
            {
                MethodInfo getMethod = propertyInfo.GetGetMethod(true);
                this.PropertyGetterHandle = (getMethod != null) ? getMethod.MethodHandle : new RuntimeMethodHandle();
                getMethod = propertyInfo.GetSetMethod(true);
                this.PropertySetterHandle = (getMethod != null) ? getMethod.MethodHandle : new RuntimeMethodHandle();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty;

        public object DefaultValue =>
            base.TypeUsage.Facets["DefaultValue"].Value;

        public bool Nullable =>
            ((bool) base.TypeUsage.Facets["Nullable"].Value);

        internal Func<object, object> ValueGetter
        {
            get => 
                this._memberGetter;
            set
            {
                Interlocked.CompareExchange<Func<object, object>>(ref this._memberGetter, value, null);
            }
        }

        internal Action<object, object> ValueSetter
        {
            get => 
                this._memberSetter;
            set
            {
                Interlocked.CompareExchange<Action<object, object>>(ref this._memberSetter, value, null);
            }
        }
    }
}

