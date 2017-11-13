namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Data.Mapping;

    internal class CellLabel
    {
        private string m_sourceLocation;
        private int m_startLineNumber;
        private int m_startLinePosition;

        internal CellLabel(StorageMappingFragment fragmentInfo) : this(fragmentInfo.StartLineNumber, fragmentInfo.StartLinePosition, fragmentInfo.SourceLocation)
        {
        }

        internal CellLabel(CellLabel source)
        {
            this.m_startLineNumber = source.m_startLineNumber;
            this.m_startLinePosition = source.m_startLinePosition;
            this.m_sourceLocation = source.m_sourceLocation;
        }

        internal CellLabel(int startLineNumber, int startLinePosition, string sourceLocation)
        {
            this.m_startLineNumber = startLineNumber;
            this.m_startLinePosition = startLinePosition;
            this.m_sourceLocation = sourceLocation;
        }

        internal string SourceLocation =>
            this.m_sourceLocation;

        internal int StartLineNumber =>
            this.m_startLineNumber;

        internal int StartLinePosition =>
            this.m_startLinePosition;
    }
}

