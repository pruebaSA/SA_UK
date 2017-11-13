namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal sealed class ParameterVar : Var
    {
        private string m_paramName;

        internal ParameterVar(int id, TypeUsage type, string paramName) : base(id, VarType.Parameter, type)
        {
            this.m_paramName = paramName;
        }

        internal override bool TryGetName(out string name)
        {
            name = this.ParameterName;
            return true;
        }

        internal string ParameterName =>
            this.m_paramName;
    }
}

