namespace System.Windows
{
    using System;
    using System.ComponentModel;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public sealed class AttachedPropertyBrowsableWhenAttributePresentAttribute : AttachedPropertyBrowsableAttribute
    {
        private Type _attributeType;

        public AttachedPropertyBrowsableWhenAttributePresentAttribute(Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }
            this._attributeType = attributeType;
        }

        public override bool Equals(object obj)
        {
            AttachedPropertyBrowsableWhenAttributePresentAttribute attribute = obj as AttachedPropertyBrowsableWhenAttributePresentAttribute;
            if (attribute == null)
            {
                return false;
            }
            return (this._attributeType == attribute._attributeType);
        }

        public override int GetHashCode() => 
            this._attributeType.GetHashCode();

        internal override bool IsBrowsable(DependencyObject d, DependencyProperty dp)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            Attribute attribute = TypeDescriptor.GetAttributes(d)[this._attributeType];
            return ((attribute != null) && !attribute.IsDefaultAttribute());
        }

        public Type AttributeType =>
            this._attributeType;
    }
}

