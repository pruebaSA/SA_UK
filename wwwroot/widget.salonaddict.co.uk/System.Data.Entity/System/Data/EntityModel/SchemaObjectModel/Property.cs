namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;

    internal abstract class Property : SchemaElement
    {
        internal Property(StructuredType parentElement) : base(parentElement)
        {
        }

        public abstract SchemaType Type { get; }
    }
}

