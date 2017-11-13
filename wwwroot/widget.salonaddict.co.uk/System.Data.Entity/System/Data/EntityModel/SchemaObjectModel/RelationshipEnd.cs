namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal sealed class RelationshipEnd : SchemaElement, IRelationshipEnd
    {
        private RelationshipMultiplicity _multiplicity;
        private List<OnOperation> _operations;
        private SchemaEntityType _type;
        private string _unresolvedType;

        public RelationshipEnd(Relationship relationship) : base(relationship)
        {
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Multiplicity"))
            {
                this.HandleMultiplicityAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Role"))
            {
                this.HandleNameAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Type"))
            {
                this.HandleTypeAttribute(reader);
                return true;
            }
            return false;
        }

        protected override void HandleAttributesComplete()
        {
            if ((this.Name == null) && (this._unresolvedType != null))
            {
                this.Name = Utils.ExtractTypeName(base.Schema.DataModel, this._unresolvedType);
            }
            base.HandleAttributesComplete();
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "OnDelete"))
            {
                this.HandleOnDeleteElement(reader);
                return true;
            }
            return false;
        }

        private void HandleMultiplicityAttribute(XmlReader reader)
        {
            if (!TryParseMultiplicity(reader.Value, out this._multiplicity))
            {
                base.AddError(ErrorCode.InvalidMultiplicity, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidRelationshipEndMultiplicity(this.ParentElement.Name, reader.Value));
            }
        }

        private void HandleOnDeleteElement(XmlReader reader)
        {
            this.HandleOnOperationElement(reader, Operation.Delete);
        }

        private void HandleOnOperationElement(XmlReader reader, Operation operation)
        {
            foreach (OnOperation operation2 in this.Operations)
            {
                if (operation2.Operation == operation)
                {
                    base.AddError(ErrorCode.InvalidOperation, EdmSchemaErrorSeverity.Error, reader, Strings.DuplicationOperation(reader.Name));
                }
            }
            OnOperation item = new OnOperation(this, operation);
            item.Parse(reader);
            this._operations.Add(item);
        }

        private void HandleTypeAttribute(XmlReader reader)
        {
            string str;
            if (Utils.GetDottedName(base.Schema, reader, out str))
            {
                this._unresolvedType = str;
            }
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        internal override void ResolveTopLevelNames()
        {
            SchemaType type;
            base.ResolveTopLevelNames();
            if (((this.Type == null) && (this._unresolvedType != null)) && base.Schema.ResolveTypeName(this, this._unresolvedType, out type))
            {
                this.Type = type as SchemaEntityType;
                if (this.Type == null)
                {
                    base.AddError(ErrorCode.InvalidRelationshipEndType, EdmSchemaErrorSeverity.Error, Strings.InvalidRelationshipEndType(this.ParentElement.Name, type.FQName));
                }
            }
        }

        private static bool TryParseMultiplicity(string value, out RelationshipMultiplicity multiplicity)
        {
            switch (value)
            {
                case "0..1":
                    multiplicity = RelationshipMultiplicity.ZeroOrOne;
                    return true;

                case "1":
                    multiplicity = RelationshipMultiplicity.One;
                    return true;

                case "*":
                    multiplicity = RelationshipMultiplicity.Many;
                    return true;
            }
            multiplicity = ~RelationshipMultiplicity.ZeroOrOne;
            return false;
        }

        internal override void Validate()
        {
            base.Validate();
            if ((this.Multiplicity == RelationshipMultiplicity.Many) && (this.Operations.Count != 0))
            {
                base.AddError(ErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified, EdmSchemaErrorSeverity.Error, Strings.EndWithManyMultiplicityCannotHaveOperationsSpecified(this.Name, this.ParentElement.FQName));
            }
        }

        public RelationshipMultiplicity Multiplicity =>
            this._multiplicity;

        public ICollection<OnOperation> Operations
        {
            get
            {
                if (this._operations == null)
                {
                    this._operations = new List<OnOperation>();
                }
                return this._operations;
            }
        }

        internal IRelationship ParentElement =>
            ((IRelationship) base.ParentElement);

        public SchemaEntityType Type
        {
            get => 
                this._type;
            private set
            {
                this._type = value;
            }
        }
    }
}

