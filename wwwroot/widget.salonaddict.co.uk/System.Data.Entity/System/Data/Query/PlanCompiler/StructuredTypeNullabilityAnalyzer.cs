namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal class StructuredTypeNullabilityAnalyzer : ColumnMapVisitor<HashSet<string>>
    {
        internal static StructuredTypeNullabilityAnalyzer Instance = new StructuredTypeNullabilityAnalyzer();

        private static void AddTypeNeedingNullSentinel(HashSet<string> typesNeedingNullSentinel, TypeUsage typeUsage)
        {
            if (TypeSemantics.IsCollectionType(typeUsage))
            {
                AddTypeNeedingNullSentinel(typesNeedingNullSentinel, TypeHelpers.GetElementTypeUsage(typeUsage));
            }
            else
            {
                if (TypeSemantics.IsRowType(typeUsage) || TypeSemantics.IsComplexType(typeUsage))
                {
                    MarkAsNeedingNullSentinel(typesNeedingNullSentinel, typeUsage);
                }
                foreach (EdmMember member in TypeHelpers.GetAllStructuralMembers(typeUsage))
                {
                    AddTypeNeedingNullSentinel(typesNeedingNullSentinel, member.TypeUsage);
                }
            }
        }

        internal static void MarkAsNeedingNullSentinel(HashSet<string> typesNeedingNullSentinel, TypeUsage typeUsage)
        {
            typesNeedingNullSentinel.Add(typeUsage.EdmType.Identity);
        }

        internal override void Visit(VarRefColumnMap columnMap, HashSet<string> typesNeedingNullSentinel)
        {
            AddTypeNeedingNullSentinel(typesNeedingNullSentinel, columnMap.Type);
            base.Visit(columnMap, typesNeedingNullSentinel);
        }
    }
}

