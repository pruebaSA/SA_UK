namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;

    internal sealed class PropertyRefElement : SchemaElement
    {
        private StructuredProperty _property;

        public PropertyRefElement(SchemaElement parentElement) : base(parentElement)
        {
        }

        internal bool ResolveNames(SchemaEntityType entityType)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return true;
            }
            this._property = entityType.FindProperty(this.Name);
            return (this._property != null);
        }

        internal override void ResolveTopLevelNames()
        {
        }

        public StructuredProperty Property =>
            this._property;
    }
}

