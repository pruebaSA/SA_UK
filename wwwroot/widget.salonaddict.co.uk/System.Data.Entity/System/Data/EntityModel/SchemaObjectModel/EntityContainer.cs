namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Xml;

    [DebuggerDisplay("Name={Name}")]
    internal sealed class EntityContainer : SchemaType
    {
        private System.Data.EntityModel.SchemaObjectModel.EntityContainer _entityContainerGettingExtended;
        private ISchemaElementLookUpTable<EntityContainerEntitySet> _entitySets;
        private ISchemaElementLookUpTable<Function> _functionImports;
        private bool _isAlreadyResolved;
        private bool _isAlreadyValidated;
        private SchemaElementLookUpTable<SchemaElement> _members;
        private ISchemaElementLookUpTable<EntityContainerRelationshipSet> _relationshipSets;
        private string _unresolvedExtendedEntityContainerName;

        public EntityContainer(Schema parentElement) : base(parentElement)
        {
        }

        private static bool AreRelationshipEndsEqual(EntityContainerRelationshipSetEnd left, EntityContainerRelationshipSetEnd right) => 
            ((object.ReferenceEquals(left.EntitySet, right.EntitySet) && object.ReferenceEquals(left.ParentElement.Relationship, right.ParentElement.Relationship)) && (left.Name == right.Name));

        private void CheckForDuplicateTableMapping(HashSet<string> tableKeys, EntityContainerEntitySet entitySet)
        {
            string name;
            string table;
            if (string.IsNullOrEmpty(entitySet.DbSchema))
            {
                name = this.Name;
            }
            else
            {
                name = entitySet.DbSchema;
            }
            if (string.IsNullOrEmpty(entitySet.Table))
            {
                table = entitySet.Name;
            }
            else
            {
                table = entitySet.Table;
            }
            string item = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { name, table });
            if (entitySet.DefiningQuery != null)
            {
                item = entitySet.Name;
            }
            if (!tableKeys.Add(item))
            {
                entitySet.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, Strings.DuplicateEntitySetTable(entitySet.Name, name, table));
            }
        }

        private void DuplicateOrEquivalentMemberNameWhileExtendingEntityContainer(SchemaElement schemaElement, AddErrorKind error)
        {
            if (error != AddErrorKind.Succeeded)
            {
                schemaElement.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, Strings.DuplicateMemberNameInExtendedEntityContainer(schemaElement.Name, this.ExtendingEntityContainer.Name, this.Name));
            }
        }

        internal EntityContainerEntitySet FindEntitySet(string name)
        {
            for (System.Data.EntityModel.SchemaObjectModel.EntityContainer container = this; container != null; container = container.ExtendingEntityContainer)
            {
                foreach (EntityContainerEntitySet set in container.EntitySets)
                {
                    if (Utils.CompareNames(set.Name, name) == 0)
                    {
                        return set;
                    }
                }
            }
            return null;
        }

        private void HandleAssociationSetElement(XmlReader reader)
        {
            EntityContainerAssociationSet type = new EntityContainerAssociationSet(this);
            type.Parse(reader);
            this.Members.Add(type, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Extends"))
            {
                this.HandleExtendsAttribute(reader);
                return true;
            }
            return false;
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "EntitySet"))
            {
                this.HandleEntitySetElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "AssociationSet"))
            {
                this.HandleAssociationSetElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "FunctionImport"))
            {
                this.HandleFunctionImport(reader);
                return true;
            }
            return false;
        }

        private void HandleEntitySetElement(XmlReader reader)
        {
            EntityContainerEntitySet type = new EntityContainerEntitySet(this);
            type.Parse(reader);
            this.Members.Add(type, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
        }

        private void HandleExtendsAttribute(XmlReader reader)
        {
            this._unresolvedExtendedEntityContainerName = base.HandleUndottedNameAttribute(reader, this._unresolvedExtendedEntityContainerName);
        }

        private void HandleFunctionImport(XmlReader reader)
        {
            FunctionImportElement type = new FunctionImportElement(this);
            type.Parse(reader);
            this.Members.Add(type, true, new Func<object, string>(Strings.DuplicateEntityContainerMemberName));
        }

        internal override void ResolveSecondLevelNames()
        {
            base.ResolveSecondLevelNames();
            foreach (SchemaElement element in this.Members)
            {
                element.ResolveSecondLevelNames();
            }
        }

        internal override void ResolveTopLevelNames()
        {
            if (!this._isAlreadyResolved)
            {
                base.ResolveTopLevelNames();
                if (!string.IsNullOrEmpty(this._unresolvedExtendedEntityContainerName))
                {
                    if (this._unresolvedExtendedEntityContainerName == this.Name)
                    {
                        base.AddError(ErrorCode.EntityContainerCannotExtendItself, EdmSchemaErrorSeverity.Error, Strings.EntityContainerCannotExtendItself(this.Name));
                    }
                    else
                    {
                        SchemaType type;
                        if (!base.Schema.SchemaManager.TryResolveType(null, this._unresolvedExtendedEntityContainerName, out type))
                        {
                            base.AddError(ErrorCode.InvalidEntityContainerNameInExtends, EdmSchemaErrorSeverity.Error, Strings.InvalidEntityContainerNameInExtends(this._unresolvedExtendedEntityContainerName));
                        }
                        else
                        {
                            this._entityContainerGettingExtended = (System.Data.EntityModel.SchemaObjectModel.EntityContainer) type;
                            this._entityContainerGettingExtended.ResolveTopLevelNames();
                        }
                    }
                }
                foreach (SchemaElement element in this.Members)
                {
                    element.ResolveTopLevelNames();
                }
                this._isAlreadyResolved = true;
            }
        }

        private static bool TypeDefinesNewConcurrencyProperties(SchemaEntityType itemType)
        {
            foreach (StructuredProperty property in itemType.Properties)
            {
                if ((property.Type is ScalarType) && (MetadataHelper.GetConcurrencyMode(property.TypeUsage) != ConcurrencyMode.None))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TypeIsSubTypeOf(SchemaEntityType itemType, Dictionary<SchemaEntityType, EntityContainerEntitySet> baseEntitySetTypes, out EntityContainerEntitySet set)
        {
            if (itemType.IsTypeHierarchyRoot)
            {
                set = null;
                return false;
            }
            for (SchemaEntityType type = itemType.BaseType as SchemaEntityType; type != null; type = type.BaseType as SchemaEntityType)
            {
                if (baseEntitySetTypes.ContainsKey(type))
                {
                    set = baseEntitySetTypes[type];
                    return true;
                }
            }
            set = null;
            return false;
        }

        internal override void Validate()
        {
            if (!this._isAlreadyValidated)
            {
                base.Validate();
                if (this.ExtendingEntityContainer != null)
                {
                    this.ExtendingEntityContainer.Validate();
                    foreach (SchemaElement element in this.ExtendingEntityContainer.Members)
                    {
                        AddErrorKind error = this.Members.TryAdd(element.Clone(this));
                        this.DuplicateOrEquivalentMemberNameWhileExtendingEntityContainer(element, error);
                    }
                }
                HashSet<string> tableKeys = new HashSet<string>();
                foreach (SchemaElement element2 in this.Members)
                {
                    EntityContainerEntitySet entitySet = element2 as EntityContainerEntitySet;
                    if ((entitySet != null) && (base.Schema.DataModel == SchemaDataModelOption.ProviderDataModel))
                    {
                        this.CheckForDuplicateTableMapping(tableKeys, entitySet);
                    }
                    element2.Validate();
                }
                this.ValidateRelationshipSetHaveUniqueEnds();
                this.ValidateOnlyBaseEntitySetTypeDefinesConcurrency();
                this._isAlreadyValidated = true;
            }
        }

        private void ValidateOnlyBaseEntitySetTypeDefinesConcurrency()
        {
            Dictionary<SchemaEntityType, EntityContainerEntitySet> baseEntitySetTypes = new Dictionary<SchemaEntityType, EntityContainerEntitySet>();
            foreach (SchemaElement element in this.Members)
            {
                EntityContainerEntitySet set = element as EntityContainerEntitySet;
                if ((set != null) && !baseEntitySetTypes.ContainsKey(set.EntityType))
                {
                    baseEntitySetTypes.Add(set.EntityType, set);
                }
            }
            foreach (SchemaType type in base.Schema.SchemaTypes)
            {
                EntityContainerEntitySet set2;
                SchemaEntityType itemType = type as SchemaEntityType;
                if (((itemType != null) && TypeIsSubTypeOf(itemType, baseEntitySetTypes, out set2)) && TypeDefinesNewConcurrencyProperties(itemType))
                {
                    base.AddError(ErrorCode.ConcurrencyRedefinedOnSubTypeOfEntitySetType, EdmSchemaErrorSeverity.Error, Strings.ConcurrencyRedefinedOnSubTypeOfEntitySetType(itemType.FQName, set2.EntityType.FQName, set2.FQName));
                }
            }
        }

        private void ValidateRelationshipSetHaveUniqueEnds()
        {
            List<EntityContainerRelationshipSetEnd> list = new List<EntityContainerRelationshipSetEnd>();
            bool flag = true;
            foreach (EntityContainerRelationshipSet set in this.RelationshipSets)
            {
                foreach (EntityContainerRelationshipSetEnd end in set.Ends)
                {
                    flag = false;
                    foreach (EntityContainerRelationshipSetEnd end2 in list)
                    {
                        if (AreRelationshipEndsEqual(end2, end))
                        {
                            base.AddError(ErrorCode.SimilarRelationshipEnd, EdmSchemaErrorSeverity.Error, Strings.SimilarRelationshipEnd(end2.Name, end2.ParentElement.FQName, end.ParentElement.FQName, end2.EntitySet.FQName, this.FQName));
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        list.Add(end);
                    }
                }
            }
        }

        public ISchemaElementLookUpTable<EntityContainerEntitySet> EntitySets
        {
            get
            {
                if (this._entitySets == null)
                {
                    this._entitySets = new FilteredSchemaElementLookUpTable<EntityContainerEntitySet, SchemaElement>(this.Members);
                }
                return this._entitySets;
            }
        }

        public System.Data.EntityModel.SchemaObjectModel.EntityContainer ExtendingEntityContainer =>
            this._entityContainerGettingExtended;

        public override string FQName =>
            this.Name;

        public ISchemaElementLookUpTable<Function> FunctionImports
        {
            get
            {
                if (this._functionImports == null)
                {
                    this._functionImports = new FilteredSchemaElementLookUpTable<Function, SchemaElement>(this.Members);
                }
                return this._functionImports;
            }
        }

        public override string Identity =>
            this.Name;

        private SchemaElementLookUpTable<SchemaElement> Members
        {
            get
            {
                if (this._members == null)
                {
                    this._members = new SchemaElementLookUpTable<SchemaElement>();
                }
                return this._members;
            }
        }

        public ISchemaElementLookUpTable<EntityContainerRelationshipSet> RelationshipSets
        {
            get
            {
                if (this._relationshipSets == null)
                {
                    this._relationshipSets = new FilteredSchemaElementLookUpTable<EntityContainerRelationshipSet, SchemaElement>(this.Members);
                }
                return this._relationshipSets;
            }
        }
    }
}

