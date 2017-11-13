namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal abstract class Var
    {
        private int m_id;
        private TypeUsage m_type;
        private System.Data.Query.InternalTrees.VarType m_varType;

        internal Var(int id, System.Data.Query.InternalTrees.VarType varType, TypeUsage type)
        {
            this.m_id = id;
            this.m_varType = varType;
            this.m_type = type;
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { this.Id });

        internal virtual bool TryGetName(out string name)
        {
            name = null;
            return false;
        }

        internal int Id =>
            this.m_id;

        internal TypeUsage Type =>
            this.m_type;

        internal System.Data.Query.InternalTrees.VarType VarType =>
            this.m_varType;
    }
}

