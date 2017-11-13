namespace System.Windows.Markup.Primitives
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    public abstract class MarkupProperty
    {
        [FriendAccessAllowed]
        internal MarkupProperty()
        {
        }

        internal virtual void VerifyOnlySerializableTypes()
        {
        }

        public abstract AttributeCollection Attributes { get; }

        public virtual System.Windows.DependencyProperty DependencyProperty =>
            null;

        public virtual bool IsAttached =>
            false;

        internal bool IsCollectionProperty
        {
            get
            {
                Type propertyType = this.PropertyType;
                if (!typeof(IList).IsAssignableFrom(propertyType) && !typeof(IDictionary).IsAssignableFrom(propertyType))
                {
                    return propertyType.IsArray;
                }
                return true;
            }
        }

        public virtual bool IsComposite =>
            false;

        public virtual bool IsConstructorArgument =>
            false;

        public virtual bool IsContent =>
            false;

        public virtual bool IsKey =>
            false;

        public virtual bool IsValueAsString =>
            false;

        public abstract IEnumerable<MarkupObject> Items { get; }

        public abstract string Name { get; }

        public virtual System.ComponentModel.PropertyDescriptor PropertyDescriptor =>
            null;

        public abstract Type PropertyType { get; }

        public abstract string StringValue { get; }

        public abstract IEnumerable<Type> TypeReferences { get; }

        public abstract object Value { get; }
    }
}

