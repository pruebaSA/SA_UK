namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;

    internal class FunctionImportElement : Function
    {
        internal FunctionImportElement(EntityContainer container) : base(container)
        {
        }

        public override string FQName =>
            this.Name;

        public override string Identity =>
            base.Name;

        public override bool IsFunctionImport =>
            true;
    }
}

