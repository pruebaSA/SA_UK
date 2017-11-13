namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class EntityIdentity
    {
        private readonly SimpleColumnMap[] m_keys;
        internal static SimpleEntityIdentity NoEntityIdentity = new SimpleEntityIdentity(null, new SimpleColumnMap[0]);

        internal EntityIdentity(SimpleColumnMap[] keyColumns)
        {
            this.m_keys = keyColumns;
        }

        internal SimpleColumnMap[] Keys =>
            this.m_keys;
    }
}

