namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;

    internal sealed class FieldDescriptor : PropertyDescriptor
    {
        private readonly Type _componentType;
        private readonly Type _fieldType;
        private readonly bool _isReadOnly;
        private readonly System.Data.Metadata.Edm.EdmProperty _property;

        internal FieldDescriptor(Type componentType, bool isReadOnly, System.Data.Metadata.Edm.EdmProperty property) : base(property.Name, null)
        {
            this._componentType = componentType;
            this._property = property;
            this._isReadOnly = isReadOnly;
            this._fieldType = this.DetermineClrType(this._property.TypeUsage);
        }

        public override bool CanResetValue(object component) => 
            false;

        private Type DetermineClrType(TypeUsage typeUsage)
        {
            Type clrType = null;
            EdmType edmType = typeUsage.EdmType;
            switch (edmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.PrimitiveType:
                    Facet facet;
                    clrType = edmType.ClrType;
                    if ((clrType.IsValueType && typeUsage.Facets.TryGetValue("Nullable", false, out facet)) && ((bool) facet.Value))
                    {
                        clrType = typeof(Nullable<>).MakeGenericType(new Type[] { clrType });
                    }
                    return clrType;

                case BuiltInTypeKind.RefType:
                    return typeof(EntityKey);

                case BuiltInTypeKind.RowType:
                    return typeof(IDataRecord);

                case BuiltInTypeKind.CollectionType:
                {
                    TypeUsage usage = ((CollectionType) edmType).TypeUsage;
                    clrType = this.DetermineClrType(usage);
                    return typeof(IEnumerable<>).MakeGenericType(new Type[] { clrType });
                }
                case BuiltInTypeKind.ComplexType:
                case BuiltInTypeKind.EntityType:
                    return edmType.ClrType;
            }
            throw EntityUtil.UnexpectedMetadataType(edmType);
        }

        public override object GetValue(object component)
        {
            EntityUtil.CheckArgumentNull<object>(component, "component");
            if (!this._componentType.IsAssignableFrom(component.GetType()))
            {
                throw EntityUtil.IncompatibleArgument();
            }
            if (component is IEntityWithChangeTracker)
            {
                return LightweightCodeGenerator.GetValue(this._property, component);
            }
            DbDataRecord record = component as DbDataRecord;
            return record?.GetValue(record.GetOrdinal(this._property.Name));
        }

        public override void ResetValue(object component)
        {
            throw EntityUtil.NotSupported();
        }

        public override void SetValue(object component, object value)
        {
            EntityUtil.CheckArgumentNull<object>(component, "component");
            if (!this._componentType.IsAssignableFrom(component.GetType()))
            {
                throw EntityUtil.IncompatibleArgument();
            }
            if (this._isReadOnly || !(component is IEntityWithChangeTracker))
            {
                throw EntityUtil.WriteOperationNotAllowedOnReadOnlyBindingList();
            }
            LightweightCodeGenerator.SetValue(this._property, component, value);
        }

        public override bool ShouldSerializeValue(object component) => 
            false;

        public override Type ComponentType =>
            this._componentType;

        internal System.Data.Metadata.Edm.EdmProperty EdmProperty =>
            this._property;

        public override bool IsBrowsable =>
            true;

        public override bool IsReadOnly =>
            this._isReadOnly;

        public override Type PropertyType =>
            this._fieldType;
    }
}

