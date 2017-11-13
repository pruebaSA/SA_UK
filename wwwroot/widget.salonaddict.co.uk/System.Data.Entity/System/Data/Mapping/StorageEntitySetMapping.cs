namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Text;

    internal class StorageEntitySetMapping : StorageSetMapping
    {
        private readonly List<StorageEntityTypeFunctionMapping> m_functionMappings;
        private readonly List<AssociationSetEnd> m_implicitlyMappedAssociationSetEnds;

        internal StorageEntitySetMapping(EntitySet extent, StorageEntityContainerMapping entityContainerMapping) : base(extent, entityContainerMapping)
        {
            this.m_functionMappings = new List<StorageEntityTypeFunctionMapping>();
            this.m_implicitlyMappedAssociationSetEnds = new List<AssociationSetEnd>();
        }

        internal void AddFunctionMapping(StorageEntityTypeFunctionMapping functionMapping)
        {
            this.m_functionMappings.Add(functionMapping);
            functionMapping.DeleteFunctionMapping.AddReferencedAssociationSetEnds((EntitySet) base.Set, this.m_implicitlyMappedAssociationSetEnds);
            functionMapping.InsertFunctionMapping.AddReferencedAssociationSetEnds((EntitySet) base.Set, this.m_implicitlyMappedAssociationSetEnds);
            functionMapping.UpdateFunctionMapping.AddReferencedAssociationSetEnds((EntitySet) base.Set, this.m_implicitlyMappedAssociationSetEnds);
        }

        [Conditional("DEBUG")]
        internal void AssertFunctionMappingInvariants(StorageEntityTypeFunctionMapping functionMapping)
        {
            using (List<StorageEntityTypeFunctionMapping>.Enumerator enumerator = this.m_functionMappings.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    StorageEntityTypeFunctionMapping current = enumerator.Current;
                }
            }
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("EntitySetMapping");
            builder.Append("   ");
            builder.Append("Name:");
            builder.Append(base.Set.Name);
            if (base.QueryView != null)
            {
                builder.Append("   ");
                builder.Append("Query View:");
                builder.Append(base.QueryView);
            }
            Console.WriteLine(builder.ToString());
            foreach (StorageTypeMapping mapping in base.TypeMappings)
            {
                mapping.Print(index + 5);
            }
            foreach (StorageEntityTypeFunctionMapping mapping2 in this.m_functionMappings)
            {
                mapping2.Print(index + 10);
            }
        }

        internal IList<StorageEntityTypeFunctionMapping> FunctionMappings =>
            this.m_functionMappings.AsReadOnly();

        internal override bool HasNoContent
        {
            get
            {
                if (this.m_functionMappings.Count != 0)
                {
                    return false;
                }
                return base.HasNoContent;
            }
        }

        internal IList<AssociationSetEnd> ImplicitlyMappedAssociationSetEnds =>
            this.m_implicitlyMappedAssociationSetEnds.AsReadOnly();
    }
}

