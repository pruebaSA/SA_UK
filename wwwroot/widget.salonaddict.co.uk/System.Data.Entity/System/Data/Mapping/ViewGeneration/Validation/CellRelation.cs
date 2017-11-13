namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Data.Common.Utils;

    internal abstract class CellRelation : InternalBase
    {
        internal int m_cellNumber;

        protected CellRelation(int cellNumber)
        {
            this.m_cellNumber = cellNumber;
        }

        protected abstract int GetHash();

        internal int CellNumber =>
            this.m_cellNumber;
    }
}

