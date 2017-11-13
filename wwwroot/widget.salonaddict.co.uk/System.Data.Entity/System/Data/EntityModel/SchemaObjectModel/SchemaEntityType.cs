namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Xml;

    [DebuggerDisplay("Name={Name}, BaseType={BaseType.FQName}, HasKeys={HasKeys}")]
    internal sealed class SchemaEntityType : StructuredType
    {
        private EntityKeyElement _keyElement;
        private ISchemaElementLookUpTable<System.Data.EntityModel.SchemaObjectModel.NavigationProperty> _navigationProperties;
        private static List<PropertyRefElement> EmptyKeyProperties = new List<PropertyRefElement>(0);
        private const char KEY_DELIMITER = ' ';

        public SchemaEntityType(Schema parentElement) : base(parentElement)
        {
        }

        protected override bool HandleAttribute(XmlReader reader) => 
            base.HandleAttribute(reader);

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "Key"))
            {
                this.HandleKeyElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "NavigationProperty"))
            {
                this.HandleNavigationPropertyElement(reader);
                return true;
            }
            return false;
        }

        private void HandleKeyElement(XmlReader reader)
        {
            this._keyElement = new EntityKeyElement(this);
            this._keyElement.Parse(reader);
        }

        private void HandleNavigationPropertyElement(XmlReader reader)
        {
            System.Data.EntityModel.SchemaObjectModel.NavigationProperty newMember = new System.Data.EntityModel.SchemaObjectModel.NavigationProperty(this);
            newMember.Parse(reader);
            base.AddMember(newMember);
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (base.BaseType != null)
            {
                if (!(base.BaseType is SchemaEntityType))
                {
                    base.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, Strings.InvalidBaseTypeForItemType(base.BaseType.FQName, this.FQName));
                }
                else if ((this._keyElement != null) && (base.BaseType != null))
                {
                    base.AddError(ErrorCode.InvalidKey, EdmSchemaErrorSeverity.Error, Strings.InvalidKeyKeyDefinedInBaseClass(this.FQName, base.BaseType.FQName));
                }
            }
            else if (this._keyElement == null)
            {
                base.AddError(ErrorCode.KeyMissingOnEntityType, EdmSchemaErrorSeverity.Error, Strings.KeyMissingOnEntityType(this.FQName));
            }
            else if ((base.BaseType != null) || (base.UnresolvedBaseType == null))
            {
                this._keyElement.ResolveTopLevelNames();
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if (this.KeyElement != null)
            {
                this.KeyElement.Validate();
            }
        }

        public IList<PropertyRefElement> DeclaredKeyProperties =>
            this.KeyElement?.KeyProperties;

        public EntityKeyElement KeyElement =>
            this._keyElement;

        public IList<PropertyRefElement> KeyProperties
        {
            get
            {
                if (this.KeyElement != null)
                {
                    return this.KeyElement.KeyProperties;
                }
                if (base.BaseType != null)
                {
                    return (base.BaseType as SchemaEntityType).KeyProperties;
                }
                return EmptyKeyProperties;
            }
        }

        public ISchemaElementLookUpTable<System.Data.EntityModel.SchemaObjectModel.NavigationProperty> NavigationProperties
        {
            get
            {
                if (this._navigationProperties == null)
                {
                    this._navigationProperties = new FilteredSchemaElementLookUpTable<System.Data.EntityModel.SchemaObjectModel.NavigationProperty, SchemaElement>(base.NamedMembers);
                }
                return this._navigationProperties;
            }
        }
    }
}

