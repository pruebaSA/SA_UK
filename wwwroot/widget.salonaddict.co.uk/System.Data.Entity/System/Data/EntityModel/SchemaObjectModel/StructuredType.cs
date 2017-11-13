namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal abstract class StructuredType : SchemaType
    {
        private StructuredType _baseType;
        private bool? _baseTypeResolveResult;
        private bool _isAbstract;
        private SchemaElementLookUpTable<SchemaElement> _namedMembers;
        private ISchemaElementLookUpTable<StructuredProperty> _properties;
        private string _unresolvedBaseType;
        private static readonly char[] NameSeparators = new char[] { '.' };

        protected StructuredType(Schema parentElement) : base(parentElement)
        {
        }

        protected void AddMember(SchemaElement newMember)
        {
            if (!string.IsNullOrEmpty(newMember.Name))
            {
                if ((base.Schema.DataModel != SchemaDataModelOption.ProviderDataModel) && (Utils.CompareNames(newMember.Name, this.Name) == 0))
                {
                    newMember.AddError(ErrorCode.BadProperty, EdmSchemaErrorSeverity.Error, Strings.InvalidMemberNameMatchesTypeName(newMember.Name, this.FQName));
                }
                this.NamedMembers.Add(newMember, true, new Func<object, string>(Strings.PropertyNameAlreadyDefinedDuplicate));
            }
        }

        private bool CheckForInheritanceCycle()
        {
            StructuredType baseType = this.BaseType;
            StructuredType objA = baseType;
            StructuredType objB = baseType;
            do
            {
                objB = objB.BaseType;
                if (object.ReferenceEquals(objA, objB))
                {
                    return true;
                }
                if (objA == null)
                {
                    return false;
                }
                objA = objA.BaseType;
                if (objB != null)
                {
                    objB = objB.BaseType;
                }
            }
            while (objB != null);
            return false;
        }

        private HowDefined DefinesMemberName(string name, out StructuredType definingType, out SchemaElement definingMember)
        {
            if (this.NamedMembers.ContainsKey(name))
            {
                definingType = this;
                definingMember = this.NamedMembers[name];
                return HowDefined.AsMember;
            }
            definingMember = this.NamedMembers.LookUpEquivalentKey(name);
            if (definingMember != null)
            {
                definingType = this;
                return HowDefined.AsMemberWithDifferentCase;
            }
            if (this.IsTypeHierarchyRoot)
            {
                definingType = null;
                definingMember = null;
                return HowDefined.NotDefined;
            }
            return this.BaseType.DefinesMemberName(name, out definingType, out definingMember);
        }

        public StructuredProperty FindProperty(string name)
        {
            StructuredProperty property = this.Properties.LookUpEquivalentKey(name);
            if (property != null)
            {
                return property;
            }
            if (this.IsTypeHierarchyRoot)
            {
                return null;
            }
            return this.BaseType.FindProperty(name);
        }

        private void HandleAbstractAttribute(XmlReader reader)
        {
            base.HandleBoolAttribute(reader, ref this._isAbstract);
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "BaseType"))
            {
                this.HandleBaseTypeAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Abstract"))
            {
                this.HandleAbstractAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleBaseTypeAttribute(XmlReader reader)
        {
            if (this.UnresolvedBaseType != null)
            {
                base.AddAlreadyDefinedError(reader);
            }
            else
            {
                string str;
                if (Utils.GetDottedName(base.Schema, reader, out str))
                {
                    this.UnresolvedBaseType = str;
                }
            }
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "Property"))
            {
                this.HandlePropertyElement(reader);
                return true;
            }
            return false;
        }

        private void HandlePropertyElement(XmlReader reader)
        {
            StructuredProperty newMember = new StructuredProperty(this);
            newMember.Parse(reader);
            this.AddMember(newMember);
        }

        public bool IsOfType(StructuredType baseType)
        {
            StructuredType type = this;
            while ((type != null) && (type != baseType))
            {
                type = type.BaseType;
            }
            return (type == baseType);
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            this.TryResolveBaseType();
            foreach (SchemaElement element in this.NamedMembers)
            {
                element.ResolveTopLevelNames();
            }
        }

        private bool TryResolveBaseType()
        {
            SchemaType type;
            if (this._baseTypeResolveResult.HasValue)
            {
                return this._baseTypeResolveResult.Value;
            }
            if (this.BaseType != null)
            {
                this._baseTypeResolveResult = true;
                return this._baseTypeResolveResult.Value;
            }
            if (this.UnresolvedBaseType == null)
            {
                this._baseTypeResolveResult = true;
                return this._baseTypeResolveResult.Value;
            }
            if (!base.Schema.ResolveTypeName(this, this.UnresolvedBaseType, out type))
            {
                this._baseTypeResolveResult = false;
                return this._baseTypeResolveResult.Value;
            }
            this.BaseType = type as StructuredType;
            if (this.BaseType == null)
            {
                base.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, Strings.InvalidBaseTypeForStructuredType(this.UnresolvedBaseType, this.FQName));
                this._baseTypeResolveResult = false;
                return this._baseTypeResolveResult.Value;
            }
            if (this.CheckForInheritanceCycle())
            {
                this.BaseType = null;
                base.AddError(ErrorCode.CycleInTypeHierarchy, EdmSchemaErrorSeverity.Error, Strings.CycleInTypeHierarchy(this.FQName));
                this._baseTypeResolveResult = false;
                return this._baseTypeResolveResult.Value;
            }
            this._baseTypeResolveResult = true;
            return true;
        }

        internal override void Validate()
        {
            base.Validate();
            foreach (SchemaElement element in this.NamedMembers)
            {
                if (this.BaseType != null)
                {
                    StructuredType type;
                    SchemaElement element2;
                    string message = null;
                    switch (this.BaseType.DefinesMemberName(element.Name, out type, out element2))
                    {
                        case HowDefined.AsMember:
                            message = Strings.DuplicateMemberName(element.Name, this.FQName, type.FQName);
                            break;

                        case HowDefined.AsMemberWithDifferentCase:
                            message = Strings.EquivalentMemberName(element.Name, this.FQName, type.FQName, element2.Name);
                            break;
                    }
                    if (message != null)
                    {
                        element.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, message);
                    }
                }
                element.Validate();
            }
        }

        public StructuredType BaseType
        {
            get => 
                this._baseType;
            private set
            {
                this._baseType = value;
            }
        }

        public bool IsAbstract =>
            this._isAbstract;

        public virtual bool IsTypeHierarchyRoot =>
            (this.BaseType == null);

        protected SchemaElementLookUpTable<SchemaElement> NamedMembers
        {
            get
            {
                if (this._namedMembers == null)
                {
                    this._namedMembers = new SchemaElementLookUpTable<SchemaElement>();
                }
                return this._namedMembers;
            }
        }

        public ISchemaElementLookUpTable<StructuredProperty> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new FilteredSchemaElementLookUpTable<StructuredProperty, SchemaElement>(this.NamedMembers);
                }
                return this._properties;
            }
        }

        protected string UnresolvedBaseType
        {
            get => 
                this._unresolvedBaseType;
            set
            {
                this._unresolvedBaseType = value;
            }
        }

        private enum HowDefined
        {
            NotDefined,
            AsMember,
            AsMemberWithDifferentCase
        }
    }
}

