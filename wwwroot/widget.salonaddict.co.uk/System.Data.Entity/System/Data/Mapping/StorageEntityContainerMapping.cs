namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class StorageEntityContainerMapping : Map
    {
        private string identity;
        private Dictionary<string, StorageSetMapping> m_associationSetMappings = new Dictionary<string, StorageSetMapping>(StringComparer.Ordinal);
        private EntityContainer m_entityContainer;
        private Dictionary<string, StorageSetMapping> m_entitySetMappings = new Dictionary<string, StorageSetMapping>(StringComparer.Ordinal);
        private Dictionary<EdmFunction, FunctionImportMapping> m_functionImportMappings = new Dictionary<EdmFunction, FunctionImportMapping>();
        private readonly Memoizer<InputForComputingCellGroups, OutputFromComputeCellGroups> m_memoizedCellGroupEvaluator;
        private string m_sourceLocation;
        private int m_startLineNumber;
        private int m_startLinePosition;
        private EntityContainer m_storageEntityContainer;
        private readonly System.Data.Mapping.StorageMappingItemCollection m_storageMappingItemCollection;

        internal StorageEntityContainerMapping(EntityContainer entityContainer, EntityContainer storageEntityContainer, System.Data.Mapping.StorageMappingItemCollection storageMappingItemCollection)
        {
            this.m_entityContainer = entityContainer;
            this.m_storageEntityContainer = storageEntityContainer;
            this.m_storageMappingItemCollection = storageMappingItemCollection;
            this.m_memoizedCellGroupEvaluator = new Memoizer<InputForComputingCellGroups, OutputFromComputeCellGroups>(new Func<InputForComputingCellGroups, OutputFromComputeCellGroups>(this.ComputeCellGroups), new InputForComputingCellGroups());
            this.identity = entityContainer.Identity;
        }

        internal void AddAssociationSetMapping(StorageSetMapping setMapping)
        {
            this.m_associationSetMappings.Add(setMapping.Set.Name, setMapping);
        }

        internal void AddEntitySetMapping(StorageSetMapping setMapping)
        {
            if (!this.m_entitySetMappings.ContainsKey(setMapping.Set.Name))
            {
                this.m_entitySetMappings.Add(setMapping.Set.Name, setMapping);
            }
        }

        internal void AddFunctionImportMapping(EdmFunction functionImport, FunctionImportMapping targetFunction)
        {
            this.m_functionImportMappings.Add(functionImport, targetFunction);
        }

        private OutputFromComputeCellGroups ComputeCellGroups(InputForComputingCellGroups args)
        {
            OutputFromComputeCellGroups groups = new OutputFromComputeCellGroups {
                Success = true
            };
            MetadataWorkspace workspace = new MetadataWorkspace();
            workspace.RegisterItemCollection(args.ContainerMapping.StorageMappingItemCollection.EdmItemCollection);
            workspace.RegisterItemCollection(args.ContainerMapping.StorageMappingItemCollection.StoreItemCollection);
            workspace.RegisterItemCollection(args.ContainerMapping.StorageMappingItemCollection);
            CellCreator creator = new CellCreator(args.ContainerMapping, workspace);
            groups.Cells = creator.GenerateCells(args.Config);
            groups.Identifiers = creator.Identifiers;
            if (groups.Cells.Count <= 0)
            {
                groups.Success = false;
                return groups;
            }
            groups.ForeignKeyConstraints = ForeignConstraint.GetForeignConstraints(args.ContainerMapping.StorageEntityContainer, workspace);
            List<System.Data.Common.Utils.Set<Cell>> list = new CellPartitioner(groups.Cells, groups.ForeignKeyConstraints).GroupRelatedCells();
            groups.CellGroups = (from setOfcells in list select new System.Data.Common.Utils.Set<Cell>(from cell in setOfcells select new Cell(cell))).ToList<System.Data.Common.Utils.Set<Cell>>();
            return groups;
        }

        internal bool ContainsAssociationSetMapping(AssociationSet associationSet) => 
            this.m_associationSetMappings.ContainsKey(associationSet.Name);

        internal OutputFromComputeCellGroups GetCellgroups(InputForComputingCellGroups args) => 
            this.m_memoizedCellGroupEvaluator.Evaluate(args);

        internal StorageSetMapping GetEntitySetMapping(string entitySetName)
        {
            EntityUtil.CheckArgumentNull<string>(entitySetName, "entitySetName");
            StorageSetMapping mapping = null;
            this.m_entitySetMappings.TryGetValue(entitySetName, out mapping);
            return mapping;
        }

        internal static string GetPrettyPrintString(ref int index)
        {
            string str = "";
            str = str.PadLeft(index, ' ');
            Console.WriteLine(str + "|");
            Console.WriteLine(str + "|");
            index++;
            str = str.PadLeft(index, ' ');
            Console.Write(str + "-");
            index++;
            str = str.PadLeft(index, ' ');
            Console.Write("-");
            index++;
            return str.PadLeft(index, ' ');
        }

        internal StorageSetMapping GetRelationshipSetMapping(string relationshipSetName)
        {
            EntityUtil.CheckArgumentNull<string>(relationshipSetName, "relationshipSetName");
            StorageSetMapping mapping = null;
            this.m_associationSetMappings.TryGetValue(relationshipSetName, out mapping);
            return mapping;
        }

        internal IEnumerable<StorageAssociationSetMapping> GetRelationshipSetMappingsFor(EntitySetBase edmEntitySet, EntitySetBase storeEntitySet) => 
            (from associationSetMap in this.m_associationSetMappings.Values.Cast<StorageAssociationSetMapping>()
                where (associationSetMap.StoreEntitySet != null) && (associationSetMap.StoreEntitySet == storeEntitySet)
                where (associationSetMap.Set as AssociationSet).AssociationSetEnds.Any<AssociationSetEnd>(associationSetEnd => associationSetEnd.EntitySet == edmEntitySet)
                select associationSetMap);

        internal StorageSetMapping GetSetMapping(string setName)
        {
            StorageSetMapping entitySetMapping = this.GetEntitySetMapping(setName);
            if (entitySetMapping == null)
            {
                entitySetMapping = this.GetRelationshipSetMapping(setName);
            }
            return entitySetMapping;
        }

        internal bool HasQueryViewForSetMap(string setName)
        {
            StorageSetMapping setMapping = this.GetSetMapping(setName);
            return ((setMapping != null) && (setMapping.QueryView != null));
        }

        internal void Print(int index)
        {
            string str = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(str);
            builder.Append("EntityContainerMapping");
            builder.Append("   ");
            builder.Append("Name:");
            builder.Append(this.m_entityContainer.Name);
            builder.Append("   ");
            Console.WriteLine(builder.ToString());
            foreach (StorageSetMapping mapping in this.m_entitySetMappings.Values)
            {
                mapping.Print(index + 5);
            }
            foreach (StorageSetMapping mapping2 in this.m_associationSetMappings.Values)
            {
                mapping2.Print(index + 5);
            }
        }

        internal bool TryGetFunctionImportMapping(EdmFunction functionImport, out FunctionImportMapping targetFunction) => 
            this.m_functionImportMappings.TryGetValue(functionImport, out targetFunction);

        internal IEnumerable<StorageSetMapping> AllSetMaps =>
            this.m_entitySetMappings.Values.Concat<StorageSetMapping>(this.m_associationSetMappings.Values);

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem;

        internal EntityContainer EdmEntityContainer =>
            this.m_entityContainer;

        internal override MetadataItem EdmItem =>
            this.m_entityContainer;

        internal ReadOnlyCollection<StorageSetMapping> EntitySetMaps =>
            new List<StorageSetMapping>(this.m_entitySetMappings.Values).AsReadOnly();

        internal override string Identity =>
            this.identity;

        internal bool IsEmpty =>
            ((this.m_entitySetMappings.Count == 0) && (this.m_associationSetMappings.Count == 0));

        internal ReadOnlyCollection<StorageSetMapping> RelationshipSetMaps =>
            new List<StorageSetMapping>(this.m_associationSetMappings.Values).AsReadOnly();

        internal string SourceLocation
        {
            get => 
                this.m_sourceLocation;
            set
            {
                this.m_sourceLocation = value;
            }
        }

        internal int StartLineNumber
        {
            get => 
                this.m_startLineNumber;
            set
            {
                this.m_startLineNumber = value;
            }
        }

        internal int StartLinePosition
        {
            get => 
                this.m_startLinePosition;
            set
            {
                this.m_startLinePosition = value;
            }
        }

        internal EntityContainer StorageEntityContainer =>
            this.m_storageEntityContainer;

        public System.Data.Mapping.StorageMappingItemCollection StorageMappingItemCollection =>
            this.m_storageMappingItemCollection;
    }
}

