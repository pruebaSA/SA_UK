namespace System.Data.Query.InternalTrees
{
    using System;

    internal class SortKey
    {
        private bool m_asc;
        private string m_collation;
        private System.Data.Query.InternalTrees.Var m_var;

        internal SortKey(System.Data.Query.InternalTrees.Var v, bool asc, string collation)
        {
            this.m_var = v;
            this.m_asc = asc;
            this.m_collation = collation;
        }

        internal bool AscendingSort =>
            this.m_asc;

        internal string Collation =>
            this.m_collation;

        internal System.Data.Query.InternalTrees.Var Var
        {
            get => 
                this.m_var;
            set
            {
                this.m_var = value;
            }
        }
    }
}

