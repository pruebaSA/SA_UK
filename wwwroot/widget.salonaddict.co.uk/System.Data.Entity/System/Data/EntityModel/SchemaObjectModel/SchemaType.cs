namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;

    internal abstract class SchemaType : SchemaElement
    {
        internal SchemaType(Schema parentElement) : base(parentElement)
        {
        }

        public override string FQName =>
            (this.Namespace + "." + this.Name);

        public override string Identity =>
            (this.Namespace + "." + this.Name);

        public string Namespace =>
            base.Schema.Namespace;
    }
}

