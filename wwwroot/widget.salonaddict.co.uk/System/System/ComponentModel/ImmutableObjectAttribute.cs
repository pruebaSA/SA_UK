namespace System.ComponentModel
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class ImmutableObjectAttribute : Attribute
    {
        public static readonly ImmutableObjectAttribute Default = No;
        private bool immutable = true;
        public static readonly ImmutableObjectAttribute No = new ImmutableObjectAttribute(false);
        public static readonly ImmutableObjectAttribute Yes = new ImmutableObjectAttribute(true);

        public ImmutableObjectAttribute(bool immutable)
        {
            this.immutable = immutable;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            ImmutableObjectAttribute attribute = obj as ImmutableObjectAttribute;
            return ((attribute != null) && (attribute.Immutable == this.immutable));
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public bool Immutable =>
            this.immutable;
    }
}

