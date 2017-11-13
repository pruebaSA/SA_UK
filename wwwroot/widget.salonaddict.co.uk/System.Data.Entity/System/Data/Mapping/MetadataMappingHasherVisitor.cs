namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal class MetadataMappingHasherVisitor : BaseMetadataMappingVisitor
    {
        private CompressingHashBuilder m_hashSourceBuilder = new CompressingHashBuilder();
        private int m_instanceNumber;
        private ItemCollection m_itemCollection;
        private Dictionary<object, int> m_itemsAlreadySeen = new Dictionary<object, int>();

        private MetadataMappingHasherVisitor(ItemCollection edmItemCollection)
        {
            this.m_itemCollection = edmItemCollection;
        }

        private void AddObjectContentToHashBuilder(object content)
        {
            if (content != null)
            {
                this.m_hashSourceBuilder.AppendLine(content.ToString());
            }
            else
            {
                this.m_hashSourceBuilder.AppendLine("NULL");
            }
        }

        private void AddObjectEndDumpToHashBuilder()
        {
            this.m_hashSourceBuilder.AppendObjectEndDump();
        }

        private void AddObjectStartDumpToHashBuilder(object o, int objectIndex)
        {
            this.m_hashSourceBuilder.AppendObjectStartDump(o, objectIndex);
        }

        private bool AddObjectToSeenListAndHashBuilder(object o, out int instanceIndex)
        {
            if (o == null)
            {
                instanceIndex = -1;
                return false;
            }
            if (!this.TryAddSeenItem(o, out instanceIndex))
            {
                this.AddObjectStartDumpToHashBuilder(o, instanceIndex);
                this.AddSeenObjectToHashBuilder(o, instanceIndex);
                this.AddObjectEndDumpToHashBuilder();
                return false;
            }
            return true;
        }

        private void AddSeenObjectToHashBuilder(object o, int instanceIndex)
        {
            this.m_hashSourceBuilder.AppendLine("Instance Reference: " + instanceIndex);
        }

        private void Clean()
        {
            this.m_hashSourceBuilder = new CompressingHashBuilder();
            this.m_instanceNumber = 0;
            this.m_itemsAlreadySeen = new Dictionary<object, int>();
        }

        internal static string GetMappingClosureHash(StorageEntityContainerMapping storageEntityContainerMapping, ItemCollection itemCollection)
        {
            MetadataMappingHasherVisitor visitor = new MetadataMappingHasherVisitor(itemCollection);
            visitor.Visit(storageEntityContainerMapping);
            return visitor.HashValue;
        }

        private bool TryAddSeenItem(object o, out int indexSeen)
        {
            if (!this.m_itemsAlreadySeen.TryGetValue(o, out indexSeen))
            {
                this.m_itemsAlreadySeen.Add(o, this.m_instanceNumber);
                indexSeen = this.m_instanceNumber;
                this.m_instanceNumber++;
                return true;
            }
            return false;
        }

        protected override void Visit(DbProviderManifest providerManifest)
        {
        }

        protected override void Visit(StorageComplexPropertyMapping storageComplexPropertyMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageComplexPropertyMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageComplexPropertyMapping, num);
                base.Visit(storageComplexPropertyMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StorageComplexTypeMapping storageComplexTypeMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageComplexTypeMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageComplexTypeMapping, num);
                base.Visit(storageComplexTypeMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StorageConditionPropertyMapping storageConditionPropertyMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageConditionPropertyMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageConditionPropertyMapping, num);
                this.AddObjectContentToHashBuilder(storageConditionPropertyMapping.IsNull);
                this.AddObjectContentToHashBuilder(storageConditionPropertyMapping.Value);
                base.Visit(storageConditionPropertyMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StorageEntityContainerMapping storageEntityContainerMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageEntityContainerMapping, out num))
            {
                if (this.m_itemsAlreadySeen.Count > 1)
                {
                    this.Clean();
                    this.Visit(storageEntityContainerMapping);
                }
                else
                {
                    this.AddObjectStartDumpToHashBuilder(storageEntityContainerMapping, num);
                    this.AddObjectContentToHashBuilder(storageEntityContainerMapping.Identity);
                    base.Visit(storageEntityContainerMapping);
                    this.AddObjectEndDumpToHashBuilder();
                }
            }
        }

        protected override void Visit(StorageMappingFragment storageMappingFragment)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageMappingFragment, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageMappingFragment, num);
                base.Visit(storageMappingFragment);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StoragePropertyMapping storagePropertyMapping)
        {
            base.Visit(storagePropertyMapping);
        }

        protected override void Visit(StorageScalarPropertyMapping storageScalarPropertyMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageScalarPropertyMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageScalarPropertyMapping, num);
                base.Visit(storageScalarPropertyMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StorageSetMapping storageSetMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageSetMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageSetMapping, num);
                base.Visit(storageSetMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(StorageTypeMapping storageTypeMapping)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(storageTypeMapping, out num))
            {
                this.AddObjectStartDumpToHashBuilder(storageTypeMapping, num);
                base.Visit(storageTypeMapping);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(AssociationEndMember associationEndMember)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(associationEndMember, out num))
            {
                this.AddObjectStartDumpToHashBuilder(associationEndMember, num);
                this.AddObjectContentToHashBuilder(associationEndMember.DeleteBehavior);
                this.AddObjectContentToHashBuilder(associationEndMember.Identity);
                this.AddObjectContentToHashBuilder(associationEndMember.IsStoreGeneratedComputed);
                this.AddObjectContentToHashBuilder(associationEndMember.IsStoreGeneratedIdentity);
                this.AddObjectContentToHashBuilder(associationEndMember.RelationshipMultiplicity);
                base.Visit(associationEndMember);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(AssociationSet associationSet)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(associationSet, out num))
            {
                this.AddObjectStartDumpToHashBuilder(associationSet, num);
                this.AddObjectContentToHashBuilder(associationSet.CachedProviderSql);
                this.AddObjectContentToHashBuilder(associationSet.Identity);
                this.AddObjectContentToHashBuilder(associationSet.Schema);
                this.AddObjectContentToHashBuilder(associationSet.Table);
                base.Visit(associationSet);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(AssociationSetEnd associationSetEnd)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(associationSetEnd, out num))
            {
                this.AddObjectStartDumpToHashBuilder(associationSetEnd, num);
                this.AddObjectContentToHashBuilder(associationSetEnd.Identity);
                base.Visit(associationSetEnd);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(AssociationType associationType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(associationType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(associationType, num);
                this.AddObjectContentToHashBuilder(associationType.Abstract);
                this.AddObjectContentToHashBuilder(associationType.Identity);
                base.Visit(associationType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(CollectionType collectionType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(collectionType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(collectionType, num);
                this.AddObjectContentToHashBuilder(collectionType.Identity);
                base.Visit(collectionType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(ComplexType complexType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(complexType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(complexType, num);
                this.AddObjectContentToHashBuilder(complexType.Abstract);
                this.AddObjectContentToHashBuilder(complexType.Identity);
                base.Visit(complexType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EdmFunction edmFunction)
        {
        }

        protected override void Visit(EdmMember edmMember)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(edmMember, out num))
            {
                this.AddObjectStartDumpToHashBuilder(edmMember, num);
                this.AddObjectContentToHashBuilder(edmMember.Identity);
                this.AddObjectContentToHashBuilder(edmMember.IsStoreGeneratedComputed);
                this.AddObjectContentToHashBuilder(edmMember.IsStoreGeneratedIdentity);
                base.Visit(edmMember);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EdmProperty edmProperty)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(edmProperty, out num))
            {
                this.AddObjectStartDumpToHashBuilder(edmProperty, num);
                this.AddObjectContentToHashBuilder(edmProperty.DefaultValue);
                this.AddObjectContentToHashBuilder(edmProperty.Identity);
                this.AddObjectContentToHashBuilder(edmProperty.IsStoreGeneratedComputed);
                this.AddObjectContentToHashBuilder(edmProperty.IsStoreGeneratedIdentity);
                this.AddObjectContentToHashBuilder(edmProperty.Nullable);
                base.Visit(edmProperty);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EdmType edmType)
        {
            base.Visit(edmType);
        }

        protected override void Visit(EntityContainer entityContainer)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(entityContainer, out num))
            {
                this.AddObjectStartDumpToHashBuilder(entityContainer, num);
                this.AddObjectContentToHashBuilder(entityContainer.Identity);
                base.Visit(entityContainer);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EntitySet entitySet)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(entitySet, out num))
            {
                this.AddObjectStartDumpToHashBuilder(entitySet, num);
                this.AddObjectContentToHashBuilder(entitySet.Name);
                this.AddObjectContentToHashBuilder(entitySet.Schema);
                this.AddObjectContentToHashBuilder(entitySet.Table);
                base.Visit(entitySet);
                foreach (EdmType type in from type in MetadataHelper.GetTypeAndSubtypesOf(entitySet.ElementType, this.m_itemCollection, false)
                    where type != entitySet.ElementType
                    select type)
                {
                    this.Visit(type);
                }
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EntitySetBase entitySetBase)
        {
            base.Visit(entitySetBase);
        }

        protected override void Visit(EntityType entityType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(entityType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(entityType, num);
                this.AddObjectContentToHashBuilder(entityType.Abstract);
                this.AddObjectContentToHashBuilder(entityType.Identity);
                base.Visit(entityType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(EntityTypeBase entityTypeBase)
        {
            base.Visit(entityTypeBase);
        }

        protected override void Visit(EnumMember enumMember)
        {
        }

        protected override void Visit(EnumType enumType)
        {
        }

        protected override void Visit(Facet facet)
        {
            int num;
            if ((facet.Name == "Nullable") && this.AddObjectToSeenListAndHashBuilder(facet, out num))
            {
                this.AddObjectStartDumpToHashBuilder(facet, num);
                this.AddObjectContentToHashBuilder(facet.Identity);
                this.AddObjectContentToHashBuilder(facet.Value);
                base.Visit(facet);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(FunctionParameter functionParameter)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(functionParameter, out num))
            {
                this.AddObjectStartDumpToHashBuilder(functionParameter, num);
                this.AddObjectContentToHashBuilder(functionParameter.Identity);
                this.AddObjectContentToHashBuilder(functionParameter.Mode);
                base.Visit(functionParameter);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(NavigationProperty navigationProperty)
        {
        }

        protected override void Visit(PrimitiveType primitiveType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(primitiveType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(primitiveType, num);
                this.AddObjectContentToHashBuilder(primitiveType.Name);
                this.AddObjectContentToHashBuilder(primitiveType.NamespaceName);
                base.Visit(primitiveType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(ReferentialConstraint referentialConstraint)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(referentialConstraint, out num))
            {
                this.AddObjectStartDumpToHashBuilder(referentialConstraint, num);
                this.AddObjectContentToHashBuilder(referentialConstraint.Identity);
                base.Visit(referentialConstraint);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(RefType refType)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(refType, out num))
            {
                this.AddObjectStartDumpToHashBuilder(refType, num);
                this.AddObjectContentToHashBuilder(refType.Identity);
                base.Visit(refType);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(RelationshipEndMember relationshipEndMember)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(relationshipEndMember, out num))
            {
                this.AddObjectStartDumpToHashBuilder(relationshipEndMember, num);
                this.AddObjectContentToHashBuilder(relationshipEndMember.DeleteBehavior);
                this.AddObjectContentToHashBuilder(relationshipEndMember.Identity);
                this.AddObjectContentToHashBuilder(relationshipEndMember.IsStoreGeneratedComputed);
                this.AddObjectContentToHashBuilder(relationshipEndMember.IsStoreGeneratedIdentity);
                this.AddObjectContentToHashBuilder(relationshipEndMember.RelationshipMultiplicity);
                base.Visit(relationshipEndMember);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        protected override void Visit(RelationshipType relationshipType)
        {
            base.Visit(relationshipType);
        }

        protected override void Visit(TypeUsage typeUsage)
        {
            int num;
            if (this.AddObjectToSeenListAndHashBuilder(typeUsage, out num))
            {
                this.AddObjectStartDumpToHashBuilder(typeUsage, num);
                base.Visit(typeUsage);
                this.AddObjectEndDumpToHashBuilder();
            }
        }

        internal string HashValue =>
            this.m_hashSourceBuilder.ComputeHash();
    }
}

