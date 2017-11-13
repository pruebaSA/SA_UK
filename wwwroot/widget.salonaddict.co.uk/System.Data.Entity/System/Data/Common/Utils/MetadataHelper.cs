namespace System.Data.Common.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal static class MetadataHelper
    {
        private static void AddDirectedEdgeBetweenEntityTypes(Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> graph, EntityType a, EntityType b)
        {
            System.Data.Common.Utils.Set<EntityType> set;
            if (graph.ContainsKey(a))
            {
                set = graph[a];
            }
            else
            {
                set = new System.Data.Common.Utils.Set<EntityType>();
                graph.Add(a, set);
            }
            set.Add(b);
        }

        internal static Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> BuildUndirectedGraphOfTypes(MetadataWorkspace workspace)
        {
            ItemCollection itemCollection = ExternalCalls.GetItemCollection(workspace, DataSpace.CSpace);
            Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>> graph = new Dictionary<EntityType, System.Data.Common.Utils.Set<EntityType>>();
            foreach (EntityType type in itemCollection.GetItems<EntityType>())
            {
                if (type.BaseType != null)
                {
                    EntityType baseType = type.BaseType as EntityType;
                    AddDirectedEdgeBetweenEntityTypes(graph, type, baseType);
                    AddDirectedEdgeBetweenEntityTypes(graph, baseType, type);
                }
            }
            return graph;
        }

        internal static bool CheckIfAllErrorsAreWarnings(IList<EdmSchemaError> schemaErrors)
        {
            int count = schemaErrors.Count;
            for (int i = 0; i < count; i++)
            {
                EdmSchemaError error = schemaErrors[i];
                if (error.Severity != EdmSchemaErrorSeverity.Warning)
                {
                    return false;
                }
            }
            return true;
        }

        internal static TypeUsage ConvertStoreTypeUsageToEdmTypeUsage(TypeUsage storeTypeUsage) => 
            storeTypeUsage.GetModelTypeUsage().ShallowCopy(FacetValues.NullFacetValues);

        internal static bool DoesEndFormKey(AssociationSet associationSet, AssociationEndMember end)
        {
            foreach (AssociationEndMember member in associationSet.ElementType.Members)
            {
                if (!member.Equals(end) && (member.RelationshipMultiplicity == RelationshipMultiplicity.Many))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool DoesEndKeySubsumeAssociationSetKey(AssociationSet assocSet, AssociationEndMember thisEnd, HashSet<Pair<EdmMember, EntityType>> associationkeys)
        {
            AssociationType elementType = assocSet.ElementType;
            EntityType thisEndsEntityType = (EntityType) ((RefType) thisEnd.TypeUsage.EdmType).ElementType;
            HashSet<Pair<EdmMember, EntityType>> other = new HashSet<Pair<EdmMember, EntityType>>(from edmMember in thisEndsEntityType.KeyMembers select new Pair<EdmMember, EntityType>(edmMember, thisEndsEntityType));
            foreach (ReferentialConstraint constraint in elementType.ReferentialConstraints)
            {
                IEnumerable<EdmMember> enumerable;
                EntityType type2;
                if (thisEnd.Equals((AssociationEndMember) constraint.ToRole))
                {
                    enumerable = Helpers.AsSuperTypeList<EdmProperty, EdmMember>(constraint.FromProperties);
                    type2 = (EntityType) ((RefType) ((AssociationEndMember) constraint.FromRole).TypeUsage.EdmType).ElementType;
                }
                else
                {
                    if (!thisEnd.Equals((AssociationEndMember) constraint.FromRole))
                    {
                        continue;
                    }
                    enumerable = Helpers.AsSuperTypeList<EdmProperty, EdmMember>(constraint.ToProperties);
                    type2 = (EntityType) ((RefType) ((AssociationEndMember) constraint.ToRole).TypeUsage.EdmType).ElementType;
                }
                foreach (EdmMember member in enumerable)
                {
                    associationkeys.Remove(new Pair<EdmMember, EntityType>(member, type2));
                }
            }
            return associationkeys.IsSubsetOf(other);
        }

        internal static bool DoesMemberExist(StructuralType type, EdmMember member)
        {
            foreach (EdmMember member2 in type.Members)
            {
                if (member2.Equals(member))
                {
                    return true;
                }
            }
            return false;
        }

        internal static string GenerateHashForAllExtentViewsContent(IEnumerable<KeyValuePair<string, string>> extentViews)
        {
            CompressingHashBuilder builder = new CompressingHashBuilder();
            foreach (KeyValuePair<string, string> pair in extentViews)
            {
                builder.AppendLine(pair.Key);
                builder.AppendLine(pair.Value);
            }
            return builder.ComputeHash();
        }

        internal static List<AssociationSet> GetAssociationsForEntitySet(EntitySet entitySet)
        {
            List<AssociationSet> list = new List<AssociationSet>();
            foreach (EntitySetBase base2 in entitySet.EntityContainer.BaseEntitySets)
            {
                if (Helper.IsRelationshipSet(base2))
                {
                    AssociationSet relationshipSet = (AssociationSet) base2;
                    if (IsExtentAtSomeRelationshipEnd(relationshipSet, entitySet))
                    {
                        list.Add(relationshipSet);
                    }
                }
            }
            return list;
        }

        internal static AssociationSet GetAssociationsForEntitySetAndAssociationType(EntityContainer entityContainer, string entitySetName, AssociationType associationType, string endName, out EntitySet entitySet)
        {
            entitySet = null;
            AssociationSet set = null;
            ReadOnlyMetadataCollection<EntitySetBase> baseEntitySets = entityContainer.BaseEntitySets;
            int count = baseEntitySets.Count;
            for (int i = 0; i < count; i++)
            {
                EntitySetBase base2 = baseEntitySets[i];
                if (object.ReferenceEquals(base2.ElementType, associationType))
                {
                    AssociationSet set2 = (AssociationSet) base2;
                    EntitySet set3 = set2.AssociationSetEnds[endName].EntitySet;
                    if (set3.Name == entitySetName)
                    {
                        set = set2;
                        entitySet = set3;
                        return set;
                    }
                }
            }
            return set;
        }

        internal static List<AssociationSet> GetAssociationsForEntitySets(EntitySet entitySet1, EntitySet entitySet2)
        {
            List<AssociationSet> list = new List<AssociationSet>();
            foreach (EntitySetBase base2 in entitySet1.EntityContainer.BaseEntitySets)
            {
                if (Helper.IsRelationshipSet(base2))
                {
                    AssociationSet relationshipSet = (AssociationSet) base2;
                    if (IsExtentAtSomeRelationshipEnd(relationshipSet, entitySet1) && IsExtentAtSomeRelationshipEnd(relationshipSet, entitySet2))
                    {
                        list.Add(relationshipSet);
                    }
                }
            }
            return list;
        }

        internal static System.Data.Common.Utils.Set<EdmMember> GetConcurrencyMembersForTypeHierarchy(EntityTypeBase superType, MetadataWorkspace workspace)
        {
            System.Data.Common.Utils.Set<EdmMember> set = new System.Data.Common.Utils.Set<EdmMember>();
            foreach (StructuralType type in GetTypeAndSubtypesOf(superType, workspace, true))
            {
                foreach (EdmMember member in type.Members)
                {
                    if (GetConcurrencyMode(member) == ConcurrencyMode.Fixed)
                    {
                        set.Add(member);
                    }
                }
            }
            return set;
        }

        internal static ConcurrencyMode GetConcurrencyMode(EdmMember member) => 
            GetConcurrencyMode(member.TypeUsage);

        internal static ConcurrencyMode GetConcurrencyMode(TypeUsage typeUsage)
        {
            Facet facet;
            if (typeUsage.Facets.TryGetValue("ConcurrencyMode", false, out facet) && (facet.Value != null))
            {
                return (ConcurrencyMode) facet.Value;
            }
            return ConcurrencyMode.None;
        }

        internal static TypeUsage GetElementType(TypeUsage typeUsage)
        {
            if (BuiltInTypeKind.CollectionType == typeUsage.EdmType.BuiltInTypeKind)
            {
                return GetElementType(((CollectionType) typeUsage.EdmType).TypeUsage);
            }
            return typeUsage;
        }

        internal static EntitySet GetEntitySetAtEnd(AssociationSet associationSet, AssociationEndMember endMember) => 
            associationSet.AssociationSetEnds[endMember.Name].EntitySet;

        internal static EntityType GetEntityTypeForEnd(AssociationEndMember end)
        {
            RefType edmType = (RefType) end.TypeUsage.EdmType;
            return (EntityType) edmType.ElementType;
        }

        internal static int GetLowerBoundOfMultiplicity(RelationshipMultiplicity multiplicity)
        {
            if ((multiplicity != RelationshipMultiplicity.Many) && (multiplicity != RelationshipMultiplicity.ZeroOrOne))
            {
                return 1;
            }
            return 0;
        }

        internal static AssociationSetEnd GetOppositeEnd(AssociationSetEnd end) => 
            (from e in end.ParentAssociationSet.AssociationSetEnds
                where !e.EdmEquals(end)
                select e).Single<AssociationSetEnd>();

        internal static AssociationEndMember GetOtherAssociationEnd(AssociationEndMember endMember)
        {
            ReadOnlyMetadataCollection<EdmMember> members = endMember.DeclaringType.Members;
            EdmMember objB = members[0];
            if (!object.ReferenceEquals(endMember, objB))
            {
                return (AssociationEndMember) objB;
            }
            return (AssociationEndMember) members[1];
        }

        internal static PrimitiveTypeKind GetPrimitiveTypeKind(TypeUsage typeUsage)
        {
            PrimitiveType edmType = (PrimitiveType) typeUsage.EdmType;
            return edmType.PrimitiveTypeKind;
        }

        internal static AssociationEndMember GetSomeEndForEntitySet(AssociationSet associationSet, EntitySet entitySet)
        {
            foreach (AssociationSetEnd end in associationSet.AssociationSetEnds)
            {
                if (end.EntitySet.Equals(entitySet))
                {
                    return end.CorrespondingAssociationEndMember;
                }
            }
            return null;
        }

        internal static StoreGeneratedPattern GetStoreGeneratedPattern(EdmMember member)
        {
            Facet facet;
            if (member.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet) && (facet.Value != null))
            {
                return (StoreGeneratedPattern) facet.Value;
            }
            return StoreGeneratedPattern.None;
        }

        internal static IEnumerable<EdmType> GetTypeAndParentTypesOf(EdmType type, ItemCollection itemCollection, bool includeAbstractTypes)
        {
            if (Helper.IsRefType(type))
            {
                type = ((RefType) type).ElementType;
            }
            for (EdmType iteratorVariable0 = type; iteratorVariable0 != null; iteratorVariable0 = iteratorVariable0.BaseType as EntityType)
            {
                if (includeAbstractTypes || !iteratorVariable0.Abstract)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        internal static IEnumerable<EdmType> GetTypeAndSubtypesOf(EdmType type, ItemCollection itemCollection, bool includeAbstractTypes)
        {
            if (Helper.IsRefType(type))
            {
                type = ((RefType) type).ElementType;
            }
            if (includeAbstractTypes || !type.Abstract)
            {
                yield return type;
            }
            IEnumerator<EdmType> enumerator = GetTypeAndSubtypesOf<EntityType>(type, itemCollection, includeAbstractTypes).GetEnumerator();
            while (enumerator.MoveNext())
            {
                EdmType current = enumerator.Current;
                yield return current;
            }
            foreach (EdmType iteratorVariable1 in GetTypeAndSubtypesOf<ComplexType>(type, itemCollection, includeAbstractTypes))
            {
                yield return iteratorVariable1;
            }
        }

        private static IEnumerable<EdmType> GetTypeAndSubtypesOf<T_EdmType>(EdmType type, ItemCollection itemCollection, bool includeAbstractTypes) where T_EdmType: EdmType
        {
            T_EdmType secondType = type as T_EdmType;
            if (secondType != null)
            {
                IEnumerable<T_EdmType> items = itemCollection.GetItems<T_EdmType>();
                foreach (T_EdmType iteratorVariable2 in items)
                {
                    if ((!secondType.Equals(iteratorVariable2) && Helper.IsSubtypeOf(iteratorVariable2, secondType)) && (includeAbstractTypes || !iteratorVariable2.Abstract))
                    {
                        yield return iteratorVariable2;
                    }
                }
            }
        }

        internal static IEnumerable<EdmType> GetTypeAndSubtypesOf(EdmType type, MetadataWorkspace workspace, bool includeAbstractTypes) => 
            GetTypeAndSubtypesOf(type, ExternalCalls.GetItemCollection(workspace, DataSpace.CSpace), includeAbstractTypes);

        internal static int? GetUpperBoundOfMultiplicity(RelationshipMultiplicity multiplicity)
        {
            if ((multiplicity != RelationshipMultiplicity.One) && (multiplicity != RelationshipMultiplicity.ZeroOrOne))
            {
                return null;
            }
            return 1;
        }

        internal static bool HasDiscreteDomain(EdmType edmType) => 
            ((edmType.BuiltInTypeKind == BuiltInTypeKind.EnumType) || (Helper.IsPrimitiveType(edmType) && (((PrimitiveType) edmType).PrimitiveTypeKind == PrimitiveTypeKind.Boolean)));

        internal static bool IsAssociationValidForEntityType(AssociationSetEnd toEnd, EntityType type) => 
            GetEntityTypeForEnd(GetOppositeEnd(toEnd).CorrespondingAssociationEndMember).IsAssignableFrom(type);

        internal static bool IsComposable(EdmFunction function)
        {
            MetadataProperty property;
            if (function.MetadataProperties.TryGetValue("IsComposableAttribute", false, out property))
            {
                return (bool) property.Value;
            }
            return true;
        }

        internal static bool IsEveryOtherEndAtLeastOne(AssociationSet associationSet, AssociationEndMember member)
        {
            foreach (AssociationSetEnd end in associationSet.AssociationSetEnds)
            {
                AssociationEndMember correspondingAssociationEndMember = end.CorrespondingAssociationEndMember;
                if (!correspondingAssociationEndMember.Equals(member) && (GetLowerBoundOfMultiplicity(correspondingAssociationEndMember.RelationshipMultiplicity) == 0))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsExtentAtSomeRelationshipEnd(AssociationSet relationshipSet, EntitySetBase extent) => 
            (Helper.IsEntitySet(extent) && (GetSomeEndForEntitySet(relationshipSet, (EntitySet) extent) != null));

        internal static bool IsMemberNullable(EdmMember member) => 
            (Helper.IsEdmProperty(member) && ((EdmProperty) member).Nullable);

        internal static bool IsNonRefSimpleMember(EdmMember member)
        {
            if (member.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
            {
                return (member.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.EnumType);
            }
            return true;
        }

        internal static bool IsParentOf(EntityType a, EntityType b)
        {
            for (EntityType type = b.BaseType as EntityType; type != null; type = type.BaseType as EntityType)
            {
                if (type.EdmEquals(a))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsPartOfEntityTypeKey(EdmMember member) => 
            ((Helper.IsEntityType(member.DeclaringType) && Helper.IsEdmProperty(member)) && ((EntityType) member.DeclaringType).KeyMembers.Contains(member));

        internal static bool IsSuperTypeOf(EdmType superType, EdmType subType)
        {
            for (EdmType type = subType; type != null; type = type.BaseType)
            {
                if (type.Equals(superType))
                {
                    return true;
                }
            }
            return false;
        }

        internal static ParameterDirection ParameterModeToParameterDirection(ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.In:
                    return ParameterDirection.Input;

                case ParameterMode.Out:
                    return ParameterDirection.Output;

                case ParameterMode.InOut:
                    return ParameterDirection.InputOutput;

                case ParameterMode.ReturnValue:
                    return ParameterDirection.ReturnValue;
            }
            return (ParameterDirection) 0;
        }

        internal static bool TryDetermineCSpaceModelType<T>(MetadataWorkspace workspace, out EntityType modelEntityType) where T: IEntityWithChangeTracker
        {
            EntityType type;
            Map map;
            workspace.LoadAssemblyForType(typeof(T), Assembly.GetCallingAssembly());
            ObjectItemCollection itemCollection = (ObjectItemCollection) workspace.GetItemCollection(DataSpace.OSpace);
            if (itemCollection.TryGetItem<EntityType>(typeof(T).FullName, out type) && workspace.TryGetMap(type, DataSpace.OCSpace, out map))
            {
                ObjectTypeMapping mapping = (ObjectTypeMapping) map;
                modelEntityType = (EntityType) mapping.EdmType;
                return true;
            }
            modelEntityType = null;
            return false;
        }

        internal static bool TryGetFunctionImportReturnEntityType(EdmFunction functionImport, out EntityType entityType)
        {
            if ((functionImport.ReturnParameter != null) && (functionImport.ReturnParameter.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType))
            {
                CollectionType edmType = (CollectionType) functionImport.ReturnParameter.TypeUsage.EdmType;
                if (edmType.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                {
                    entityType = (EntityType) edmType.TypeUsage.EdmType;
                    return true;
                }
            }
            entityType = null;
            return false;
        }



        [CompilerGenerated]
        private sealed class <GetTypeAndSubtypesOf>d__c<T_EdmType> : IEnumerable<EdmType>, IEnumerable, IEnumerator<EdmType>, IEnumerator, IDisposable where T_EdmType: EdmType
        {
            private int <>1__state;
            private EdmType <>2__current;
            public bool <>3__includeAbstractTypes;
            public ItemCollection <>3__itemCollection;
            public EdmType <>3__type;
            public IEnumerator<T_EdmType> <>7__wrap10;
            private int <>l__initialThreadId;
            public T_EdmType <specificType>5__d;
            public T_EdmType <typeInWorkSpace>5__f;
            public IEnumerable<T_EdmType> <typesInWorkSpace>5__e;
            public bool includeAbstractTypes;
            public ItemCollection itemCollection;
            public EdmType type;

            [DebuggerHidden]
            public <GetTypeAndSubtypesOf>d__c(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally11()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap10 != null)
                {
                    this.<>7__wrap10.Dispose();
                }
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<specificType>5__d = this.type as T_EdmType;
                            if (this.<specificType>5__d == null)
                            {
                                goto Label_011C;
                            }
                            this.<typesInWorkSpace>5__e = this.itemCollection.GetItems<T_EdmType>();
                            this.<>7__wrap10 = this.<typesInWorkSpace>5__e.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_0106;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_0106;

                        default:
                            goto Label_011C;
                    }
                Label_0079:
                    this.<typeInWorkSpace>5__f = this.<>7__wrap10.Current;
                    if ((!this.<specificType>5__d.Equals(this.<typeInWorkSpace>5__f) && Helper.IsSubtypeOf(this.<typeInWorkSpace>5__f, this.<specificType>5__d)) && (this.includeAbstractTypes || !this.<typeInWorkSpace>5__f.Abstract))
                    {
                        this.<>2__current = this.<typeInWorkSpace>5__f;
                        this.<>1__state = 2;
                        return true;
                    }
                Label_0106:
                    if (this.<>7__wrap10.MoveNext())
                    {
                        goto Label_0079;
                    }
                    this.<>m__Finally11();
                Label_011C:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<EdmType> IEnumerable<EdmType>.GetEnumerator()
            {
                MetadataHelper.<GetTypeAndSubtypesOf>d__c<T_EdmType> _c;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    _c = (MetadataHelper.<GetTypeAndSubtypesOf>d__c<T_EdmType>) this;
                }
                else
                {
                    _c = new MetadataHelper.<GetTypeAndSubtypesOf>d__c<T_EdmType>(0);
                }
                _c.type = this.<>3__type;
                _c.itemCollection = this.<>3__itemCollection;
                _c.includeAbstractTypes = this.<>3__includeAbstractTypes;
                return _c;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Data.Metadata.Edm.EdmType>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally11();
                        }
                        return;
                }
            }

            EdmType IEnumerator<EdmType>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

