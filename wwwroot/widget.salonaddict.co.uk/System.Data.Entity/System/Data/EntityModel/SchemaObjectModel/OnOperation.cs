namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Xml;

    internal sealed class OnOperation : SchemaElement
    {
        private System.Data.EntityModel.SchemaObjectModel.Action _Action;
        private System.Data.EntityModel.SchemaObjectModel.Operation _Operation;

        public OnOperation(RelationshipEnd parentElement, System.Data.EntityModel.SchemaObjectModel.Operation operation) : base(parentElement)
        {
            this.Operation = operation;
        }

        private void HandleActionAttribute(XmlReader reader)
        {
            RelationshipKind relationshipKind = this.ParentElement.ParentElement.RelationshipKind;
            switch (reader.Value.Trim())
            {
                case "None":
                    this.Action = System.Data.EntityModel.SchemaObjectModel.Action.None;
                    return;

                case "Cascade":
                    this.Action = System.Data.EntityModel.SchemaObjectModel.Action.Cascade;
                    return;
            }
            base.AddError(ErrorCode.InvalidAction, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidAction(reader.Value, this.ParentElement.FQName));
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Action"))
            {
                this.HandleActionAttribute(reader);
                return true;
            }
            return false;
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        public System.Data.EntityModel.SchemaObjectModel.Action Action
        {
            get => 
                this._Action;
            private set
            {
                this._Action = value;
            }
        }

        public System.Data.EntityModel.SchemaObjectModel.Operation Operation
        {
            get => 
                this._Operation;
            private set
            {
                this._Operation = value;
            }
        }

        private RelationshipEnd ParentElement =>
            ((RelationshipEnd) base.ParentElement);
    }
}

