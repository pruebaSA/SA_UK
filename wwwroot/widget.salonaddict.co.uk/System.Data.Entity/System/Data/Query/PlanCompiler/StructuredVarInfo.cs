namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class StructuredVarInfo : System.Data.Query.PlanCompiler.VarInfo
    {
        private List<EdmProperty> m_newProperties;
        private RowType m_newType;
        private TypeUsage m_newTypeUsage;
        private List<Var> m_newVars;
        private Dictionary<EdmProperty, Var> m_propertyToVarMap;

        internal StructuredVarInfo(RowType newType, List<Var> newVars, List<EdmProperty> newTypeProperties)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(newVars.Count == newTypeProperties.Count, "count mismatch");
            this.m_newVars = newVars;
            this.m_newProperties = newTypeProperties;
            this.m_newType = newType;
            this.m_newTypeUsage = TypeUsage.Create(newType);
        }

        private void InitPropertyToVarMap()
        {
            if (this.m_propertyToVarMap == null)
            {
                this.m_propertyToVarMap = new Dictionary<EdmProperty, Var>();
                IEnumerator<Var> enumerator = this.m_newVars.GetEnumerator();
                foreach (EdmProperty property in this.m_newProperties)
                {
                    enumerator.MoveNext();
                    this.m_propertyToVarMap.Add(property, enumerator.Current);
                }
                enumerator.Dispose();
            }
        }

        internal bool TryGetVar(EdmProperty p, out Var v) => 
            this.m_propertyToVarMap?.TryGetValue(p, out v);

        internal List<EdmProperty> Fields =>
            this.m_newProperties;

        internal override bool IsStructuredType =>
            true;

        internal RowType NewType =>
            this.m_newType;

        internal TypeUsage NewTypeUsage =>
            this.m_newTypeUsage;

        internal override List<Var> NewVars =>
            this.m_newVars;
    }
}

