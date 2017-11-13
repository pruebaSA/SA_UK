namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Linq.Expressions;

    internal class RecordStateFactory
    {
        internal readonly int ColumnCount;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<string> ColumnNames;
        internal readonly System.Data.Common.DataRecordInfo DataRecordInfo;
        private readonly string Description;
        internal readonly System.Data.FieldNameLookup FieldNameLookup;
        internal readonly Func<Shaper, bool> GatherData;
        internal readonly bool HasNestedColumns;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<bool> IsColumnNested;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<RecordStateFactory> NestedRecordStateFactories;
        internal readonly int StateSlotNumber;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<TypeUsage> TypeUsages;

        public RecordStateFactory(int stateSlotNumber, int columnCount, RecordStateFactory[] nestedRecordStateFactories, System.Data.Common.DataRecordInfo dataRecordInfo, Expression gatherData, string[] propertyNames, TypeUsage[] typeUsages)
        {
            this.StateSlotNumber = stateSlotNumber;
            this.ColumnCount = columnCount;
            this.NestedRecordStateFactories = new System.Collections.ObjectModel.ReadOnlyCollection<RecordStateFactory>(nestedRecordStateFactories);
            this.DataRecordInfo = dataRecordInfo;
            this.GatherData = Translator.Compile<bool>(gatherData);
            this.Description = gatherData.ToString();
            this.ColumnNames = new System.Collections.ObjectModel.ReadOnlyCollection<string>(propertyNames);
            this.TypeUsages = new System.Collections.ObjectModel.ReadOnlyCollection<TypeUsage>(typeUsages);
            this.FieldNameLookup = new System.Data.FieldNameLookup(this.ColumnNames, -1);
            bool[] list = new bool[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                switch (typeUsages[i].EdmType.BuiltInTypeKind)
                {
                    case BuiltInTypeKind.CollectionType:
                    case BuiltInTypeKind.ComplexType:
                    case BuiltInTypeKind.EntityType:
                    case BuiltInTypeKind.RowType:
                    {
                        list[i] = true;
                        this.HasNestedColumns = true;
                        continue;
                    }
                }
                list[i] = false;
            }
            this.IsColumnNested = new System.Collections.ObjectModel.ReadOnlyCollection<bool>(list);
        }

        internal RecordState Create(CoordinatorFactory coordinatorFactory) => 
            new RecordState(this, coordinatorFactory);
    }
}

