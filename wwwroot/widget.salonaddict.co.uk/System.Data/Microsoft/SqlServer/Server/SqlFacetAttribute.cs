namespace Microsoft.SqlServer.Server
{
    using System;

    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class SqlFacetAttribute : Attribute
    {
        private bool m_IsFixedLength;
        private bool m_IsNullable;
        private int m_MaxSize;
        private int m_Precision;
        private int m_Scale;

        public bool IsFixedLength
        {
            get => 
                this.m_IsFixedLength;
            set
            {
                this.m_IsFixedLength = value;
            }
        }

        public bool IsNullable
        {
            get => 
                this.m_IsNullable;
            set
            {
                this.m_IsNullable = value;
            }
        }

        public int MaxSize
        {
            get => 
                this.m_MaxSize;
            set
            {
                this.m_MaxSize = value;
            }
        }

        public int Precision
        {
            get => 
                this.m_Precision;
            set
            {
                this.m_Precision = value;
            }
        }

        public int Scale
        {
            get => 
                this.m_Scale;
            set
            {
                this.m_Scale = value;
            }
        }
    }
}

