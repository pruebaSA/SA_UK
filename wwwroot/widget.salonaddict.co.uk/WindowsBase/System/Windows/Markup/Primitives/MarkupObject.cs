namespace System.Windows.Markup.Primitives
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Markup;

    public abstract class MarkupObject
    {
        [FriendAccessAllowed]
        internal MarkupObject()
        {
        }

        public abstract void AssignRootContext(IValueSerializerContext context);
        internal abstract IEnumerable<MarkupProperty> GetProperties(bool mapToConstructorArgs);

        public abstract AttributeCollection Attributes { get; }

        public abstract object Instance { get; }

        public abstract Type ObjectType { get; }

        public virtual IEnumerable<MarkupProperty> Properties =>
            this.GetProperties(true);
    }
}

