namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Xml;

    internal sealed class FunctionImportEntityTypeMapping
    {
        internal readonly ReadOnlyCollection<FunctionImportEntityTypeMappingCondition> Conditions;
        internal readonly ReadOnlyCollection<System.Data.Metadata.Edm.EntityType> EntityTypes;
        internal readonly ReadOnlyCollection<System.Data.Metadata.Edm.EntityType> IsOfTypeEntityTypes;
        internal readonly IXmlLineInfo LineInfo;

        internal FunctionImportEntityTypeMapping(IEnumerable<System.Data.Metadata.Edm.EntityType> isOfTypeEntityTypes, IEnumerable<System.Data.Metadata.Edm.EntityType> entityTypes, IEnumerable<FunctionImportEntityTypeMappingCondition> conditions, IXmlLineInfo lineInfo)
        {
            this.IsOfTypeEntityTypes = new ReadOnlyCollection<System.Data.Metadata.Edm.EntityType>(EntityUtil.CheckArgumentNull<IEnumerable<System.Data.Metadata.Edm.EntityType>>(isOfTypeEntityTypes, "isOfTypeEntityTypes").ToList<System.Data.Metadata.Edm.EntityType>());
            this.EntityTypes = new ReadOnlyCollection<System.Data.Metadata.Edm.EntityType>(EntityUtil.CheckArgumentNull<IEnumerable<System.Data.Metadata.Edm.EntityType>>(entityTypes, "entityTypes").ToList<System.Data.Metadata.Edm.EntityType>());
            this.Conditions = new ReadOnlyCollection<FunctionImportEntityTypeMappingCondition>(EntityUtil.CheckArgumentNull<IEnumerable<FunctionImportEntityTypeMappingCondition>>(conditions, "conditions").ToList<FunctionImportEntityTypeMappingCondition>());
            this.LineInfo = lineInfo;
        }

        internal IEnumerable<string> GetDiscriminatorColumns() => 
            (from condition in this.Conditions select condition.ColumnName);

        internal IEnumerable<System.Data.Metadata.Edm.EntityType> GetMappedEntityTypes(ItemCollection itemCollection) => 
            this.EntityTypes.Concat<System.Data.Metadata.Edm.EntityType>((from entityType in this.IsOfTypeEntityTypes select MetadataHelper.GetTypeAndSubtypesOf(entityType, itemCollection, false).Cast<System.Data.Metadata.Edm.EntityType>()));
    }
}

