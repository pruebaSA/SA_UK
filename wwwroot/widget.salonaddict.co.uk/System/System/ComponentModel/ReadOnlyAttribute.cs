namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ReadOnlyAttribute : Attribute
    {
        public static readonly ReadOnlyAttribute Default = No;
        private bool isReadOnly;
        public static readonly ReadOnlyAttribute No = new ReadOnlyAttribute(false);
        public static readonly ReadOnlyAttribute Yes = new ReadOnlyAttribute(true);

        public ReadOnlyAttribute(bool isReadOnly)
        {
            this.isReadOnly = isReadOnly;
        }

        public override bool Equals(object value)
        {
            if (this == value)
            {
                return true;
            }
            ReadOnlyAttribute attribute = value as ReadOnlyAttribute;
            return ((attribute != null) && (attribute.IsReadOnly == this.IsReadOnly));
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            (this.IsReadOnly == Default.IsReadOnly);

        public bool IsReadOnly =>
            this.isReadOnly;
    }
}

