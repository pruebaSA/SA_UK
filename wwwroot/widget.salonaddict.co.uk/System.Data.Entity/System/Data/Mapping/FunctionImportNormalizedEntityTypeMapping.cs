namespace System.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Globalization;
    using System.Linq;

    internal sealed class FunctionImportNormalizedEntityTypeMapping
    {
        internal readonly ReadOnlyCollection<FunctionImportEntityTypeMappingCondition> ColumnConditions;
        internal readonly BitArray ComplementImpliedEntityTypes;
        internal readonly BitArray ImpliedEntityTypes;

        internal FunctionImportNormalizedEntityTypeMapping(FunctionImportMapping parent, List<FunctionImportEntityTypeMappingCondition> columnConditions, BitArray impliedEntityTypes)
        {
            EntityUtil.CheckArgumentNull<FunctionImportMapping>(parent, "parent");
            EntityUtil.CheckArgumentNull<List<FunctionImportEntityTypeMappingCondition>>(columnConditions, "discriminatorValues");
            EntityUtil.CheckArgumentNull<BitArray>(impliedEntityTypes, "impliedEntityTypes");
            this.ColumnConditions = new ReadOnlyCollection<FunctionImportEntityTypeMappingCondition>(columnConditions.ToList<FunctionImportEntityTypeMappingCondition>());
            this.ImpliedEntityTypes = impliedEntityTypes;
            this.ComplementImpliedEntityTypes = new BitArray(this.ImpliedEntityTypes).Not();
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "Values={0}, Types={1}", new object[] { StringUtil.ToCommaSeparatedString(this.ColumnConditions), StringUtil.ToCommaSeparatedString(this.ImpliedEntityTypes) });
    }
}

